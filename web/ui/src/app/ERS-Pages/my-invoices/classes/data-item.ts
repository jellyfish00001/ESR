export class InvoiceInfo {
  id: string;
  buyername: string;  // 购货方名称
  buyertaxno: string; // 购买方税号
  sellername: string;  // 销售方名称
  salestaxno: string;  // 销售方税号
  invcode: string;  // 发票代码
  invno: string;  // 发票号码
  invtype: string;  // 发票类型
  invdate: string;  // 发票日期
  oamount: number;  // 含税总金额
  taxamount: number;  // 税额/營業稅/增值稅
  amount: number;  // 不含税金额
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
  startstation: string; // 出发地
  endstation: string; // 到达地
  source: string; // 发票来源
  ocrid: string; //OCR識別記錄表ID
  identificationno: string; //存票據上唯一值識別碼
  invoicetitle: string; //發票標題
  taxbase: number; //營業/增值税基
  importtaxamount: number; //進口稅
  servicefee: number; //服務費/推廣貿易服務費
  shippingfee: number; //運輸費
  transactionfee: number; //手續費
  quantity: number; //數量
  productinfo: string; //商品資訊
  invoicecategory: string; //發票類型
  remarks: string; //備註
  responsibleparty: string; //負責方
  taxtype: string; //課稅別
}

export enum InvoiceFieldDefinition {
  invCode = 'invcode',
  invNo = 'invno',
  invDate = 'invdate',
  invType = 'invtype',
  curr = 'curr',
  oAmount = 'oamount',
  amount = 'amount',
  taxAmount = 'taxamount',
  startStation = 'startstation',
  endStation = 'endstation',
  buyerTaxNo = 'buyertaxno',
  salesTaxNo = 'salestaxno',
  invoiceTitle = 'invoicetitle',
  taxBase = 'taxbase',
  importTaxAmount = 'importtaxamount',
  serviceFee = 'servicefee',
  shippingFee = 'shippingfee',
  transactionFee = 'transactionfee',
  quantity = 'quantity',
  productInfo = 'productinfo',
  taxType = 'taxtype',
  remarks = 'remarks',
  isAbnormal = 'isabnormal',
  abnormalReason = 'abnormalreason',
  responsibleParty= "responsibleparty",
  invTypeCode = 'invtypecode',
}