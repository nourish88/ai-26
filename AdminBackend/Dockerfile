FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
COPY certs/corporate-intermediate-ca.crt /usr/local/share/ca-certificates/corporate-intermediate-ca.crt
COPY certs/corporate-subca.crt /usr/local/share/ca-certificates/corporate-subca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY AdminBackend ./AdminBackend
WORKDIR /source/AdminBackend/AdminBackend
RUN dotnet restore AdminBackend.sln
RUN dotnet publish AdminBackend.sln -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AdminBackend.Api.dll"]
