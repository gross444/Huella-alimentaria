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

    // GET: api/products (NUEVO - para listar todos)
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_db.Products);
    }

    [HttpPost]
    public ActionResult<Product> Create(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) 
            return BadRequest("Name es requerido");
        
        // Generar ID si no viene
        if (string.IsNullOrEmpty(p.Id))
            p.Id = Guid.NewGuid().ToString();
            
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
