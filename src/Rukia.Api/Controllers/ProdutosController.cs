using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rukia.Api.Contracts;
using Rukia.Api.Contracts.Produtos;
using Rukia.Api.Errors;
using Rukia.Domain.Produtos;
using Rukia.Domain.Produtos.ValueObjects;
using Rukia.Infrastructure.Persistence;

namespace Rukia.Api.Controllers;

[ApiController]
[Route("produtos")]
[Tags("Produtos")]
public sealed class ProdutosController : ControllerBase
{
    private readonly RukiaDbContext _db;

    public ProdutosController(RukiaDbContext db) => _db = db;

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoRequest request, CancellationToken ct)
    {
        var codigo = CodigoProduto.Create(request.Codigo);
        var unidade = UnidadeMedida.Create(request.UnidadeMedida);

        var produto = new Produto(
            id: Guid.NewGuid(),
            codigo: codigo,
            nome: request.Nome,
            descricao: request.Descricao,
            categoria: request.Categoria,
            unidadeMedida: unidade
        );

        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync(ct);

        var response = Map(produto);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var produto = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (produto is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Produto não encontrado.",
                errorCode: ErrorCodes.ProdutoNaoEncontrado
            ));
        }

        return Ok(Map(produto));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] bool includeInativos = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(ApiProblemDetails.Create(
                status: StatusCodes.Status400BadRequest,
                title: "Erro de validação",
                detail: "Parâmetros de paginação inválidos.",
                errorCode: ErrorCodes.ValidacaoRequisicaoInvalida
            ));
        }

        var query = _db.Produtos.AsQueryable();

        if (!includeInativos)
            query = query.Where(x => x.Ativo);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return Ok(new PagedResponse<ProdutoResponse>(items, page, pageSize, total));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProdutoRequest request, CancellationToken ct)
    {
        var produto = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (produto is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Produto não encontrado.",
                errorCode: ErrorCodes.ProdutoNaoEncontrado
            ));
        }

        var unidade = UnidadeMedida.Create(request.UnidadeMedida);

        produto.AtualizarDados(
            nome: request.Nome,
            descricao: request.Descricao,
            categoria: request.Categoria,
            unidadeMedida: unidade
        );

        await _db.SaveChangesAsync(ct);

        return Ok(Map(produto));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var produto = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (produto is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Produto não encontrado.",
                errorCode: ErrorCodes.ProdutoNaoEncontrado
            ));
        }

        produto.Desativar();
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    private static ProdutoResponse Map(Produto p) =>
        new(
            p.Id,
            p.Codigo.Value,
            p.Nome,
            p.Descricao,
            p.Categoria,
            p.UnidadeMedida.Value,
            p.Ativo,
            p.CreatedAtUtc,
            p.UpdatedAtUtc
        );
}