using Facebook;
using FacebookGroupArchiver.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    /// <summary>
    /// Retrieves user's facebook authentication token
    /// </summary>
    public class FaceBookAuthentication
    {
        /// <summary>
        /// Retrieves user's facebook authentication token
        /// </summary>
        /// <param name="configManager">An instance if IConfigurationManager interface</param>
        /// <returns>User extended token</returns>
        public async Task<string> RetrieveUserToken(IConfigurationManager configManager)
        {
            string savedToken = configManager.ReadSetting<string>("token");
            if (!String.IsNullOrEmpty(savedToken))
            {
                if(await TokenIsValid(savedToken))
                {
                    return savedToken;
                }
            }
            Uri url = await RetrieveAuthenticationURL(configManager);
            string token = await RetrieveShortToken(configManager, url);
            string extendedToken = await ExtendToken(configManager, token);
            configManager.WriteSetting<string>("token", extendedToken);
            return extendedToken;
        }

        /// <summary>
        /// Checks if an authentication token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the token is valid, false otherwise</returns>
        public async Task<bool> TokenIsValid(string token)
        {
            FacebookClient client = new FacebookClient(token);
            try
            {
                await client.GetTaskAsync("/me");
            }
            catch (FacebookOAuthException)
            {
                return false;
            }
            return true;
        }
        private async Task<Uri> RetrieveAuthenticationURL(IConfigurationManager configManager)
        {
            return await Task.Run(() => { 
            ParametersBuilder paramBuilder = new ParametersBuilder(configManager);
            dynamic parameters = paramBuilder
                .AddClientId()
                .AddRedirectUri()
                .AddCustom("response_type", "token")
                .AddCustom("type", "web_server")
                .AddCustom("scope", "user_groups")
                .Build();
            var fb = new FacebookClient();
            Uri url = fb.GetLoginUrl(parameters);
            return url;
            });
        }

        private async Task<string> RetrieveShortToken(IConfigurationManager configManager, Uri url)
        {
            //Start code listener and open Facebook's authentication page after the listener has been started
            string code = await new HttpTokenRetriever().StartCodeListener(configManager, new Action(() => { SendTokenRequest(url.ToString()); }));
           
            FacebookClient fb = new FacebookClient();
            ParametersBuilder paramBuilder = new ParametersBuilder(configManager);
            dynamic parameters = paramBuilder
                .AddClientId()
                .AddRedirectUri()
                .AddClientSecret()
                .AddCustom("code", code)
                .Build();
            var response =await fb.GetTaskAsync("oauth/access_token", parameters);
            string token = response.access_token;
            return token;
            
        }

        private async Task<string> ExtendToken(IConfigurationManager configManager, string oldToken)
        {
            var fb = new FacebookClient();
            ParametersBuilder paramBuilder = new ParametersBuilder(configManager);
            dynamic parameters = paramBuilder
                .AddClientId()
                .AddClientSecret()
                .AddCustom("grant_type", "fb_exchange_token")
                .AddCustom("fb_exchange_token", oldToken)
                .Build();
            dynamic result = await fb.GetTaskAsync("oauth/access_token", parameters);
            return result.access_token;

        }

        private void SendTokenRequest(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    
        /// <summary>
        /// Fluent builder pattern to construct Facebook client's parameters
        /// </summary>
        private class ParametersBuilder
        {
            IConfigurationManager _configManager;
            dynamic _parameters = new ExpandoObject();
            public ParametersBuilder(IConfigurationManager configManager)
            {
                _configManager = configManager;
            }

            public ParametersBuilder AddClientId()
            {
                _parameters.client_id = _configManager.ReadSetting<string>("clientId");
                return this;
            }

            public ParametersBuilder AddRedirectUri()
            {
                _parameters.redirect_uri = _configManager.ReadSetting<string>("redirectUri");
                return this;
            }

            public ParametersBuilder AddClientSecret()
            {
                _parameters.client_secret = _configManager.ReadSetting<string>("clientSecret");
                return this;
            }

            public ParametersBuilder AddCustom(string name,string value)
            {
                var paramsDict = _parameters as IDictionary<string, object>;
                paramsDict.Add(name, value);
                _parameters = paramsDict;
                return this;
            }

            public dynamic Build()
            {
                return _parameters;
            }
        }
    }
}
