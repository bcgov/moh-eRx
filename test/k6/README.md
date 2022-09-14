# K6 moh-eRx Non-Functional (Performance) Tests

k6 is a modern load testing tool, building on Load Impact's years of experience in the load and performance testing industry. It provides a clean, approachable scripting API, local and cloud execution, and flexible configuration.

## Installing K6

If you don't have Docker, go to docker.com and download Docker on your computer. Make sure the command line  tool, docker, is in your PATH.


For more information on getting started with K6, see [k6 Getting Started](https://k6.io/docs/getting-started/installation)

## Running k6 tests

There are three environment variables you can set. The only mandatory one is the client secret. The other values has the following default values:

ERX_ENV=dev
ERX_VUS=1
ERX_ITERATIONS=1
ERX_CLIENT=erx_development
ERX_CLIENT_SECRET=

### Example

```code
cd api/Claim
ERX_CLIENT_SECRET={CLIENT SECRET HERE} docker compose up
```

For specifics on running the k6 scripts for the APIs, see documentation below. 

### See also

[Running k6](https://k6.io/docs/getting-started/running-k6)

k6 is a free and open-source load testing tool written in Go language with tests scripted in JavaScript.

## Virtual Users (vus)

k6 uses the concept of virtual users. When specifying the number of virtual users, they run concurrently over the script. Each iteration of the script represents a single user operating that script. So, if 10 vus ran for 1 minute and each iteration of the script took 10 seconds, then the total number of iterations run would be 10 x 60/10 = 60 iterations completed.

For more information on virtual users, see the k6 documentation: [what are virtual users](https://k6.io/docs/cloud/cloud-faq/what-are-vus-virtual-users).

### Calculation the number of VUS max

```code
VUs = (hourly sessions * average session duration in seconds)/3600
```

> For example, if we expect up to 10,000 transactions hitting the moh-eRx APIs within a given hour, and we expect that the average session duration is 1 minute, then the VUS would be 167.

See [k6 calculation guidance](https://k6.io/docs/cloud/cloud-faq/what-are-vus-virtual-users)

## Smoke Testing

The Smoke Test's role is to verify that your System can handle minimal load, without any problems. 
Our smoke tests uses by default 1 virtual user and executes the API call iteratively for 10 seconds duration to ensure it returns a 200 OK.

Any errors here are an indication of functionality not working under basic load.

The choices for ERX_ENV are 'dev' or 'vs1' (vendor staging 1)

```bash
export ERX_VUS=1
export ERX_ITERATIONS=1
export ERX_ENV=dev
export ERX_CLIENT_SECRET=<client_credentials_grant_secret> 
cd api/Claim
docker compose up
```

### When to run the smoke test

Run this test often, after each system change/release.  This ensures that functionality has not broken under basic loads.

## Load Testing

Load testing is primarily concerned with assessing the systems performance, the purpose of stress testing is to assess the availability and stability of the system under heavy load. You can play with the VUS and Iterations within the shell script.

```bash
export ERX_VUS=50
export ERX_VUS=100
export ERX_ENV=dev
export ERX_CLIENT_SECRET=<client_credentials_grant_secret> 
docker compose up
```

Future: Add more sophisticated load test script that includes ramp up time and groups logical sequences of api calls mimicking real world flow.

## Stress Testing

Stress Testing is a type of load testing used to determine the limits of the system. The purpose of this test is to verify the stability and reliability of the system under extreme conditions. Set the vus in the load test to be really high.
