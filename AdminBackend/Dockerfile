FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY AdminBackend/. ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AdminBackend.Api.dll"]
