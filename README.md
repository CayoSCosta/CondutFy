# 🚀 CondutFy

**CondutFy** é um SaaS multi-tenant moderno de automação e orquestração de atendimento multicanal (WhatsApp, Instagram, Telegram, etc.), desenvolvido com **.NET 9**, **Clean Architecture** e **Domain-Driven Design (DDD)**.

## 🛠️ Tecnologias e Arquitetura

* **Backend:** .NET 9 (C#)
* **Arquitetura:** Clean Architecture + DDD
* **Persistência Híbrida:** 
  * **Entity Framework Core 9** para Commands, Migrations e Domínio.
  * **Dapper** para Queries de alta performance (Leitura).
* **Multi-tenancy:** Isolamento lógico por coluna (`TenantId`) com **Global Query Filters** do EF Core.
* **Bancos de Dados:** 
  * **SQLite** (Ambiente de Desenvolvimento local).
  * **PostgreSQL** (Ambiente de Homologação / Produção).

---

## 📂 Estrutura da Solução

```text
CondutFy/
├── CondutFy.Domain/          # Entidades ricas, Value Objects e regras de negócio puras
├── CondutFy.Application/     # Casos de uso e Handlers (CQRS)
├── CondutFy.Infrastructure/  # DbContext, EF Core, Repositórios e Dapper
├── CondutFy.API/             # Endpoints, Middlewares de Tenant e Program.cs
└── CondutFy.Tests/           # Testes unitários e de integração (xUnit)