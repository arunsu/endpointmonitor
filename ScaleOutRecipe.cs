using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AzureML.OnlineEndpoints.RecipeFunction
{
    public static class ScaleOutRecipe
    {
        [FunctionName("ScaleOutRecipe")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest recipeRequest,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Scale out recipe function processed a request.");

            string responseMessage = "Scale out recipe function started but did not scale instances.";
            try
            {
                var config = ConfigurationHelper.GetConfiguration(log);

                var alert = await ValidateHelper.ValidateRequest(recipeRequest, log);
                
                // Get target azure resource id
                var targetResourceId = alert.Data.Essentials.AlertTargetIDs.FirstOrDefault();
                log.LogInformation(targetResourceId);

                // Get online deployment resource
                var onlineDeployment = await OnlineEndpointsHelper.GetDeploymentResource(config, targetResourceId, log);

                // Check settings to verify scale out
                var invokeScaleOut = VerifySettings(config, onlineDeployment, log);
                if (invokeScaleOut)
                {              
                    var scaleOutStep = Convert.ToInt32(config["ScalingPolicy:ScaleOutStep"]);
                    onlineDeployment.properties.scaleSettings.instanceCount = currentInstances + scaleOutStep;

                    await OnlineEndpointsHelper.UpdateDeploymentResource(config, targetResourceId, onlineDeployment, log);
                    responseMessage = "Scale out recipe function succeeded. Incremented instance count by " + scaleOutStep;
                }
            }
            catch(Exception ex)
            {
                responseMessage = string.Concat("Scale out recipe function failed. ", ex.Message);

                log.LogInformation("Failed to process a request with exception ", ex.ToString());
            }

            return new OkObjectResult(responseMessage);
        }

        public static bool VerifySettings(IConfiguration config, OnlineDeployment onlineDeployment, ILogger log)
        {   
            bool invokeScaleOut = false;

            var maxInstances = onlineDeployment.properties.scaleSettings.Maximum;
            var currentInstances = onlineDeployment.properties.scaleSettings.instanceCount;

            if (string.IsNullOrWhiteSpace(maxInstances))
            {
                log.LogInformation("Cannot scale out. Maximum instances on deployment scale settings is not set.");
            }
            else if (currentInstances >= Convert.ToInt32(maxInstances))
            {
                log.LogInformation("Cannot scale out. Current instances already reached maximum available instances.");
            }
            else if (string.IsNullOrWhiteSpace(config["ScalingPolicy:ScaleOutStep"]))
            {
                log.LogInformation("Cannot scale out. Scale out step is not specified in config.");
            }
            else if (!string.IsNullOrWhiteSpace(config["ScalingPolicy:MaxScale"]))
            {
                var maxInstancesConfig = Convert.ToInt32(config["ScalingPolicy:MaxScale"]);
                if (currentInstances >= maxInstancesConfig)
                {
                    log.LogInformation("Cannot scale out. Current instances already reached maximum instances specified in config.");
                }
            }
            else
            {
                invokeScaleOut = true;
            }

            return invokeScaleOut;
        }
    }
}
