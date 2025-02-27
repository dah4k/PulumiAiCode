// Get Azure existing subnet
// https://www.pulumi.com/ai/conversations/f2636bcb-c967-4900-a1e8-bc3f475b962f

using Pulumi;
using Pulumi.AzureNative.Network;
using System.Collections.Generic;

return await Deployment.RunAsync(async () =>
{
    var subnet = await GetSubnet.InvokeAsync(new GetSubnetArgs
    {
        ResourceGroupName = "your-resource-group-name",
        VirtualNetworkName = "your-vnet-name",
        SubnetName = "your-subnet-name",
    });

    return new Dictionary<string, object?>
    {
        ["subnetId"] = subnet.Id,
        ["subnetAddressPrefix"] = subnet.AddressPrefix
    };
});
