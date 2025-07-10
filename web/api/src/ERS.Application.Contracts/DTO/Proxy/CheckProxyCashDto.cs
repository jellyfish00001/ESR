using System.Collections.Generic;
namespace ERS.DTO
{
    public class CheckProxyCashDto
    {
        /// <summary>
        /// 是否可代理
        /// </summary>
        public bool isproxy { get; set; }
        /// <summary>
        /// 登录人
        /// </summary>
        public string cuser { get; set; }
        /// <summary>
        /// 可代理报销对象
        /// </summary>
        public List<string> proxylist { get; set; }
    }
}
