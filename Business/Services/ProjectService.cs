using Data.Enteties;
using Data.Repositories;
using Domain.Models;
using Business.Models;


namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormData formData);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;


    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };


        var projectEntity = new ProjectEntity
        {
            ProjectName = formData.ProjectName,
            Description = formData.Description,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            ClientId = formData.ClientId,
            UserId = formData.UserId,
            StatusId = formData.StatusId,
        };
        var statusResult = await _statusService.GetStatusByIdAsync(1);
        var status = statusResult.Result;

        projectEntity.StatusId = status!.Id;

        var result = await _projectRepository.AddAsync(projectEntity);
        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var repoResult = await _projectRepository.GetAllAsync(
            selector: e => new Domain.Models.Project
            {
                Id = e.Id,
                Image = e.Image,
                ProjectName = e.ProjectName,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Budget = e.Budget,
                Client = new Domain.Models.Client
                {
                    Id = e.Client.Id,
                    ClientName = e.Client.ClientName
                },
                User = new Domain.Models.User
                {
                    Id = e.User.Id,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName
                },
                Status = new Domain.Models.Status
                {
                    Id = e.Status.Id,
                    StatusName = e.Status.StatusName
                }
            },
            orderByDescending: true,
            sortBy: e => e.Created
        );

        return new ProjectResult<IEnumerable<Project>>
        {
            Succeeded = repoResult.Succeeded,
            StatusCode = repoResult.StatusCode,
            Result = repoResult.Result
        };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var repoResult = await _projectRepository.GetAllAsync(
            e => new Domain.Models.Project
            {
                Id = e.Id,
                Image = e.Image,
                ProjectName = e.ProjectName,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Budget = e.Budget,
                Client = new Domain.Models.Client
                {
                    Id = e.Client.Id,
                    ClientName = e.Client.ClientName
                },
                User = new Domain.Models.User
                {
                    Id = e.User.Id,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName
                },
                Status = new Domain.Models.Status
                {
                    Id = e.Status.Id,
                    StatusName = e.Status.StatusName
                }
            },
            false,
            null,
            e => e.Id == id,
            e => e.User,
            e => e.Status,
            e => e.Client
        );

        if (!repoResult.Succeeded || repoResult.Result == null)
            return new ProjectResult<Project>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Project '{id}' not found."
            };

        var project = repoResult.Result.Single();

        return new ProjectResult<Project>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = project
        };
    }

    public async Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "No update made" };

        var projectEntity = new ProjectEntity
        {
            Id = formData.Id,
            Image = formData.Image,
            ProjectName = formData.ProjectName,
            Description = formData.Description,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            ClientId = formData.ClientId,
            UserId = formData.UserId,
            StatusId = formData.StatusId
        };

        var Result = await _projectRepository.UpdateAsync(projectEntity);

        return Result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = Result.StatusCode, Error = Result.Error };
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new ProjectResult
            { Succeeded = false, StatusCode = 400, Error = $"Project '{id}' was not found." };

        var existsResult = await _projectRepository.ExistsAsync(x => x.Id == id);
        if (!existsResult.Succeeded)
            return new ProjectResult
            { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };


        var stub = new ProjectEntity { Id = id };
        var deleteResult = await _projectRepository.DeleteAsync(stub);

        return deleteResult.Succeeded
            ? new ProjectResult
            { Succeeded = true, StatusCode = 200 }
            : new ProjectResult
            { Succeeded = false, StatusCode = deleteResult.StatusCode, Error = deleteResult.Error };
    }
}
