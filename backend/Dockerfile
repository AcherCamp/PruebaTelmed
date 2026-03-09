FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY TelMedAPI/*.csproj ./TelMedAPI/
RUN dotnet restore ./TelMedAPI/TelMedAPI.csproj

COPY . .
RUN dotnet publish ./TelMedAPI/TelMedAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "TelMedAPI.dll"]