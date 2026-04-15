FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY AuthService.csproj ./
RUN dotnet restore AuthService.csproj

COPY . ./
RUN dotnet publish AuthService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app .
EXPOSE 5001
ENTRYPOINT ["dotnet", "AuthService.dll"]
