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
using PortfolioMVC.Models.Enums;
using Xunit;

namespace PortfolioMVC.Tests.Controllers;

[TestSubject(typeof(TeamMemberController))]
public class TeamMemberControllerTest
{

    [Fact]
    public async Task DeleteTeamMember_IsDeleted()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
            
        var teamMembers = new List<TeamMember>
        {
            new TeamMember { Id = 1, Name = "Team Member 1", Role = Role.Developer, Email = "tm1@test.com", ProjectId = 10 }
        };
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.TeamMembers.FindAsync(1)).ReturnsAsync(teamMembers.First());
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var controller = new TeamMemberController(mockContext.Object);
        var result = await controller.DeleteTeamMember(1);

        Assert.IsType<NoContentResult>(result);
        mockDbSet.Verify(m => m.Remove(It.IsAny<TeamMember>()), Times.Once());
    }
    
    [Fact]
    public async Task DeleteTeamMember_NotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var teamMembers = new List<TeamMember>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.TeamMembers.FindAsync(1)).ReturnsAsync((TeamMember)null);

        var controller = new TeamMemberController(mockContext.Object);
        var result = await controller.DeleteTeamMember(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_NotMatch()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var teamMembers = new List<TeamMember>
        {
            new TeamMember { Id = 1, Name = "Team Member 1", Role = Role.Developer, Email = "tm1@test.com", ProjectId = 10 }
        };
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);

        var controller = new TeamMemberController(mockContext.Object);
        var updateDto = new TeamMemberDto
        {
            Id = 2,
            Name = "Updated Name",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };

        var result = await controller.UpdateTeamMember(1, updateDto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_NotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var teamMembers = new List<TeamMember>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.TeamMembers.FindAsync(1)).ReturnsAsync((TeamMember)null);

        var controller = new TeamMemberController(mockContext.Object);
        var updateDto = new TeamMemberDto
        {
            Id = 1,
            Name = "Updated Name",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };

        var result = await controller.UpdateTeamMember(1, updateDto);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_Successful()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var teamMembers = new List<TeamMember>
        {
            new TeamMember { Id = 1, Name = "Team Member 1", Role = Role.Developer, Email = "tm1@test.com", ProjectId = 10 }
        };
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.TeamMembers.FindAsync(1)).ReturnsAsync(teamMembers.First());
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        var controller = new TeamMemberController(mockContext.Object);
        var updateDto = new TeamMemberDto
        {
            Id = 1,
            Name = "Updated Team Member",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };

        var result = await controller.UpdateTeamMember(1, updateDto);
        var updatedMember = teamMembers.First();
        
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(updateDto.Name, updatedMember.Name);
        Assert.Equal(updateDto.Role, updatedMember.Role);
        Assert.Equal(updateDto.Email, updatedMember.Email);
        Assert.Equal(updateDto.ProjectId, updatedMember.ProjectId);
    }

    [Fact]
    public async Task CreateTeamMember_Invalid()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var mockContext = new Mock<AppDbContext>(options);
        
        var teamMembers = new List<TeamMember>();
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        var controller = new TeamMemberController(mockContext.Object);
        
        controller.ModelState.AddModelError("Name", "Required");
        var newTeamMemberDto = new TeamMemberDto
        {
            Name = "",
            Role = Role.Developer,
            Email = "tm@test.com",
            ProjectId = 10
        };
        var result = await controller.CreateTeamMember(newTeamMemberDto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateTeamMember_ValidTeamMember()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var teamMembers = new List<TeamMember>();
        var mockContext = new Mock<AppDbContext>(options);
        var mockDbSet = teamMembers.AsQueryable().BuildMockDbSet();
        
        mockDbSet.Setup(m => m.Add(It.IsAny<TeamMember>()))
                 .Callback<TeamMember>(tm => { tm.Id = 99; teamMembers.Add(tm); });
        mockContext.Setup(c => c.TeamMembers).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
        
        var controller = new TeamMemberController(mockContext.Object);
        var newTeamMemberDto = new TeamMemberDto
        {
            Name = "New Team Member",
            Role = Role.Developer,
            Email = "newtm@test.com",
            ProjectId = 10
        };
        var result = await controller.CreateTeamMember(newTeamMemberDto);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDto = Assert.IsType<TeamMemberDto>(createdAtActionResult.Value);
        
        Assert.Equal(99, returnDto.Id);
        Assert.Equal(newTeamMemberDto.Email, returnDto.Email); 
    }
}