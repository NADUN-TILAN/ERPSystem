using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Text.Json;

namespace OrderService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IKafkaProducer _kafkaProducer;

        public OrdersController(AppDbContext context, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Items).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Send event to Kafka for other services (Inventory, Billing, etc.)
            var orderJson = JsonSerializer.Serialize(order);
            await _kafkaProducer.ProduceAsync("order-created", orderJson);

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }
    }
}
