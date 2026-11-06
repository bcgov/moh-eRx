#### Introduction
This project is unsupported and proved as "as-is". Use at your own risk.

The goal of this project is to demonstrate a transaction end-to-end through the pharmanet system with a minimum number of lines of code.

Review and update `PharmaNetClaim.java` to provide the required input parameters to suit your environment (client secret, vendor ID, etc).
You must use your own Client ID, pfx file, key file and HL7 message.

# Example

#### Extract key
If you need to, extract the key from your .pfx file such as in the following example.

`openssl pkcs12 -legacy -in PNET-39999999.pfx -nodes -nocerts -passin pass:'Your-Password' > PNET-39999999.key`

Edit your `.key` file to include only the key, nothing else.

**compile:**

`mvn compile package`

**run**

`mvn compile exec:java`

### PowerShell:
'mvn compile exec:java "-Dexec.args=PNET-39999999'

### Optional
To create a deployable Java program, create a `libs` folder and run:
`mvn dependency:copy-dependencies -DoutputDirectory=libs`
That will put all the dependencies into the libs folder. 
```
mvn compile package
java -cp "target\moh-erx-java-1.0.0.jar;./libs/*" demo.PharmaNetClaim PNET-39999999
```

#### Create a signed JWT and write it to STDOUT
```
mvn compile package
java -cp "target\moh-erx-java-1.0.0.jar;./libs/*" demo.JwtService PNET-39999999 https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token
```