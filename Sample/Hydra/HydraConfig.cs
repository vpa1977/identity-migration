namespace Sample.Hydra
{
    public class HydraConfig
    {
        public string PublicUrl { get; set;  } = "http://localhost:4444";
        public string AdminUrl { get; set; } = "http://localhost:4445";
        public string Secret { get; set; } = "secret";
        public string ClientID { get; set; } = "Sample";
        public string Scope { get; set; } = "SampleAPI openid profile";
        public string RedirectUri { get; set; } = "https://localhost:44416/authentication/login-callback";
        public string PostLogoutRedirectUri { get; set; } = "https://localhost:44416/";

    }
}