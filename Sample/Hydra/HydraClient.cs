using Microsoft.Extensions.Options;
using Ory.Hydra.Client.Api;
using Ory.Hydra.Client.Model;

namespace Sample.Hydra
{
    public class HydraClient
    {
        private Ory.Hydra.Client.Api.AdminApi _admin;
        private PublicApi _pubApi;
        private HydraConfig _config;
        public HydraClient(IOptions<HydraConfig> config) 
        {
             _admin = new(config.Value.AdminUrl);

            _pubApi = new Ory.Hydra.Client.Api.PublicApi(config.Value.PublicUrl);
            _config = config.Value;
        }

        public void CreateOpenIdClient()
        {

            var resp = _admin.CreateOAuth2Client(new Ory.Hydra.Client.Model.HydraOAuth2Client()
            {
                ClientId = _config.ClientID,
                Scope = _config.Scope,
                ClientSecret = _config.Secret, // only for api tests
                TokenEndpointAuthMethod = "client_secret_post", // only for api tests
                GrantTypes = new() { "authorization_code", "client_credentials" }, // client_credentials for API tests
                ResponseTypes = new() { "code", "id_token" },
                RedirectUris = new() { _config.RedirectUri },
                PostLogoutRedirectUris = new() { _config.PostLogoutRedirectUri }
            }); 
        }

        public void DeleteOpenIdClient()
        {
            try
            {
                _admin.DeleteOAuth2Client(_config.ClientID);
            }
            catch { }
        }

        public string AcceptConsent(string consent_challenge, Dictionary<string, object> additionalData)
        {
            var consentRequest = _admin.GetConsentRequest(consent_challenge);
            var ret = _admin.AcceptConsentRequest(consent_challenge, new()
            {
                Remember = true, 
                Session = new HydraConsentRequestSession()
                {
                    AccessToken = additionalData
                },
                GrantAccessTokenAudience = consentRequest.RequestedAccessTokenAudience, 
                GrantScope = consentRequest.RequestedScope,
            });
            return ret.RedirectTo;
        }

        // returns redirect url to continue the flow
        public string AcceptLoginRequest(string challenge, string subject, Action<HydraAcceptLoginRequest> accept)
        {
            var res = new HydraAcceptLoginRequest( subject: subject);
            accept.Invoke(res);
            var resp = _admin.AcceptLoginRequest(challenge, res);
            return resp.RedirectTo;
        }

        public string RetrieveToken()
        {
            // USE command line api 
            /// docker exec -it hydra_hydra_1 hydra token client --endpoint http://localhost:4444 --client-id Sample --client-secret secret
            return null;
        }
    }
}
