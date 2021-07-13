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

    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography;
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

            JwtResponse jwtResponse = this.CreateToken(oidcConfig.Issuer, oidcConfig.Audience, oidcConfig.ClientId);

            return string.Empty;
        }
        private JwtResponse CreateToken(string issuer, string audience, string subject)
        {
            JwtSigningConfig jwtConfig = new JwtSigningConfig();
            this.configuration.Bind(JwtSigningConfig.ConfigSectionName, jwtConfig);

            var privateKey = jwtConfig.RsaPrivateKey.ToByteArray();

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
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
    }

}