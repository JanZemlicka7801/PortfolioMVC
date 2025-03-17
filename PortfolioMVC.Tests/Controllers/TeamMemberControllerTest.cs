using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortfolioMVC.Controllers;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.Enums;
using PortfolioMVC.Services;
using Xunit;

namespace PortfolioMVC.Tests.Controllers;

[TestSubject(typeof(TeamMemberController))]
public class TeamMemberControllerTest
{
    [Fact]
    public async Task DeleteTeamMember_Success()
    {
        var mockService = new Mock<ITeamMemberService>();
        mockService.Setup(s => s.DeleteTeamMemberAsync(1)).ReturnsAsync(true);
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.DeleteTeamMember(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTeamMember_NotExist()
    {
        var mockService = new Mock<ITeamMemberService>();
        mockService.Setup(s => s.DeleteTeamMemberAsync(1)).ReturnsAsync(false);
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.DeleteTeamMember(1);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_IdMismatch()
    {
        var updateDto = new TeamMemberDto
        {
            Id = 2,
            Name = "Updated Name",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };
        var mockService = new Mock<ITeamMemberService>();
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.UpdateTeamMember(1, updateDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_NotExist()
    {
        var updateDto = new TeamMemberDto
        {
            Id = 1,
            Name = "Updated Name",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };
        var mockService = new Mock<ITeamMemberService>();
        mockService.Setup(s => s.UpdateTeamMemberAsync(1, updateDto)).ReturnsAsync(false);
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.UpdateTeamMember(1, updateDto);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTeamMember_Success()
    {
        var updateDto = new TeamMemberDto
        {
            Id = 1,
            Name = "Updated Team Member",
            Role = Role.Tester,
            Email = "updated@test.com",
            ProjectId = 20
        };
        var mockService = new Mock<ITeamMemberService>();
        mockService.Setup(s => s.UpdateTeamMemberAsync(1, updateDto)).ReturnsAsync(true);
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.UpdateTeamMember(1, updateDto);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateTeamMember_Invalid()
    {
        var mockService = new Mock<ITeamMemberService>();
        var controller = new TeamMemberController(mockService.Object);
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
    public async Task CreateTeamMember_Success()
    {
        var newTeamMemberDto = new TeamMemberDto
        {
            Name = "New Team Member",
            Role = Role.Developer,
            Email = "newtm@test.com",
            ProjectId = 10
        };
        var createdDto = new TeamMemberDto
        {
            Id = 99,
            Name = newTeamMemberDto.Name,
            Role = newTeamMemberDto.Role,
            Email = newTeamMemberDto.Email,
            ProjectId = newTeamMemberDto.ProjectId
        };
        var mockService = new Mock<ITeamMemberService>();
        mockService.Setup(s => s.CreateTeamMemberAsync(newTeamMemberDto))
                   .ReturnsAsync(createdDto);
        var controller = new TeamMemberController(mockService.Object);
        var result = await controller.CreateTeamMember(newTeamMemberDto);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDto = Assert.IsType<TeamMemberDto>(createdAtActionResult.Value);
        Assert.Equal(99, returnDto.Id);
        Assert.Equal(newTeamMemberDto.Email, returnDto.Email);
    }
}