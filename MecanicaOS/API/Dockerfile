# Estágio de restauração e build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas arquivos de projeto para restaurar dependências
COPY ["MecanicaOS/API/API.csproj", "MecanicaOS/API/"]
COPY ["MecanicaOS/Infraestrutura/Infraestrutura.csproj", "MecanicaOS/Infraestrutura/"]
COPY ["MecanicaOS/Dominio/Dominio.csproj", "MecanicaOS/Dominio/"]
COPY ["MecanicaOS/Aplicacao/Aplicacao.csproj", "MecanicaOS/Aplicacao/"]

# Restaura dependências
RUN dotnet new sln -n MecanicaOS \
    && dotnet sln MecanicaOS.sln add MecanicaOS/API/API.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Infraestrutura/Infraestrutura.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Dominio/Dominio.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Aplicacao/Aplicacao.csproj \
    && dotnet restore "MecanicaOS.sln" --ignore-failed-sources

# Copia todo o código-fonte
COPY . .

# Publica a aplicação
WORKDIR /src/MecanicaOS/API
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final: imagem enxuta para produção
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instala dependências do sistema
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus libc6-dev libx11-dev \
    && rm -rf /var/lib/apt/lists/*

# Copia arquivos publicados
COPY --from=build /app/publish .

# Expondo porta padrão
EXPOSE 80

# Variáveis de ambiente configuráveis
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "API.dll"]
