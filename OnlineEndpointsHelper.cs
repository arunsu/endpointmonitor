using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.AzureML.OnlineEndpoints.RecipeFunction
{
    /// <summary>
    /// Online endpoint client to get and update deployments.
    /// </summary>
    public class OnlineEndpointsHelper
    {
        public static async Task<OnlineDeployment> GetDeploymentResource(IConfigurationRoot config, string targetResourceId, ILogger log)
        {
            using (var httpClient = new HttpClient())	
            {
                var accessToken = await GetToken(config, targetResourceId);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var resourceUri = GetResourceUri(config, targetResourceId);
                var responseBody = await httpClient.GetStringAsync(resourceUri);
                log.LogInformation(responseBody);

                var deployment = JsonConvert.DeserializeObject<OnlineDeployment>(responseBody);

                return deployment;
            }
        }

        public static async Task UpdateDeploymentResource(IConfigurationRoot config, string targetResourceId, OnlineDeployment deployment, ILogger log)
        {
            using (var httpClient = new HttpClient())	
            {
                var accessToken = await GetToken(config, targetResourceId);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var resourceUri = GetResourceUri(config, targetResourceId);
                var deploymentJson = JsonConvert.SerializeObject(deployment);

                var content = new StringContent(deploymentJson, Encoding.UTF8, "application/json");
                await httpClient.PutAsync(resourceUri, content);
            }
        }

        private static async Task<string> GetToken(IConfigurationRoot config, string targetResourceId)
        {
            // Read access token from config
            var accessToken = config["AuthToken"];

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                // Get access token
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(targetResourceId);
            }

            return accessToken;
        }

        private static string GetResourceUri(IConfigurationRoot config, string targetResourceId)
        {
            var armUri = config["ArmUri"];

            if (string.IsNullOrWhiteSpace(armUri))
            {
                armUri = "https://management.azure.com:443";
            }

            return string.Concat(armUri, "/", targetResourceId, "?api-version=2020-12-01-preview");
        }
    }
}
