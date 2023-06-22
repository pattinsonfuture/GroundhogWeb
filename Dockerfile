#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GroundhogWeb.csproj", "."]
RUN dotnet restore "./GroundhogWeb.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GroundhogWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroundhogWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "GroundhogWeb.dll"]