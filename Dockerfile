# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiraj vse datoteke
COPY . .

# Publish aplikacijo za linux-x64 runtime
RUN dotnet publish LunchApp.csproj -c Release -o /app/publish -r linux-x64 --self-contained false /p:PublishTrimmed=false

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Ustvari mape za Data Protection in SQLite DB
RUN mkdir -p /app/keys /app/data

# Kopiraj publish iz build stage
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:10000
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 10000

# Start aplikacije
ENTRYPOINT ["dotnet", "LunchApp.dll"]
