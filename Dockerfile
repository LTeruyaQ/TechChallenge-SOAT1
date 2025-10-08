# Etapa 1: Restaurar dependências
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copia os arquivos de projeto e solução primeiro para otimizar o cache do Docker
COPY ["MecanicaOS.sln", "."]
COPY ["MecanicaOS/API/API.csproj", "MecanicaOS/API/"]
COPY ["MecanicaOS/Adapters/Adapters.csproj", "MecanicaOS/Adapters/"]
COPY ["MecanicaOS/Core/Core.csproj", "MecanicaOS/Core/"]
COPY ["MecanicaOS/Infraestrutura/Infraestrutura.csproj", "MecanicaOS/Infraestrutura/"]
COPY ["test/MecanicaOS.UnitTests/MecanicaOS.UnitTests.csproj", "test/MecanicaOS.UnitTests/"]

# Restaura as dependências
RUN dotnet restore "MecanicaOS.sln"

# Etapa 2: Publicar a aplicação
FROM restore AS publish
WORKDIR /src

# Copia todo o código-fonte (as dependências já estão em cache da etapa anterior)
COPY . .

# Publica a aplicação
RUN dotnet publish "MecanicaOS/API/API.csproj" -c Release -o /app/publish --no-restore

# Etapa 3: Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia apenas os artefatos publicados da etapa de publicação
COPY --from=publish /app/publish .

# Garante que os templates de e-mail estejam presentes na imagem final
# O caminho de origem precisa ser ajustado com base na estrutura do publish
COPY --from=publish /app/publish/Templates ./Templates

# Expondo a porta padrão do Kestrel
EXPOSE 80

# Variável de ambiente para ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80

# Ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "API.dll"]
