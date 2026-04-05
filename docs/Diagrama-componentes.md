# Diagrama de Componentes

Este documento apresenta a visão de componentes da API, incluindo nuvem, APIs, banco de dados e monitoramento.

## Visão geral

```mermaid
flowchart LR
  subgraph Consumers["Consumidores"]
    Web["Aplicação Web / Cliente"]
    Backoffice["Sistema Interno da Oficina"]
  end

  subgraph Edge["Camada de Borda (AWS)"]
    APIGW["API Gateway"]
    COG["Cognito (JWT)"]
  end

  subgraph Runtime["Execução da API"]
    API["Mecanica.Hermes.Api\nMinimal API"]
    APP["Application\nCQRS + MediatR"]
    DOM["Domain\nRegras de Negócio"]
    INFRA["Infrastructure\nEF Core + SMTP + Auth"]
  end

  subgraph DataObs["Dados e Observabilidade"]
    RDS[("PostgreSQL (RDS)")]
    SMTP["SMTP"]
    NR["New Relic"]
  end

  Web --> APIGW
  Backoffice --> APIGW
  APIGW --> API
  APIGW --> COG
  COG --> API

  API --> APP
  API --> INFRA
  APP --> DOM
  INFRA --> APP
  INFRA --> DOM

  INFRA --> RDS
  INFRA --> SMTP
  API --> NR
```

## Responsabilidades por componente

- **API (`Mecanica.Hermes.Api`)**
  - Endpoints HTTP (Minimal API)
  - Middleware de exceções/autenticação em desenvolvimento
  - Exposição de OpenAPI/Scalar
- **Application (`Mecanica.Hermes.Application`)**
  - Casos de uso com CQRS
  - Handlers MediatR
  - Portas (interfaces) de saída
- **Domain (`Mecanica.Hermes.Domain`)**
  - Agregados, Value Objects, Domain Events
  - State Pattern para ciclo da Ordem de Serviço
- **Infrastructure (`Mecanica.Hermes.Infrastructure`)**
  - Persistência (EF Core/Npgsql)
  - Implementações de repositórios
  - Integração SMTP
  - Configuração de autenticação

## Fluxos de integração relevantes

1. **Autenticação**: token JWT emitido por Cognito, validado pela API.
2. **Persistência**: escrita/leitura no PostgreSQL via EF Core.
3. **Notificações**: envio de e-mails via SMTP.
4. **Observabilidade**: métricas e telemetria enviadas ao New Relic no ambiente Kubernetes.
