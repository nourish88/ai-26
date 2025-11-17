FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY certs/aa.crt /usr/local/share/ca-certificates/aa.crt
COPY certs/pypi.org.crt /usr/local/share/ca-certificates/pypi.org.crt
COPY certs/nuget.org.crt /usr/local/share/ca-certificates/nuget.org.crt
COPY certs/api.nuget.org.crt /usr/local/share/ca-certificates/api.nuget.org.crt
RUN apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && echo 'APT::Get::AllowUnauthenticated "true";' > /etc/apt/apt.conf.d/99allow-unauthenticated \
    && apt-get update --allow-releaseinfo-change 2>&1 | grep -v "GPG error" | grep -v "not signed" || true \
    && apt-get install -y --no-install-recommends ca-certificates gnupg2 \
    && rm -f /etc/apt/apt.conf.d/99allow-unauthenticated \
    && apt-get update 2>&1 | grep -v "GPG error" | grep -v "not signed" || true \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY ToolGateway ./ToolGateway
WORKDIR /source/ToolGateway/ToolGateway
ENV NUGET_XMLDOC_MODE=skip \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    NUGET_PACKAGES=/root/.nuget/packages \
    TMPDIR=/root/.nuget/temp \
    TEMP=/root/.nuget/temp \
    TMP=/root/.nuget/temp
RUN mkdir -p /root/.nuget/temp /root/.nuget/packages \
    && rm -rf /tmp/* /tmp/.* 2>/dev/null || true \
    && dotnet nuget locals all --clear \
    && dotnet restore ToolGateway.sln --verbosity minimal --no-cache || \
    (rm -rf /tmp/* /tmp/.* /root/.nuget/temp/* 2>/dev/null || true \
     && dotnet nuget locals all --clear \
     && dotnet restore ToolGateway.sln --verbosity minimal) \
    && rm -rf /tmp/* /tmp/.* /root/.nuget/temp/* 2>/dev/null || true
RUN dotnet publish ToolGateway.sln -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ToolGateway.Api.dll"]
