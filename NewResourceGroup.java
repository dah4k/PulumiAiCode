// New Azure Resource Group
// https://www.pulumi.com/registry/packages/azure-native/api-docs/resources/resourcegroup/

import com.pulumi.azurenative.resources.ResourceGroup;
import com.pulumi.azurenative.resources.ResourceGroupArgs;
import java.util.List;
import java.util.ArrayList;
import java.util.Map;
import java.io.File;
import java.nio.file.Files;
import java.nio.file.Paths;

public class App {
    public static void main(String[] args) {
        Pulumi.run(App::stack);
    }

    public static void stack(Context ctx) {
        var resourceGroup = new ResourceGroup("resourceGroup", ResourceGroupArgs.builder()
            .location("eastus")
            .resourceGroupName("my-resource-group")
            .build());

    }
}
