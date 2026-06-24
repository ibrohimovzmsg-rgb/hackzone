FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
RUN addgroup --system hackzone && adduser --system --ingroup hackzone hackzone
RUN mkdir -p /app/logs && chown -R hackzone:hackzone /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/HackZone.Api/HackZone.Api.csproj", "HackZone.Api/"]
COPY ["src/HackZone.Infrastructure/HackZone.Infrastructure.csproj", "HackZone.Infrastructure/"]
COPY ["src/HackZone.Application/HackZone.Application.csproj", "HackZone.Application/"]
COPY ["src/HackZone.Domain/HackZone.Domain.csproj", "HackZone.Domain/"]
RUN dotnet restore "HackZone.Api/HackZone.Api.csproj"
COPY src/ .
RUN dotnet build "HackZone.Api/HackZone.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HackZone.Api/HackZone.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R hackzone:hackzone /app
USER hackzone
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "HackZone.Api.dll"]
