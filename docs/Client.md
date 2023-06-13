# Example API Client Console Application

This example client application helps developers understand and debug:
- authentication using a client id and a client secret (for non-production environements)
- authentication using a signed JWT
- invoking an example service (forthcoming)

This example client uses .NET 6.0, which you can use to create server apps, command line apps, or WinForms desktop apps.

## Build
Build the project by running `dotnet build` in the command line from `Client/src/`.

## Debug
The .vscode/launch.json file is included in this repo to show you how tp configure debugging this console app.

## Run
Fill out the fields in `Client/src/appsettings.json` with the appropriate configuration values. Run the project by running `dotnet run` in the command line from `Client/src/`.

# Generating and Using Signed JWT for Client Authentication with Keycloak

Keycloak can generate a keystore.jks file containing the private key for the client application.
It can also accept the upload of a JKS or PFX certificate or PEM for validating the signed JWT provided by the client during authentication. You can also have Keycloak point to a Url where you keep your public keys, in a JWKS file, and then you are responsible for generating/maintaining your certificates and their corresponding public/private key pairs. This might be attractive for all to minimize configurations needed at Keycloak.

In this example client, the Keycloak administrator provides us with a keystore.jks file and its password.  We need to convert this file to a pfx certificate file to allow us to sign the JWT.

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

## Getting the public key from your PFX file (Optional)

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

If you paste your base64 encoded Signed Jwt into https://jwt.io, you can then paste in the contents of your public key file to verify that your Jwt is properly signed. This is what Keycloak will do to verify that you signed your Jwt.

# Warning

The above method of using appsettings.json to store the private key is not recommended for production.
You will want to store your private key in a vault or secure area, and retrieve it in code to sign the JWT.
