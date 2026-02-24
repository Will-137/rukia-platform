---
sidebar_position: 1
---

# FLUXO DE PRODUÇÃO — V1 (MVP)

## Objetivo

Definir formalmente o fluxo operacional mínimo da RUKIA para substituir o LINX na linha de frente da produção e controle de oficinas externas.

Este fluxo é a base para:

- Modelo de Dados
- Definição de APIs
- Implementação do MVP
- Evolução futura do domínio

Fluxo principal:

Pedido → Ordem de Produção → Envio para Oficina → Retorno → Custo → Dashboard

---

# 1. Visão Geral do Fluxo

1. Pedido é criado e aprovado.
2. A partir do Pedido, é gerada uma Ordem de Produção (OP).
3. A OP pode ser enviada total ou parcialmente para uma ou mais Oficinas.
4. A Oficina retorna os produtos (total ou parcial).
5. São registrados custos e possíveis perdas.
6. O Dashboard reflete o estado operacional em tempo real.

---

# 2. Entidades de Negócio

## 2.1 Pedido

Representa a demanda comercial.

Tabela: `pedidos`

Campos principais:
- id
- tenant_id
- cliente_id
- status (rascunho, aprovado, cancelado)
- data_criacao
- data_atualizacao

Itens do Pedido:

Tabela: `itens_pedido`

Campos:
- id
- pedido_id
- produto_id
- quantidade_total

Regra:
- Apenas pedidos com status "aprovado" podem gerar OP.

---

## 2.2 Ordem de Produção (OP)

Tabela: `ordens_producao`

Campos principais:
- id
- tenant_id
- pedido_id
- produto_id
- quantidade_planejada
- status (aberta, em_producao, enviada_oficina, retorno_parcial, finalizada, cancelada)
- data_prevista
- data_criacao

Regras:
- Uma OP pode ter múltiplos envios.
- Uma OP só é finalizada quando não há saldo pendente.

---

## 2.3 Oficina

Tabela: `oficinas`

Campos:
- id
- tenant_id
- nome
- tipo_servico
- valor_padrao_unitario (opcional)
- ativo

---

## 2.4 Envio para Oficina

Tabela: `envios_oficina`

Campos:
- id
- tenant_id
- ordem_producao_id
- oficina_id
- quantidade_enviada
- data_envio
- data_prevista_retorno
- status (enviado, retorno_parcial, retornado, encerrado)

Regras:
- Uma OP pode ter vários envios.
- O envio controla o saldo pendente.

---

## 2.5 Retorno da Oficina

Tabela: `retornos_oficina`

Campos:
- id
- tenant_id
- envio_oficina_id
- quantidade_retornada
- quantidade_perda
- data_retorno
- observacoes

Cálculo:
Produzido = soma(retornos) − soma(perdas)

---

## 2.6 Custo da Oficina

Tabela: `custos_oficina`

Campos:
- id
- tenant_id
- ordem_producao_id
- valor_unitario
- valor_total
- status (estimado, confirmado)
- referencia (opcional)

No MVP não é financeiro completo.

---

## 2.7 Log de Eventos Operacionais

Tabela: `eventos_operacionais`

Campos:
- id
- tenant_id
- entidade
- entidade_id
- tipo_evento
- dados_json
- data_evento
- usuario_id

Função:
Garantir rastreabilidade mínima sem implementar Event Sourcing.

---

# 3. Regras Operacionais

## 3.1 Saldo Pendente

saldo_pendente = quantidade_enviada − (retornada + perdas)

---

## 3.2 Conclusão da OP

Uma OP é finalizada quando:

- quantidade_planejada ≤ quantidade_produzida
- não há envios pendentes

---

## 3.3 Transições Permitidas da OP

- aberta → em_producao
- em_producao → enviada_oficina
- enviada_oficina → retorno_parcial
- retorno_parcial → enviada_oficina
- enviada_oficina → finalizada
- qualquer estado → cancelada (com justificativa)

---

# 4. Dashboard Operacional

Deve exibir:

- OP abertas
- OP em oficina
- OP atrasadas
- Produção por período
- Custo por OP
- Oficinas com pendências

---

# 5. Fora do Escopo (MVP)

- Controle profundo de estoque de tecido
- Financeiro completo
- Fiscal SEFAZ
- Integrações automáticas
- Custos industriais avançados
- MRP

---

# 6. Princípio Estrutural

Nenhuma funcionalidade será desenvolvida fora deste fluxo antes da validação do MVP com cliente piloto.