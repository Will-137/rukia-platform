---
title: "Estrutura Canônica do Projeto (RUKIA)"
sidebar_position: 10
---

# Estrutura Canônica do Projeto (RUKIA)

**Status:** ATIVO  
**Última atualização:** 2026-02-28  
**Escopo:** Repositório `rukia-platform` (solution `Rukia.sln`)  
**Objetivo:** Definir a estrutura, padrões e comandos oficiais do projeto para desenvolvimento, suporte e manutenção, evitando divergências e “padrões paralelos”.

---

## 1) Princípios Canônicos

### 1.1 Idioma (Regra Obrigatória)
- **Todas as respostas e títulos voltados ao cliente devem estar em Português (PT-BR).**
- Mensagens de erro internas podem existir em inglês, porém:
  - **ProblemDetails (API)** deve usar **título e descrição em PT-BR**.
  - Logs técnicos podem permanecer em inglês (infra), mas não devem vazar para o cliente como UX principal.

### 1.2 Padrão de Evolução
- Alterações **pequenas, testáveis e verificáveis** antes de avançar.
- Cada fase (Fxx) deve:
  - construir em camadas (Domain → Infrastructure → Api → Testes)
  - criar migrations **canônicas**
  - validar persistência real no Postgres
  - finalizar com commit canônico

### 1.3 DDD / Arquitetura (confirmado)
- Stack: **.NET 8**
- Estilo: **Modular Monolith**
- Camadas: `Domain / Application / Infrastructure / Api`
- ORM: **EF Core 8.0.0**
- Banco: **PostgreSQL 16** (docker `rukia-postgres-dev`)
- DB Dev: `rukia_dev` | User: `rukia` | Port: `5432`

---

## 2) Estrutura de Pastas (Fonte de Verdade confirmada)

### 2.1 Raiz de trabalho
- Repo: `rukia-platform`
- Solution:
  - `C:\DEV\RUKIA\rukia-platform\src\Rukia.sln`

### 2.2 Projetos (confirmado)
- `src\Rukia.Domain\`
- `src\Rukia.Application\`
- `src\Rukia.Infrastructure\`
- `src\Rukia.Api\`
- `src\Rukia.Api.IntegrationTests\` (existente)

---

## 3) Convenções de Banco de Dados (Obrigatórias)

### 3.1 Naming (Regra Obrigatória)
- Tabelas e colunas: **snake_case minúsculo**
- **Sem aspas** nos nomes (evitar CamelCase no DB)
- Não mexer em `__EFMigrationsHistory`

### 3.2 Migrations (Regra Obrigatória)
- Migrations ficam no **projeto Infrastructure**
- MigrationId: timestamp + nome canônico (gerado pelo EF)
- Nome canônico por fase:
  - `0002_add_clientes`
  - `0003_add_produtos`
- Exemplo real confirmado:
  - `20260228031102_Initial_Create`
  - `20260228042724_0002_add_clientes`
  - `20260228114101_0003_add_produtos`

### 3.3 Índices e Constraints (Padrão canônico)
- Índices nomeados com prefixo `ix_...`
- Únicos condicionais (soft delete / ativo):
  - Ex.: Produtos: `ix_produtos_codigo` **UNIQUE** com filtro `ativo = true`
  - Ex.: Clientes: índices com filtro para campos opcionais (`IS NOT NULL`)
- Sempre documentar:
  - nome do índice
  - coluna(s)
  - filtro (quando houver)
  - razão do filtro

---

## 4) Padrões Canônicos de Domain (DDD)

### 4.1 Regra: Domain é “puro”
- Sem EF Core
- Sem DataAnnotations
- Sem dependência de Infrastructure/Api

### 4.2 Entidades (Entity)
- `private constructor` para EF (quando aplicável)
- Invariantes validadas no construtor e/ou em métodos de domínio
- Campos com `private set`
- Auditoria simples:
  - `CreatedAtUtc`
  - `UpdatedAtUtc`

### 4.3 Value Objects (VO)
- Usar VO quando houver regra de normalização e validação que represente o domínio.
- Padrão canônico:
  - normalização (trim, uppercase quando aplicável)
  - validação (min/max)
  - `Create(...)` como fábrica
- Importante (confirmado):
  - Quando VO for `record struct` (value type), **não usar `?? throw`** porque não é nullable.

**Exemplo (conceitual):**
- `CodigoProduto` (VO obrigatório)
- `UnidadeMedida` (VO obrigatório)
- Cliente possui `DocumentoFiscal` (VO com converter EF)

---

## 5) Padrões Canônicos de Infrastructure (EF Core)

### 5.1 Mapeamento EF: Fluent API
- Para cada entidade: `IEntityTypeConfiguration<T>`
- Pasta padrão confirmada:
  - `src\Rukia.Infrastructure\Persistence\Configurations\`

### 5.2 ValueConverter para VO (Regra Obrigatória)
Sempre que uma propriedade do Domain for VO (ex.: `CodigoProduto`), o EF deve mapear usando `HasConversion(...)`.

**Padrão canônico:**
- Converter VO → string no banco
- Converter string do banco → VO via `.Create(...)`

### 5.3 DbContext (confirmado)
- Context: `RukiaDbContext`
- Namespace/Type visto no comando:
  - `Rukia.Infrastructure.Persistence.RukiaDbContext`

---

## 6) Padrões Canônicos de API

### 6.1 Endpoints CRUD mínimos por domínio (confirmado como padrão)
- POST `/entidade` (create)
- GET `/entidade/{id}` (get)
- GET `/entidade` (list paginado + includeInativos)
- PUT `/entidade/{id}` (update)
- DELETE `/entidade/{id}` (delete lógico: desativar)

### 6.2 Respostas padrão (ProblemDetails)
- 400: erro de validação
- 404: não encontrado
- 409: conflito (unique violation `23505`)
- Os textos voltados ao cliente devem estar em PT-BR.

### 6.3 Paginação (confirmado)
- Envelope: `PagedResponse<T>`
- Propriedades:
  - `Items`
  - `Page`
  - `PageSize`
  - `Total`

---

## 7) Domínios já consolidados

### 7.1 F07 — Cliente (confirmado)
- Tabela: `clientes`
- VO: `DocumentoFiscal` com converter EF
- CRUD mínimo via Swagger
- Delete lógico (desativar)
- Paginação com envelope
- ProblemDetails padronizado
- Índices únicos (com filtros)

### 7.2 F08 — Produto (confirmado)
- Tabela: `produtos`
- Colunas:
  - `id` (uuid)
  - `codigo` (varchar(50), obrigatório)
  - `nome` (varchar(200), obrigatório)
  - `descricao` (varchar(1000), opcional)
  - `categoria` (varchar(100), opcional)
  - `unidade_medida` (varchar(10), obrigatório)
  - `ativo` (bool, default true)
  - `created_at_utc` / `updated_at_utc`
- Índices:
  - `ix_produtos_codigo` UNIQUE com filtro `ativo = true`

---

## 8) Comandos Oficiais (CLI)

> Observação: todos os comandos abaixo são executados a partir de:
`C:\DEV\RUKIA\rukia-platform\src`

### 8.1 Build e execução
```powershell
dotnet build .\Rukia.sln
dotnet run --project .\Rukia.Api\Rukia.Api.csproj
8.2 Criar migration (padrão canônico)

Ajustar --output-dir conforme pasta padrão usada no repo (confirmado que vocês usaram Migrations em um ponto do fluxo).

dotnet ef migrations add 0003_add_produtos `
  --project .\Rukia.Infrastructure\Rukia.Infrastructure.csproj `
  --startup-project .\Rukia.Api\Rukia.Api.csproj `
  --context RukiaDbContext `
  --output-dir Migrations
8.3 Aplicar migrations no banco
dotnet ef database update `
  --project .\Rukia.Infrastructure\Rukia.Infrastructure.csproj `
  --startup-project .\Rukia.Api\Rukia.Api.csproj `
  --context RukiaDbContext
8.4 Inspecionar DbContext (diagnóstico)
dotnet ef dbcontext info `
  --project .\Rukia.Infrastructure\Rukia.Infrastructure.csproj `
  --startup-project .\Rukia.Api\Rukia.Api.csproj `
  --context RukiaDbContext
9) Comandos Oficiais (Banco via Docker)
9.1 Listar bancos
docker exec -it rukia-postgres-dev psql -U rukia -d postgres -c "\l"
9.2 Entrar no banco dev
docker exec -it rukia-postgres-dev psql -U rukia -d rukia_dev
9.3 Validações SQL canônicas
-- migrations aplicadas
select * from "__EFMigrationsHistory" order by "MigrationId" desc limit 10;

-- estrutura
\d produtos
\d clientes

-- amostra de dados
select * from produtos order by created_at_utc desc limit 10;
select * from clientes order by created_at_utc desc limit 10;
10) Testes Automatizados (Padrão Canônico)
10.1 Objetivo

Cada domínio deve ter testes automatizados cobrindo:

CRUD mínimo completo

dados inválidos (400)

duplicidade/índice único (409)

delete lógico e comportamento com includeInativos

10.2 Projeto de testes (confirmado)

src\Rukia.Api.IntegrationTests\Rukia.Api.IntegrationTests.csproj

10.3 Comando para rodar todos os testes
dotnet test .\Rukia.Api.IntegrationTests\Rukia.Api.IntegrationTests.csproj
10.4 Padrão para rodar testes por módulo (recomendado)

Regra: cada classe de teste deve ter:

[Trait("Modulo", "Produtos")]

ou

[Trait("Modulo", "Clientes")]

Então os comandos ficam:

Rodar teste de Produto:

dotnet test .\Rukia.Api.IntegrationTests\Rukia.Api.IntegrationTests.csproj --filter "Modulo=Produtos"

Rodar teste de Cliente:

dotnet test .\Rukia.Api.IntegrationTests\Rukia.Api.IntegrationTests.csproj --filter "Modulo=Clientes"

Modelo para futuro:

dotnet test .\Rukia.Api.IntegrationTests\Rukia.Api.IntegrationTests.csproj --filter "Modulo=NomeDoModulo"
11) Soft Delete (Regra Canônica)
11.1 Definição

DELETE não remove registro.

DELETE desativa:

Ativo = false

UpdatedAtUtc = now

11.2 Listagem

Por padrão: retorna apenas Ativo = true

Com includeInativos=true: retorna ativos e inativos

11.3 Índice único condicional

Para permitir reuso de “chaves naturais” após desativação:

Ex.: codigo do produto pode ser reutilizado somente se o antigo estiver inativo.

12) Checklist obrigatório antes do Commit (Canônico)
12.1 Compilação

dotnet build .\Rukia.sln sem erros

12.2 Banco

Migration criada e aplicada

__EFMigrationsHistory atualizado

Tabelas/colunas snake_case OK

Índices com nomes canônicos OK

12.3 API

Endpoints CRUD do domínio acessíveis no Swagger

400/404/409 no padrão ProblemDetails e PT-BR

12.4 Testes

dotnet test passando

(Recomendado) filtro por módulo funcionando

13) Mensagens de Commit Canônicas
13.1 Padrão

feat: F07 cliente ...

feat: F08 produto ...

13.2 Exemplo (F08)

feat: F08 produto (primeira entidade real) com VO, mapping EF, migrations e endpoints

14) Observações e Campos “a confirmar”

Os itens abaixo existem como boas práticas, mas precisam ser confirmados diretamente no repositório antes de serem considerados regra:

local exato da pasta de migrations (foi usado Migrations no fluxo atual; confirmar se existe padrão alternativo Persistence\Migrations)

local do PagedResponse<T> e namespace oficial

padrão de “Seeder”/dados iniciais (se existe e como é controlado)

padrão de tradução de mensagens de validação em todas as rotas

15) Encerramento

Este documento é a fonte de verdade da estrutura atual do RUKIA no estado confirmado em 2026-02-28.
Toda mudança estrutural deve:

ser aplicada em pequena alteração

ser testada (API + SQL + testes automatizados)

ser registrada em commit e, se necessário, atualizar este documento.