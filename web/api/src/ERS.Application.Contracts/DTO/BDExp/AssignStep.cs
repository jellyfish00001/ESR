using System.Collections.Generic;
using ERS.Application.Contracts.DTO.EmployeeInfo;
namespace ERS.DTO
{
    public class AssignStep
    {
        public string name { get; set; }
        public string position { get; set; }
        public List<EmployeeInfoDto> approverList { get; set; }
    }
}
