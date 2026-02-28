using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rukia.Api.Contracts.Clientes;
using Rukia.Api.Contracts.Common;
using Rukia.Domain.Clientes;
using Rukia.Infrastructure.Persistence;

namespace Rukia.Api.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly RukiaDbContext _db;

        public ClientesController(RukiaDbContext db)
        {
            _db = db;
        }

        // POST /clientes
        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateClienteRequest request, CancellationToken ct)
        {
            try
            {
                var entity = new Cliente(
                    id: Guid.NewGuid(),
                    nome: request.Nome,
                    documento: request.Documento,
                    email: request.Email,
                    telefone: request.Telefone,
                    ativo: true
                );

                _db.Clientes.Add(entity);
                await _db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(entity));
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, out var constraint))
            {
                return Conflict(ProblemUnique(constraint));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ProblemValidation(ex.Message));
            }
        }

        // GET /clientes/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var entity = await _db.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                return NotFound(ProblemNotFound("Cliente não encontrado."));

            return Ok(Map(entity));
        }

        // GET /clientes?includeInativos=false&page=1&pageSize=20
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ClienteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> List(
            [FromQuery] bool includeInativos = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1)
                return BadRequest(ProblemValidation("Parâmetro 'page' deve ser >= 1."));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ProblemValidation("Parâmetro 'pageSize' deve estar entre 1 e 100."));

            var query = _db.Clientes.AsNoTracking();

            if (!includeInativos)
                query = query.Where(x => x.Ativo);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ClienteResponse
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Documento = x.Documento == null ? null : x.Documento.Value,
                    Email = x.Email,
                    Telefone = x.Telefone,
                    Ativo = x.Ativo,
                    CreatedAtUtc = x.CreatedAtUtc,
                    UpdatedAtUtc = x.UpdatedAtUtc
                })
                .ToListAsync(ct);

            var response = new PagedResponse<ClienteResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            };

            return Ok(response);
        }

        // PUT /clientes/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateClienteRequest request, CancellationToken ct)
        {
            var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return NotFound(ProblemNotFound("Cliente não encontrado."));

            try
            {
                entity.AtualizarDados(
                    nome: request.Nome,
                    documento: request.Documento,
                    email: request.Email,
                    telefone: request.Telefone
                );

                await _db.SaveChangesAsync(ct);

                return Ok(Map(entity));
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, out var constraint))
            {
                return Conflict(ProblemUnique(constraint));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ProblemValidation(ex.Message));
            }
        }

        // DELETE /clientes/{id}  (lógico: desativar)
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate([FromRoute] Guid id, CancellationToken ct)
        {
            var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return NotFound(ProblemNotFound("Cliente não encontrado."));

            entity.Desativar();
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        private static ClienteResponse Map(Cliente x) => new ClienteResponse
        {
            Id = x.Id,
            Nome = x.Nome,
            Documento = x.Documento == null ? null : x.Documento.Value,
            Email = x.Email,
            Telefone = x.Telefone,
            Ativo = x.Ativo,
            CreatedAtUtc = x.CreatedAtUtc,
            UpdatedAtUtc = x.UpdatedAtUtc
        };

        // ---------- Helpers (ProblemDetails + Unique) ----------

        private static bool IsUniqueViolation(DbUpdateException ex, out string? constraint)
        {
            constraint = null;

            if (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                constraint = pg.ConstraintName;
                return true;
            }

            return false;
        }

        private ProblemDetails ProblemValidation(string detail)
        {
            return new ProblemDetails
            {
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
                Instance = HttpContext?.Request?.Path.Value
            };
        }

        private ProblemDetails ProblemUnique(string? constraint)
        {
            var pd = new ProblemDetails
            {
                Title = "Unique constraint violation",
                Status = StatusCodes.Status409Conflict,
                Detail = "Cliente já existe para um campo único.",
                Instance = HttpContext?.Request?.Path.Value
            };

            if (!string.IsNullOrWhiteSpace(constraint))
                pd.Extensions["constraint"] = constraint;

            return pd;
        }

        private ProblemDetails ProblemNotFound(string detail)
        {
            return new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = detail,
                Instance = HttpContext?.Request?.Path.Value
            };
        }
    }
}