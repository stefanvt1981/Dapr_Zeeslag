$RESOURCE_GROUP = "rg-kennissessie-nov2022"
$ENVIRONMENT = "managedEnvironment-rgkennissessien-92be"
$ACR_NAME = "acrkennissessie"

$VERSION = "1.0.0"

function CreateContainerApp {
    param (
        $ServiceName
    )

    az acr build --registry $ACR_NAME --image ${ServiceName}:$VERSION ..\src\ -f ..\src\$ServiceName\Dockerfile

    az containerapp create `
        --name $ServiceName `
        --resource-group $RESOURCE_GROUP `
        --environment $ENVIRONMENT `
        --image "$ACR_NAME.azurecr.io/${ServiceName}:$VERSION"  `
        --target-port 80 `
        --ingress 'internal' `
        --registry-server "$ACR_NAME.azurecr.io" `
        --min-replicas 1 `
        --max-replicas 1 `
        --enable-dapr `
        --dapr-app-port 80     
}

# CreateContainerApp -ServiceName "boardservice"
# CreateContainerApp -ServiceName "gameservice"
# CreateContainerApp -ServiceName "shipservice"
# CreateContainerApp -ServiceName "playerservice"

az acr build --registry $ACR_NAME --image client:$VERSION ..\. -f ..\src\ZeeslagFrontEnd\Server\Dockerfile
az containerapp create `
    --name "client" `
    --resource-group $RESOURCE_GROUP `
    --environment $ENVIRONMENT `
    --image "$ACR_NAME.azurecr.io/client:$VERSION"  `
    --target-port 80 `
    --ingress 'external' `
    --registry-server "$ACR_NAME.azurecr.io" `
    --min-replicas 1 `
    --max-replicas 1 `
    --enable-dapr `
    --dapr-app-port 80  

