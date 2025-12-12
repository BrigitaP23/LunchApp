# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiramo vse datoteke iz repozitorija
COPY . .

# Publish aplikacijo v Release
RUN dotnet publish ./LunchApp.csproj -c Release -o /app/publish

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopiramo rezultat builda iz prejšnje faze
COPY --from=build /app/publish .

# Nastavimo port za Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Zagon aplikacije
ENTRYPOINT ["dotnet", "LunchApp.dll"]
