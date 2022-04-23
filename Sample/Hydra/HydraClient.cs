using Microsoft.Extensions.Options;
using Ory.Hydra.Client.Model;

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
                ResponseTypes =new() { "code", "id_token"}, 
                RedirectUris = new() { _config.RedirectUri }, 
                PostLogoutRedirectUris = new() {  _config.PostLogoutRedirectUri}
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
    }
}
