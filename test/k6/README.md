# k6 moh-eRx Non-Functional (Performance) Tests

k6 is a modern load testing tool, building on Load Impact's years of experience in the load and performance testing industry. It provides a clean and approachable scripting API, local and cloud execution, and flexible configuration.

## Installing k6
If you haven't yet, go to docker.com and install Docker on your computer. Make sure the command line tool, docker, is in your PATH. Also make sure that you have the docker compose tool installed and can run it from the command line.

Also see [k6 Getting Started](https://k6.io/docs/getting-started/installation) for more information on getting started with k6.

## Running Tests Locally
When running smoke tests, it may be easier to run the test scripts on local versions of the APIs. You may also want to run a local PharmaNet proxy to avoid sending unnessecary requests to PharmaNet.

### Setting up a PharmaNet proxy in Docker
1. Build the PharmaNet proxy by running `dotnet build` in the command line from `moh-eRx/test/functional/pnet-proxy/`, which contains the proxy's .csproj file and Dockerfile.
2. Build the Docker image for the proxy by running `docker build -t pnet-proxy/image .` from the command line in the same directory. If you are running the command from somewhere else, replace `.` with the absolute path to the directory.
3. Run the Docker container with the command `docker run --name pnet-proxy -p 8080:8080 pnet-proxy/image`.
4. For each service you'll be testing with, update the PharmaNet proxy endpoint to "http://host.docker.internal:8080/submit" in `moh-eRx/Services/{name-of-service}Service/src/appsettings.json`. It's important to use "host.docker.internal" here, since "localhost" maps to Docker from within Docker containers, not to the machine running Docker. If you've already compiled your services, you'll need to recompile them to apply the change.
Now the PharmaNet proxy is listening on localhost:8080. If you map the ports differently, be sure to make the same change everywhere.

### Setting up a service in Docker
1. Modify the property assignment of `options.RequireHttpsMetadata = true;` in `moh-eRx/Services/Common/src/AspNetConfiguration/StartupConfiguration.cs` at line 135 to `false`. The k6 program is only configured to send HTTP requests, not HTTPS, so the program needs to accept those requests. Don't forget to revert this change when you're done with testing!
2. In the service's `appsettings.json` file, update the Claims Issuer and Authority endpoints under the OpenIDConnect section. These should both be set to the URL that the program is fetching the access token from.
1. Build the service you want to test by running `dotnet build` in the command line from `moh-eRx/Services/{name-of-service}Service/src/`.
2. Build the Docker image for the service by running `docker build -t {name-of-service}/image .` from the same directory, which also contains the service's Dockerfile.
3. Run the Docker container with the command `docker run --name {name-of-service} -p {unused-port}:8080 {name-of-service}/image`.
Now the API is listening on localhost at the chosen port.

## Running k6 Tests
The k6 test suite included with this repo acts as a client to the API Services. Given an environment, a client ID, and a client secret, it retrieves a signed JWT access token that it passes to the API with the request. Each API service has a corresponding JavaScript program and docker-compose.yaml file to run the k6 test suite.

To run a test on a service, first make sure the API is running and listening on the correct port.

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
