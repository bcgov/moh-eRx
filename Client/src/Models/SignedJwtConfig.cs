namespace PharmaNet.Client.Models
{
    public class JwtSigningConfig
    {
        public static string ConfigSectionName = "JwtSigning";

        public string RsaPrivateKey { get; set; }
        public string RsaPublicKey { get; set; }
    }
}