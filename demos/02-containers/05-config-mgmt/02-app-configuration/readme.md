# Use Azure App Configuration Service

Execute `create-app-config.azcli` to create an Azure App Configuration Service. 

Set the `UseAppConfig` to `true` and use Azure App Configuration in:

Run config-api container and override values from appsettings.json with environment variables:

```bash
docker run -d -p 5051:80 -e "App:UseAppConfig=true" -e "App:AppConfigConnection=$configCon" config-api
```

Browse to `http://localhost:5051/settings`. The title should be `Config Api Dev` as this value was set in the `development` environment in Azure App Configuration Service.    