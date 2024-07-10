FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
USER app
WORKDIR /app
EXPOSE 4711

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Matilda.WebApi.csproj", "Matilda.WebApi/"]
RUN dotnet restore "./Matilda.WebApi/Matilda.WebApi.csproj"
COPY . ./Matilda.WebApi/
WORKDIR "/src/Matilda.WebApi"
RUN dotnet build "./Matilda.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Matilda.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Matilda.WebApi.dll"]
