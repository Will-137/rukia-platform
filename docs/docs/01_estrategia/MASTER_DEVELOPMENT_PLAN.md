---
sidebar_position: 2
---

# MASTER DEVELOPMENT PLAN — RUKIA

## Diretriz Principal

A RUKIA será construída com foco inicial em sobrevivência financeira, entregando um MVP funcional para produção e controle de oficinas externas, com crescimento progressivo até absorver completamente sistemas legados.

A estratégia é de migração gradual e controlada.

---

# FASES OFICIAIS

## F00 — Infraestrutura Base
**Objetivo:** Estrutura GitHub + Documentação + Ambiente SaaS  
**Status:** Concluído  

---

## F01 — Governança Estratégica
**Objetivo:** Congelar visão, princípios e roadmap  
**Resultado:** Base documental sólida  

---

## F02 — MVP Scope Freeze
**Objetivo:** Definir exatamente o que entra no protótipo vendável  
**Escopo MVP:**
- Pedido estruturado
- Ordem de Produção (OP)
- Módulo de Oficinas externas
- Rastreamento de status
- Dashboard mínimo
- Custo básico por OP

Nada além disso entra nesta fase.

---

## F03 — Arquitetura Modular MVP
**Objetivo:** Definir estrutura Modular Monolith com DDD

Módulos iniciais:
- Identity
- MasterData
- Orders
- Production
- Subcontracting
- Reporting

---

## F04 — Modelo de Dados MVP
**Objetivo:** Criar estrutura de banco mínima e correta  
Inclui:
- Multi-tenant
- Auditoria básica
- Índices essenciais

---

## F05 — Contracts & API
**Objetivo:** Definir endpoints e contratos do MVP  

---

## F06 — Build Vertical Slice
**Objetivo:** Entregar fluxo ponta-a-ponta:

Pedido → OP → Envio Oficina → Retorno → Dashboard

Cliente já consegue operar.

---

## F07 — Billing & Subscription
**Objetivo:** Proteger receita após uso real  
Inclui:
- Subscription
- Middleware enforcement
- Grace period
- Webhook pagamento

---

## F08 — DevOps & Estabilidade
**Objetivo:** Deploy previsível e logs estruturados

---

## F09 — Absorção Progressiva do Legado
**Objetivo:** Reduzir dependência do LINX e TINY  

Ordem provável:
1. Produção totalmente migrada
2. Estoque básico migrado
3. Custos industriais
4. Financeiro simplificado
5. Fiscal

---

# ESTRATÉGIA DE MIGRAÇÃO

A RUKIA não substituirá o LINX imediatamente.

Ela começará como sistema paralelo de produção, assumindo gradualmente os fluxos críticos até tornar o legado dispensável.

---

# CRITÉRIO DE SUCESSO DO MVP

O MVP é considerado bem-sucedido quando:

- A produção diária passa a ser operada pela RUKIA.
- Oficinas externas são controladas pela RUKIA.
- O cliente percebe ganho operacional imediato.
- Existe possibilidade real de cobrança mensal.

---

# REGRA ESTRUTURAL

Nenhuma nova funcionalidade fora do escopo MVP será iniciada antes da conclusão do Vertical Slice.