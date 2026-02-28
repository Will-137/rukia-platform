# Changelog (RUKIA)

Todas as mudanças relevantes do projeto serão documentadas aqui, por versão.

Padrões:
- Versionamento: SemVer (MAJOR.MINOR.PATCH)
- Formato: inspirado em "Keep a Changelog"
- Idioma: PT-BR (mensagens voltadas ao time e manutenção)

---

## [0.2.0] - 2026-02-28

### Adicionado
- F07 (Cliente):
  - Entidade Cliente com CRUD mínimo via API.
  - Value Object DocumentoFiscal com normalização e validação.
  - Persistência EF Core com Fluent API, conversores e constraints.
  - Soft delete (Ativo=false) e listagem com includeInativos.
  - Respostas padronizadas (ProblemDetails) e paginação (PagedResponse<T>).
  - Índices únicos (com filtro quando aplicável).

- F08 (Produto):
  - Entidade Produto (MVP) com persistência completa.
  - Value Objects (DDD):
    - CodigoProduto (normalização + validação).
    - UnidadeMedida (normalização + validação).
  - Migration 0003_add_produtos e criação da tabela produtos (snake_case).
  - Índice único condicional:
    - ix_produtos_codigo (UNIQUE WHERE ativo = true).
  - CRUD mínimo via Swagger (/produtos) com soft delete e paginação.

### Alterado
- Padronização de mensagens e respostas de erro no formato ProblemDetails (PT-BR) para:
  - 400 (validação)
  - 404 (não encontrado)
  - 409 (violação de unicidade / conflito)

### Corrigido
- Correções no mapeamento EF Core envolvendo Value Objects (ValueConverter), garantindo funcionamento em design-time (migrations) e runtime.

---

## [0.1.0] - 2026-02-28

### Adicionado
- Bootstrap do backend:
  - Arquitetura Modular Monolith (.NET 8) com separação Domain/Application/Infrastructure/Api.
  - EF Core 8 configurado.
  - PostgreSQL dev (docker) configurado.
  - Base inicial de projeto e estrutura de solução.

---