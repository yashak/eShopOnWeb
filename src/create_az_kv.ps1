#az login
$location = "westeurope"
$resourceGroup = "rg-4"


# az identity create --name "managedidentity2" `
#                    --resource-group $resourceGroup


# # https://docs.microsoft.com/en-us/cli/azure/role/assignment?view=azure-cli-latest#az-role-assignment-create
az role assignment create --role "Owner" --assignee "managedidentity2" 




# $keyVaultName = "eShopOnWeb-test-kv-1024"
# az keyvault create --location $location --name $keyVaultName --resource-group $resourceGroup

$webvm = "vmweb"
$spID=$(az resource list -n $webvm --query [*].identity.principalId --out tsv)

az role assignment create --assignee $spID --role 'Owner' --scope "/subscriptions/xxx/resourceGroups/xx/providers/Microsoft.Sql/servers/xxx/databases/xxx"

#assign 