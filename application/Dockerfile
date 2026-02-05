# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the pre-built binaries from the local 'publish' directory
COPY bin/Release/net10.0/publish/ .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "KubeKataApp.dll"]
