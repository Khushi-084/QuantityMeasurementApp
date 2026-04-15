FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ApiGateway.csproj ./
RUN dotnet restore ApiGateway.csproj

COPY . ./
RUN dotnet publish ApiGateway.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
