using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;
using WebApp.Models;
using Domain.Extentions;
using Domain.Models;
using System.Security.Claims;

namespace WebApp.Controllers;

[Route("admin/projects")]

public class ProjectsController(
    IProjectService projectService,
    IClientService clientService,
    IUserService userService,
    IStatusService statusService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IUserService _userService = userService;
    private readonly IStatusService _statusService = statusService;

    [HttpGet("")]

    public async Task<IActionResult> Index()
    {
        var clientResult = await _clientService.GetClientsAsync();
        var userResult = await _userService.GetUsersAsync();
        var statusResult = await _statusService.GetStatusesAsync();
        var projectResult = await _projectService.GetProjectsAsync();

        var clients = clientResult.Result!
            .Select(c => new SelectListItem { Value = c.Id, Text = c.ClientName })
            .ToList();

        var members = userResult.Result!
            .Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FirstName} {u.LastName}" })
            .ToList();

        var statuses = statusResult.Result!
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StatusName })
            .ToList();

        var viewModel = new ProjectsViewModel
        {
            Projects = projectResult.Result!
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectImage = p.Image ?? "/images/default.",
                    ClientName = p.Client.ClientName,
                    Description = p.Description ?? "",
                }),
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = clients,
                Members = members,
                Statuses = statuses,
                StatusId = int.Parse(statuses.First().Value)
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = clients,
                Members = members,
                Statuses = statuses
            }
        };

        return View(viewModel);
    }

    [HttpPost("create")]

    public async Task<IActionResult> Create(AddProjectViewModel model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToList()
                );

            return BadRequest(new { success = false, errors });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new InvalidOperationException("Not signed in");

        var formData = new AddProjectFormData
        {
            Image = model.Image,
            ProjectName = model.ProjectName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId, 
            UserId = model.MemberId,    
            StatusId = model.StatusId        
        };

        var result = await _projectService.CreateProjectAsync(formData);
        if (result.Succeeded)
            return RedirectToAction(nameof(Index));

        return Problem(result.Error, statusCode: result.StatusCode);
    }

    [HttpPost("edit")]

    public async Task<IActionResult> Edit(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToList()
                );

            return BadRequest(new { success = false, errors });
        }

        var formData = new UpdateProjectFormData
        {
            Id = model.Id,
            Image = model.Image,
            ProjectName = model.ProjectName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId,
            UserId = model.MemberId,
            StatusId = model.StatusId
        };

        var result = await _projectService.UpdateProjectAsync(formData);
        if (result.Succeeded)
            return RedirectToAction(nameof(Index));

        return Problem(result.Error, statusCode: result.StatusCode);
    }

    [HttpGet("{id}")]
    [Route("admin/projects/{id}")]

    public async Task<IActionResult> Get(string id)
    {
        var projResult = await _projectService.GetProjectAsync(id);
        if (!projResult.Succeeded)
            return NotFound();

        var p = projResult.Result!;
        var clients = (await _clientService.GetClientsAsync()).Result!
            .Select(c => new SelectListItem(c.ClientName, c.Id)).ToList();
        var members = (await _userService.GetUsersAsync()).Result!
            .Select(u => new SelectListItem($"{u.FirstName} {u.LastName}", u.Id)).ToList();
        var statuses = (await _statusService.GetStatusesAsync()).Result!
            .Select(s => new SelectListItem(s.StatusName, s.Id.ToString())).ToList();

        var vm = new EditProjectViewModel
        {
            Id = p.Id,
            Image = p.Image,
            ProjectName = p.ProjectName,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Budget = p.Budget,

            ClientId = p.Client.Id,
            MemberId = p.User.Id,
            StatusId = p.Status.Id,

            Clients = clients,
            Members = members,
            Statuses = statuses
        };
        return Json(vm);
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Delete(string id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if (result.Succeeded)
            return Ok();          
        return Problem(result.Error, statusCode: result.StatusCode);
    }

}
