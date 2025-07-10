using System;
namespace ERS.DTO.FinApprover
{
    public class FinApproverDto
    {
        public Guid? Id { get; set; }//Id
        public int category { get; set; }
        public string company { get; set; }//公司別
        public string company_code { get; set; }//公司別代碼
        public string plant { get; set; }//廠別
        public string rv1 { get; set; }//會計初審1
        public string rv2 { get; set; }//會計初審2
        public string rv3 { get; set; }//會計初審3
        public DateTime? mdate { get; set; }//更新時間
        public DateTime? cdate { get; set; }//創建時間
        public string cuser { get; set; }//創建人
        public string muser { get; set; }//更新人
        
    }
}