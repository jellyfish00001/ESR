export class InvoiceInfo {
  id: string;
  buyername: string;  // 购货方名称
  sellername: string;  // 销售方名称
  sellertaxid: string;  // 销售方税号
  invcode: string;  // 发票代码
  invno: string;  // 发票号码
  invtype: string;  // 发票类型
  invdate: string;  // 发票日期
  untaxamount: number;  // 不含税金额
  taxamount: number;  // 税额
  amount: number;  // 含税总金额
  taxrate: number;  // 税率
  verifytype: string;  // 发票核验状态
  paytype: string;  // 发票请款状态
  abnormalreason: string;  // 异常原因
  rno: string;  // ERS报销单号
  filepath: string;  // 发票路径
  emplid: string;  // 
  cuser: string;  // 申请人
  isfill: boolean;
  url: string;  // 发票图片url
  curr: string; // 币别
  cdate: string;  // 添加日期
  buyertaxid: string; // 购买方税号
  remark: string;
}
