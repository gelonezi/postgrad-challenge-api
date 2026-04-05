# Tecnologias Utilizadas

## Core

- **.NET 10** — Framework principal
- **C#** — Linguagem de programação
- **ASP.NET Core Minimal API** — Web API sem controllers tradicionais

## Persistência

- **Entity Framework Core** — ORM com abordagem Code-First
- **Npgsql** — Driver PostgreSQL para EF Core
- **PostgreSQL 16** — Banco de dados relacional
  - Extensões: `pg_trgm`, `unaccent` (busca textual sem acento)

## Injeção de Dependência e Mediação

- **Autofac** — Container de DI (substituição ao container nativo)
- **MediatR** — Implementação do padrão Mediator para CQRS e Domain Events
- **AutoMapper** — Mapeamento entre objetos (domínio ↔ DTO, domínio ↔ entidade de persistência)

## Segurança

- **JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)** — Validação de tokens JWT
- Autenticação federada via provedor externo compatível com OpenID Connect

## Documentação da API

- **OpenAPI 3.0 (`Microsoft.AspNetCore.OpenApi`)** — Especificação da API
- **Scalar** — Interface interativa para exploração e testes dos endpoints (`/scalar/v1`)

## E-mail

- **MailKit** — Envio de e-mail via SMTP
- **Mailpit** — Servidor SMTP local para desenvolvimento e testes

## Testes

- **xUnit** — Framework de testes unitários e de integração
- **Coverlet** — Coleta de cobertura de código (formato OpenCover)
- **ReportGenerator** — Geração de relatório HTML de cobertura
- **Testcontainers** — Provisionamento de containers PostgreSQL efêmeros para testes de integração

## DevOps e CI/CD

- **Docker** — Containerização (multi-stage build, imagem non-root)
- **Docker Compose** — Orquestração local (PostgreSQL, pgAdmin, Mailpit)
- **GitHub Actions** — Pipelines de CI (build, testes, cobertura, SonarCloud) e CD (Docker Hub)
- **SonarCloud** — Análise de qualidade de código (executado na branch `main`)