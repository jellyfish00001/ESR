using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Auth
{
    public interface IAuthService
    {
        Task<List<string>> QueryMenus(string userId);

        Task<string> GetIDMToken();

        Task<bool> IsUserAuthorized(string userId, string apikey);

        Task<List<string>> GetCompanys(string userId);

        Task<bool> IsAuthRole(string userId,List<string> roleKeys);
    }
}