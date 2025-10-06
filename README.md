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
        *   Abra o terminal na pasta do projeto API onde está o arquivo `.csproj`.
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
        *   Abra o terminal na pasta do projeto API onde está o arquivo `.csproj`.
        *   Inicialize o user-secrets (caso ainda não tenha feito): `dotnet user-secrets init`
        *   Adicione a secret necessária: `dotnet user-secrets set "SendGrid:ApiKey" "<SUA_CHAVE_SENDGRID_AQUI>"`
        *   Confirme que a secret foi configurada: `dotnet user-secrets list`

## Executando os Testes

O projeto possui uma suíte de testes de unidade e integração. Para executá-los, navegue até a raiz do projeto no terminal e execute o seguinte comando:

```bash
dotnet test
```

Isso irá executar todos os testes nos projetos de teste da solução e exibir os resultados no console.

---

## Fase 2: Orquestração e Entrega Contínua

Nesta fase, o projeto evolui para uma arquitetura nativa da nuvem, com foco na automação, orquestração de contêineres e entrega contínua (CI/CD). O objetivo é garantir que a aplicação **MecanicaOS** seja implantada de forma confiável, escalável e segura em um ambiente de produção.

### Arquitetura Proposta

A arquitetura da solução foi desenhada para ser escalável, resiliente e automatizada, utilizando as melhores práticas de DevOps e Cloud Native.

#### Componentes da Aplicação

*   **API (MecanicaOS):** A aplicação principal, responsável pela lógica de negócio, agora está containerizada com Docker para garantir a portabilidade e consistência entre os ambientes.
*   **Banco de Dados (PostgreSQL):** O banco de dados relacional que armazena todos os dados da aplicação. Para o ambiente de produção no Kubernetes, recomenda-se o uso de um serviço de banco de dados gerenciado como o Amazon RDS para maior confiabilidade e performance.
*   **pgAdmin:** Ferramenta de administração para o PostgreSQL, útil para desenvolvimento e depuração.

#### Infraestrutura Provisionada (Terraform)

A infraestrutura como código (IaC) é gerenciada pelo Terraform, permitindo o provisionamento e a gestão de todos os recursos de forma declarativa e versionada. O arquivo `terraform/main.tf` demonstra um exemplo básico de provisionamento de uma instância na AWS, que pode ser expandido para criar uma infraestrutura completa, incluindo:

*   **VPC (Virtual Private Cloud):** Uma rede virtual isolada na AWS para garantir a segurança dos recursos.
*   **EKS (Elastic Kubernetes Service):** Um cluster Kubernetes gerenciado pela AWS, que orquestra a execução dos contêineres da aplicação.
*   **ECR (Elastic Container Registry):** Um registro de contêineres privado e seguro para armazenar as imagens Docker da aplicação.

#### Fluxo de Deploy (CI/CD)

O fluxo de deploy é automatizado através de um pipeline de CI/CD, que pode ser implementado com ferramentas como GitHub Actions, Jenkins ou GitLab CI. O processo típico é:

1.  **Commit & Push:** O desenvolvedor envia o código para o repositório Git.
2.  **Build & Test:** O pipeline de CI é acionado, compilando o código, executando os testes automatizados e construindo a imagem Docker da aplicação.
3.  **Push to ECR:** A imagem Docker é tagueada e enviada para o registro de contêineres (Amazon ECR).
4.  **Deploy to EKS:** O pipeline de CD atualiza os manifestos do Kubernetes (como o `k8s-deployment-api.yaml`) com a nova versão da imagem e aplica as mudanças no cluster EKS, realizando o deploy sem downtime (rolling update).

![Arquitetura da Solução](https://user-images.githubusercontent.com/1234567/89012345-abcde-1234-5678-90ab-cdef12345678.png)
**Atenção:** Substitua o link acima pela URL da imagem da sua arquitetura. Você pode usar ferramentas como o [draw.io](https://draw.io) ou o [Lucidchart](https://lucidchart.com) para criar o seu diagrama.

---

## Instruções de Deploy e Provisionamento

### Provisionamento da Infraestrutura com Terraform

Os arquivos do Terraform (`.tf`) no diretório `terraform/` descrevem a infraestrutura necessária na AWS.

1.  **Pré-requisitos:**
    *   [Terraform CLI](https://learn.hashicorp.com/tutorials/terraform/install-cli) instalado.
    *   [AWS CLI](https://aws.amazon.com/cli/) instalado e configurado com credenciais de acesso à sua conta AWS (`aws configure`).

2.  **Inicialização:** Navegue até o diretório `terraform` e inicialize o Terraform para baixar os providers necessários.
    ```bash
    cd terraform
    terraform init
    ```

3.  **Planejamento:** Gere um plano de execução para visualizar os recursos que serão criados.
    ```bash
    terraform plan
    ```

4.  **Aplicação:** Provisione a infraestrutura na AWS.
    ```bash
    terraform apply
    ```
    O Terraform solicitará uma confirmação. Digite `yes` para continuar.

### Deploy em Kubernetes

Os arquivos de manifesto (`.yaml`) na raiz do projeto definem os recursos do Kubernetes para a aplicação.

1.  **Pré-requisitos:**
    *   [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) instalado.
    *   Um cluster Kubernetes ativo e o `kubectl` configurado para se conectar a ele (ex: `aws eks update-kubeconfig ...`).

2.  **Aplicação dos Manifestos:** Na raiz do projeto, aplique os manifestos para criar os serviços e deployments.
    ```bash
    # Cria o Service e o Deployment para o Banco de Dados
    kubectl apply -f k8s-service-db.yaml
    kubectl apply -f k8s-deployment-db.yaml

    # Cria o Service e o Deployment para a API
    kubectl apply -f k8s-service-api.yaml
    kubectl apply -f k8s-deployment-api.yaml
    ```

3.  **Verificação:** Verifique se os pods estão em execução.
    ```bash
    kubectl get pods
    ```
    Para obter o endereço de acesso da API, verifique o `LoadBalancer` criado pelo serviço:
    ```bash
    kubectl get services
    ```

---

## Links Adicionais

### Documentação da API (Collection)

Para facilitar a interação e os testes com a API, disponibilizamos uma collection que pode ser importada em ferramentas como Postman ou Insomnia.

*   **Link da Collection:** [**Atenção:** Insira aqui o link público para o seu arquivo de collection (ex: Postman, Swagger JSON)]

### Vídeo Demonstrativo

Um vídeo de até 15 minutos foi gravado para demonstrar a solução completa, incluindo a arquitetura, o pipeline de CI/CD, o provisionamento da infraestrutura e a aplicação em funcionamento.

*   **Link do Vídeo:** [**Atenção:** Insira aqui o link público ou não listado para o seu vídeo no YouTube ou Vimeo]
