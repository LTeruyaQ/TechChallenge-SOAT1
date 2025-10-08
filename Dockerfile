# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura dependências
COPY ["MecanicaOS/API/API.csproj", "MecanicaOS/API/"]
COPY ["MecanicaOS/Adapters/Adapters.csproj", "MecanicaOS/Adapters/"]
COPY ["MecanicaOS/Infraestrutura/Infraestrutura.csproj", "MecanicaOS/Infraestrutura/"]
RUN dotnet restore "MecanicaOS/API/API.csproj"

# Copia todo o código-fonte
COPY . .

WORKDIR "/src/MecanicaOS/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Garante que os templates de e-mail estejam presentes
COPY --from=build /src/MecanicaOS/API/Templates ./Templates

# Expondo a porta padrão do Kestrel
EXPOSE 80

# Variável de ambiente para ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "API.dll"]