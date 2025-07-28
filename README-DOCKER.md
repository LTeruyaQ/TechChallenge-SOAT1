# MecanicaOS - Docker

Este guia explica como executar a aplicação MecanicaOS em containers Docker.

## Pré-requisitos

- Docker Desktop (Windows/Mac) ou Docker Engine (Linux) instalado
- Docker Compose
- Git (opcional, para clonar o repositório)

## Configuração

1. **Clonar o repositório** (se ainda não tiver feito):
   ```bash
   git clone <url-do-repositorio>
   cd TechChallenge-SOAT1
   ```

2. **Configurar variáveis de ambiente** (opcional):
   - O arquivo `docker-compose.yml` já vem com configurações padrão que devem funcionar imediatamente.
   - Se necessário, você pode modificar as seguintes variáveis no arquivo `docker-compose.yml`:
     - `POSTGRES_DB`: Nome do banco de dados (padrão: `mecanicaos`)
     - `POSTGRES_USER`: Usuário do banco de dados (padrão: `postgres`)
     - `POSTGRES_PASSWORD`: Senha do banco de dados (padrão: `postgres`)
     - `ConnectionStrings__DefaultConnection`: String de conexão com o banco de dados

## Executando a aplicação

1. **Iniciar os containers**:
   ```bash
   docker-compose up -d
   ```

2. **Acompanhar os logs da aplicação**:
   ```bash
   docker-compose logs -f mecanicaos.api
   ```

3. **Acessar a aplicação**:
   - A API estará disponível em: http://localhost:80
   - Documentação Swagger: http://localhost:80/swagger

4. **Acessar o pgAdmin** (opcional):
   - Acesse: http://localhost:5050
   - Faça login com:
     - Email: admin@mecanicaos.com
     - Senha: admin
   - Para conectar ao banco de dados:
     1. Clique com o botão direito em "Servers" > "Create" > "Server..."
     2. Na aba "General", dê um nome ao servidor (ex: "MecanicaOS DB")
     3. Na aba "Connection":
        - Host name/address: `db`
        - Port: `5432`
        - Maintenance database: `mecanicaos`
        - Username: `postgres`
        - Password: `postgres`

## Comandos úteis

- **Parar os containers**:
  ```bash
  docker-compose down
  ```

- **Reconstruir a imagem da aplicação**:
  ```bash
  docker-compose build --no-cache
  ```

- **Visualizar logs do banco de dados**:
  ```bash
  docker-compose logs -f db
  ```

- **Abrir terminal no container da aplicação**:
  ```bash
  docker-compose exec mecanicaos.api bash
  ```

- **Executar migrações do banco de dados** (se necessário):
  ```bash
  docker-compose exec mecanicaos.api dotnet ef database update
  ```

## Solução de problemas

- **Erro de conexão com o banco de dados**:
  - Verifique se o container do banco de dados está em execução: `docker-compose ps`
  - Verifique os logs do banco de dados: `docker-compose logs db`
  - Certifique-se de que as credenciais no `docker-compose.yml` correspondem às configurações da aplicação

- **Aplicação não inicia**:
  - Verifique os logs da aplicação: `docker-compose logs mecanicaos.api`
  - Certifique-se de que todas as dependências foram instaladas corretamente

## Limpando os recursos

Para remover todos os recursos criados pelo Docker Compose (incluindo volumes de dados):

```bash
docker-compose down -v
```
> **Atenção**: Isso irá remover todos os dados do banco de dados.
