
namespace Microsoft.AzureML.OnlineEndpoints.RecipeFunction
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The common alert schema to monitor Online Inferencing Endpoint.
    /// </summary>
    public class CommonAlertSchema
    {
        /// <summary>
        /// The common alert schema.
        /// https://docs.microsoft.com/en-us/azure/azure-monitor/platform/alerts-common-schema-definitions
        /// </summary>
        [JsonProperty(PropertyName = "schemaId")]
        [Required]
        public string SchemaId { get; set; }

        /// <summary>
        /// The common alert schema details.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        [Required]
        public CommonAlertSchemaData Data { get; set; }
    }
    
    public class CommonAlertSchemaData
    {
        /// <summary>
        /// A set of standardized fields, which describe what resource the alert is on with metadata.
        /// </summary>
        [JsonProperty(PropertyName = "essentials")]
        [Required]
        public CommonAlertEssentials Essentials { get; set; }
        
        /// <summary>
        /// A set of fields that describes the cause of the alert, with fields that vary based on the alert type.
        /// </summary>
        [JsonProperty(PropertyName = "alertContext")]
        [Required]
        public CommonAlertContext AlertContext { get; set; }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Severity
    {
        Sev0,
        Sev1,
        Sev2,
        Sev3,
        Sev4
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SignalType
    {
        Metric,
        Log,
        ActivityLog
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonitorCondition
    {
        Fired,
        Resolved
    }

    public class CommonAlertEssentials
    {
        /// <summary>
        /// The GUID uniquely identifying the alert instance
        /// </summary>
        [JsonProperty(PropertyName = "alertId")]
        [Required]
        public string AlertId { get; set; }

        /// <summary>
        /// The name of the alert rule that generated the alert instance.
        /// </summary>
        [JsonProperty(PropertyName = "alertRule")]
        [Required]
        public string AlertRule { get; set; }
        
        /// <summary>
        /// The severity of the alert. Possible values: Sev0, Sev1, Sev2, Sev3, or Sev4.
        /// </summary>
        [JsonProperty(PropertyName = "severity")]
        [Required]
        public Severity Severity { get; set; }
        
        /// <summary>
        /// Identifies the signal on which the alert rule was defined. Possible values: Metric, Log, or Activity Log.
        /// </summary>
        [JsonProperty(PropertyName = "signalType")]
        [Required]
        public SignalType SignalType { get; set; }
        
        /// <summary>
        /// When an alert fires, the alert's monitor condition is set to Fired. When the 
        /// underlying condition that caused the alert to fire clears, the monitor condition is set to Resolved.
        /// </summary>
        [JsonProperty(PropertyName = "monitorCondition")]
        [Required]
        public MonitorCondition MonitorCondition { get; set; }

        /// <summary>
        /// The monitoring service or solution that generated the alert.
        /// </summary>
        [JsonProperty(PropertyName = "monitoringService")]
        [Required]
        public string MonitoringService { get; set; }

        /// <summary>
        /// The list of the Azure Resource Manager IDs that are affected targets of an alert.
        /// </summary>
        [JsonProperty(PropertyName = "alertTargetIDs")]
        [Required]
        public string[] AlertTargetIDs { get; set; }

        /// <summary>
        /// The date and time when the alert instance was fired in UTC
        /// </summary>
        [JsonProperty(PropertyName = "firedDateTime")]
        [Required]
        public DateTime FiredDateTime { get; set; }

        /// <summary>
        /// The date and time when the monitor condition for the alert instance is set to Resolved in UTC
        /// </summary>
        [JsonProperty(PropertyName = "resolvedDateTime")]
        [Required]
        public DateTime ResolvedDateTime { get; set; }
    }
    
    public class CommonAlertContext
    {
        /// <summary>
        /// Alert properties
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        [Required]
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Condition Type
        /// </summary>
        [JsonProperty(PropertyName = "conditionType")]
        [Required]
        public string ConditionType { get; set; }
        
        /// <summary>
        /// Alert condition
        /// </summary>
        [JsonProperty(PropertyName = "condition")]
        [Required]
        public AlertCondition Condition { get; set; }
    }
    
    public class AlertCondition
    {
        /// <summary>
        /// Condition Window Size
        /// </summary>
        [JsonProperty(PropertyName = "windowSize")]
        [Required]
        public string WindowSize { get; set; }
        
        /// <summary>
        /// Alert condition details
        /// </summary>
        [JsonProperty(PropertyName = "allOf")]
        [Required]
        public Metric[] AllOf { get; set; }
        
        /// <summary>
        /// Window StartTime
        /// </summary>
        [JsonProperty(PropertyName = "windowStartTime")]
        [Required]
        public DateTime WindowStartTime { get; set; }
        
        /// <summary>
        /// Window EndTime
        /// </summary>
        [JsonProperty(PropertyName = "windowEndTime")]
        [Required]
        public DateTime WindowEndTime { get; set; }
    }
    
    public class Metric
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "metricName")]
        [Required]
        public string MetricName { get; set; }

        /// <summary>
        /// Metric Namespace
        /// </summary>
        [JsonProperty(PropertyName = "metricNamespace")]
        [Required]
        public string MetricNamespace { get; set; }
        
        /// <summary>
        /// Metric Operator
        /// </summary>
        [JsonProperty(PropertyName = "operator")]
        [Required]
        public string Operator { get; set; }

        /// <summary>
        /// Metric Threshold
        /// </summary>
        [JsonProperty(PropertyName = "threshold")]
        [Required]
        public string Threshold { get; set; }

        /// <summary>
        /// Metric Time Aggregation
        /// </summary>
        [JsonProperty(PropertyName = "timeAggregation")]
        [Required]
        public string TimeAggregation { get; set; }
        
        /// <summary>
        /// Metric Value
        /// </summary>
        [JsonProperty(PropertyName = "metricValue")]
        [Required]
        public string MetricValue { get; set; }
        
        /// <summary>
        /// Metric Dimensions
        /// </summary>
        [JsonProperty(PropertyName = "dimensions")]
        [Required]
        public MetricDimension[] Dimensions { get; set; }
    }
    
    public class MetricDimension
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        [Required]
        public string Value { get; set; }
    }
}