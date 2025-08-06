# MecanicaOS: Sistema de Gerenciamento de Oficina

## Visão Geral do Projeto

O **MecanicaOS** é um sistema de gerenciamento para oficinas mecânicas, desenvolvido como parte do Tech Challenge da Pós-Graduação em Arquitetura de Software. O objetivo do projeto é aplicar os conceitos de Arquitetura Limpa (Clean Architecture) e Domain-Driven Design (DDD) na construção de uma API RESTful robusta, escalável e de fácil manutenção.

O sistema permite o gerenciamento completo de clientes, veículos, ordens de serviço, estoque de peças e serviços oferecidos pela oficina.

### Principais Funcionalidades

*   **Gerenciamento de Clientes:** Cadastro, atualização e consulta de clientes (Pessoa Física e Jurídica).
*   **Gerenciamento de Veículos:** Cadastro de veículos associados a clientes.
*   **Controle de Ordens de Serviço (OS):** Criação, acompanhamento de status (em orçamento, aprovada, em andamento, finalizada, cancelada), e registro de insumos utilizados.
*   **Gestão de Estoque:** Controle de peças e produtos, com alertas de estoque baixo.
*   **Catálogo de Serviços:** Cadastro e consulta dos serviços prestados pela oficina.
*   **Autenticação e Autorização:** Sistema de login com perfis de usuário (Administrador, Funcionário) e controle de acesso baseado em permissões.

## Arquitetura

O projeto foi estruturado seguindo os princípios da **Clean Architecture**, proposta por Robert C. Martin (Uncle Bob). Essa abordagem garante a separação de responsabilidades, o baixo acoplamento entre as camadas e a testabilidade do código.

A arquitetura é dividida em quatro camadas principais:

### 1. Domínio (Domain)

O coração do sistema. Esta camada contém as entidades de negócio, os objetos de valor, as exceções de domínio e as interfaces dos repositórios. É a camada mais interna e não depende de nenhuma outra.

*   **Localização:** `MecanicaOS/Dominio`
*   **Principais componentes:**
    *   `Entidades`: Classes que representam os objetos de negócio (ex: `Cliente`, `Veiculo`, `OrdemServico`).
    *   `Enumeradores`: Tipos enumerados para status e categorias (ex: `StatusOrdemServico`).
    *   `Interfaces/Repositorios`: Contratos para a camada de infraestrutura, definindo as operações de persistência de dados.
    *   `Especificacoes`: Implementação do padrão Specification para criar consultas de forma declarativa e reutilizável.

### 2. Aplicação (Application)

Esta camada orquestra as entidades de domínio para executar os casos de uso do sistema (use cases). Ela contém a lógica de aplicação, mas não a lógica de negócio (que reside no domínio).

*   **Localização:** `MecanicaOS/Aplicacao`
*   **Principais componentes:**
    *   `DTOs`: Data Transfer Objects para transferir dados entre as camadas, evitando o acoplamento com as entidades de domínio.
    *   `Servicos`: Classes que implementam os casos de uso (ex: `ClienteServico`, `OrdemServicoServico`).
    *   `Interfaces`: Contratos para os serviços da camada de aplicação.
    *   `Mapeamentos`: Perfis do AutoMapper para mapear DTOs para Entidades e vice-versa.
    *   `Jobs`: Tarefas em segundo plano (background jobs) com Hangfire.

### 3. Infraestrutura (Infrastructure)

A camada de infraestrutura é responsável por implementar as interfaces definidas na camada de domínio e aplicação. Ela lida com detalhes técnicos como acesso a banco de dados, envio de e-mails, autenticação, etc.

*   **Localização:** `MecanicaOS/Infraestrutura`
*   **Principais componentes:**
    *   `Dados`: Implementação do Entity Framework Core, incluindo o `DbContext`, repositórios genéricos e mapeamentos das entidades para o banco de dados.
    *   `Autenticacao`: Serviços para geração e validação de tokens JWT.
    *   `Servicos`: Implementações concretas de serviços externos, como o `ServicoEmail` (usando SendGrid).
    *   `Migrations`: Migrações do Entity Framework para o banco de dados.

### 4. API

A camada mais externa, responsável por expor o sistema para o mundo exterior através de uma API RESTful. Ela recebe as requisições HTTP, as direciona para a camada de aplicação e retorna as respostas.

*   **Localização:** `MecanicaOS/API`
*   **Principais componentes:**
    *   `Controllers`: Controladores ASP.NET Core que gerenciam as rotas da API.
    *   `Program.cs`: Configuração da aplicação, injeção de dependências, middlewares, etc.
    *   `Middlewares`: Handlers para tratamento global de exceções e logging.
    *   `Filters`: Filtros para validação de permissões.

## Tecnologias Utilizadas

*   **.NET 9.0:** Plataforma de desenvolvimento.
*   **ASP.NET Core 9.0:** Framework para construção da API RESTful.
*   **Entity Framework Core 9.0:** ORM para acesso a dados.
*   **PostgreSQL:** Banco de dados relacional.
*   **Docker & Docker Compose:** Para containerização da aplicação e do banco de dados.
*   **xUnit & Moq:** Para testes de unidade e integração.
*   **AutoMapper:** Para mapeamento de objetos.
*   **FluentValidation:** Para validações de DTOs.
*   **MediatR:** Para implementação do padrão Mediator.
*   **Hangfire:** Para execução de tarefas em segundo plano.
*   **JWT (JSON Web Tokens):** Para autenticação.
*   **Swagger (OpenAPI):** Para documentação da API.

## Como Iniciar o Projeto

Existem duas maneiras de iniciar o projeto: usando Docker (recomendado) ou configurando o ambiente localmente.

### Opção 1: Executando com Docker (Recomendado)

Esta é a forma mais simples e rápida de executar a aplicação, pois todo o ambiente já está configurado.

1.  **Pré-requisitos:**
    *   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) ou Docker Engine (Linux).
    *   Docker Compose.

2.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_REPOSITORIO>
    cd TechChallenge-SOAT1
    ```

3.  **Inicie os containers:**
    No terminal, na raiz do projeto, execute o comando:
    ```bash
    docker-compose up -d
    ```
    Este comando irá construir a imagem da API e iniciar os containers da aplicação, do banco de dados (PostgreSQL) e do pgAdmin.

4.  **Acesse a Aplicação:**
    *   **API:** A API estará disponível em `http://localhost:80`.
    *   **Documentação Swagger:** Para explorar e testar os endpoints, acesse `http://localhost:80/swagger`.

5.  **(Opcional) Acessar o Banco de Dados com pgAdmin:**
    *   Acesse `http://localhost:5050`.
    *   Login: `admin@mecanicaos.com` / Senha: `admin`.
    *   Para conectar ao servidor do banco de dados:
        *   Host: `db`
        *   Port: `5432`
        *   Usuário: `postgres`
        *   Senha: `postgres`
        *   Database: `mecanicaos`

6. **Configuração de secrets:**
   Para rodar o projeto localmente, é necessário configurar a variável secreta que o projeto utiliza.
    *   Passos para configurar user-secrets:
        *   Abra o terminal na pasta do projeto onde está o arquivo `.csproj`.
        *   Inicialize o user-secrets (caso ainda não tenha feito): `dotnet user-secrets init`
        *   Adicione a secret necessária: `dotnet user-secrets set "SendGrid:ApiKey" "<SUA_CHAVE_SENDGRID_AQUI>"`
        *   Confirme que a secret foi configurada: `dotnet user-secrets list`

### Opção 2: Executando Localmente (Sem Docker)

Para executar o projeto diretamente na sua máquina, siga os passos abaixo.

1.  **Pré-requisitos:**
    *   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
    *   Um servidor PostgreSQL instalado e em execução.
    *   Um editor de código ou IDE (Visual Studio, VS Code, Rider).

2.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_REPOSITORIO>
    cd TechChallenge-SOAT1
    ```

3.  **Configure o Banco de Dados:**
    *   Crie um novo banco de dados no seu servidor PostgreSQL.
    *   Abra o arquivo `MecanicaOS/API/appsettings.Development.json`.
    *   Modifique a `DefaultConnection` na seção `ConnectionStrings` para apontar para o seu banco de dados. Exemplo:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Host=localhost;Port=5432;Database=mecanicaos;Username=postgres;Password=sua_senha"
        }
        ```

4.  **Aplique as Migrações (Migrations):**
    No terminal, navegue até a pasta da API e execute o comando para criar as tabelas no banco de dados:
    ```bash
    cd MecanicaOS/API
    dotnet ef database update
    ```

5.  **Execute a Aplicação:**
    Ainda no terminal, na pasta da API, execute o comando:
    ```bash
    dotnet run
    ```
    A API estará disponível em `https://localhost:7199` ou `http://localhost:5246` (verifique o output do terminal).
    
6. **Configuração de secrets:**
   Para rodar o projeto localmente, é necessário configurar a variável secreta que o projeto utiliza.
    *   Passos para configurar user-secrets:
        *   Abra o terminal na pasta do projeto onde está o arquivo `.csproj`.
        *   Inicialize o user-secrets (caso ainda não tenha feito): `dotnet user-secrets init`
        *   Adicione a secret necessária: `dotnet user-secrets set "SendGrid:ApiKey" "<SUA_CHAVE_SENDGRID_AQUI>"`
        *   Confirme que a secret foi configurada: `dotnet user-secrets list`

## Executando os Testes

O projeto possui uma suíte de testes de unidade e integração. Para executá-los, navegue até a raiz do projeto no terminal e execute o seguinte comando:

```bash
dotnet test
```

Isso irá executar todos os testes nos projetos de teste da solução e exibir os resultados no console.
