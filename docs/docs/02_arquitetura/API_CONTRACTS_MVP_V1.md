---
sidebar_position: 4
---

# CONTRATOS DE API — MVP V1 (RUKIA)

## Objetivo

Definir os contratos mínimos de API para suportar o MVP da RUKIA, baseado no fluxo:

Pedido → Ordem de Produção → Envio para Oficina → Retorno → Custo → Dashboard

Regras:
- Todas as rotas são versionadas: `/api/v1`
- Autenticação obrigatória via JWT
- `tenant_id` é resolvido pelo token (não vem no body)
- Erros padronizados
- CRUD mínimo para cadastros essenciais

---

# 1) Padrões Gerais

## 1.1 Headers
Obrigatórios:
- `Authorization: Bearer <token>`

Opcional:
- `X-Request-Id` (para rastreabilidade)

## 1.2 Resposta de erro (padrão)
```json
{
  "codigo": "VALIDACAO_ERRO",
  "mensagem": "Mensagem amigável",
  "detalhes": [
    { "campo": "nome", "erro": "Campo obrigatório" }
  ],
  "correlationId": "uuid-opcional"
}
1.3 Paginação (padrão)

Query:

page (default 1)

pageSize (default 20)

Resposta:

{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "total": 0
}
2) Identidade (MVP)
2.1 Autenticação
POST /api/v1/auth/login

Request:

{ "email": "string", "senha": "string" }

Response:

{
  "accessToken": "jwt",
  "expiresIn": 3600,
  "usuario": { "id": "uuid", "nome": "string", "papel": "admin" }
}
POST /api/v1/auth/logout

Response: 204 No Content

3) Cadastros Essenciais (Master Data)
3.1 Clientes
GET /api/v1/clientes

Response (paginado)

GET /api/v1/clientes/{id}

Response:

{ "id": "uuid", "nome": "string", "documento": "string", "contato": "string", "ativo": true }
POST /api/v1/clientes

Request:

{ "nome": "string", "documento": "string", "contato": "string", "ativo": true }

Response: 201 Created + objeto

PUT /api/v1/clientes/{id}

Request igual ao POST
Response: 200 OK

DELETE /api/v1/clientes/{id}

Response: 204 No Content (remoção lógica opcional; no MVP pode ser "ativo=false")

3.2 Produtos
GET /api/v1/produtos
GET /api/v1/produtos/{id}
POST /api/v1/produtos

Request:

{ "codigo": "string", "referencia": "string", "descricao": "string", "ativo": true }
PUT /api/v1/produtos/{id}
DELETE /api/v1/produtos/{id}
3.3 Tamanhos (grade simples)
GET /api/v1/tamanhos
POST /api/v1/tamanhos

Request:

{ "codigo": "string", "ordem": 1, "ativo": true }
PUT /api/v1/tamanhos/{id}
DELETE /api/v1/tamanhos/{id}
3.4 Produto x Tamanhos
GET /api/v1/produtos/{produtoId}/tamanhos

Response:

{
  "produtoId": "uuid",
  "tamanhos": [
    { "id": "uuid", "codigo": "P", "ordem": 1 }
  ]
}
PUT /api/v1/produtos/{produtoId}/tamanhos

Request:

{ "tamanhoIds": ["uuid", "uuid"] }

Response: 200 OK

3.5 Oficinas
GET /api/v1/oficinas
GET /api/v1/oficinas/{id}
POST /api/v1/oficinas

Request:

{
  "nome": "string",
  "tipoServico": "string",
  "contato": "string",
  "valorPadraoUnitario": 0,
  "ativo": true
}
PUT /api/v1/oficinas/{id}
DELETE /api/v1/oficinas/{id}
4) Pedidos
4.1 Listar / Detalhar
GET /api/v1/pedidos
GET /api/v1/pedidos/{id}

Response:

{
  "id": "uuid",
  "clienteId": "uuid",
  "status": "rascunho",
  "itens": [
    { "id": "uuid", "produtoId": "uuid", "quantidadeTotal": 100 }
  ],
  "dataCriacao": "2026-03-04T10:00:00Z"
}
4.2 Criar Pedido
POST /api/v1/pedidos

Request:

{
  "clienteId": "uuid",
  "itens": [
    { "produtoId": "uuid", "quantidadeTotal": 100 }
  ]
}

Response: 201 Created + objeto

4.3 Atualizar Pedido (somente rascunho)
PUT /api/v1/pedidos/{id}

Request igual ao POST

Regra:

Só permite alteração se status = rascunho.

4.4 Aprovar Pedido
POST /api/v1/pedidos/{id}/aprovar

Response: 200 OK

4.5 Cancelar Pedido
POST /api/v1/pedidos/{id}/cancelar

Request:

{ "motivo": "string" }

Response: 200 OK

5) Produção (OP)
5.1 Gerar OP a partir de Pedido aprovado
POST /api/v1/ordens-producao/gerar

Request:

{ "pedidoId": "uuid" }

Response:

{
  "ordensProducao": [
    { "id": "uuid", "pedidoId": "uuid", "produtoId": "uuid", "quantidadePlanejada": 100, "status": "aberta" }
  ]
}
5.2 Listar / Detalhar OP
GET /api/v1/ordens-producao

Filtros (query):

status

produtoId

pedidoId

oficinaId (via join dos envios)

GET /api/v1/ordens-producao/{id}
5.3 Alterar Status da OP (controlado)
POST /api/v1/ordens-producao/{id}/status

Request:

{ "status": "em_producao" }

Regra:

Validar transições permitidas (conforme FLUXO_PRODUCAO_V1).

6) Oficinas (Envios / Retornos / Custos)
6.1 Criar Envio para Oficina
POST /api/v1/envios-oficina

Request:

{
  "ordemProducaoId": "uuid",
  "oficinaId": "uuid",
  "quantidadeEnviada": 100,
  "dataPrevistaRetorno": "2026-03-15"
}

Response: 201 Created

6.2 Listar Envios
GET /api/v1/envios-oficina

Filtros:

ordemProducaoId

oficinaId

status

6.3 Registrar Retorno
POST /api/v1/retornos-oficina

Request:

{
  "envioOficinaId": "uuid",
  "quantidadeRetornada": 60,
  "quantidadePerda": 0,
  "observacoes": "string"
}

Response: 201 Created

Regra:

Não permitir retorno acima do saldo pendente.

6.4 Registrar Custo de Oficina (simplificado)
POST /api/v1/custos-oficina

Request:

{
  "ordemProducaoId": "uuid",
  "valorUnitario": 2.50,
  "valorTotal": 250.00,
  "status": "confirmado",
  "referencia": "recibo/nota"
}

Response: 201 Created

7) Dashboard (somente leitura)
7.1 Resumo Operacional
GET /api/v1/dashboard/resumo

Response:

{
  "opsAbertas": 10,
  "opsEmOficina": 7,
  "opsAtrasadas": 2,
  "opsFinalizadasHoje": 3
}
7.2 Pendências por Oficina
GET /api/v1/dashboard/oficinas-pendencias

Response:

{
  "items": [
    { "oficinaId": "uuid", "nome": "Oficina X", "opsPendentes": 5, "quantidadePendente": 120 }
  ]
}
8) Observações (MVP)

Grade por tamanho em pedidos/OP pode ser evolução (V1.1).

Financeiro real (contas a pagar/receber) fica fora do MVP.

Integração com LINX/TINY fica fora do MVP (migração será fase posterior).

Segurança e auditoria avançadas evoluem depois do MVP validado.