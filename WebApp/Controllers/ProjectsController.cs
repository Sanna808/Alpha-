using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProjectsController : Controller
{
    [Route("admin/projects")]



    public IActionResult Index()
    {
        var viewModel = new ProjectsViewModel()
        {
            Projects = SetProjects(),
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = SetClients(),
                Members = SetMembers(),
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = SetClients(),
                Members = SetMembers(),
                Statuses = SetStatuses()
            }
        };

        return View(viewModel);
    }

    private IEnumerable<ProjectViewModel> SetProjects()
    {
        var projects = new List<ProjectViewModel>
        {
            new() {
                Id = Guid.NewGuid().ToString(),
                ProjectName = "Website Redisign",
                ProjectImage = "/images/projects/purple-project-template.svg",
                Description = "<p>It is <strong>necessary</strong> to develop a webbsite redesign in a corporate style.</p>",
                ClientName = "GitLab Inc.",
                TimeLeft = "1 week left",
                Members = ["/images/users/user-template-male-green.svg"]

            }
        };


        return projects;
    }

    private IEnumerable<SelectListItem> SetClients()
    {
        var clients = new List<SelectListItem> { 
            new() { Value  = "1", Text = "EPN Sverige AB" },
            new() { Value  = "2", Text = "Microsoft Sverige AB" },
            new() { Value  = "3", Text = "Innofactor AB" }
        };

        return clients;
    }

    private IEnumerable<SelectListItem> SetMembers()
    {
        var members = new List<SelectListItem> {
            new() { Value  = "1", Text = "Sanna Mäkimaa" },
            new() { Value  = "2", Text = "August Mäkimaa" },
            new() { Value  = "3", Text = "Andreas Karlsson" }
        };

        return members;
    }

    private IEnumerable<SelectListItem> SetStatuses()
    {
        var statuses = new List<SelectListItem> {
            new() { Value  = "1", Text = "Started", Selected = true },
            new() { Value  = "2", Text = "Completed" }
        };

        return statuses;
    }
}
