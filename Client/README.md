# Example API Client Console Application written in C# .Net 5

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

## Convert the JKS to a pkcs12 (pfx) certificate file format

If Keycloak is supplying your client's private key, it will hang onto the public key to verify your client authentication. Keycloak provides a JKS file format for your private keys. It will need to be converted to a pfx certificate file in order to use it with the Client code provided here.  

You will need to know the JKS source password, but first you will be prompted to enter a destination password for the pfx file. It will need to be at least 6 characters long.  

```bash
keytool -importkeystore -srckeystore keystore.jks \
 -destkeystore keystore.pfx -srcstoretype jks -deststoretype pkcs12
```

You can now use this pfx file in the Client example, and its private key will be used to sign the JWT
used for OAuth 2.0 authentication against Keycloak. You will use the password you set in the appsettings.json configuration file as well.

- you will want to read up on how to store certificates privately and available to the app configurations.

## Getting the public key from your PFX file - in case you want to check your signing.

If you want to check the signing of the JWT using your public key, you can create a public key file and then use it to check the signing.  In this case, follow these steps.

### Step 1 - Convert the PKCS file to a PEM file

Using openssl, you will now convert the p12 file to pem format. The pem will also require a passphrase to be set.

```bash
openssl pkcs12 -in keystore.pfx -out keystore.pem
```

### Step 2 - Extract the RSA256 Public Key Output
```bash
openssl rsa -in keystore.pem -RSAPublicKey_out > keystore_public.txt
```


# Warning

The above method of using appsettings.json to store the private key is not recommended for production.
You will want to store your private key in a vault or secure area, and retrieve it in code to sign the JWT.






