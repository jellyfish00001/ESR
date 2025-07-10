using System.Collections.Generic;

namespace ERS.UberToERS
{
    public interface IUberToERSRepository
    {
        //获取昨天交易数据生成签核
        void TransactionToSign();
    
        void test();

        void SyncSignStatus();

        string GetDeptIdToCsv();
        IList<Model.uber_employees> GetAllEmployees();
        string GetEmployeesToCsv(IList<Model.uber_employees> uber_Employees, bool isReMove = false);
        void CsvInsDB(string csvFiledName, string csvContent, bool isDailly = true);
    }
}