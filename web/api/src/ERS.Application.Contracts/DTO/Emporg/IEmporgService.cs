using ERS.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Emporg
{
    public interface IEmporgService
    {
        Task<string> GetDeptid(string deptid, string company);
        Task<List<EmporgDto>> QueryVagues(string deptid);
        Task<IList<EmporgDto>> GetPartDeptidList();
        Task<Result<string>> GetCodtDeptid(string deptid, string company);
    }
}