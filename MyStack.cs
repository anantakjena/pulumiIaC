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
    }
}