# Build & Publish Config Api & Config UI - Simple 2-tier app

# Local Development

Open folder [00-app](/demos/00-app/)

Build and run `config-api` locally:

```bash
cd config-api
docker build --rm -f Dockerfile -t config-api .
docker run -it --rm -p 5051:80 config-api
cd ..
http://localhost:5051/
```

Build and run `config-ui` locally. Inject the api url using environment variable `ENV_API_URL`:

```bash
cd config-ui
docker build --rm -f Dockerfile -t config-ui .  
docker run -it --rm -p 5052:80 --env ENV_API_URL="http://localhost:5051"  config-ui
cd ..
http://localhost:5052/
```

Stop Containers:

```bash
docker ps
docker stop <container-id>
```

# Publish to Azure Container Registry

Execute [publish-images.azcli](/demos/03-containers/02-publish/publish-images.azcli) to build and publish to Azure Container Registry.

>Note: Remove existing `node_modules` and `.angular` folders in `config-ui`, if present to reduce upload time to Azure container registry.
