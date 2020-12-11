# Endpoint Monitor Recipe
This is the alternate solution to Azure monitor autoscale. The number of instances for a deployment under an online endpoint can be scale up and scale out using alerts configured on the metrics for that deployment.

## Online endpoint deployment
Deployments are created under an online endpoint using Azure ML Sdk or CLI. As part of deployment specify the scale settings under which the service is operated.
```
    public class ScaleSettings
    {
        public enum ScaleTypeMode
        {
            Automatic,
            Manual,
            None
        }

        public int? Minimum { get; set; }

        public int? Maximum { get; set; }

        public int? InstanceCount { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ScaleTypeMode ScaleType { get; set; }
    }
```

## Azure functions
This repo have two azure functions which are used to scale out the current instances to the maximum with a specified increment step or scale in the current instance to the minimum with a specified decrement step.

### Azure function settings
Use the recipe.settings.json to configure the increment step for scale out or decrement step for scale in. You can also set the Maximum and Minimum to operate within the original scale settings.
```
    "ScalingPolicy": {
      "MinScale": 2,
      "MaxScale": 8,
      "ScaleOutStep": 1,
      "ScaleInStep": 3
    }
```

## Metrics and Alerts
Deployments are configured to emit the following metrics. Each metric can be aggregated over the specified time range to configure alerts.
```
  Cpu Utilization
  Deployment Capacity
  Disk Utilization
  Memory Utilization
```

### Alert template
This repo has an alert template that can be edited with your deployments and applied to the resource group.
