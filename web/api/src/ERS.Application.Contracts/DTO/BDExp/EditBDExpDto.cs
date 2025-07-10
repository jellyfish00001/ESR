using System;
using System.Collections.Generic;
namespace ERS.DTO.BDExp
{
    public class EditBDExpDto
    {
        public Guid? Id { get; set; }
        public string expcode { get; set; }//费用代碼
        public string auditlevelcode { get; set; }//核決權限代碼
        public string expname { get; set; }//费用名称
        public string keyword { get; set; }
        public string acctcode { get; set; }//會計科目
        //public string flag { get; set; }
        public string companycategory { get; set; }//公司别
        //public string invoiceflag { get; set; }//是否自動產生發票
        //public string type { get; set; }//類型
        //public string description { get; set; }//费用描述
        public string wording { get; set; }//備註或者摘要Wording
        //public string special { get; set; }//費用類別特殊卡控
        //public string iszerotaxinvter { get; set; }//是否零稅和免稅發票
        // public string filecategory { get; set; }
        // public string isupload { get; set; }
        //public string filepoints { get; set; }
        //public string isinvoice { get; set; }
        // public string invoicecategory { get; set; }
        public string isdeduction { get; set; }
        public string addsignstep { get; set; }
        public string addsign { get; set; }
        public int departday { get; set; }
        public int sectionday { get; set; }
        public int calmethod { get; set; }
        public int extraformcode { get; set; }
        public string category { get; set; }
        public string assignment { get; set; }
        public string costcenter { get; set; }
        public string pjcode { get; set; }
        public string datelevel { get; set; }
        /// <summary>
        /// 授权人
        /// </summary>
        public string authorizer { get; set; }
        /// <summary>
        /// 被授权人
        /// </summary>
        public string authorized { get; set; }
        /// <summary>
        /// 授权开始日期
        /// </summary>
        public DateTime? sdate { get; set; }
        /// <summary>
        /// 授权结束日期
        /// </summary>
        public DateTime? edate { get; set; }

        public List<AssignStep> assignSteps { get; set; }
        //public string expdesc { get; set; }
        public string descriptionnotice { get; set; }
        //public string issendremindemail { get; set; }
        public string senarioname { get; set; }
        /// <summary>
        /// 是否需附上發票憑證
        /// </summary>
        public string requiresinvoice { get; set; }
        /// <summary>
        /// 是否需繳交紙本附件
        /// </summary>
        public string requirespaperattachment { get; set; }
        /// <summary>
        /// 是否需上傳附件
        /// </summary>
        public string requiresattachment { get; set; }
        public string attachmentname { get; set; }
        
        /// <summary>
        /// 是否可抵扣稅金
        /// </summary>
        public string isvatdeductable { get; set; }
        /// <summary>
        /// 是否可跳過會計簽核
        /// </summary>
        public string canbypassfinanceapproval { get; set; }

        public string attachmentnotice { get; set; }
        
    }
}