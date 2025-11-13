FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY AdminBackend/. ./
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CERTIFICATE_ENFORCEMENT=none
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AdminBackend.Api.dll"]
