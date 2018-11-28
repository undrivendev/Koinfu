FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Mtd.Koinfu.Service/Mtd.Koinfu.Service.csproj", "src/Mtd.Koinfu.Service/"]
RUN dotnet restore "src/Mtd.Koinfu.Service/Mtd.Koinfu.Service.csproj"
COPY . .
WORKDIR "/src/src/Mtd.Koinfu.Service"
RUN dotnet build "Mtd.Koinfu.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mtd.Koinfu.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mtd.Koinfu.Service.dll"]