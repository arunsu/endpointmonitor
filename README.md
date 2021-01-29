# ML App Deployment Monitoring

## Alert creation
From the Azure portal go to the ML App Deployment resource and perform following steps.

* In the ML App Deployment resource page select Alerts.
* Click on + New alert rule 
* Check the Resource is selected otherwise select a ML App Deployment resource.
* Add a condition 
* In the signals list, select the Signal type as “Metrics” 
* Select the metric you want to set an alert on.
* In case your metrics expose dimensions, click on each metric to see all the dimensions that the metric has. Select the dimension. 
* Set a threshold to make sure the alert will fire. 
* Add an action group to trigger the scale in or scale out Azure function.
* Select "Actions" tab, set "Action typeL" as "Azure Function" and "Name" as "ScleInRecipe".  
* Specify the alert details and click on Create alert rule

## Azure Functions
The template has to azure functions to scale out number of instances and to scale in number of instance on an ML App Deployment.

* Create an Azure Function App
* Deploy the template to create ScaleInRecipe and ScaleOutRecipe Azure functions.

## Recipe Settings
Tune the following settings to control when the scale out or scale in step size.

```
"ScalingPolicy": {
      "MinScale": 2,
      "MaxScale": 8,
      "ScaleOutStep": 1,
      "ScaleInStep": 3
    }