// Get Azure existing Resource Group
// https://www.pulumi.com/ai/conversations/5812c0af-5b14-415b-b689-87dc5b39896c

using Pulumi;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

return await Deployment.RunAsync(async () =>
{
    var existingResourceGroup = await GetResourceGroup.InvokeAsync(new GetResourceGroupArgs
    {
        ResourceGroupName = "my-existing-resource-group", // Replace with your resource group name
    });

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = existingResourceGroup.Name,
        ["location"] = existingResourceGroup.Location
    };
});
