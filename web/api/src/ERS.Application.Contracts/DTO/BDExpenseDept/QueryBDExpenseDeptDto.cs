using System;

namespace ERS.DTO.BDExpenseDept
{
    public class QueryBDExpenseDeptDto
    {
        public Guid? Id { get; set; }
        public string company{ get; set; } //公司別
        public string deptid{ get; set; } //部門代碼
        public string isvirtualdept{ get; set; } //是否為虛擬部門
    }
}