FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5030

ENV ASPNETCORE_URLS=http://+:5030

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["orders-events-store.csproj", "./"]
RUN dotnet restore "orders-events-store.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "orders-events-store.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "orders-events-store.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "orders-events-store.dll"]


# Build Image
# docker build --rm -f Dockerfile -t food-orders-api .
# docker run -it --rm -p 5054:80 food-orders-api

# docker tag food-orders-api arambazamba/food-orders-api
# docker push arambazamba/food-orders-api

# Injecting environment variables into the container
# docker run -it --rm -p 5054:80 food-orders-api -e "App__2__AuthEnabled"="false"

# Browse using: 
# http://localhost:5054