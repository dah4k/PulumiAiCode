// Get Azure existing Resource Group
// https://www.pulumi.com/ai/conversations/4077b279-368b-48b4-a220-c4a51c5913dc

import com.pulumi.Pulumi;
import com.pulumi.azurenative.resources.ResourceGroup;
import com.pulumi.azurenative.resources.inputs.GetResourceGroupArgs;
import com.pulumi.azurenative.resources.outputs.GetResourceGroupResult;

public class Main {
    public static void main(String[] args) {
        Pulumi.run(ctx -> {
            final var resourceGroup = ResourceGroup.get("existingResourceGroup", GetResourceGroupArgs.builder()
                .resourceGroupName("my-resource-group") // replace with your resource group name
                .build());

            ctx.export("resourceGroupName", resourceGroup.name());
            ctx.export("resourceGroupLocation", resourceGroup.location());
        });
    }
}

