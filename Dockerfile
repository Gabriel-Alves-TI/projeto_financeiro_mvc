# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas o csproj
COPY projeto_financeiro_mvc/*.csproj ./
RUN dotnet restore projeto_financeiro_mvc.csproj

# Copia o restante do código
COPY projeto_financeiro_mvc/ ./

# Publica
RUN dotnet publish projeto_financeiro_mvc.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime (AGORA COM SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "projeto_financeiro_mvc.dll"]