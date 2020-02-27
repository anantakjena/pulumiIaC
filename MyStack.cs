using System;
using Pulumi;
using Azure = Pulumi.Azure;

class MyStack : Stack
{
    public MyStack()
    {
        var resourceGroup = new Azure.Core.ResourceGroup("testResourceGroup",
                new Azure.Core.ResourceGroupArgs { Location = "ukwest" });

        var serviceBusNamespace = new Azure.ServiceBus.Namespace("testServiceBusNamespace", new Azure.ServiceBus.NamespaceArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = "Basic"
        });

        var messageQueue = new Azure.ServiceBus.Queue("testMessageQueue", new Azure.ServiceBus.QueueArgs
        {
            ResourceGroupName = resourceGroup.Name,
            NamespaceName = serviceBusNamespace.Name,
            MaxSizeInMegabytes = 1024,
            EnablePartitioning = false,
            DefaultMessageTtl = System.Xml.XmlConvert.ToString(TimeSpan.FromSeconds(30))
        });

        var appServicePlan = new Azure.AppService.Plan("testConsumptionPlan", new Azure.AppService.PlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "FunctionApp",
            Sku = new Azure.AppService.Inputs.PlanSkuArgs
            {
                Tier = "Dynamic",
                Size = "Y1"
            }
        });

        var functionStorage = new Azure.Storage.Account("testFuncStorage", new Azure.Storage.AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountReplicationType = "LRS",
            AccountTier = "Standard"
        });

        var appInsights = new Azure.AppInsights.Insights("testAppInsights", new Azure.AppInsights.InsightsArgs
        {
            ResourceGroupName = resourceGroup.Name,
            RetentionInDays = 30,
            ApplicationType = "web",
            Location = "uksouth"
        });

        var functionApp = new Azure.AppService.FunctionApp("testFunctionApp", new Azure.AppService.FunctionAppArgs
        {
            AppServicePlanId = appServicePlan.Id,
            ResourceGroupName = resourceGroup.Name,
            StorageConnectionString = functionStorage.PrimaryConnectionString,
            Version = "~3",
            AppSettings = { { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey } },
            ConnectionStrings = new Azure.AppService.Inputs.FunctionAppConnectionStringsArgs
            {
                Name = "ServiceBusConnection",
                Value = serviceBusNamespace.DefaultPrimaryConnectionString,
                Type = "ServiceBus"
            }
        });
    }
}