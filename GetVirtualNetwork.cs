// Get Azure existing VirtualNetwork
// https://www.pulumi.com/ai/conversations/b67e1130-154b-4fbe-8f1c-58d5adca06cf

using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using System.Threading.Tasks;

return await Deployment.RunAsync(async () =>
{
    var resourceGroupName = "example-resource-group";
    var virtualNetworkName = "example-vnet";

    var vnet = await GetVirtualNetwork.InvokeAsync(new GetVirtualNetworkArgs
    {
        ResourceGroupName = resourceGroupName,
        VirtualNetworkName = virtualNetworkName
    });

    return new Dictionary<string, object?>
    {
        ["virtualNetworkId"] = vnet.Id,
        ["addressSpace"] = vnet.AddressSpace
    };
});
