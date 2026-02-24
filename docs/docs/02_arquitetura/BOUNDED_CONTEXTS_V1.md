---
sidebar_position: 2
---

# CONTEXTOS DELIMITADOS — V1 (MVP)

## Objetivo

Definir oficialmente os Contextos Delimitados da RUKIA V1, garantindo separação clara de responsabilidades e evitando acoplamento indevido entre módulos.

A arquitetura adotada é:

Monólito Modular com DDD

---

# Visão Geral dos Contextos

Na V1 (MVP), a RUKIA é composta pelos seguintes contextos:

1. Identidade
2. Cadastros (Master Data)
3. Pedidos
4. Produção
5. Oficinas
6. Relatórios

---

# 1. Identidade

Responsável por:

- Usuários
- Perfis (Admin / Operador)
- Autenticação
- Controle por Tenant

Não contém regras de negócio da produção.

---

# 2. Cadastros (Master Data)

Responsável por:

- Produtos
- Clientes
- Oficinas

Regras:

- Não conhece Pedidos
- Não conhece Produção
- Apenas fornece dados

---

# 3. Pedidos

Responsável por:

- Criação de pedidos
- Itens do pedido
- Aprovação
- Cancelamento

Regras:

- Só gera Ordem de Produção
- Não controla oficina
- Não controla custo

---

# 4. Produção

Responsável por:

- Ordem de Produção
- Status da OP
- Controle de quantidade planejada
- Conclusão da OP

Regras:

- Pode solicitar envio para Oficina
- Não registra retorno diretamente (isso é Oficina)

---

# 5. Oficinas

Responsável por:

- Envios
- Retornos
- Perdas
- Custo por OP

Regras:

- Não cria OP
- Não cria Pedido
- Atua sobre OP existente

---

# 6. Relatórios

Responsável por:

- Consultas agregadas
- Dashboard operacional
- Indicadores simples

Regras:

- Não altera dados
- Apenas leitura

---

# Relação entre Contextos

Pedido → gera → Produção  
Produção → envia para → Oficinas  
Oficinas → impacta → Produção  
Relatórios → lê todos  

Cadastros → suportam todos  
Identidade → controla acesso

---

# Regra Arquitetural

Nenhum contexto pode acessar diretamente tabelas de outro contexto.

Interação deve ocorrer via:

- Camada de Aplicação
- Serviços internos
- Eventos operacionais

---

# Estrutura Física (Monólito Modular)

Sugestão de organização:

- ModuloIdentidade
- ModuloCadastros
- ModuloPedidos
- ModuloProducao
- ModuloOficinas
- ModuloRelatorios

Cada módulo possui:

- Entidades
- Casos de uso
- Repositórios
- Serviços de domínio

---

# Limite do MVP

Financeiro, Fiscal, Estoque Avançado e Billing não fazem parte da V1.

Eles serão novos contextos no futuro.

---

# Princípio Estrutural

Se um módulo precisar acessar regra interna de outro módulo, a arquitetura está errada.