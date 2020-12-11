namespace Microsoft.AzureML.OnlineEndpoints.RecipeFunction
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    
    /// <summary>
    /// Validates the HTTPTrigger request and common alert schema.
    /// </summary>
    public class ValidateHelper
    {
        private const string EmptyOrInvalidPayload = "The request payload was either empty or invalid. Try again with a well-formed common alert schema in the query string or in the request body.";
        private const string InvalidPayload = "The request payload is invalid. The parameter {0} has invalid value {1}.";

        public static async Task<CommonAlertSchema> ValidateRequest(HttpRequest recipeRequest, ILogger log)
        {
            if (recipeRequest == null)
            {
                throw new Exception(EmptyOrInvalidPayload);
            }

            string requestBody = await new StreamReader(recipeRequest.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            var alert = JsonConvert.DeserializeObject<CommonAlertSchema>(requestBody);

            ValidateCommonParameters(alert);
            
            return alert;
        }

        private static void ValidateCommonParameters(CommonAlertSchema alert)
        {
            if (alert == null || alert.Data == null || alert.Data.Essentials == null || alert.Data.AlertContext == null)
            {
                throw new Exception(EmptyOrInvalidPayload);
            }

            string schemaId = alert?.SchemaId;
            if (string.IsNullOrEmpty(schemaId) || string.Compare(schemaId, "azureMonitorCommonAlertSchema", true) != 0)
            {
                throw new Exception(string.Format(InvalidPayload, "SchemaId", schemaId));
            }

            if (alert.Data.Essentials.SignalType != SignalType.Metric)
            {
                throw new Exception(string.Format(InvalidPayload, "SignalType", alert.Data.Essentials.SignalType.ToString()));
            }
            
            // For metric alerts the monitoring service is set to Platform
            var service = alert.Data.Essentials.MonitoringService;
            if (string.IsNullOrEmpty(service) || string.Compare(service, "Platform", true) != 0)
            {
                throw new Exception(string.Format(InvalidPayload, "MonitoringService", service));
            }
        }

    }
}