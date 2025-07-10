using System;
using ERS.Entities;
using Volo.Abp.Domain.Repositories;

namespace ERS.IRepositories
{
    public interface IDataDictionaryRepository : IRepository<DataDictionary, Guid>
    {
    }
}