#az login

$location = "westeurope"
$resourceGroup = "TODO:SET"

#$tag="create-and-configure-database"
$server="TODO:SET"
$database1="Microsoft.eShopOnWeb.CatalogDb"
$database2="Microsoft.eShopOnWeb.Identity"
$login="TODO:SET"
$password="TODO:SET"
# Specify appropriate IP address values for your environment
# to limit access to the SQL Database server
$startIp="TODO:SET"
$endIp="TODO:SET"

Write-Output "Using resource group $resourceGroup with login: $login, password: $password..."

Write-Output "Creating $resourceGroup in $location..."
az group create --name $resourceGroup --location $location

#create vnet + subnet

$vnet = "vnet"
$subnetpriv = "myBackendSubnet"

az network vnet create `
    --resource-group $resourceGroup `
    --location $location `
    --name $vnet `
    --address-prefixes 10.0.0.0/16 `
    --subnet-name $subnetpriv `
    --subnet-prefixes 10.0.0.0/24


#Create private endpoint

#!!!!!!!!!!!!!!
# Update the subnet to disable private endpoint network policies for the private endpoint 
# az network vnet subnet update `
#     --name $subnetpriv `
#     --resource-group $resourceGroup `
#     --vnet-name $vnet `
#     --disable-private-endpoint-network-policies true    

Write-Output "Creating $server in $location..."
az sql server create --name $server --resource-group $resourceGroup --location $location --admin-user $login --admin-password $password

Write-Output "Configuring firewall..."
az sql server firewall-rule create --resource-group $resourceGroup --server $server -n AllowYourIp --start-ip-address $startIp --end-ip-address $endIp

$serverid = $(az sql server list `
    --resource-group $resourceGroup `
    --query '[].[id]' `
    --output tsv)

$privateEndpoint4db = "myPrivateEndpoint"

az network private-endpoint create `
    --name $privateEndpoint4db `
    --resource-group $resourceGroup `
    --vnet-name $vnet `
    --subnet $subnetpriv `
    --private-connection-resource-id $serverid `
    --group-ids sqlServer `
    --connection-name myConnection

az network private-dns zone create `
    --resource-group $resourceGroup `
    --name "privatelink.database.windows.net"

az network private-dns link vnet create `
    --resource-group $resourceGroup `
    --zone-name "privatelink.database.windows.net" `
    --name MyDNSLink `
    --virtual-network $vnet `
    --registration-enabled false

az network private-endpoint dns-zone-group create `
   --resource-group $resourceGroup `
   --endpoint-name $privateEndpoint4db `
   --name MyZoneGroup `
   --private-dns-zone "privatelink.database.windows.net" `
   --zone-name sql

# no need to create db - it will be imported from bakpac
# # Write-Output "Creating $database on $server..."
# # az sql db create --resource-group $resourceGroup --server $server --name $database1  --edition Basic   --zone-redundant false # zone redundancy is only supported on premium and business critical service tiers

# az storage account create -n "dbrestorestorageacc" -g "manual-rg" -l "westeurope" --sku Standard_LRS
# az storage container create --name "blobstorage"  --account-name "dbrestorestorageacc"
# mgmt studio >  export to Microsoft.eShopOnWeb.CatalogDb.bacpac
# The Azure SQL Server firewall did not allow the operation to connect. To resolve this, please select the "Allow All Azure" checkbox in the Sql Server's configuration blade.
# cloud > import database - use Standard DPU (not basic - scale down after import)
# scale down standard -> basic



