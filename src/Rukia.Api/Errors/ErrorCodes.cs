namespace Rukia.Api.Errors;

public static class ErrorCodes
{
    // Genéricos (se quiser manter)
    public const string DadosInvalidos = "DADOS_INVALIDOS";
    public const string RecursoNaoEncontrado = "RECURSO_NAO_ENCONTRADO";
    public const string ConflitoDados = "CONFLITO_DE_DADOS";

    // Cliente
    public const string ClienteNaoEncontrado = "CLIENTE_NAO_ENCONTRADO";
    public const string ClienteDocumentoDuplicado = "CLIENTE_DOCUMENTO_DUPLICADO";
    public const string ClienteEmailDuplicado = "CLIENTE_EMAIL_DUPLICADO";
    public const string ClienteDadosInvalidos = "CLIENTE_DADOS_INVALIDOS";

    // Produto
    public const string ProdutoNaoEncontrado = "PRODUTO_NAO_ENCONTRADO";
    public const string ProdutoCodigoDuplicado = "PRODUTO_CODIGO_DUPLICADO";
    public const string ProdutoDadosInvalidos = "PRODUTO_DADOS_INVALIDOS";

    // Cross-cutting (API)
    public const string ValidacaoRequisicaoInvalida = "VALIDACAO_REQUISICAO_INVALIDA";
    public const string InfraErroInterno = "INFRA_ERRO_INTERNO";

}
 
