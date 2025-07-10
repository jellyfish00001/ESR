namespace ERS.Application.Contracts.DTO.Employee
{
    public class EmployeeDto
    {
        public string emplid { get; set; } // 工号
        public string deptid { get; set; }//部门代码
        public string cname { get; set; } //员工名字
        public string ename { get; set; } //员工英文名字
        public string email { get; set; } //员工英文名字
        public string phone { get; set; }//分机
        public bool isaccount { get; set; }//是否含有银行账号
        public string company { get; set; }//公司代码 例如：130

    }
}