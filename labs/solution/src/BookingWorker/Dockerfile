FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY src/BookingWorker/ src/BookingWorker/
COPY src/ServiceDefaults/ src/ServiceDefaults/
COPY src/Contracts/ src/Contracts/
RUN dotnet publish src/BookingWorker/BookingWorker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookingWorker.dll"]
