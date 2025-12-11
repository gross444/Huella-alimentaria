using HuellaApi.Data;
using HuellaApi.Models;
using HuellaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HuellaApi.Controllers;

[ApiController]
[Route("api/trace")]
public class TraceController : ControllerBase
{
    private readonly InMemoryDb _db;
    private readonly HashService _hash;
    private readonly QrService _qr;
    private readonly ILogger<TraceController> _log;

    public TraceController(InMemoryDb db, HashService hash, QrService qr, ILogger<TraceController> log)
    {
        _db = db; _hash = hash; _qr = qr; _log = log;
    }

    [HttpPost("batch")]
    public ActionResult<Batch> CreateBatch(Batch b)
    {
        if (!_db.Products.Any(p => p.Id == b.ProductId))
            return BadRequest("ProductId no existe");

        if (string.IsNullOrWhiteSpace(b.LotCode))
            return BadRequest("LotCode es requerido");

        _db.Batches.Add(b);
        return CreatedAtAction(nameof(GetBatch), new { id = b.Id }, b);
    }

    [HttpGet("batch/{id}")]
    public ActionResult<object> GetBatch(string id)
    {
        var b = _db.Batches.FirstOrDefault(x => x.Id == id);
        if (b is null) return NotFound();

        var events = _db.Events.Where(e => e.BatchId == id)
            .OrderBy(e => e.Timestamp).ToList();

        // validar cadena
        string prev = "";
        bool ok = true;
        foreach (var e in events)
        {
            // recomputar esperado con prev real
            var tmp = new TraceEvent
            {
                BatchId = e.BatchId,
                Stage = e.Stage,
                Location = e.Location,
                Timestamp = e.Timestamp,
                Data = e.Data,
                PrevHash = prev
            };
            var expected = _hash.ComputeFor(tmp);
            if (!string.Equals(expected, e.Hash, StringComparison.OrdinalIgnoreCase))
            {
                ok = false; break;
            }
            prev = e.Hash;
        }

        return Ok(new { batch = b, events, chainValid = ok });
    }

    public record AddEventDto(string BatchId, string Stage, string Location, string Data);

    [HttpPost("event")]
    public ActionResult<TraceEvent> AddEvent(AddEventDto dto)
    {
        if (!_db.Batches.Any(b => b.Id == dto.BatchId))
            return BadRequest("BatchId no existe");

        var last = _db.Events.Where(e => e.BatchId == dto.BatchId)
            .OrderBy(e => e.Timestamp).LastOrDefault();

        var ev = new TraceEvent
        {
            BatchId = dto.BatchId,
            Stage   = dto.Stage,
            Location= dto.Location,
            Data    = string.IsNullOrWhiteSpace(dto.Data) ? "{}" : dto.Data,
            PrevHash = last?.Hash ?? ""
        };
        ev.Hash = _hash.ComputeFor(ev);

        _db.Events.Add(ev);
        _log.LogInformation("Nuevo evento {Id} para batch {Batch}", ev.Id, ev.BatchId);
        return CreatedAtAction(nameof(GetBatch), new { id = dto.BatchId }, ev);
    }

    // QR para consulta pública
    [HttpGet("batch/{id}/qr")]
    public IActionResult GetBatchQr(string id)
    {
        var url = $"{Request.Scheme}://{Request.Host}/api/trace/batch/{id}";
        var png = _qr.GeneratePng(url);
        return File(png, "image/png");
    }
}
