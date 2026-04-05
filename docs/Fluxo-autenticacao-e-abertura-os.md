# Fluxo de Autenticação e Abertura de Ordem de Serviço

Este documento detalha o fluxo de autenticação JWT e o fluxo de abertura de ordem de serviço na API.

## Diagrama de sequência

```mermaid
sequenceDiagram
    participant Cliente as Cliente/Sistema Consumidor
    participant IdP as Cognito (OIDC)
    participant API as Mecânica Hermes API
    participant App as Application Layer (CQRS)
    participant Dom as Domain (OrdemDeServico)
    participant DB as PostgreSQL
    participant SMTP as Serviço de E-mail

    Cliente->>IdP: POST /oauth2/token (client_credentials)
    IdP-->>Cliente: access_token (JWT)

    Cliente->>API: POST /api/ordens-de-servico\nAuthorization: Bearer <token>
    API->>API: Valida assinatura/issuer/scope

    alt Token inválido ou scope insuficiente
        API-->>Cliente: 401/403
    else Token válido
        API->>App: Send(AddOrdemDeServicoCommand)
        App->>Dom: Criar(cliente, veiculo, problema, observacoes)
        Dom-->>App: Result<OrdemDeServico>

        alt Falha de validação de negócio
            App-->>API: Result erro (400/409)
            API-->>Cliente: Erro padronizado
        else Sucesso
            App->>DB: Persistir OrdemDeServico + Status inicial
            App->>SMTP: Publicar notificação (Domain Event)
            App-->>API: Result sucesso
            API-->>Cliente: 201 Created (id da OS)
        end
    end
```

## Regras importantes no fluxo

- O status inicial da OS é **`Recebida`**.
- A autorização é baseada em escopos JWT (`AUTH__CLIENTE_SCOPE` e `AUTH__ADMIN_SCOPE`).
- Erros de domínio usam o `Result Pattern`, retornando respostas HTTP coerentes.
- Notificações são disparadas por **Domain Events** após persistência.
