// Create an azure-native.compute.VirtualMachine Domain Controller server in F#
// https://www.pulumi.com/ai/conversations/13fb300d-c234-4b22-9b07-84d2139c876b

open Pulumi
open Pulumi.AzureNative.Compute
open Pulumi.AzureNative.Compute.Inputs
open Pulumi.AzureNative.Network
open Pulumi.AzureNative.Network.Inputs
open Pulumi.AzureNative.Resources

let infra () =
    task {
        // Create a resource group
        let! resourceGroup = ResourceGroup("adResourceGroup")

        // Create a virtual network
        let! virtualNetwork = VirtualNetwork("adVnet",
                                   VirtualNetworkArgs(
                                       ResourceGroupName = input resourceGroup.Name,
                                       AddressSpaces = inputList [ "10.0.0.0/16" ]))

        // Create a subnet within the virtual network
        let! subnet = Subnet("adSubnet",
                             SubnetArgs(
                                 ResourceGroupName = input resourceGroup.Name,
                                 VirtualNetworkName = input virtualNetwork.Name,
                                 AddressPrefix = "10.0.1.0/24"))

        // Create a public IP address
        let! publicIPAddress = PublicIPAddress("adPublicIP",
                                               PublicIPAddressArgs(
                                                   ResourceGroupName = input resourceGroup.Name,
                                                   PublicIPAllocationMethod = input PublicIPAllocationMethod.Static))

        // Create a network interface
        let! networkInterface = NetworkInterface("adNic",
                                                 NetworkInterfaceArgs(
                                                     ResourceGroupName = input resourceGroup.Name,
                                                     IpConfigurations = inputList [
                                                         NetworkInterfaceIPConfigurationArgs(
                                                             Name = "adNicIPConfig",
                                                             Subnet = input subnet.GetResource().ToReference(),
                                                             PrivateIPAllocationMethod = input PrivateIPAllocationMethod.Dynamic,
                                                             PublicIPAddress = input publicIPAddress.GetResource().ToReference())
                                                     ]))

        // Define the storage profile
        let storageProfile = StorageProfileArgs(
            OsDisk = OSDiskArgs(
                Caching = input CachingTypes.ReadWrite,
                ManagedDisk = ManagedDiskParametersArgs(
                    StorageAccountType = StorageAccountTypes.Standard_LRS),
                Name = "adOsDisk",
                CreateOption = input DiskCreateOptionTypes.FromImage),
            ImageReference = ImageReferenceArgs(
                Publisher = "MicrosoftWindowsServer",
                Offer = "WindowsServer",
                Sku = "2019-Datacenter",
                Version = "latest"))

        // Define the OS profile
        let osProfile = OSProfileArgs(
            ComputerName = "adVM",
            AdminUsername = "admin-user",
            AdminPassword = input "P@ssw0rd!",
            WindowsConfiguration = WindowsConfigurationArgs(
                ProvisionVMAgent = input true,
                EnableAutomaticUpdates = input true))

        // Define the network profile
        let networkProfile = NetworkProfileArgs(
            NetworkInterfaces = inputList [ NetworkInterfaceReferenceArgs(Id = input networkInterface.Id) ])

        // Create the virtual machine
        let! vm = VirtualMachine("adVM",
                                 VirtualMachineArgs(
                                     ResourceGroupName = input resourceGroup.Name,
                                     NetworkProfile = networkProfile,
                                     OsProfile = osProfile,
                                     StorageProfile = storageProfile,
                                     HardwareProfile = HardwareProfileArgs(VmSize = VirtualMachineSizeTypes.Standard_DS2_v2),
                                     Location = input resourceGroup.Location))

        return {
            Pulumi.UserName = vm.Id
        }
    }

[<EntryPoint>]
let main _ =
    Deployment.run infra
