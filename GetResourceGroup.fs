// Get Azure existing Resource Group in F#
// https://www.pulumi.com/ai/conversations/dc60e9ba-daaa-451f-a6ae-ebe60ce3b14c

open Pulumi
open Pulumi.AzureNative.Resources

let getResourceGroup resourceGroupName =
    async {
        let! rg = GetResourceGroup.InvokeAsync (GetResourceGroupArgs(ResourceGroupName = resourceGroupName))
        return rg
    }

let deployment = Deployment.runAsync (fun () ->
    async {
        let! rg = getResourceGroup "your-resource-group-name" // Replace with the actual resource group name
        // Outputs resource group details
        return dict [ ("resourceGroupName", Output<string>.Create(rg.Name)) ]
    }
) :> _

deployment
