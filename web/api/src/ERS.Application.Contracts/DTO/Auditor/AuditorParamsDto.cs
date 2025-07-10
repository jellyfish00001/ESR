using System;
using System.Collections.Generic;
namespace ERS.DTO.Auditor
{
    /// <summary>
    /// bd04查询参数dto
    /// </summary>
    public class AuditorParamsDto
    {
        public Guid? Id { get; set; }
        public string formcode { get; set; }
        public string emplid { get; set; }
        public string deptid { get; set; }//写死all
        public string auditid { get; set; }//Auditor工号
        public DateTime sdate { get; set; }//开始日期
        public DateTime edate { get; set; }//结束日期
        public List<string> companyList { get; set; }
        public string company { get; set; }
    }
}