using Xunit;
using OrderService.Controller;
using OrderService.Data;
using OrderService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class OrdersControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "OrderTestDb")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        var context = GetDbContext();
        context.Orders.Add(new Order { Id = 1, OrderNumber = "ORD001", Items = new List<OrderItem>() });
        context.SaveChanges();

        var kafkaMock = new Mock<IKafkaProducer>();
        var controller = new OrdersController(context, kafkaMock.Object);

        var result = await controller.GetOrders();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Order>>>(result);
        var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(actionResult.Value);
        Assert.Single(orders);
    }
}