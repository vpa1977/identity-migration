using Microsoft.Extensions.Options;

namespace Sample.Hydra
{
    // return some hardcoded values that are relevant for the sample
    public class OIDCConfigurationService
    {
        private HydraConfig _config;

        public OIDCConfigurationService(IOptions<HydraConfig> config)
        {
            _config = config.Value;
        }

        public IDictionary<string, string> GetClientParameters(HttpContext context, string clientId)
        {
            return new Dictionary<string, string>
            {
                ["authority"] = $"{_config.PublicUrl}",
                ["client_id"] = _config.ClientID,
                ["redirect_uri"] = $"https://{context.Request.Host.Value}/authentication/login-callback",
                ["post_logout_redirect_uri"] = $"https://{context.Request.Host.Value}/authentication/logout-callback",
                ["response_type"] = "code",
                ["scope"] = _config.Scope
            };
        }
    }
}
