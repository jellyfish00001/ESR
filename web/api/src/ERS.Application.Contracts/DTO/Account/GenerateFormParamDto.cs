using System;
namespace ERS.DTO.Account
{
    public class GenerateFormParamDto
    {
        public string carryno { get; set; }//入账清单流水号
        public string acctant { get; set; }//签核会计（工号）
        public DateTime? startpostdate { get; set; }//结转日期起始时间
        public DateTime? endpostdate { get; set; }//结转日期结束时间
    }
}