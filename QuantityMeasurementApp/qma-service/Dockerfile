FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY QmaService.csproj ./
RUN dotnet restore QmaService.csproj

COPY . ./
RUN dotnet publish QmaService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app .
EXPOSE 5002
ENTRYPOINT ["dotnet", "QmaService.dll"]
