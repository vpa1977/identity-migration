namespace Sample.Hydra
{
    public class HydraConfig
    {
        public string AdminUrl { get; set; } = "http://localhost:4445";
        public string Secret { get; set; } = "secret";
        public string ClientID { get; set; } = "Sample";
        public string Scope { get; set; } = "SampleAPI openid profile";
    }
}