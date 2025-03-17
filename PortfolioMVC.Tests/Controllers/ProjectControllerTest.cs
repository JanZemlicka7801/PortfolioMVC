using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortfolioMVC.Controllers;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Services;
using Xunit;

namespace PortfolioMVC.Tests.Controllers;

[TestSubject(typeof(ProjectController))]
public class ProjectControllerTest
{
    [Fact]
    public async Task GetProjects_Success()
    {
        var projectDtos = new List<ProjectDto>
        {
            new ProjectDto
            {
                Id = 1,
                Name = "Project 1",
                Description = "Description 1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                ManagerId = "10"
            },
            new ProjectDto
            {
                Id = 2,
                Name = "Project 2",
                Description = "Description 2",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(31),
                ManagerId = "20"
            }
        };
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.GetAllProjectsAsync())
                   .ReturnsAsync(projectDtos);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.GetAllProjects();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Equal(projectDtos.Count, returnValue.Count());
        Assert.Equal(projectDtos.First().Name, returnValue.First().Name);
    }

    [Fact]
    public async Task GetProject_Success()
    {
        var projectDto = new ProjectDto
        {
            Id = 1,
            Name = "Project 1",
            Description = "Description 1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            ManagerId = "10"
        };
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.GetProjectByIdAsync(1))
                   .ReturnsAsync(projectDto);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.GetProject(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnDto = Assert.IsType<ProjectDto>(okResult.Value);
        Assert.Equal(1, returnDto.Id);
        Assert.Equal(projectDto.Name, returnDto.Name);
    }

    [Fact]
    public async Task GetProject_Fail()
    {
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.GetProjectByIdAsync(It.IsAny<int>()))
                   .ReturnsAsync((ProjectDto)null);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.GetProject(999);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProject_Fail()
    {
        var mockService = new Mock<IProjectService>();
        var controller = new ProjectController(mockService.Object);
        controller.ModelState.AddModelError("Name", "Required");
        var newProjectDto = new ProjectDto
        {
            Name = "",
            Description = "Some description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            ManagerId = "10"
        };
        var result = await controller.CreateProject(newProjectDto);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProject_Success()
    {
        var newProjectDto = new ProjectDto
        {
            Name = "New Project",
            Description = "New project description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(45),
            ManagerId = "10"
        };
        var createdProjectDto = new ProjectDto
        {
            Id = 42,
            Name = newProjectDto.Name,
            Description = newProjectDto.Description,
            StartDate = newProjectDto.StartDate,
            EndDate = newProjectDto.EndDate,
            ManagerId = newProjectDto.ManagerId
        };
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.CreateProjectAsync(newProjectDto))
                   .ReturnsAsync(createdProjectDto);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.CreateProject(newProjectDto);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDto = Assert.IsType<ProjectDto>(createdAtActionResult.Value);
        Assert.Equal(42, returnDto.Id);
        Assert.Equal(newProjectDto.Name, returnDto.Name);
    }

    [Fact]
    public async Task PutProject_Fail_IdMismatch()
    {
        var updateDto = new ProjectDto
        {
            Id = 2,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = "20"
        };
        var mockService = new Mock<IProjectService>();
        var controller = new ProjectController(mockService.Object);
        var result = await controller.UpdateProject(1, updateDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutProject_Fail_NotFound()
    {
        var updateDto = new ProjectDto
        {
            Id = 1,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = "20"
        };
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.UpdateProjectAsync(1, updateDto))
                   .ReturnsAsync(false);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.UpdateProject(1, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutProject_Success()
    {
        var updateDto = new ProjectDto
        {
            Id = 1,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(31),
            ManagerId = "20"
        };

        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.UpdateProjectAsync(1, updateDto))
                   .ReturnsAsync(true);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.UpdateProject(1, updateDto);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProject_Fail()
    {
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.DeleteProjectAsync(It.IsAny<int>()))
                   .ReturnsAsync(false);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.DeleteProject(1);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProject_Success()
    {
        var mockService = new Mock<IProjectService>();
        mockService.Setup(s => s.DeleteProjectAsync(It.IsAny<int>()))
                   .ReturnsAsync(true);
        var controller = new ProjectController(mockService.Object);
        var result = await controller.DeleteProject(1);
        Assert.IsType<NoContentResult>(result);
    }
}