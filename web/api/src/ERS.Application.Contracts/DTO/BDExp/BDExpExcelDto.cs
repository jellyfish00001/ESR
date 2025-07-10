namespace ERS.DTO.BDExp
{
    public class BDExpExcelDto
    {
        public string company { get; set; }//公司别
        public string expcode { get; set; }//费用项目
        public string expname { get; set; }//费用名称
        public string keyword { get; set; }
        public string acctcode { get; set; }//费用代码
        public string classno { get; set; }//费用签核项目
        public string isupload { get; set; }
        public string filecategory { get; set; }
        public string filepoints { get; set; }
        public string invoiceflag { get; set; }//是否自動產生發票
        public string invoicecategory { get; set; }
        public string isdeduction { get; set; }
        public string addsign { get; set; }
        public string addsignstep { get; set; }
        public int calmethod { get; set; }
        public int departday { get; set; }
        public int sectionday { get; set; }
        public int expcategory { get; set; }
        public int category { get; set; }
        public string assignment { get; set; }
        public string costcenter { get; set; }
        public string pjcode { get; set; }
        public string datelevel { get; set; }
    }
}