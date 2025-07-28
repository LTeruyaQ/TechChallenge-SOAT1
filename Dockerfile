# Estágio de restauração
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copiar arquivos de projeto
COPY ["MecanicaOS/API/API.csproj", "MecanicaOS/API/"]
COPY ["MecanicaOS/Infraestrutura/Infraestrutura.csproj", "MecanicaOS/Infraestrutura/"]
COPY ["MecanicaOS/Dominio/Dominio.csproj", "MecanicaOS/Dominio/"]
COPY ["MecanicaOS/Aplicacao/Aplicacao.csproj", "MecanicaOS/Aplicacao/"]

# Criar solução e restaurar pacotes
RUN dotnet new sln -n MecanicaOS \
    && dotnet sln MecanicaOS.sln add MecanicaOS/API/API.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Infraestrutura/Infraestrutura.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Dominio/Dominio.csproj \
    && dotnet sln MecanicaOS.sln add MecanicaOS/Aplicacao/Aplicacao.csproj \
    && dotnet restore "MecanicaOS.sln" --ignore-failed-sources

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e de restauração
COPY --from=restore /src /src

# Copiar todo o código-fonte
COPY . .

# Publicar a aplicação
WORKDIR "/src/MecanicaOS/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar o runtime do PostgreSQL e outras dependências
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    libc6-dev \
    libgdiplus \
    libx11-dev \
    && rm -rf /var/lib/apt/lists/*

# Copiar os arquivos publicados
COPY --from=build /app/publish .

# Expor a porta 80
EXPOSE 80

# Definir variáveis de ambiente
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Comando de inicialização
ENTRYPOINT ["dotnet", "API.dll"]
