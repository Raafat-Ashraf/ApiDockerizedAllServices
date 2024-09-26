using DemoApiImageWithLocalDatabase.Api.Data;
using DemoApiImageWithLocalDatabase.Api.Models;
using DemoApiImageWithLocalDatabase.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApiImageWithLocalDatabase.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ProductDbContext context, ICacheService cacheService, ProductDbContext appDbContext) : ControllerBase
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly ProductDbContext _appDbContext = appDbContext;


    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        var cacheData = _cacheService.GetData<IEnumerable<Product>>("products");
        if (cacheData != null && cacheData.Count() > 0)
            return Ok(cacheData);

        cacheData = await _appDbContext.Product.ToListAsync();

        var expiryTime = DateTimeOffset.Now.AddSeconds(30);

        _cacheService.SetData<IEnumerable<Product>>("products", cacheData, expiryTime);

        return Ok(cacheData);
    }

    // GET: api/Product/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Product.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // PUT: api/Product/5
    // To protect from overPosting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        context.Entry(product).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Product
    // To protect from overPosting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        context.Product.Add(product);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Product/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        context.Product.Remove(product);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return context.Product.Any(e => e.Id == id);
    }
}
