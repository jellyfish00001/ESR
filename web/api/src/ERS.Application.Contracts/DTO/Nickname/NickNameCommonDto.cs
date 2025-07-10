using System;
using System.Collections.Generic;
namespace ERS.DTO
{
    public class NickNameCommonDto
    {
        public Guid? Id { get; set; }
        public string company { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否带昵称
        /// </summary>
        public string iscarry { get; set; }
        public DateTime? cdate { get; set; }
        public string cuser { get; set; }
        public string muser { get; set; }
        public DateTime? mdate { get; set; }
        public List<string> companyList { get; set; }
    }
}
