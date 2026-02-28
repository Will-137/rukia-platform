using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rukia.Api.Contracts;
using Rukia.Api.Contracts.Clientes;
using Rukia.Api.Errors;
using Rukia.Domain.Clientes;
using Rukia.Infrastructure.Persistence;

namespace Rukia.Api.Controllers;

[ApiController]
[Route("clientes")]
[Tags("Clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly RukiaDbContext _db;

    public ClientesController(RukiaDbContext db) => _db = db;

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request, CancellationToken ct)
    {
        var cliente = new Cliente(
            id: Guid.NewGuid(),
            nome: request.Nome,
            documento: request.Documento,
            email: request.Email,
            telefone: request.Telefone
        );

        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync(ct);

        var response = Map(cliente);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (cliente is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Cliente não encontrado.",
                errorCode: ErrorCodes.ClienteNaoEncontrado
            ));
        }

        return Ok(Map(cliente));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ClienteResponse>), StatusCodes.Status200OK)]
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

        var query = _db.Clientes.AsQueryable();

        if (!includeInativos)
            query = query.Where(x => x.Ativo);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return Ok(new PagedResponse<ClienteResponse>(items, page, pageSize, total));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateClienteRequest request, CancellationToken ct)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (cliente is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Cliente não encontrado.",
                errorCode: ErrorCodes.ClienteNaoEncontrado
            ));
        }

        cliente.AtualizarDados(
            nome: request.Nome,
            documento: request.Documento,
            email: request.Email,
            telefone: request.Telefone
        );

        await _db.SaveChangesAsync(ct);

        return Ok(Map(cliente));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (cliente is null)
        {
            return NotFound(ApiProblemDetails.Create(
                status: StatusCodes.Status404NotFound,
                title: "Não encontrado",
                detail: "Cliente não encontrado.",
                errorCode: ErrorCodes.ClienteNaoEncontrado
            ));
        }

        cliente.Desativar();
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    private static ClienteResponse Map(Cliente c) =>
        new(
            c.Id,
            c.Nome,
            //c.Documento?.Value,
            c.Documento?.ToString(),
            c.Email,
            c.Telefone,
            c.Ativo,
            c.CreatedAtUtc,
            c.UpdatedAtUtc
        );
}