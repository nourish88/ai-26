FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY certs/aa.crt /usr/local/share/ca-certificates/aa.crt
COPY certs/pypi.org.crt /usr/local/share/ca-certificates/pypi.org.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY ToolGateway ./ToolGateway
WORKDIR /source/ToolGateway/ToolGateway
RUN dotnet restore ToolGateway.sln
RUN dotnet publish ToolGateway.sln -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ToolGateway.Api.dll"]
