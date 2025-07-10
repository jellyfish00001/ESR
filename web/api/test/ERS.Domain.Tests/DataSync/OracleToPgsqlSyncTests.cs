using ERS.OracleToPostgreSQL;

namespace ERS.DataSync
{
    public class OracleToPgsqlSyncTests : ERSDomainTestBase
    {
        private IOracleToPgsqlRepository _oracleToPgsqlRepository;

        public OracleToPgsqlSyncTests()
        {
            _oracleToPgsqlRepository = GetRequiredService<IOracleToPgsqlRepository>();
        }

        //[Fact(DisplayName = "3_DB_PS_WHQEmployee数据同步")]
        //public void PS_WHQEmployeeDataSync()
        //{
        //    // XmlDocument xmldoc = new XmlDocument();
        //    // xmldoc.Load(Path.Combine(Path.Combine("Files","XML","3_DB_PS_WHQEmployee.xml")));
        //    // string xmlStr = xmldoc.InnerXml;
        //    // DataSet xmlset = _oracleToPgsqlRepository.GetXMLSet(xmlStr);
        //    _oracleToPgsqlRepository.OracleToPgsqlDataSync("3_DB_PS_WHQEmployee.xml");

        //}

        //[Fact(DisplayName = "0_DB_PS_StardandOrg2数据同步")]
        //public void PS_StardandOrg2DataSync()
        //{
        //    _oracleToPgsqlRepository.OracleToPgsqlDataSync("0_DB_PS_StardandOrg2.xml");
        //}

        // [Fact(DisplayName = "1_DB_PS_CorssOrg数据同步")]// 公司别不为空问题
        // public void PS_CorssOrgDataSync()
        // {
        //     _oracleToPgsqlRepository.OracleToPgsqlDataSync("1_DB_PS_CorssOrg.xml");
        // }

        //[Fact(DisplayName = "2_DB_PS_StardandOrg2数据同步")]
        //public void DB_PS_StardandOrg2DataSync()
        //{
        //    _oracleToPgsqlRepository.OracleToPgsqlDataSync("2_DB_PS_StardandOrg2.xml");
        //}

        //[Fact(DisplayName = "4_DB_LOT_Employee数据同步")]
        //public void DB_LOT_EmployeeDataSync()
        //{
        //    _oracleToPgsqlRepository.OracleToPgsqlDataSync("4_DB_LOT_Employee.xml");
        //}

        //[Fact(DisplayName = "05_DB_Bank_Account_PRD数据同步")]
        //public void DB_Bank_Account_PRDDataSync()
        //{
        //    _oracleToPgsqlRepository.OracleToPgsqlDataSync("05_DB_Bank_Account_PRD.xml");
        //}

        // [Fact(DisplayName = "5_DB_PS_Cohead数据同步")] //公司别字段缺失
        // public void DB_PS_CoheadDataSync()
        // {
        //     _oracleToPgsqlRepository.OracleToPgsqlDataSync("5_DB_PS_Cohead.xml");
        // }

        // [Fact(DisplayName = "ProjectCode 同步")]
        // public void PJCodeDataSync()
        // {
        //     _oracleToPgsqlRepository.OracleToPgsqlDataSync("6_PMCS_PJCODE.xml");
        // }

        // [Fact(DisplayName = "SAP_EXCH_RATE 同步")]
        // public void SAP_EXCH_RATESync()
        // {
        //     _oracleToPgsqlRepository.OracleToPgsqlDataSync("7_SAP_EXCH_RATE.xml");
        // }
    }
}