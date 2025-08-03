FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ui/Shopping.Api/Shopping.Api.csproj", "src/Ui/Shopping.Api/"]
COPY ["src/Infrastructure/Shopping.Infrastructure.Persistence/Shopping.Infrastructure.Persistence.csproj", "src/Infrastructure/Shopping.Infrastructure.Persistence/"]
COPY ["src/Core/Shopping.Domain/Shopping.Domain.csproj", "src/Core/Shopping.Domain/"]
COPY ["src/Core/Shopping.Application/Shopping.Application.csproj", "src/Core/Shopping.Application/"]
COPY ["Infrastructure/Shopping.Infrastructure.Identity/Shopping.Infrastructure.Identity.csproj", "Infrastructure/Shopping.Infrastructure.Identity/"]
COPY ["Infrastructure/Shopping.Infrastructure.CrossCutting/Shopping.Infrastructure.CrossCutting.csproj", "Infrastructure/Shopping.Infrastructure.CrossCutting/"]
COPY ["src/Ui/Shopping.WebFramework/Shopping.WebFramework.csproj", "src/Ui/Shopping.WebFramework/"]
RUN dotnet restore "src/Ui/Shopping.Api/Shopping.Api.csproj"
COPY . .
WORKDIR "/src/src/Ui/Shopping.Api"
RUN dotnet build "./Shopping.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Shopping.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.Api.dll"]
