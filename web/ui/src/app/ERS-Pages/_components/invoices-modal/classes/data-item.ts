export class InvoiceDetail {
  invoiceid: string;     // 票夾發票id
  invcode: string;     // 發票代碼
  invno: string;     // 發票號碼
  invdate: string;     // 發票日期
  amount: number;     // 開票金額（不含稅）
  taxamount: number;     // 稅額
  oamount: number;     // 含稅總金額
  taxloss: number;     // 税金损失
  curr: string;     // 幣別
  invtype: string;     // 發票類型
  verifyStateDesc: string;     // 校驗狀態
  collectionName: string;     // 收款方名称
  collectionNo: string;     // 收款方证件号
  paymentName: string;     // 付款方名称
  paymentNo: string;     // 款方证件号
  paymentStat: boolean;     // 請款狀態
  expdesc: string;     // 異常原因
  fileurl: string;     // 發票文件url
  filepath: string;     // 發票文件路徑
  expcode: string;     // 異常描述？
  invstat: string;     // 發票狀態
  invdesc: string;     // 發票描述
  paymentStatDesc: string;    // 请款状态
  disabled: boolean;
  remark: string;
  sellertaxid: string;     // 卖方统编
  source: string;     // 來源
  affordParty: string;      // 承担方
}