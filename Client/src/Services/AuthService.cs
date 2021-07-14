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
        private readonly IConfiguration configuration;

        private readonly HttpClient client = new HttpClient();

        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string AuthenticateUsingClientCredentials()
        {
            return string.Empty;

        }

        public string AuthenticateUsingSignedJWT()
        {
            OpenIdConnectConfig oidcConfig = new OpenIdConnectConfig();
            this.configuration.Bind(OpenIdConnectConfig.ConfigSectionName, oidcConfig);

            // 1. Fetch Json response from OpenID Connect Discovery Endpoint, aka well-known endpoint of the Authority (Keycloak IAM)
            //   We need this to obtain the Token Endpoint Value  used as the 'aud' or Audience of the JWT.
           string audience = this.GetTokenEndpoint(oidcConfig.Authority);

            // 2. Using the pfx file configured, create a signed Json Web Token.
            JwtResponse jwtResponse = this.CreateSignedJsonWebToken(oidcConfig.Authority, audience, oidcConfig.ClientId);

            // 2. Now using OIDC flow, authenticate using the SignedJWT as the client credential.
            //Task<string> task =  Task.Run<string>(async() => await 


            // 3. Return the Access Token for subsequent use as the Bearer Token for Pharmanet API Calls.
            return string.Empty;
        }


        private JwtResponse CreateSignedJsonWebToken(string issuer, string audience, string subject)
        {
            RSA rsa = this.GetRSAFromPfxCertificate();

            SigningCredentials signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            DateTime now = DateTime.Now;
            long unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: issuer,
                expires: now.AddMinutes(15),
                signingCredentials: signingCredentials,
                claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Nbf, unixTimeSeconds.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, subject),
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, audience),
                    new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            else {
                Console.WriteLine("Certificate used has no private key.");
            }
            return null;
        }

        private string GetTokenEndpoint(string authorityUrl)
        {
            try
            {
                string wkEndPointPath = "/.well-known/openid-configuration";
                Task<string> task =  Task.Run<string>(async() => await client.GetStringAsync(authorityUrl + wkEndPointPath));
                OidcConfiguration config =  JsonSerializer.Deserialize<OidcConfiguration>(task.Result);
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