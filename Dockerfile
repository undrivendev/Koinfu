FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY ["src/*", "/src/"]
RUN dotnet restore "Mtd.Koinfu.Service/Mtd.Koinfu.Service.csproj"
RUN dotnet publish "Mtd.Koinfu.Service/Mtd.Koinfu.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mtd.Koinfu.Service.dll"]