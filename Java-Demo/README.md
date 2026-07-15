#### Introduction

The goal of this project is to demonstrate a working transaction end-to-end through the pharmanet system with a minimum number of lines of code.

Review and update `PharmaNetClaim.java` to provide the required input parameters to suit your environment (client secret, vendor ID, etc).

**get dependencies, save in local folder libs:**

`mkdir libs`

`mvn dependency:copy-dependencies -DoutputDirectory=libs`

**compile:**

`mvn compile package`

**run**

*notice the use of a backslash indicating Windows environment*:

`java -cp "target\moh-erx-java-1.0.0.jar;./libs/*" PharmaNetClaim`
