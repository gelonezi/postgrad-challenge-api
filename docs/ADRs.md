# ADRs da API

## ADR-001 — Entidades de persistência separadas do domínio

- **Status:** Aceito
- **Decisão:** EF Core opera sobre classes `*Entity`; domínio permanece isolado de anotações de persistência.
- **Justificativa:** preservar pureza do domínio e reduzir acoplamento com framework ORM.

## ADR-002 — Result Pattern para fluxos de negócio

- **Status:** Aceito
- **Decisão:** comandos/queries retornam `Result`/`Result<T>` em vez de exceções para regras de negócio.
- **Justificativa:** padroniza tratamento de erro e simplifica mapeamento HTTP.

## ADR-003 — State Pattern para ciclo de vida da Ordem de Serviço

- **Status:** Aceito
- **Decisão:** modelar transições de estado por classes concretas de status.
- **Justificativa:** encapsula regras de transição e evita condicionais distribuídas.

## ADR-004 — Domain Events pós-commit da Unit of Work

- **Status:** Aceito
- **Decisão:** coletar eventos no agregado e despachar após persistência bem-sucedida.
- **Justificativa:** evita efeitos colaterais antes da confirmação transacional.

## ADR-005 — Soft delete com índices filtrados

- **Status:** Aceito
- **Decisão:** manter `IsDeleted` e aplicar filtros/índices únicos condicionais para ativos.
- **Justificativa:** preserva histórico e mantém restrições de unicidade sem exclusão física.
