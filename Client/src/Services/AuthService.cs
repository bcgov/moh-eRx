//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace PharmaNet.Client.Services
{
    using System;

    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IdentityModel.Tokens;
    using System.IO;

    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using PharmaNet.Client.Models;

    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
          Convert.FromBase64String(value);
    }

    public class AuthService : IAuthService
    {
        private OpenIdConnectConfig oidcConfig;
        private readonly IConfiguration configuration;

        private readonly HttpClient client = new HttpClient();

        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
            oidcConfig = new OpenIdConnectConfig();
            this.configuration.Bind(OpenIdConnectConfig.ConfigSectionName, oidcConfig);
        }
        public string AuthenticateUsingClientCredentials()
        {
            Task<string> result = this.ClientCredentialsGrant();
            return result.Result;
        }

        public string AuthenticateUsingSignedJWT()
        {
            Task<string> result = this.SignedJwtAssertion();
            return result.Result;
        }

        private async Task<string> SignedJwtAssertion()
        {
            string accessTokenResponse = string.Empty;

            // 1. Fetch Json response from OpenID Connect Discovery Endpoint, aka well-known endpoint of the Authority (Keycloak IAM)
            //   We need this to obtain the Token Endpoint Value  used as the 'aud' or Audience of the JWT.
            string tokenUrl = this.GetTokenEndpoint(oidcConfig.Authority);

            // 2. "Signed Jwt" - Using the pfx file specified in the app settings config, create a signed Json Web Token.
            JwtResponse jwtResponse = this.CreateSignedJsonWebToken(oidcConfig.ClientId, tokenUrl);

            // 3. Now using OIDC client assertion direct flow, authenticate using the SignedJWT as the client credential.
            try
            {
                IEnumerable<KeyValuePair<string, string>> oauthParams = new[]
                    {
                    new KeyValuePair<string, string>(@"client_id", oidcConfig.ClientId),
                    new KeyValuePair<string, string>(@"client_secret", oidcConfig.ClientSecret),
                    new KeyValuePair<string, string>(@"grant_type", @"client_credentials"),
                    new KeyValuePair<string, string>(@"client_assertion", jwtResponse.Token), // the signed JWT
                    new KeyValuePair<string, string>(@"audience", oidcConfig.Audience),
                    new KeyValuePair<string, string>(@"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                    new KeyValuePair<string, string>(@"scope", oidcConfig.Scope),
                };
                using var content = new FormUrlEncodedContent(oauthParams);
                content.Headers.Clear();
                content.Headers.Add(@"Content-Type", @"application/x-www-form-urlencoded");

                using HttpResponseMessage response = await client.PostAsync(tokenUrl, content).ConfigureAwait(true);

                accessTokenResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                Console.WriteLine($"JWT Token response: {accessTokenResponse}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message {e.Message}");
            }

            // 4. Return the Access Token for subsequent use as the Bearer Token for Pharmanet API Calls.
            return accessTokenResponse;
        }

        private async Task<string> ClientCredentialsGrant()
        {
            string accessTokenResponse = string.Empty;

            string tokenUrl = this.GetTokenEndpoint(oidcConfig.Authority);
            try
            {
                IEnumerable<KeyValuePair<string, string>> oauthParams = new[]
                {
                    new KeyValuePair<string, string>(@"client_id", oidcConfig.ClientId),
                    new KeyValuePair<string, string>(@"client_secret", oidcConfig.ClientSecret),
                    new KeyValuePair<string, string>(@"audience", oidcConfig.Audience),
                    new KeyValuePair<string, string>(@"scope", oidcConfig.Scope),
                    new KeyValuePair<string, string>(@"grant_type", @"client_credentials"),
                };
                using var content = new FormUrlEncodedContent(oauthParams);
                content.Headers.Clear();
                content.Headers.Add(@"Content-Type", @"application/x-www-form-urlencoded");

                using HttpResponseMessage response = await client.PostAsync(tokenUrl, content).ConfigureAwait(true);

                accessTokenResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                Console.WriteLine($"JWT Token response: {accessTokenResponse}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message {e.Message}");
            }

            return accessTokenResponse;
        }


        private JwtResponse CreateSignedJsonWebToken(string clientId, string tokenUrl, string clientSecret = null)
        {
            SigningCredentials signingCredentials;

            if (clientSecret != null)
            {
                //  This is when Keycloak client Credentials are configured as "Signed Jwt with Client Secret"
                // HS256 symmetric algorithm with shared secret (the client_secret) for this client_id
                Byte[] keyBytes = clientSecret.ToByteArray();

                var symmetricKey = new SymmetricSecurityKey(keyBytes);
                signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

            }
            else
            {
                // This is when the Keycloak client credentials are configured as "Signed Jwt"
                // We use asymmetric key with public/private key pair from certificate.
                RSA rsa = this.GetRSAFromPfxCertificate();
                RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(rsa);
                signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
            }

            DateTime now = DateTime.Now;
            long unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: oidcConfig.ClientId,
                expires: now.AddMinutes(15),
                signingCredentials: signingCredentials,
                claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Iss, oidcConfig.ClientId),
                    new Claim(JwtRegisteredClaimNames.Sub, oidcConfig.ClientId),
                    new Claim(JwtRegisteredClaimNames.Aud, tokenUrl),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                }
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtResponse
            {
                Token = token,
                ExpiresAt = unixTimeSeconds,
            };
        }

        private RSA GetRSAFromPfxCertificate()
        {
            JwtSigningConfig jwtConfig = new JwtSigningConfig();
            this.configuration.Bind(JwtSigningConfig.ConfigSectionName, jwtConfig);

            X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable;
            X509Certificate2 cert = new X509Certificate2(jwtConfig.CertificatePfxFile, jwtConfig.CertificatePassword, flags);
            if (cert.HasPrivateKey)
            {
                RSA rsa = (RSA)cert.GetRSAPrivateKey();
                return rsa;
            }
            else
            {
                Console.WriteLine("Certificate used has no private key.");
            }
            return null;
        }

        private Byte[] GetKeyFromCert()
        {
            JwtSigningConfig jwtConfig = new JwtSigningConfig();
            this.configuration.Bind(JwtSigningConfig.ConfigSectionName, jwtConfig);

            X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable;
            X509Certificate2 cert = new X509Certificate2(jwtConfig.CertificatePfxFile, jwtConfig.CertificatePassword, flags);
            if (cert.HasPrivateKey)
            {

                Byte[] keyBytes = cert.PrivateKey.ExportPkcs8PrivateKey();
                return keyBytes;

            }
            else
            {
                Console.WriteLine("Certificate used has no private key.");
            }
            return null;
        }

        private string GetTokenEndpoint(string authorityUrl)
        {
            try
            {
                string wkEndPointPath = "/.well-known/openid-configuration";
                Task<string> task = Task.Run<string>(async () => await client.GetStringAsync(authorityUrl + wkEndPointPath));
                OidcConfiguration config = JsonSerializer.Deserialize<OidcConfiguration>(task.Result);
                return config.token_endpoint;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return "";
            }

        }

    }
}