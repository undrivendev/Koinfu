FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY ["src/", "/src/"]
COPY ["NuGet.Config", "/src/"]
RUN dotnet restore "Mds.Koinfu.Service/Mds.Koinfu.Service.csproj" --configfile "./NuGet.Config"
RUN dotnet publish "Mds.Koinfu.Service/Mds.Koinfu.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mds.Koinfu.Service.dll"]