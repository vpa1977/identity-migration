using Microsoft.Extensions.Options;

namespace Sample.Hydra
{
    public class HydraClient
    {
        private Ory.Hydra.Client.Api.AdminApi _admin;
        private HydraConfig _config;
        public HydraClient(IOptions<HydraConfig> config) 
        {
             _admin = new(config.Value.AdminUrl);
            _config = config.Value;
        }

        public void CreateOpenIdClient()
        {

           var resp = _admin.CreateOAuth2Client(new Ory.Hydra.Client.Model.HydraOAuth2Client()
            {
                ClientId = _config.ClientID,
                Scope = _config.Scope, 
                TokenEndpointAuthMethod = "none", 
                GrantTypes = new() { "authorization_code"}, 
                ResponseTypes =new() { "code", "id_token"}
            });
        }

        public void DeleteOpenIdClient()
        {
            _admin.DeleteOAuth2Client(_config.ClientID);
        }
    }
}
