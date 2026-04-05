# RFCs da API

## RFC-001 — Arquitetura Clean + DDD + CQRS

- **Status:** Aceito
- **Contexto:** necessidade de modularidade para evolução por fases do projeto.
- **Decisão:** adotar Clean Architecture com DDD e CQRS (MediatR).
- **Impactos:** separação clara de responsabilidades e alta testabilidade.

## RFC-002 — Autenticação federada por JWT/OIDC

- **Status:** Aceito
- **Contexto:** evitar gestão local de credenciais e suportar autenticação em borda.
- **Decisão:** usar provedor externo compatível com OIDC (Cognito) e validação JWT na API.
- **Impactos:** simplifica autenticação da aplicação e centraliza identidade no IdP.

## RFC-003 — PostgreSQL como banco principal

- **Status:** Aceito
- **Contexto:** domínio relacional com necessidade de consistência transacional.
- **Decisão:** usar PostgreSQL em RDS com EF Core/Npgsql.
- **Impactos:** garante integridade e capacidade de consultas estruturadas no domínio.

## RFC-004 — Deploy em Kubernetes com observabilidade

- **Status:** Aceito
- **Contexto:** necessidade de escala e padronização operacional em nuvem.
- **Decisão:** publicar API em EKS (repositório `mecanica-hermes-k8s`) e integrar com New Relic.
- **Impactos:** melhora escalabilidade horizontal e monitoramento em runtime.
