using ERS.Entities;
using System;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IOcrResultRepository : IRepository<OCRResults, Guid>
    {
    }
}
