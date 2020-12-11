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
    /// Helper to load configuration settings.
    /// </summary>
    public class ConfigurationHelper
    {
        public static IConfiguraitonRoot GetConfiguration(ILogger log)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("recipe.settings.json", optional: false, reloadOnChange: true)
                    // .AddEnvironmentVariables()
                    .Build();

            log.LogInformation(JsonConvert.SerializeObject(SerializeConfig(config)));
        }
        
        private static JToken SerializeConfig(IConfiguration config)
        {
            JObject obj = new JObject();
            foreach (var child in config.GetChildren())
            {
                obj.Add(child.Key, SerializeConfig(child));
            }

            if (!obj.HasValues && config is IConfigurationSection section)
                return new JValue(section.Value);

            return obj;
        }
    }
}