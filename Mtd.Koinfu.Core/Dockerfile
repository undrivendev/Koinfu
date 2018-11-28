FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Mtd.Koinfu.Core/Mtd.Koinfu.Core.csproj", "Mtd.Koinfu.Core/"]
RUN dotnet restore "Mtd.Koinfu.Core/Mtd.Koinfu.Core.csproj"
COPY . .
WORKDIR "/src/Mtd.Koinfu.Core"
RUN dotnet build "Mtd.Koinfu.Core.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mtd.Koinfu.Core.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mtd.Koinfu.Core.dll"]