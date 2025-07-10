using System;
namespace ERS.DTO.Auditor
{
    /// <summary>
    /// bd04查询结果dto
    /// </summary>
    public class AuditorDto
    {
        public Guid? Id { get; set; }
        public string formcode { get; set; }//单据类型代码
        public string formname { get; set; }//单据类型名称
        public string emplid { get; set; }//工号
        public string cname { get; set; }//姓名
        public string auditid { get; set; }//Auditor工号
        public string auditname { get; set; }//Auditor姓名
        public DateTime sdate { get; set; }//开始日期
        public DateTime edate { get; set; }//结束日期
        public string muser { get; set; }//更新人
        public DateTime? mdate { get; set; }//更新日期
        public string company { get; set; }//公司别
    }
}