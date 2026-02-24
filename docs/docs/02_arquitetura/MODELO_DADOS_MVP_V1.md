---
sidebar_position: 3
---

# MODELO DE DADOS — MVP V1 (RUKIA)

## Objetivo

Definir o modelo de dados mínimo e correto para suportar o MVP da RUKIA, focado no fluxo:

Pedido → Ordem de Produção → Envio para Oficina → Retorno → Custo → Dashboard

Regras estruturais:
- Multiempresa via `tenant_id`
- Auditoria básica (datas e usuário)
- Integridade por chaves estrangeiras
- Índices para operação diária

---

# 1) Convenções

## 1.1 Nomes
- Tabelas: plural, snake_case (ex: `ordens_producao`)
- Colunas: snake_case (ex: `data_criacao`)
- Chaves primárias: `id` (uuid)
- Multiempresa: `tenant_id` (uuid) em todas as tabelas principais

## 1.2 Auditoria (mínimo)
Em tabelas operacionais:
- `data_criacao` (timestamp)
- `usuario_criacao_id` (uuid, opcional no MVP)
- `data_atualizacao` (timestamp, opcional)
- `usuario_atualizacao_id` (uuid, opcional)

> Observação: campos de deleção lógica podem ficar para fase posterior.

---

# 2) Tabelas — Contexto Identidade

## 2.1 tenants
- id (uuid, pk)
- nome (text)
- ativo (bool)
- data_criacao (timestamp)

**Índices**
- uq_tenants_nome (opcional)

## 2.2 usuarios
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- nome (text)
- email (text)
- senha_hash (text)
- papel (text) — (admin | operador)
- ativo (bool)
- data_criacao (timestamp)

**Índices**
- ix_usuarios_tenant_id
- uq_usuarios_tenant_email (unique)

---

# 3) Tabelas — Contexto Cadastros (Master Data)

## 3.1 clientes
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- nome (text)
- documento (text, opcional)
- contato (text, opcional)
- ativo (bool)
- data_criacao (timestamp)

**Índices**
- ix_clientes_tenant_id
- ix_clientes_nome (opcional)

## 3.2 produtos
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- codigo (text, opcional)
- referencia (text)
- descricao (text)
- ativo (bool)
- data_criacao (timestamp)

**Índices**
- ix_produtos_tenant_id
- ix_produtos_referencia (opcional)

## 3.3 tamanhos
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- codigo (text)   — ex: P, M, G, GG, 36, 38
- ordem (int)     — ordenação de exibição
- ativo (bool)

**Índices**
- ix_tamanhos_tenant_id
- uq_tamanhos_tenant_codigo (unique)

## 3.4 produto_tamanhos
Tabela de vínculo para habilitar grade simples por produto.

- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- produto_id (uuid, fk → produtos.id)
- tamanho_id (uuid, fk → tamanhos.id)

**Índices**
- ix_produto_tamanhos_produto_id
- ix_produto_tamanhos_tamanho_id
- uq_produto_tamanhos (tenant_id, produto_id, tamanho_id) unique

## 3.5 oficinas
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- nome (text)
- tipo_servico (text, opcional) — ex: costura, acabamento
- contato (text, opcional)
- valor_padrao_unitario (numeric(18,2), opcional)
- ativo (bool)
- data_criacao (timestamp)

**Índices**
- ix_oficinas_tenant_id
- ix_oficinas_nome (opcional)

---

# 4) Tabelas — Contexto Pedidos

## 4.1 pedidos
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- cliente_id (uuid, fk → clientes.id)
- status (text) — (rascunho | aprovado | cancelado)
- data_criacao (timestamp)
- data_atualizacao (timestamp, opcional)

**Índices**
- ix_pedidos_tenant_id
- ix_pedidos_cliente_id
- ix_pedidos_status (opcional)

## 4.2 itens_pedido
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- pedido_id (uuid, fk → pedidos.id)
- produto_id (uuid, fk → produtos.id)
- quantidade_total (int)

> Se for necessário por grade no MVP, criaremos `itens_pedido_tamanhos` depois.
> Por enquanto: quantidade_total.

**Índices**
- ix_itens_pedido_pedido_id
- ix_itens_pedido_produto_id

---

# 5) Tabelas — Contexto Produção

## 5.1 ordens_producao
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- pedido_id (uuid, fk → pedidos.id)
- produto_id (uuid, fk → produtos.id)
- quantidade_planejada (int)
- status (text) — (aberta | em_producao | enviada_oficina | retorno_parcial | finalizada | cancelada)
- data_prevista (date, opcional)
- data_criacao (timestamp)

**Índices**
- ix_ops_tenant_id
- ix_ops_status
- ix_ops_pedido_id
- ix_ops_produto_id

---

# 6) Tabelas — Contexto Oficinas

## 6.1 envios_oficina
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- ordem_producao_id (uuid, fk → ordens_producao.id)
- oficina_id (uuid, fk → oficinas.id)
- quantidade_enviada (int)
- data_envio (timestamp)
- data_prevista_retorno (date, opcional)
- status (text) — (enviado | retorno_parcial | retornado | encerrado)

**Índices**
- ix_envios_tenant_id
- ix_envios_op_id
- ix_envios_oficina_id
- ix_envios_status (opcional)

## 6.2 retornos_oficina
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- envio_oficina_id (uuid, fk → envios_oficina.id)
- quantidade_retornada (int)
- quantidade_perda (int, default 0)
- data_retorno (timestamp)
- observacoes (text, opcional)

**Índices**
- ix_retornos_tenant_id
- ix_retornos_envio_id
- ix_retornos_data (opcional)

## 6.3 custos_oficina
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- ordem_producao_id (uuid, fk → ordens_producao.id)
- valor_unitario (numeric(18,2))
- valor_total (numeric(18,2))
- status (text) — (estimado | confirmado)
- referencia (text, opcional)
- data_criacao (timestamp)

**Índices**
- ix_custos_tenant_id
- ix_custos_op_id
- ix_custos_status (opcional)

---

# 7) Tabelas — Log Operacional (MVP)

## 7.1 eventos_operacionais
- id (uuid, pk)
- tenant_id (uuid, fk → tenants.id)
- entidade (text)       — ex: "pedido", "ordem_producao", "envio_oficina"
- entidade_id (uuid)
- tipo_evento (text)    — ex: "pedido_aprovado", "envio_criado"
- dados_json (jsonb)
- data_evento (timestamp)
- usuario_id (uuid, opcional)

**Índices**
- ix_eventos_tenant_id
- ix_eventos_entidade_entidade_id
- ix_eventos_data (opcional)

---

# 8) Observações Importantes (MVP)

1. Grade por tamanho foi modelada de forma simples (produto_tamanhos).
2. Quantidade por tamanho em pedido/OP pode entrar como evolução (V1.1).
3. Custo é registro operacional (não financeiro completo).
4. Estoque de tecido e insumos fica fora do MVP.

---

# 9) Próximo passo canônico

Após este documento, criaremos:

- `API_CONTRACTS_MVP_V1.md`
- e o primeiro "Vertical Slice" do fluxo no código.