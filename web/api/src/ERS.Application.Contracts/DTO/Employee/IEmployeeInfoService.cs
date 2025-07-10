using ERS.Application.Contracts.DTO.Employee;
using ERS.Application.Contracts.DTO.EmployeeInfo;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Application.Contracts.DTO.Communication
{
    public interface IEmployeeInfoService
    {
        // Get employee information by keyword
        Task<List<EmployeeInfoDto>> GetEmployeeInfos(string keyword);

        Task<EmployeeDto> GetEmployeeAsync(string user);
    }
}