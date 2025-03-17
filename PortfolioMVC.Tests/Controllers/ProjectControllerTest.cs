using System;
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

namespace PortfolioMVC.Tests.Controllers;

[TestSubject(typeof(ProjectController))]
public class ProjectControllerTest
{

    [Fact]
    public async Task GetProjects_Success()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>
        {
            new Project 
            { 
                Id = 1, 
                Name = "Project 1", 
                Description = "Description 1", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(30), 
                ManagerId = 10 
            },
            new Project 
            { 
                Id = 2, 
                Name = "Project 2", 
                Description = "Description 2", 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(31), 
                ManagerId = 20 
            }
        };

        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var controller = new ProjectController(mockContext.Object);

        var result = await controller.GetProjects();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Equal(projects.Count, returnValue.Count());
        Assert.Equal(projects.First().Name, returnValue.First().Name);
    }

    [Fact]
    public async Task GetProject_Success()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>
        {
            new Project 
            { 
                Id = 1, 
                Name = "Project 1", 
                Description = "Description 1", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(30), 
                ManagerId = 10 
            },
            new Project 
            { 
                Id = 2, 
                Name = "Project 2", 
                Description = "Description 2", 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(31), 
                ManagerId = 20 
            }
        };

        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var controller = new ProjectController(mockContext.Object);
        int testId = 1;

        var result = await controller.GetProject(testId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var projectDto = Assert.IsType<ProjectDto>(okResult.Value);
        Assert.Equal(testId, projectDto.Id);
        Assert.Equal(projects.First(p => p.Id == testId).Name, projectDto.Name);
    }

    [Fact]
    public async Task GetProject_Fail()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var controller = new ProjectController(mockContext.Object);
        int testId = 999;
        var result = await controller.GetProject(testId);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProject_Fail()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var mockContext = new Mock<AppDbContext>(options);
        
        var projects = new List<Project>();
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        
        var controller = new ProjectController(mockContext.Object);
        controller.ModelState.AddModelError("Name", "Required");
        
        var newProjectDto = new ProjectDto
        {
            Name = "",
            Description = "Some description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            ManagerId = 10
        };

        var result = await controller.CreateProject(newProjectDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProject_Success()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>();
        var mockContext = new Mock<AppDbContext>(options);
        
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.Add(It.IsAny<Project>()))
                 .Callback<Project>(p => { p.Id = 42; projects.Add(p); });
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
        
        var controller = new ProjectController(mockContext.Object);
        var newProjectDto = new ProjectDto
        {
            Name = "New Project",
            Description = "New project description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(45),
            ManagerId = 10
        };

        var result = await controller.CreateProject(newProjectDto);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDto = Assert.IsType<ProjectDto>(createdAtActionResult.Value);
        Assert.Equal(42, returnDto.Id);
        Assert.Equal(newProjectDto.Name, returnDto.Name);
    }
    
    [Fact]
    public async Task PutProject_Fail()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        var projects = new List<Project>
        {
            new Project 
            { 
                Id = 1, 
                Name = "Original Project", 
                Description = "Original Description", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(30), 
                ManagerId = 10 
            }
        };
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var controller = new ProjectController(mockContext.Object);
        var updateDto = new ProjectDto
        {
            Id = 2,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = 20
        };

        var result = await controller.PutProject(1, updateDto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutProject_NotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync((Project)null);
        
        var controller = new ProjectController(mockContext.Object);
        var updateDto = new ProjectDto
        {
            Id = 1,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = 20
        };
        var result = await controller.PutProject(1, updateDto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutProject_Success()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        var projects = new List<Project>
        {
            new Project 
            { 
                Id = 1, 
                Name = "Original Project", 
                Description = "Original Description", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(30), 
                ManagerId = 10 
            }
        };
        
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync(projects.First());
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var controller = new ProjectController(mockContext.Object);
        var updateDto = new ProjectDto
        {
            Id = 1,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = 20
        };
        var result = await controller.PutProject(1, updateDto);
        var updatedProject = projects.First();

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(updateDto.Name, updatedProject.Name);
        Assert.Equal(updateDto.Description, updatedProject.Description);
        Assert.Equal(updateDto.StartDate, updatedProject.StartDate);
        Assert.Equal(updateDto.EndDate, updatedProject.EndDate);
        Assert.Equal(updateDto.ManagerId, updatedProject.ManagerId);
    }

    [Fact]
    public async Task DeleteProject_Fail()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync((Project)null);
        
        var controller = new ProjectController(mockContext.Object);
        var result = await controller.DeleteProject(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProject_Success()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var projects = new List<Project>
        {
            new Project 
            { 
                Id = 1, 
                Name = "Project to Delete", 
                Description = "Description", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(30), 
                ManagerId = 10 
            }
        };
        
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = projects.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.Projects.FindAsync(1)).ReturnsAsync(projects.First());
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var controller = new ProjectController(mockContext.Object);
        var result = await controller.DeleteProject(1);
        
        Assert.IsType<NoContentResult>(result);
        
        mockDbSet.Verify(m => m.Remove(It.IsAny<Project>()), Times.Once());
    }
}