using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureML.OnlineEndpoints.RecipeFunction
{
    public static class ScaleInRecipe
    {
        [FunctionName("ScaleInRecipe")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest recipeRequest,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Scale in recipe function processed a request.");

            string responseMessage = "Scale in recipe function started but did not scale down instances.";
            try
            {
                var config = ConfigurationHelper.GetConfiguration(log);

                var alert = await ValidateHelper.ValidateRequest(recipeRequest, log);
                
                // Get target azure resource id
                var targetResourceId = alert.Data.Essentials.AlertTargetIDs.FirstOrDefault();
                log.LogInformation(targetResourceId);

                // Get online deployment resource
                var onlineDeployment = await OnlineEndpointsHelper.GetDeploymentResource(config, targetResourceId, log);

                // Check settings to verify scale in
                var invokeScaleIn = VerifySettings(config, onlineDeployment, log);
                if (invokeScaleIn)
                {              
                    var scaleInStep = Convert.ToInt32(config["ScalingPolicy:ScaleInStep"]);
                    onlineDeployment.properties.scaleSettings.instanceCount = currentInstances - scaleInStep;

                    await OnlineEndpointsHelper.UpdateDeploymentResource(config, targetResourceId, onlineDeployment, log);
                    responseMessage = "Scale in recipe function succeeded. Decremented instance count by " + scaleInStep;
                }
            }
            catch(Exception ex)
            {
                responseMessage = string.Concat("Scale in recipe function failed. ", ex.Message);

                log.LogInformation("Failed to process a request with exception ", ex.ToString());
            }

            return new OkObjectResult(responseMessage);
        }
        
        public static bool VerifySettings(IConfiguration config, OnlineDeployment onlineDeployment, ILogger log)
        {   
            bool invokeScaleIn = false;

            var minInstances = onlineDeployment.properties.scaleSettings.Minimum;
            var currentInstances = onlineDeployment.properties.scaleSettings.instanceCount;

            if (string.IsNullOrWhiteSpace(minInstances))
            {
                log.LogInformation("Cannot scale in. Minimum instances on deployment scale settings is not set.");
            }
            else if (currentInstances <= Convert.ToInt32(minInstances))
            {
                log.LogInformation("Cannot scale in. Current instances already reached minimum instances.");
            }
            else if (string.IsNullOrWhiteSpace(config["ScalingPolicy:ScaleInStep"]))
            {
                log.LogInformation("Cannot scale in. Scale in step is not specified in config.");
            }
            else if (!string.IsNullOrWhiteSpace(config["ScalingPolicy:MinScale"]))
            {
                var minInstancesConfig = Convert.ToInt32(config["ScalingPolicy:MinScale"]);
                if (currentInstances <= minInstancesConfig)
                {
                    log.LogInformation("Cannot scale in. Current instances are at minimum instances as specified in config.");
                }
            }
            else
            {
                invokeScaleIn = true;
            }

            return invokeScaleIn;
        }
    }
}
