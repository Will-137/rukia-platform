---
sidebar_position: 3
---

# MVP SCOPE — V1 (Production First)

## Objetivo

Entregar um protótipo funcional que substitua o LINX na linha de frente da produção e controle de oficinas externas, operando em paralelo ao sistema legado até sua absorção completa.

O MVP tem foco exclusivo no fluxo:

Pedido → OP → Oficina → Retorno → Custo → Dashboard

Nada fora desse fluxo será desenvolvido nesta versão.

---

# 1. ESCOPO INCLUÍDO

## 1.1 Cadastros Essenciais (Master Data do MVP)

Apenas os cadastros necessários para o fluxo operacional:

### Produto
- Código interno
- Referência
- Descrição
- Grade (tamanhos simplificados)
- Ativo/Inativo

### Cliente
- Nome
- Documento
- Contato básico

### Oficina Externa
- Nome
- Tipo de serviço
- Contato
- Valor padrão por peça (opcional)

### Usuário
- Nome
- Email
- Papel (Admin / Operador)

Nenhum outro cadastro entra no MVP.

---

## 1.2 Pedido

- Criação manual
- Cliente
- Lista de produtos
- Quantidade por grade
- Status:
  - Aberto
  - Aprovado
  - Cancelado

Sem integração automática com PDF nesta fase (pode ser manual).

---

## 1.3 Ordem de Produção (OP)

- Gerada a partir do pedido
- Produto
- Quantidade
- Status:
  - Aberta
  - Em Produção
  - Enviada para Oficina
  - Retorno Parcial
  - Finalizada
- Histórico de alterações

---

## 1.4 Módulo de Oficinas

- Envio de OP para oficina
- Registro de quantidade enviada
- Registro de quantidade recebida
- Controle de saldo pendente
- Registro de perda (se houver)
- Data de envio
- Data prevista
- Data de retorno

---

## 1.5 Custo Básico por OP

- Valor pago por oficina
- Quantidade produzida
- Custo unitário calculado
- Total por OP

Sem cálculo completo de custo industrial nesta fase.

---

## 1.6 Dashboard Operacional

- OP abertas
- OP em oficina
- OP atrasadas
- OP finalizadas
- Total produzido por período

---

# 2. ESCOPO EXCLUÍDO

Não entra no MVP:

- Controle avançado de estoque de tecido
- MRP
- Compras
- Financeiro completo
- Fiscal SEFAZ
- Billing SaaS
- Integrações automáticas
- Analytics SaaS
- BI avançado

---

# 3. REGRAS ESTRUTURAIS DO MVP

1. Toda funcionalidade deve estar ligada ao fluxo principal.
2. Nenhum módulo adicional será criado fora do escopo definido.
3. A meta é operação real com cliente piloto.
4. Perfeccionismo técnico não pode bloquear entrega funcional.

---

# 4. CRITÉRIO DE SUCESSO

O MVP é considerado validado quando:

- A produção diária começa a ser operada na RUKIA.
- As oficinas externas são controladas na RUKIA.
- O cliente percebe ganho operacional imediato.
- Existe viabilidade clara de cobrança mensal.