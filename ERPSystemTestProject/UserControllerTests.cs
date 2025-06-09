using Xunit;
using UserService.Controllers;
using UserService.Data;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "UserTestDb")
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void GetUsersReturnsAllUsers()
    {
        // Arrange
        var context = GetDbContext();
        context.Users.Add(new User { Id = 1, GitHubId = "gh1", Email = "test@example.com", Name = "Test User" });
        context.Users.Add(new User { Id = 2, GitHubId = "gh2", Email = "test2@example.com", Name = "Second User" });
        context.SaveChanges();

        var controller = new UserController(context);

        // Act
        var result = controller.GetUsers();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
        var users = Assert.IsAssignableFrom<IEnumerable<User>>(actionResult.Value);
        Assert.Equal(2, System.Linq.Enumerable.Count(users));
    }

    [Fact]
    public void GetUserByIdReturnsUserWhenUserExists()
    {
        // Arrange
        var context = GetDbContext();
        var user = new User { Id = 1, GitHubId = "gh1", Email = "test@example.com", Name = "Test User" };
        context.Users.Add(user);
        context.SaveChanges();

        var controller = new UserController(context);

        // Act
        var result = controller.GetUser(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<User>>(result);
        var returnedUser = Assert.IsType<User>(actionResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(user.Email, returnedUser.Email);
    }

    [Fact]
    public void GetUserByIdReturnsNotFoundWhenUserDoesNotExist()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new UserController(context);

        // Act
        var result = controller.GetUser(99);

        // Assert
        var actionResult = Assert.IsType<ActionResult<User>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
}