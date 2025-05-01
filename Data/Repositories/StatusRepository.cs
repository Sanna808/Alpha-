﻿using Data.Contexts;
using Data.Enteties;
using Domain.Models;

namespace Data.Repositories;
public interface IStatusRepository : IBaseRepository<StatusEntity, Status>
{
}

public class StatusRepository(AppDbContext context) : BaseRepository<StatusEntity, Status>(context), IStatusRepository
{
}
