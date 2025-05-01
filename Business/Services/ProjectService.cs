using Business.Models;
using Data.Enteties;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;

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

        var projectEntity = formData.MapTo<ProjectEntity>();
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
        var response = await _projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortBy: s => s.Created,
                where: null,
                include => include.User,
                include => include.Status,
                include => include.Client
            );

        return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
                include => include.Status,
                include => include.Client
            );
        return response.Succeeded
        ? new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
        : new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };
    }

    public async Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "No update made" };

        if (!int.TryParse(formData.StatusId, out var statusId))
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = $"Status: '{formData.StatusId}' not valid." };

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
            StatusId = statusId
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
