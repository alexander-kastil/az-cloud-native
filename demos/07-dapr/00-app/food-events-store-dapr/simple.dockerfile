# Build Image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /build

COPY . .
RUN dotnet restore "orders-events-store.csproj"
RUN dotnet publish -c Release -o /app

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "orders-events-store.dll"]

# Build Image
# docker build --rm -f Dockerfile -t food-orders-events-store .
# docker run -it --rm -p 5054:80 food-orders-events-store

# docker tag food-orders-events-store arambazamba/food-orders-events-store
# docker push arambazamba/food-orders-events-store

# Injecting environment variables into the container
# docker run -it --rm -p 5054:80 food-orders-events-store -e "App__2__AuthEnabled"="false"

# Browse using: 
# http://localhost:5054
