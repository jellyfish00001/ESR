using System;
using System.Collections.Generic;
namespace ERS.DTO.Invoice
{
    /// <summary>
    /// ers生成付款清单时传递参数
    /// </summary>
    public class ERSPayDto
    {
        /// <summary>
        /// ers申请单号
        /// </summary>
        public string ERSRno { get; set; }
        /// <summary>
        /// 传票号
        /// </summary>
        public string DocNo { get; set; }
        /// <summary>
        /// 传票日期
        /// </summary>
        public DateTime? DocDate { get; set; }
        /// <summary>
        /// 发票
        /// </summary>
        public List<ERSInvDto> invoices { get; set; }
    }
}