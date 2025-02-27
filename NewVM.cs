// Create an azure-native.compute.VirtualMachine Domain Controller server
// https://www.pulumi.com/ai/conversations/d60bac32-0d8f-40e3-b18e-d3c48bca1f45

using Pulumi;
using Pulumi.AzureNative.Compute;
using Pulumi.AzureNative.Compute.Inputs;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new ResourceGroup("domainControllerResourceGroup");

    // Create a Virtual Network
    var virtualNetwork = new VirtualNetwork("domainControllerVNet", new VirtualNetworkArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AddressSpace = new AddressSpaceArgs
        {
            AddressPrefixes = { "10.0.0.0/16" }
        }
    });

    // Create a Subnet
    var subnet = new Subnet("domainControllerSubnet", new SubnetArgs
    {
        ResourceGroupName = resourceGroup.Name,
        VirtualNetworkName = virtualNetwork.Name,
        AddressPrefix = "10.0.1.0/24"
    });

    // Create a Public IP Address
    var publicIp = new PublicIPAddress("domainControllerPublicIp", new PublicIPAddressArgs
    {
        ResourceGroupName = resourceGroup.Name,
        PublicIPAllocationMethod = "Dynamic"
    });

    // Create a Network Interface
    var networkInterface = new NetworkInterface("domainControllerNic", new NetworkInterfaceArgs
    {
        ResourceGroupName = resourceGroup.Name,
        IpConfigurations =
        {
            new NetworkInterfaceIpConfigurationArgs
            {
                Name = "primary",
                Primary = true,
                Subnet = new SubnetArgs
                {
                    Id = subnet.Id,
                },
                PrivateIPAllocationMethod = "Dynamic",
                PublicIPAddress = new PublicIPAddressArgs
                {
                    Id = publicIp.Id,
                }
            }
        }
    });

    // Create a Virtual Machine
    var virtualMachine = new VirtualMachine("domainControllerVM", new VirtualMachineArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        NetworkProfile = new NetworkProfileArgs
        {
            NetworkInterfaces =
            {
                new NetworkInterfaceReferenceArgs
                {
                    Id = networkInterface.Id,
                    Primary = true,
                }
            }
        },
        HardwareProfile = new HardwareProfileArgs
        {
            VmSize = "Standard_B2s",
        },
        OsProfile = new OsProfileArgs
        {
            AdminUsername = "adminuser",
            AdminPassword = "Password12345!",
            ComputerName = "domaincontroller"
        },
        StorageProfile = new StorageProfileArgs
        {
            OsDisk = new OSDiskArgs
            {
                CreateOption = DiskCreateOption.FromImage,
                ManagedDisk = new ManagedDiskParametersArgs
                {
                    StorageAccountType = "Standard_LRS"
                }
            },
            ImageReference = new ImageReferenceArgs
            {
                Offer = "WindowsServer",
                Publisher = "MicrosoftWindowsServer",
                Sku = "2019-Datacenter",
                Version = "latest"
            }
        }
    });

    return new Dictionary<string, object?>
    {
        ["vmName"] = virtualMachine.Name,
        ["publicIpAddress"] = publicIp.IpAddress
    };
});

