using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using PortfolioMVC.Controllers;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;
using Xunit;

/*
 * Resources:
 * https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing
 * https://docs.microsoft.com/en-us/dotnet/core/testing/
 * https://docs.microsoft.com/en-us/ef/core/testing/
 * https://github.com/moq/moq4
 */
namespace PortfolioMVC.Tests.Controllers
{
    [TestSubject(typeof(ManagerController))]
    public class ManagerControllerTest
    {
        
        [Fact]
        public async Task GetManagers_List()
        {
            // Create DbContextOptions for AppDbContext using an in-memory database.
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            
            // Pass the options to the mock constructor.
            var mockContext = new Mock<AppDbContext>(options);
            
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Test Manager 1", Email = "manager1@example.com", Department = Department.Hr, Picture = "picture1.png" },
                new Manager { Id = 2, Name = "Test Manager 2", Email = "manager2@example.com", Department = Department.It, Picture = "picture2.jpg" }
            };

            // Use extension to create a mock DbSet from the list.
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);

            var controller = new ManagerController(mockContext.Object);
            var result = await controller.GetManagers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ManagerDto>>(okResult.Value);
            Assert.Equal(managers.Count, returnValue.Count());
            Assert.Equal(managers.First().Email, returnValue.First().Email);
        }
        
        [Fact]
        public async Task GetManagers_EmptyList()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>();
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            var controller = new ManagerController(mockContext.Object);
            var result = await controller.GetManagers();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ManagerDto>>(okResult.Value);

            Assert.Empty(returnValue);
        }
        
        [Fact]
        public async Task GetManager_ManagerExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Test Manager 1", Email = "manager1@example.com", Department = Department.Hr, Picture = "picture1.png" },
                new Manager { Id = 2, Name = "Test Manager 2", Email = "manager2@example.com", Department = Department.It, Picture = "picture2.jpg" }
            };
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            var controller = new ManagerController(mockContext.Object);
            int testId = 1;
            var result = await controller.GetManager(testId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var managerDto = Assert.IsType<ManagerDto>(okResult.Value);
            Assert.Equal(testId, managerDto.Id);
            Assert.Equal(managers.First(m => m.Id == testId).Email, managerDto.Email);
        }
        
        [Fact]
        public async Task GetManager_ManagerNotExist()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Test Manager 1", Email = "manager1@example.com", Department = Department.Hr, Picture = "picture1.png" }
            };
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            var controller = new ManagerController(mockContext.Object);
            int testId = 999;
            var result = await controller.GetManager(testId);
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
        [Fact]
        public async Task CreateManager_IsInvalid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>();
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            var controller = new ManagerController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "Required");
            var newManagerDto = new ManagerDto 
            { 
                Name = "", 
                Email = "test@test.com", 
                Department = Department.It, 
                Picture = "pic.jpg" 
            };
            var result = await controller.CreateManager(newManagerDto);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task CreateManager_IsValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var managers = new List<Manager>();
            var mockContext = new Mock<AppDbContext>(options);
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockDbSet.Setup(m => m.Add(It.IsAny<Manager>()))
                .Callback<Manager>(m => { m.Id = 42; managers.Add(m); });
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            var controller = new ManagerController(mockContext.Object);
            var newManagerDto = new ManagerDto 
            { 
                Name = "New Manager", 
                Email = "new@test.com", 
                Department = Department.Hr, 
                Picture = "pic.png" 
            };
            var result = await controller.CreateManager(newManagerDto);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnDto = Assert.IsType<ManagerDto>(createdAtActionResult.Value);
            Assert.Equal(42, returnDto.Id);
            Assert.Equal(newManagerDto.Email, returnDto.Email);
        }
        
        [Fact]
        public async Task UpdateManager_NotMatchId()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Manager 1", Email = "manager1@test.com", Department = Department.Hr, Picture = "pic1.png" }
            };
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            var controller = new ManagerController(mockContext.Object);
            var updateDto = new ManagerDto 
            { 
                Id = 2, 
                Name = "Manager 2", 
                Email = "manager2@test.com", 
                Department = Department.It, 
                Picture = "pic2.png" 
            };
            var result = await controller.UpdateManager(1, updateDto);
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public async Task UpdateManager_NotExist()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockContext = new Mock<AppDbContext>(options);
            var managers = new List<Manager>();
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.Managers.FindAsync(1)).ReturnsAsync((Manager)null);
            var controller = new ManagerController(mockContext.Object);
            var updateDto = new ManagerDto 
            { 
                Id = 1, 
                Name = "Updated Manager", 
                Email = "updated@test.com", 
                Department = Department.It, 
                Picture = "pic-updated.png" 
            };
            var result = await controller.UpdateManager(1, updateDto);
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task UpdateManager_Successful()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Manager 1", Email = "manager1@test.com", Department = Department.Hr, Picture = "pic1.png" }
            };
            var mockContext = new Mock<AppDbContext>(options);
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.Managers.FindAsync(1)).ReturnsAsync(managers.First());
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            var controller = new ManagerController(mockContext.Object);
            var updateDto = new ManagerDto 
            { 
                Id = 1, 
                Name = "Updated Manager", 
                Email = "updated@test.com", 
                Department = Department.It, 
                Picture = "pic-updated.png" 
            };
            var result = await controller.UpdateManager(1, updateDto);
            Assert.IsType<NoContentResult>(result);
            var updatedManager = managers.First();
            Assert.Equal(updateDto.Name, updatedManager.Name);
            Assert.Equal(updateDto.Email, updatedManager.Email);
            Assert.Equal(updateDto.Department, updatedManager.Department);
            Assert.Equal(updateDto.Picture, updatedManager.Picture);
        }
        
        [Fact]
        public async Task DeleteManager_NotExist()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var managers = new List<Manager>();
            var mockContext = new Mock<AppDbContext>(options);
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.Managers.FindAsync(1)).ReturnsAsync((Manager)null);
            var controller = new ManagerController(mockContext.Object);
            var result = await controller.DeleteManager(1);
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task DeleteManager_Successful()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Manager 1", Email = "manager1@test.com", Department = Department.Hr, Picture = "pic1.png" }
            };
            var mockContext = new Mock<AppDbContext>(options);
            var mockDbSet = managers.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Managers).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.Managers.FindAsync(1)).ReturnsAsync(managers.First());
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            var controller = new ManagerController(mockContext.Object);
            var result = await controller.DeleteManager(1);
            Assert.IsType<NoContentResult>(result);
            mockDbSet.Verify(m => m.Remove(It.IsAny<Manager>()), Times.Once());
        }
    }
}
