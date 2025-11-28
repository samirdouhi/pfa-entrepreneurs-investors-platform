FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le projet .NET
COPY *.csproj ./
RUN dotnet restore

# Copier le reste du code et compiler
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Image finale d'exécution
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ⚠️ Change "Authservice.dll" si ton projet s'appelle autrement
ENTRYPOINT ["dotnet", "Authservice.dll"]
