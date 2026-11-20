FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Kursplassen_Eksamen_Emne-3.csproj", "./"]
RUN dotnet restore "Kursplassen_Eksamen_Emne-3.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./Kursplassen_Eksamen_Emne-3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kursplassen_Eksamen_Emne-3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kursplassen_Eksamen_Emne-3.dll"]
