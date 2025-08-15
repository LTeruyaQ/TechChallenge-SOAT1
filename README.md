# MecanicaOS 🚗🔧  
Sistema de Gerenciamento de Oficina — Clean Architecture, DDD e .NET 9

## Sumário
1. Visão Geral  
2. Benefícios de Uso  
3. Principais Funcionalidades  
4. Arquitetura & Padrões de Projeto  
5. Tecnologias Empregadas  
6. Estrutura de Pastas  
7. Como Executar  
   * Docker (recomendado)  
8. Testes  
9. Contribuição  

---

## 1. Visão Geral
O **MecanicaOS** digitaliza processos de oficinas mecânicas, da recepção do cliente ao faturamento.  
Foca em escalabilidade, manutenção simples e integração fácil com front-ends ou parceiros externos (marketplaces de peças, ERPs).

## 2. Benefícios de Uso
* **Eficiência** – Jobs automáticos (Hangfire) evitam rotinas manuais de estoque e prazos de orçamento.  
* **Confiabilidade** – Transações atômicas (Unit of Work), retries de banco e suíte de testes garantem integridade.  
* **Segurança** – JWT, políticas de autorização e tratamento global de exceções.  
* **Agilidade** – Swagger/ReDoc para explorar API; Docker para subir o stack completo em minutos.  
* **Escalabilidade** – Arquitetura Limpa e DI facilitam novos módulos (ex.: agendamento on-line, pagamento).  

## 3. Principais Funcionalidades
| Módulo | Descrição |
|--------|-----------|
| Clientes & Veículos | CRUD completo, ligação 1:N veículo-cliente |
| Ordens de Serviço  | Fluxo → orçamento → aprovação → execução → finalização/cancelamento |
| Estoque & Alertas  | Controle de peças, alerta de nível crítico |
| Catálogo de Serviços | Lista de serviços com preço-base |
| Autenticação | JWT, perfis (Admin, Funcionário) |
| Relatórios e Logs | Hangfire Dashboard, logs estruturados, ID de correlação |

## 4. Arquitetura & Padrões de Projeto
* **Clean Architecture** – Quatro camadas: Domain, Application, Infrastructure, API.  
* **Domain-Driven Design (DDD)** – Entidades ricas, Value Objects, Domain Events.  
* **Repository + Unit of Work** – Abstraem acesso ao banco.  
* **Specification Pattern** – Consultas reutilizáveis e expressivas.  
* **Mediator (MediatR)** – Desacoplamento entre eventos e handlers.  
* **Data Mapper (AutoMapper)**, **Chain of Responsibility (Middlewares)**, **Strategy** para serviços externos.  

## 5. Tecnologias Empregadas
* .NET 9, C# 12  
* EF Core 9 + PostgreSQL  
* Hangfire 2  
* MediatR 12  
* AutoMapper 13  
* Swagger / ReDoc  
* Docker & Docker Compose  

## 6. Estrutura de Pastas
```
TechChallenge-SOAT1
│
├─ MecanicaOS
│  ├─ Dominio          ← entidades, specs, repositórios (contratos)
│  ├─ Aplicacao        ← use-cases, DTOs, jobs, handlers
│  ├─ Infraestrutura   ← EF Core, JWT, e-mail, repositórios
│  └─ API              ← controllers, middlewares, DI
└─ test                ← projetos de teste (xUnit)
```

## 7. Como Executar

```bash
git clone https://github.com/<org>/TechChallenge-SOAT1.git
cd TechChallenge-SOAT1
docker-compose up -d
```
Acesse:  
* API → `http://localhost:80/api/v1`  
* Swagger UI → `http://localhost:80/api/v1/docs`  
* ReDoc → `http://localhost:80/api/v1/api-docs`  
* Hangfire Dashboard (modo DEBUG) → `http://localhost:80/api/v1/hangfire`  
* pgAdmin → `http://localhost:5050` (admin/admin)

## 8. Testes
```bash
dotnet test
```
Executa testes unitários e de integração em `test/`.

## 9. Contribuição
1. Crie sua branch: `git checkout -b feature/MinhaFeature`  
2. Faça commits atômicos e revise com `git diff`.  
3. Abra um Pull Request descrevendo a mudança e vincule issues.  

---
> **Made with ❤ by Pós-Graduação Arquitetura de Software**
