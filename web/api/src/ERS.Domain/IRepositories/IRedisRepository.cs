using System.Threading.Tasks;

namespace ERS.IRepositories
{
    public interface IRedisRepository
    {
        Task<bool> AddLockAutoNoAsync(string key);
        Task DelLockAsync(string key);
    }
}
