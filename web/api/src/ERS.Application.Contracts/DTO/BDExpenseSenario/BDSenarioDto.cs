using System;
using System.Collections.Generic;

namespace ERS.DTO.BDExpenseSenario
{
    /// <summary>
    /// 報銷情景 DTO
    /// </summary>
    public class BDSenarioDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 公司別
        /// </summary>
        public string companycategory { get; set; }

        /// <summary>
        /// 報銷情景名稱
        /// </summary>
        public string senarioname { get; set; }

        ///// <summary>
        ///// 關鍵字
        ///// </summary>
        //public string keyword { get; set; }

        /// <summary>
        /// 費用類別代碼
        /// </summary>
        public string expensecode { get; set; }

        /// <summary>
        /// 會計科目
        /// </summary>
        public string accountcode { get; set; }

        /// <summary>
        /// 核決權限代碼
        /// </summary>
        public string auditlevelcode { get; set; }

        ///// <summary>
        ///// 摘要提示
        ///// </summary>
        //public string descriptionnotice { get; set; }

        /// <summary>
        /// 附件提示
        /// </summary>
        public string attachmentnotice { get; set; }

        /// <summary>
        /// 是否需繳交紙本附件
        /// </summary>
        public bool requirespaperattatchment { get; set; }

        /// <summary>
        /// 是否需附上發票憑證
        /// </summary>
        public bool requiresinvoice { get; set; }

        /// <summary>
        /// 是否可抵扣稅金
        /// </summary>
        public bool isvatdeductable { get; set; }

        /// <summary>
        /// 是否可跳過會計簽核
        /// </summary>
        public bool canbypassfinanceapproval { get; set; }

        /// <summary>
        /// 報銷模塊
        /// </summary>
        public int extraformcode { get; set; }

        ///// <summary>
        ///// Assignment
        ///// </summary>
        //public string assignment { get; set; }

        ///// <summary>
        ///// Cost center
        ///// </summary>
        //public string costcenter { get; set; }

        /// <summary>
        /// Project code
        /// </summary>
        public string projectcode { get; set; }

        public List<ExtraStepsDto> extraSteps { get; set; }

        public string accountname { get; set; }

        //public string category { get; set; }


        public string expcode { get; set; }//费用项目
        public string classno { get; set; }//费用签核项目
        public string expname { get; set; }//费用名称
        public string acctcode { get; set; }//费用代码
        public string flag { get; set; }
        public string stat { get; set; }//状态
        public string cuser { get; set; }
        public DateTime? cdate { get; set; }
        public string muser { get; set; }
        public DateTime? mdate { get; set; }
        public string company { get; set; }//公司别
        public string invoiceflag { get; set; }//是否自動產生發票
        public string type { get; set; }//類型
        public string invter { get; set; }//特殊表单签核人
        public string description { get; set; }//费用描述
        public string wording { get; set; }//備註或者摘要Wording
        public string special { get; set; }//費用類別特殊卡控
        public string iszerotaxinvter { get; set; }//是否零稅和免稅發票
        public string issendremindemail { get; set; }//是否發送提醒交件Eamil
        public string keyword { get; set; }
        public string assignment { get; set; }
        public string costcenter { get; set; }
        public string pjcode { get; set; }
        // public string filecategory { get; set; }
        // public string isupload { get; set; }
        public string filepoints { get; set; }
        public string isinvoice { get; set; }
        // public string invoicecategory { get; set; }
        public string isdeduction { get; set; }
        public string addsignstep { get; set; }
        public string addsign { get; set; }
        public int departday { get; set; }
        public int sectionday { get; set; }
        public int calmethod { get; set; }
        public int expcategory { get; set; }
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
        public string expdesc { get; set; }
        public string descriptionnotice { get; set; }
    }
}
