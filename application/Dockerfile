# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Update CA certificates to avoid SSL issues (common in some environments)
RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

# Set environment variable to help with some SSL certificate issues
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Copy the source code
COPY . .

# Restore and publish
RUN dotnet restore --verbosity normal
RUN dotnet publish KubeKataApp.csproj -c Release -o /app /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "KubeKataApp.dll"]
