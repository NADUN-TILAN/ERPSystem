﻿# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers (better caching)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

# Expose port 80
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "UserService.dll"]
