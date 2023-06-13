# k6 moh-eRx Non-Functional (Performance) Tests

k6 is a modern load testing tool, building on Load Impact's years of experience in the load and performance testing industry. It provides a clean and approachable scripting API, local and cloud execution, and flexible configuration.

## Installing k6
If you haven't yet, go to docker.com and install Docker on your computer. Make sure the command line tool, docker, is in your PATH. Also make sure that you have the docker compose tool installed and can run it from the command line.

Also see [k6 Getting Started](https://k6.io/docs/getting-started/installation) for more information on getting started with k6.

## Running Tests Locally
When running smoke tests, it may be easier to run the test scripts on local versions of the APIs. You may also want to run a local PharmaNet proxy to avoid sending unnessecary requests to PharmaNet.

### Setting up a PharmaNet proxy in Docker
1. Build the Docker image for the proxy by running `docker build -t pnet-proxy/image test/functional/pnet-proxy/` in the command line.
2. Run the Docker container with the command `docker run --name pnet-proxy -p 8080:8080 pnet-proxy/image`.
3. For each service you'll be testing with, update the PharmaNet proxy endpoint to `http://host.docker.internal:8080/submit` in `Services/{name-of-service}/src/appsettings.json`. If you've already compiled your services, you'll need to recompile them to apply the change.
Now the PharmaNet proxy is listening on localhost:8080. If you map the ports differently, be sure to make the same change everywhere.

### Setting up a service in Docker
1. Modify the property assignment of `options.RequireHttpsMetadata` in `Services/Common/src/AspNetConfiguration/StartupConfiguration.cs` at line 135 to `false`. The k6 program is only configured to send HTTP requests, not HTTPS, so the program needs to accept those requests. Don't forget to revert this change when you're done with testing!
2. In the service's appsettings.json file, update the Claims Issuer and Authority endpoints under the OpenIdConnect section. These should both be set to the URL that the program is fetching the access token from. Since this set up is for running tests, the URL will probably be either the Dev endpoint (`https://common-logon-dev.hlth.gov.bc.ca/auth/realms/v2_pos`) or the Test endpoint (`https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications`).
3. Also in appsettings.json, make sure the PharmaNet proxy endpoint is set to the PharmaNet URL you want the API to make requests to. If you're using a local PharmaNet proxy, this URL should be `http://host.docker.internal:8080/submit`. If the PharmaNet system requires additional properties such as Username, Password, or a Client Certificate file, set those fields to the appropriate values.
4. Build the service you want to test by running `dotnet build` in the command line from `Services/{name-of-service}/src/`, which contains the services .csproj file.
5. Build the Docker image for the service by running `docker build -t {name-of-service}/image .` from the same directory, which also contains the service's Dockerfile.
6. Run the Docker container with the command `docker run --name {name-of-service} -p {unused-port}:8080 {name-of-service}/image`.
Now the API is listening on localhost at the chosen port.

## Running k6 Tests
The k6 test suite included with this repo acts as a client to the API Services. Given an environment, a client ID, and a client secret, it retrieves a signed JWT access token that it passes to the API with the request. Each API service has a corresponding JavaScript program and docker-compose.yaml file to run the k6 test suite.

Before running a test on a service, make sure the API is running and mapped to the correct port. Follow the steps above to configure it if you haven't yet, and check the values for the service URLs in `test/k6/inc/common.js` to make sure they are pointed to the right server (`host.docker.internal` if you are running them locally) and the right port.

The test program will need some values as environment variables in order to authenticate the client to the API. There are three that are required for authentication:
1. ERX_ENV: The environment in which the API exists. This value can be either "dev", "vs1", or "vs2", and defaults to "dev" if left blank. The "dev" environment corresponds to one URL endpoint the program fetches the access token from, and "vs1" and "vs2" correspond to another.
2. ERX_CLIENT: The client ID used to get the access token. This value should be "erx_development" if the environment is "dev" or "ppm_development" if the environment is "vs1" or "vs2". If left blank, the default value is "erx_development".
3. ERX_CLIENT_SECRET: The client secret used to get the access token. This value can be retrieved from Keycloak and should correspond directly to the client ID. This variable has no default value and must always be set.

There are also two testing parameters, ERX_VUS and ERX_ITERATIONS, which both have a default value of 1. See below for more information about what these values mean.

Finally, to execute the test, navigate to `test/k6/api/{name-of-service}/` and execute the command `docker compose up`. Prefix the command with assignments to the environment variables you want to set. You may need to execute the command in a bash terminal for the prefixed variables to be read properly. You can also export the values into global environment variables, but be careful about storing the client secret this way.

### Example
```bash
/moh-eRx/test/k6/api/Claim$ ERX_ENV=dev ERX_CLIENT=erx_development ERX_CLIENT_SECRET=xxxxx-xxxxx-xxxxx-xxxxx docker compose up
```

## Virtual Users (VUs)

k6 applies the concept of Virtual Users. A Virtual User represents a single user interacting with the program, and each iteration represents one interaction or session. Virtual Users can run concurrently and are capable of providing significant load to the application. For example, if 10 VUs ran for 1 minute and each iteration of the script took 10 seconds, the total number of iterations run would be (10 VUs) * (1 minute) * (60 seconds per minute) / (10 seconds per iteration) = 60 VU iterations completed.

For more information on Virtual Users, see the [k6 documentation](https://k6.io/docs/getting-started/running-k6).

### Calculating the number of VUs for a target load
In order to effectively perform a load test, you need to know how many transactions or sessions you can expect in a given time frame and how long each session lasts. Then you can create a number of VUs equal to the expected number of concurrent sessions.
```code
number of concurrent sessions = (sessions per hour) * (seconds per session) / (3600 seconds per hour)
```
For example, if we expect up to 10,000 transactions hitting the moh-eRx APIs within a given hour, and we expect that the average session duration is 1 minute, then we should create 167 VUs to simulate peak traffic.

## Smoke Testing
The Smoke Test's role is to verify that your System can handle minimal load, without any problems. By default, our smoke tests use 1 virtual user and execute between 1 and 13 API calls to ensure it returns a 200 OK HTTP response. If the API returns an error response, the program has a functional issue and needs to be fixed.

### When to run the smoke test
Run this test often, after each system change/release. This ensures that functionality has not broken under basic loads.

## Load Testing
Load testing is primarily concerned with assessing the systems performance. The purpose of load testing is to assess the availability and stability of the system under heavy load. You can play with the VUS and Iterations within the shell script to try out different configurations.
A possible future goal of this project is to add a more sophisticated load test script that includes ramp up time and groups logical sequences of API calls mimicking real world flow.

## Stress Testing
Stress Testing is a type of load testing used to determine the limits of the system. The purpose of this test is to assess the stability and reliability of the system under extreme conditions. Set the number of VUs in the load test to be very high to simulate a huge number of concurrent users.
