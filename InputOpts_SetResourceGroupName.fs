// Create an azure-native.compute.VirtualMachine Domain Controller server on existing Resource Group MyRG no public IP in F#
// https://www.pulumi.com/ai/conversations/cd1060da-e016-4aab-814c-2a9f414241f7

open Pulumi
open Pulumi.AzureNative.Compute
open Pulumi.AzureNative.Resources
open Pulumi.AzureNative.Network
open Pulumi.AzureNative.Compute.Inputs
open Pulumi.AzureNative.Network.Inputs

let infra () =
    let resourceGroup = InputOpts().SetResourceGroupName("MyRG")

    let virtualNetwork = new VirtualNetwork("myVNet", VirtualNetworkArgs
        (
            ResourceGroupName = resourceGroup.Name,
            AddressSpace = AddressSpaceArgs ( AddressPrefixes = InputList [ "10.0.0.0/16" ] )
        ))

    let subnet = new Subnet("mySubnet", SubnetArgs
        (
            ResourceGroupName = resourceGroup.Name,
            VirtualNetworkName = virtualNetwork.Name,
            AddressPrefix = "10.0.1.0/24"
        ))

    let networkInterface = new NetworkInterface("myNIC", NetworkInterfaceArgs
        (
            ResourceGroupName = resourceGroup.Name,
            IpConfigurations = InputList [
                NetworkInterfaceIpConfigurationArgs
                (
                    Name = "myNICConfig",
                    Subnet = SubnetArgsId subnet.Id,
                    PrivateIpAddressAllocation = Models.IPAllocationMethod.Dynamic
                )
            ]
        ))

    let virtualMachine = new VirtualMachine("myVM", VirtualMachineArgs
        (
            ResourceGroupName = resourceGroup.Name,
            NetworkProfile = NetworkProfileArgs
            (
                NetworkInterfaces = InputList [
                    NetworkInterfaceReferenceArgs
                    (
                        Id = networkInterface.Id,
                        Primary = true
                    )
                ]
            ),
            HardwareProfile = HardwareProfileArgs
            (
                VmSize = "Standard_D2s_v3"
            ),
            OsProfile = OsProfileArgs
            (
                ComputerName = "mydomaincontroller",
                AdminUsername = "adminuser",
                AdminPassword = "P@ssw0rd!"
            ),
            StorageProfile = StorageProfileArgs
            (
                OsDisk = OsDiskArgs
                (
                    Name = "myOsDisk",
                    CreateOption = DiskCreateOptionTypes.FromImage,
                    ManagedDisk = ManagedDiskParametersArgs
                    ( StorageAccountType = "Standard_LRS" )
                ),
                ImageReference = ImageReferenceArgs
                (
                    Publisher = "MicrosoftWindowsServer",
                    Offer = "WindowsServer",
                    Sku = "2016-Datacenter",
                    Version = "latest"
                )
            )
        ))

    dict [ "vmName" ==> virtualMachine.Name ]

[<EntryPoint>]
let main _ =
    Deployment.Run infra |> ignore
    0
