using System;
using System.Collections.Generic;
namespace ERS.DTO.BDExp
{
    public class BDExpFormDto
    {
        public Guid? Id { get; set; }
        public string expcode { get; set; }//费用项目
        /// <summary>
        /// 核決權限代碼
        /// </summary>
        public string auditlevelcode { get; set; }
        public string expname { get; set; }//费用名称
        public string acctcode { get; set; }//费用代码
        //public string flag { get; set; }
        public string stat { get; set; }//状态
        public string cuser { get; set; }
        public DateTime? cdate { get; set; }
        public string muser { get; set; }
        public DateTime? mdate { get; set; }
        public string companycategory { get; set; }//公司别
        //public string invoiceflag { get; set; }//是否自動產生發票
        //public string type { get; set; }//類型
        //public string invter { get; set; }//特殊表单签核人
        /// <summary>
        /// 费用描述
        /// </summary>
        //public string description { get; set; }
        //public string special { get; set; }//費用類別特殊卡控
        //public string iszerotaxinvter { get; set; }//是否零稅和免稅發票
        /// <summary>
        /// 是否需繳交紙本附件
        /// </summary>
        public bool requirespaperattachment { get; set; }
        public string keyword { get; set; }
        public string assignment { get; set; }
        public string costcenter { get; set; }
        public string pjcode { get; set; }
        // public string filecategory { get; set; }
        // public string isupload { get; set; }
        //public string filepoints { get; set; }
        public bool requiresinvoice { get; set; }
        // public string invoicecategory { get; set; }
        /// <summary>
        /// 是否可抵扣稅金
        /// </summary>
        public bool isvatdeductable { get; set; }
        public string addsignstep { get; set; }
        public string addsign { get; set; }
        public int departday { get; set; }
        public int sectionday { get; set; }
        public int calmethod { get; set; }
        /// <summary>
        /// 報銷模塊
        /// </summary>
        public int extraformcode { get; set; }
        /// <summary>
        /// 費用類別
        /// </summary>
        public string category { get; set; }
        public string datelevel { get; set; }
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
        /// <summary>
        /// 摘要提示
        /// </summary>
        public string descriptionnotice { get; set; }
        public string senarioname { get; set; }
        /// <summary>
        /// 上傳附件提示
        /// </summary>
        public string attachmentnotice { get; set; }
        /// <summary>
        /// 上傳附件名稱
        /// </summary>
        public string attachmentname { get; set; }
        /// <summary>
        /// 是否可跳過會計簽核
        /// </summary>
        public bool canbypassfinanceapproval { get; set; }
        public bool requiresattachment { get; set; }
    }
}