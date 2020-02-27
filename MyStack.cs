using Pulumi;
using Azure = Pulumi.Azure;

class MyStack : Stack
{
    public MyStack()
    {
        var resourceGroup = new Azure.Core.ResourceGroup("testResourceGroup",
                new Azure.Core.ResourceGroupArgs {Location = "ukwest"});
    }
}