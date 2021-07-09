namespace PharmaNet.Client.Models
{

    public class OpenIdConnectConfig
    {
        public static string ConfigSectionName = "OpenIdConnect";

        public string Audience { get; set; }
        public string Authority { get; set; }
        public string Issuer { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}