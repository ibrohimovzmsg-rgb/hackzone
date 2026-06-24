FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001
RUN addgroup --system hackzone && adduser --system --ingroup hackzone hackzone

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/HackZone.Web/HackZone.Web.csproj", "HackZone.Web/"]
RUN dotnet restore "HackZone.Web/HackZone.Web.csproj"
COPY src/HackZone.Web/ HackZone.Web/
RUN dotnet build "HackZone.Web/HackZone.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HackZone.Web/HackZone.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/.aspnet/DataProtection-Keys && \
    chown -R hackzone:hackzone /app
USER hackzone
ENV ASPNETCORE_URLS=http://+:5001
ENTRYPOINT ["dotnet", "HackZone.Web.dll"]
