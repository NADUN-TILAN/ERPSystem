using Xunit;
using InventoryService.Controllers;
using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ERPSystemTestProject
{
    public class ProductControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureDeleted(); // Ensure clean state
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            // Arrange
            var context = GetDbContext();
            context.Products.Add(new Product { Sku = "A", Name = "ProductA", Quantity = 1, Price = 10 });
            context.Products.Add(new Product { Sku = "B", Name = "ProductB", Quantity = 2, Price = 20 });
            context.SaveChanges();

            var kafkaMock = new Mock<IKafkaProducer>();
            var controller = new ProductController(context, kafkaMock.Object);

            // Act
            var result = await controller.GetAllProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(actionResult.Value);
            Assert.Equal(2, ((List<Product>)products).Count);
        }

        [Fact]
        public async Task AddProduct_CreatesProduct_AndPublishesEvent()
        {
            // Arrange
            var context = GetDbContext();
            var kafkaMock = new Mock<IKafkaProducer>();
            kafkaMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(context, kafkaMock.Object);
            var product = new Product { Sku = "C", Name = "ProductC", Quantity = 3, Price = 30 };

            // Act
            var result = await controller.AddProduct(product);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("C", createdProduct.Sku);
            kafkaMock.Verify(k => k.ProduceAsync("product-events", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_UpdatesExistingProduct()
        {
            // Arrange
            var context = GetDbContext();
            var product = new Product { Sku = "D", Name = "ProductD", Quantity = 4, Price = 40 };
            context.Products.Add(product);
            context.SaveChanges();

            var kafkaMock = new Mock<IKafkaProducer>();
            kafkaMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(context, kafkaMock.Object);
            var updatedProduct = new Product { Sku = "D", Name = "Updated", Quantity = 5, Price = 50 };

            // Act
            var result = await controller.UpdateProduct(product.Id, updatedProduct);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Updated", returnedProduct.Name);
            Assert.Equal(5, returnedProduct.Quantity);
            kafkaMock.Verify(k => k.ProduceAsync("product-events", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_RemovesProduct_AndPublishesEvent()
        {
            // Arrange
            var context = GetDbContext();
            var product = new Product { Sku = "E", Name = "ProductE", Quantity = 6, Price = 60 };
            context.Products.Add(product);
            context.SaveChanges();

            var kafkaMock = new Mock<IKafkaProducer>();
            kafkaMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(context, kafkaMock.Object);

            // Act
            var result = await controller.DeleteProduct(product.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Products);
            kafkaMock.Verify(k => k.ProduceAsync("product-events", It.IsAny<string>()), Times.Once);
        }
    }
}