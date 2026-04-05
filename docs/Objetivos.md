# Objetivos do Projeto (Tech Challenge)

Este projeto consiste na construção e evolução do back-end de um **Sistema Integrado de Atendimento e Execução de Serviços** para uma oficina mecânica, permitindo **gestão interna eficiente** e acompanhamento do serviço pelo cliente via API.

Atualmente, o projeto está na **Fase 2**, e ainda haverá **mais 3 fases** após esta.

---

## Fase 1 — MVP do Back-end (Fundação do Sistema)

### Objetivo

Desenvolver a **primeira versão (MVP)** do back-end, com foco em:

* Gestão de **Ordens de Serviço**
* Gestão de **Clientes**
* Gestão de **Veículos**
* Gestão de **Peças e Insumos**
* Aplicação de **DDD (Domain-Driven Design)**
* Boas práticas de **Qualidade de Software** e **Segurança**

### Funcionalidades esperadas

**Criação da Ordem de Serviço (OS)**:

* Identificar cliente por **CPF/CNPJ**
* Cadastro de veículo (**placa, marca, modelo, ano**)
* Inclusão de serviços solicitados
* Inclusão de peças/insumos necessários
* Orçamento gerado automaticamente (serviços + peças)
* Envio do orçamento para aprovação do cliente

**Acompanhamento da OS**:

* Status: **Recebida**, **Em diagnóstico**, **Aguardando aprovação**, **Em execução**, **Finalizada**, **Entregue**
* Mudança automática de status conforme ações do sistema
* Consulta do status pelo cliente via API

**Gestão administrativa (CRUDs)**:

* Clientes
* Veículos
* Serviços
* Peças e insumos (com controle de estoque)
* Listagem e detalhamento de OS
* Monitoramento do tempo médio de execução

### Qualidade e Segurança esperadas

* Autenticação **JWT** para APIs administrativas
* Validação de dados sensíveis (**CPF/CNPJ** e **placa**)
* Testes unitários e de integração para fluxos principais

### Requisitos técnicos esperados

* Back-end **monolítico**, com arquitetura em camadas (MVP)
* Banco de dados livre (com justificativa)
* APIs REST documentadas (Swagger ou similar)
* Dockerfile e docker-compose
* Cobertura mínima de **80%** nos domínios críticos
* Execução local simples (README)

---

## Fase 2 — Evolução para Qualidade, Resiliência e Escalabilidade (Fase Atual)

### Objetivo

Evoluir a aplicação criada na Fase 1 para suportar:

* Crescimento de demanda e novas unidades
* Alta disponibilidade
* Redução de riscos operacionais
* Provisionamento e deploy automatizados
* Código sustentável para evolução contínua
* Escalabilidade dinâmica em horários de pico

### Evolução esperada na aplicação

* Refatoração aplicando:

  * **Clean Code**
  * **Clean Architecture** ou **Arquitetura Hexagonal**
  * Testes automatizados para fluxos críticos

### Ajustes e novas APIs esperadas

* **Abertura de OS**: recebe dados e retorna identificador único da OS
* **Consulta de status da OS**
* **Aprovação de orçamento**: endpoint para receber aprovação/recusa externa
* **Listagem de OS**, com regras:

  * Ordenação por status:

    * Em Execução > Aguardando Aprovação > Diagnóstico > Recebida
  * Mais antigas primeiro
  * Não listar OS finalizadas/entregues (exclusão lógica)
* Atualização de status via ferramenta como **e-mail**

### Infraestrutura esperada

**Containerização**

* Dockerfile atualizado
* docker-compose para desenvolvimento local

**Kubernetes (K8s)**

* Manifestos YAML contemplando:

  * Deployments
  * Services
  * ConfigMaps e Secrets
  * HPA (Horizontal Pod Autoscaler) por CPU/memória

**Infraestrutura como Código (Terraform)**

* Provisionar cluster Kubernetes (local ou cloud)
* Provisionar banco de dados
* Documentar recursos e como aplicar

**CI/CD**
Pipeline com:

* Build da aplicação
* Execução de testes
* Build da imagem Docker
* Deploy no Kubernetes
* Deploy do banco
* Aplicação dos manifests YAML

### Organização esperada no repositório

* `/k8s`: manifestos Kubernetes
* `/infra`: scripts Terraform
* Pipeline CI/CD versionada
* README atualizado com arquitetura, instruções e links
