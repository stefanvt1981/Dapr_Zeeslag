dapr run --app-id boardmanager --app-port 6000 --dapr-http-port 3600 --dapr-grpc-port 60000 dotnet run
dapr run --app-id gamemanager --app-port 6001 --dapr-http-port 3601 --dapr-grpc-port 60001 dotnet run
dapr run --app-id playermanager --app-port 6002 --dapr-http-port 3602 --dapr-grpc-port 60002 dotnet run
dapr run --app-id shipmanager --components-path ../Components/ --app-port 6003 --dapr-http-port 3603 --dapr-grpc-port 60003 dotnet run
dapr run --app-id client --components-path ../Components/ dotnet run