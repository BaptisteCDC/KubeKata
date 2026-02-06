# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the pre-built binaries from the local 'publish' directory
COPY bin/Release/net10.0/publish/ .

# OpenTelemetry Prometheus Exporter default port is 9464
EXPOSE 9464
ENTRYPOINT ["dotnet", "KubeKataWorker.dll"]
