CHANGELOG (RUKIA)

Todas as mudanças relevantes do projeto RUKIA serão documentadas neste arquivo.

Este projeto segue:

Versionamento: SemVer (MAJOR.MINOR.PATCH)

Formato: baseado em Keep a Changelog

Idioma: PT-BR

Governança: cada versão exige build + testes verdes + atualização deste documento + criação de tag.

[0.2.0] - 2026-02-28
🎯 Objetivo da Versão

Consolidar os módulos iniciais (Cliente e Produto), padronizar o modelo de erro da API e estabelecer a Camada de Resiliência Operacional como base estrutural do backend.

✨ Adicionado
F07 – Cliente

Entidade Cliente com CRUD mínimo via API.

Value Object DocumentoFiscal:

Normalização (remoção de máscara).

Validação de formato.

Persistência EF Core com:

Fluent API.

ValueConverter para VO.

Índice único condicional para Documento.

Soft delete (Ativo = false).

Listagem com includeInativos.

Paginação via PagedResponse<T>.

F08 – Produto

Entidade Produto (MVP) com persistência completa.

Value Objects (DDD):

CodigoProduto (normalização + validação).

UnidadeMedida (normalização + validação).

Migration 0003_add_produtos.

Tabela produtos em snake_case.

Índice único condicional:

ix_produtos_codigo (UNIQUE WHERE ativo = true).

CRUD mínimo via Swagger (/produtos).

Soft delete + paginação.

🛡 Camada de Resiliência Operacional (CRO)

Implementação da base de proteção da API:

Middleware global de exceção.

Padronização de ProblemDetails.

Inclusão obrigatória de errorCode em todas as respostas de erro.

Separação clara entre:

Validação automática (ModelState)

Regra de domínio (DomainException)

Conflito de dados (UniqueViolation)

Erro inesperado (500)

Mapeamentos oficiais:

Situação	Status	errorCode
Validação automática	400	VALIDACAO_REQUISICAO_INVALIDA
Regra de domínio	400	*_DADOS_INVALIDOS
Recurso não encontrado	404	*_NAO_ENCONTRADO
Violação de unicidade	409	*_DUPLICADO
Erro inesperado	500	INFRA_ERRO_INTERNO
🧪 Testes

Testes de integração para:

Produtos (CRUD + erros)

Clientes (CRUD + erros)

Validação automática de:

Mensagens em PT-BR

Presença obrigatória de errorCode

Banco isolado via CustomWebApplicationFactory

Geração de documentos únicos para evitar flakiness

Build e testes 100% verdes

🔧 Alterado

Remoção de try/catch redundantes nos Controllers.

Controllers agora delegam exceções ao middleware global.

Padronização definitiva dos códigos de erro:

CLIENTE_*

PRODUTO_*

VALIDACAO_REQUISICAO_INVALIDA

INFRA_ERRO_INTERNO

🛠 Corrigido

Correções no mapeamento EF Core envolvendo Value Objects.

Ajustes de conversão para design-time (migrations) e runtime.

Ajustes em testes para evitar conflitos de dados entre execuções.

📌 Status da Fase

✔ Módulos Cliente e Produto estabilizados
✔ Estrutura de API protegida contra comportamento inconsistente
✔ Base pronta para expansão modular

[0.1.0] - 2026-02-28
✨ Adicionado
Bootstrap Backend

Arquitetura Modular Monolith (.NET 8).

Separação em:

Domain

Application

Infrastructure

Api

EF Core 8 configurado.

PostgreSQL Dev via Docker.

Estrutura inicial da solução.

Base de governança do backend.

🧭 Próximas Versões Planejadas
[0.3.0] (Planejado)

Expansão do padrão de erro para novos módulos.

Mapeamento de constraint name → errorCode específico.

Logging estruturado para auditoria.

Teste automatizado para erro 500 controlado.

Base para integração com módulo IRIS (IA).

🏛 Governança de Versionamento

Formato: MAJOR.MINOR.PATCH

MAJOR → Mudança incompatível.

MINOR → Nova funcionalidade compatível.

PATCH → Correções internas.

Toda versão deve:

Compilar sem erros.

Passar em todos os testes.

Atualizar este CHANGELOG.

Receber tag correspondente.

Projeto: RUKIA ERP SaaS
Responsável Técnico: Will Galvão
Arquitetura: Modular Monolith (.NET 8 + PostgreSQL)
Status Atual: Base estrutural estabilizada