# Example API Client Console Application written in C# .Net Core 5

This example is meant for developer's to view the source code to understand how to:

- authenticate using client id and secret method (for non-production environements)
- authenticate using signed JWT
- See how to invoke an example service

This uses .NET 5, which you can use to create server apps, command line apps, or WinForms desktop apps.

## Build

```bash
cd src \
dotnet build
```

## Debug

The .vscode/launch.json file is included in this repo to show you how you configure debugging
this console app.

## Run

- Configure settings in appsettings.local.json
- Run:

```bash
dotnet run
```

# Keycloak and generating and using Signed JWT for Client Authentication

Keycloak can generate a keystore.jks file containing the private key for the client application.
It can also accept the upload of a JKS or PEM certificate or public Key PEM for validating the signed JWT provided by the client during authentication.

In this example client, the Keycloak administrator provided us with a keystore.jks file.  We need to convert this file to a pem file and then to a RSAPublicKey_out using openssl to allow us to sign the JWT.

Here are the steps, assuming that the private key is created by the Keycloak administrator for you.

## Step 1 - Convert the JKS to a pkcs12 file format

You will need to know the JKS source password, but first you will be prompted to enter a destination password for the p12 file. It will need to be at least 6 characters long.  

```bash
keytool -importkeystore -srckeystore keystore.jks \
 -destkeystore keystore.p12 -srcstoretype jks -deststoretype pkcs12
```

## Step 2 - Convert the PKCS file to a PEM file

Using openssl, you will now convert the p12 file to pem format. The pem will also require a passphrase to be set.

```bash
openssl pkcs12 -in keystore.p12 -out keystore.pem
```

## Step 3 - Extract the RSA256 Public Key Output
```bash
openssl rsa -in keystore.pem -RSAPublicKey_out > keystore_public.txt
```

## Step 4 - Set the appsettings.json values for the RSA Public and Private keys

a. Paste the Base64 private key string form the keystore.pem file into the RSAPrivateKey setting.
b. Paste the Base64 public key string from the keystore_public.txt file into the RSAPublicKey setting.

## Warning

The above method of using appsettings.json to store the private key is not recommended for production.
You will want to store your private key in a vault or secure area, and retrieve it in code to sign the JWT.






