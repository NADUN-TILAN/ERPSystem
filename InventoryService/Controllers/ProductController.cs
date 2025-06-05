using InventoryService.Data;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public IActionResult Index()
        {
            return View();
        }

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
        }
    }
}
