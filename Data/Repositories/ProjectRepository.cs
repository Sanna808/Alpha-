﻿using Data.Contexts;
using Data.Enteties;
using Domain.Models;

namespace Data.Repositories;
public interface IProjectRepository : IBaseRepository<ProjectEntity, Project>
{
}

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectRepository
{
}
