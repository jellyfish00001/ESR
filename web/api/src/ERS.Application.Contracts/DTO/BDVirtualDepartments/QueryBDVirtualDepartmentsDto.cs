using System;

namespace ERS.DTO.BDVirtualDepartments
{
    public class QueryBDVirtualDepartmentsDto
    {
        public Guid? Id { get; set; }
        public string company { get; set; } //公司別
        public string deptid { get; set; } //部門代碼
    }
}