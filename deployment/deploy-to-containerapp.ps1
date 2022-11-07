$RESOURCE_GROUP = "rg-kennissessie-nov2022"
$ENVIRONMENT = "managedEnvironment-rgkennissessien-92be"
$ACR_NAME = "acrkennissessie"

$VERSION = "1.0.1"

function CreateContainerApp {
    param (
        $ServiceName
    )

    az acr build --registry $ACR_NAME --image ${ServiceName}:$VERSION ..\src\ -f ..\src\$ServiceName\Dockerfile

    az containerapp revision copy `
        --name $ServiceName `
        --resource-group $RESOURCE_GROUP `
        --image "$ACR_NAME.azurecr.io/${ServiceName}:$VERSION"          
}

CreateContainerApp -ServiceName "boardservice"
CreateContainerApp -ServiceName "gameservice"
CreateContainerApp -ServiceName "shipservice"
CreateContainerApp -ServiceName "playerservice"

az acr build --registry $ACR_NAME --image client:$VERSION ..\. -f ..\src\ZeeslagFrontEnd\Server\Dockerfile
az containerapp revision copy `
    --name "client" `
    --resource-group $RESOURCE_GROUP `
    --image "$ACR_NAME.azurecr.io/client:$VERSION"
