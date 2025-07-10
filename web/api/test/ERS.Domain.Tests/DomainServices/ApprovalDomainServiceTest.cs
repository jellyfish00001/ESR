using ERS.Entities;
using ERS.IDomainServices;
using ERS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERS.DomainServices
{
    public class ApprovalDomainServiceTest : ERSDomainTestBase
    {
        private IApprovalDomainService _ApprovalDomainService;

        public ApprovalDomainServiceTest()
        {
            _ApprovalDomainService = GetRequiredService<IApprovalDomainService>();
        }


        //[Fact(DisplayName = "获取主管签核")]
        //public async Task AddDefinePermissionSign()
        //{
        //    string token = "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjFBRTQwOEU4RDUwNDk5MzlBQjhGMjVGRTAyMzA3QjAyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NjU3MTQ0NDksImV4cCI6MTY2NTcxODA0OSwiaXNzIjoiaHR0cHM6Ly9hdXRoZHd0Lndpc3Ryb24uY29tL2F1dGgiLCJjbGllbnRfaWQiOiJFbGVjdHJvbmljRG9jIiwic3ViIjoiWjE5MDgxOTYxIiwiYXV0aF90aW1lIjoxNjY1NzE0NDQ2LCJpZHAiOiJsb2NhbCIsIm5hbWUiOiJDb2R5IEh1YW5nIiwiZW1haWwiOiJDb2R5X0h1YW5nQHdpc3Ryb24uY29tIiwibG9jYXRpb24iOiJBMTMiLCJyb2xlIjoiYWRtaW4iLCJhY3Rfc3ViIjoiIiwianRpIjoiQ0NDMjBBRkE2MjZCQTBDNEY0NTMxMkU1MUYwOTJENzQiLCJzaWQiOiI5NzdFNDcwMjM3MTNCODgyOEYzMjQxMjc4RDQwNUU3RSIsImlhdCI6MTY2NTcxNDQ0OSwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImVtYWlsIl0sImFtciI6WyJwd2QiXX0.aFoXR_j9rPcp6pzAPf27OK8n9govuhvftmnmLt11uuyhuAYqnDyYuHy9qBcUc31-zYpjekGugo78_f-PnfyuzEaXw1QwHsx5_Lr7qfQhUXxJhu-HK3ZaE2UixvRrVLA4FA3g2BgG11nvtcw9bWkwpDboafsr0zoQEUFBH0cC--k6jRWLH9xN0JLvMdAP5Uoy9v2pp2Bs7DQ3TyXSronaCTB_dTDlFlh5Uol8YlUaC2dxcFI_A2XCV4XT-_U9mDp9ohWtCjwM8lZhav46TezksCM2NfLfiWqgkUC_2L9FvQL1vZi-emBJcQMy17TV39uqzhOMbQg8OSBES8BJL3icmg";
        //    CashHead list = new()
        //    {
        //        dtype = "A1",
        //        CashDetailList = new List<CashDetail>() { new CashDetail() { baseamt = 10000} },
        //        currency = "RMB",
        //        cuser = "Z19081961",
        //        company = "WZS"
        //    };

        //    await _ApprovalDomainService.AddDefinePermissionSign(list, token);

        //}

        [Theory(DisplayName = "添加收款人")]
        [InlineData("Z19081961", "Z19081962")]
        public void AddPayeeSign(string cuser, string payeeid)
        {
            Signs data = _ApprovalDomainService.AddPayeeSign(cuser, payeeid);
            Assert.NotNull(data);
        }

        [Fact(DisplayName = "费用情景加签（前）")]
        public async Task AddCostSignBefore()
        {
            CashHead list = new()
            {
                formcode = "CASH_2",
                company = "WZS",
                CashDetailList = new List<CashDetail>() { new CashDetail() { expcode = "EXP10" } }
            };

            IList<Signs> data = await _ApprovalDomainService.AddCostSignBefore(list);
            Assert.True(data.Count > 0);
        }

        // [Fact(DisplayName = "费用情景加签（后）")]
        // public async Task AddCostSignAfter()
        // {
        //     CashHead list = new()
        //     {
        //         formcode = "CASH_2",
        //         company = "WZS",
        //         CashDetailList = new List<CashDetail>() { new CashDetail() { expcode = "EXP22" } }
        //     };

        //     IList<Signs> data = await _ApprovalDomainService.AddCostSignAfter(list);
        //     Assert.True(data.Count > 0);
        // }

        [Fact(DisplayName = "财务加签")]
        public async Task AddFinanceSign()
        {
            CashHead list = new()
            {
                formcode = "CASH_2",
                company = "WZS",
                cuser = "Z19081961",
                CashDetailList = new List<CashDetail>() { new CashDetail() { baseamt = 10000, deptid = "JML121" } }
            };
            IList<Signs> data = await _ApprovalDomainService.AddFinanceSign(list, true);
            Assert.True(data.Count > 0);
        }
    }
}
