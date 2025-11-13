FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY AdminBackend/. ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AdminBackend.Api.dll"]
