using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleOauthApp
{
    class Program
    {
        static void Main()
        {
            Uri authorizationServerTokenIssuerUri = new Uri("http://conta.dev.chequeespecial.container.btgpactual.net/");
            string clientId = "overdraft";
            string clientSecret = "overdraft";
            const string scope = "scope.readaccess";

            //access token request
            string rawJwtToken = RequestTokenToAuthorizationServerAsync(
                    authorizationServerTokenIssuerUri,
                    clientId,
                    scope,
                    clientSecret)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task<string> RequestTokenToAuthorizationServerAsync(Uri uriAuthorizationServer, string clientId, string scope, string clientSecret)
        {
            HttpClient client = new HttpClient();
            Uri baseUri = uriAuthorizationServer;
            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;

            //Post body content
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            var content = new FormUrlEncodedContent(values);

            var authenticationString = $"{clientId}:{clientSecret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authenticationString));
            content.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/oauth/token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            requestMessage.Content = content;

            //make the request
            var task = client.SendAsync(requestMessage);
            var response = await task.ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseBody;
        }
    }
}
