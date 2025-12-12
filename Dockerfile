# 1. Base image za build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiramo samo mapo projekta (kjer je LunchApp.csproj)
COPY LunchApp/ ./LunchApp/

# Publish projekta
RUN dotnet publish ./LunchApp/LunchApp.csproj -c Release -o /app/publish

# 2. Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopiramo rezultat builda
COPY --from=build /app/publish .

# Nastavimo port
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Zagon aplikacije
ENTRYPOINT ["dotnet", "LunchApp.dll"]
