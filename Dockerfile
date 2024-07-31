# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY IDORDemoApp.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
COPY ["Views", "./Views/"]
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "IDORDemoApp.dll"]

# Expose port 8880
EXPOSE 8880

# Set the environment variable for the port
ENV ASPNETCORE_URLS=http://+:8880