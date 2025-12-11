using HuellaApi.Data;
using HuellaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HuellaApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly InMemoryDb _db;

    public ProductsController(InMemoryDb db) => _db = db;

    [HttpPost]
    public ActionResult<Product> Create(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) return BadRequest("Name es requerido");
        _db.Products.Add(p);
        return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> Get(string id)
    {
        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }
}
