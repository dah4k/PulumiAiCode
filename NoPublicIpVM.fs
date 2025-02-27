// Create an azure-native.compute.VirtualMachine Domain Controller server no public IP in F#
// https://www.pulumi.com/ai/conversations/2ccf15e0-3749-4c86-a184-f700d8061490

open Pulumi
open Pulumi.AzureNative.Compute
open Pulumi.AzureNative.Network
open Pulumi.AzureNative.Resources

let infra () = async {
    let resourceGroup = ResourceGroup("resourceGroup")

    let virtualNetwork = VirtualNetwork("virtualNetwork",
        VirtualNetworkArgs(
            ResourceGroupName = resourceGroup.Name,
            AddressSpace = AddressSpaceArgs(AddressPrefixes = [|"10.0.0.0/16"|])
        ))

    let subnet = Subnet("subnet",
        SubnetArgs(
            ResourceGroupName = resourceGroup.Name,
            VirtualNetworkName = virtualNetwork.Name,
            AddressPrefix = "10.0.1.0/24"
        ))

    let networkInterface = NetworkInterface("networkInterface",
        NetworkInterfaceArgs(
            ResourceGroupName = resourceGroup.Name,
            IpConfigurations = [| NetworkInterfaceIpConfigurationArgs(
                Name = "internal",
                Subnet = SubnetArgs(Id = Output.Create(subnet.Id)),
                PrivateIpAddressAllocation = "Dynamic"
            ) |]
        ))

    let virtualMachine = VirtualMachine("virtualMachine",
        VirtualMachineArgs(
            ResourceGroupName = resourceGroup.Name,
            NetworkProfile = NetworkProfileArgs(
                NetworkInterfaces = [| NetworkInterfaceReferenceArgs(Id = networkInterface.Id) |]
            ),
            HardwareProfile = HardwareProfileArgs(VmSize = "Standard_B2s"),
            StorageProfile = StorageProfileArgs(
                OsDisk = OsDiskArgs(
                    CreateOption = "FromImage"
                ),
                ImageReference = ImageReferenceArgs(
                    Publisher = "MicrosoftWindowsServer",
                    Offer = "WindowsServer",
                    Sku = "2016-Datacenter",
                    Version = "latest"
                )
            ),
            OsProfile = OsProfileArgs(
                ComputerName = "hostname",
                AdminUsername = "adminuser",
                AdminPassword = "password"
            )
        ))

    // Export the resource group and VM IDs
    return dict [
        "resourceGroupName", resourceGroup.Name :> obj
        "virtualMachineId", virtualMachine.Id :> obj
    ]
}

Deployment.runAsync infra |> ignore
