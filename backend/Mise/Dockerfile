FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Mise.API/Mise.API.csproj", "MISE.API/"]
COPY ["Mise.Application/Mise.Application.csproj", "Mise.Application/"]
COPY ["Mise.Domain/Mise.Domain.csproj", "Mise.Domain/"]
COPY ["Mise.Infrastructure/Mise.Infrastructure.csproj","Mise.Infrastructure/"]
RUN dotnet restore "Mise.API/Mise.API.csproj"
COPY . .
WORKDIR "/src/Mise.API"
RUN dotnet build "Mise.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mise.API.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mise.API.dll"]