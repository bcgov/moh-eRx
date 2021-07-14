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

        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Authenticate()
        {
            return string.Empty;

        }

        public string AuthenticateUsingSignedJWT()
        {
            OpenIdConnectConfig oidcConfig = new OpenIdConnectConfig();
            this.configuration.Bind(OpenIdConnectConfig.ConfigSectionName, oidcConfig);

            // 1. Using the pfx file configured, create a signed Json Web Token.
            JwtResponse jwtResponse = this.CreateSignedJsonWebToken(oidcConfig.Authority, oidcConfig.Audience, oidcConfig.ClientId);

            // 2. Now using OIDC flow, authenticate using the SignedJWT as the client credential.

            // 3. Return the Access Token for subsequent use as the Bearer Token for Pharmanet API Calls.
            return string.Empty;
        }
        

        private JwtResponse CreateSignedJsonWebToken(string issuer, string audience, string subject)
        {
            RSACryptoServiceProvider rsaCryptoSP = this.GetRSAFromPfxCertificate();
            using RSA rsa = RSA.Create();

            rsa.ImportRSAPrivateKey(rsaCryptoSP.ExportRSAPrivateKey(), out _);

            SigningCredentials signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            DateTime now = DateTime.Now;
            long unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: issuer,
                expires: now.AddMinutes(15),
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

        private RSACryptoServiceProvider GetRSAFromPfxCertificate()
        {
            JwtSigningConfig jwtConfig = new JwtSigningConfig();
            this.configuration.Bind(JwtSigningConfig.ConfigSectionName, jwtConfig);

            X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable;
            X509Certificate2 cert = new X509Certificate2( jwtConfig.CertifcatePfxFile, jwtConfig.CertificatePassword, flags);

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            return rsa;
        }
    }

}