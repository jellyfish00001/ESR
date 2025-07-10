using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.DTO.Permission
{
    public interface IPermissionService
    {
        Result<IEnumerable<category>> Roles();
        Task<Result<bool>> Add(SelectParams param, string token);
        Task<Result<IEnumerable<RoleResult>>> Query(SelectParams param, string token);
        Task<Result<bool>> Delete(RoleResult param, string token);
        Task<Result<IEnumerable<UserAcl>>> Permissions(string token);
    }
}
