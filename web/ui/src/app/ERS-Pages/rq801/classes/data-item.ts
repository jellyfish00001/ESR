import { InvoiceDetail } from './../../_components/invoices-modal/classes/data-item';
export interface Cash5ExcelDto
{
  rno: string;  //報銷單號
  senarioname: string;  //費用類別
  deptid: string;  //費用歸屬部門
  agentemplid: string;  //承辦人
  unifycode: string;  //廠商統一編號
  billnoandsummary: string;  //提單號碼/費用摘要
  reportno: string; //報單號碼
  invoice: string;  //稅單號碼/發票號碼/憑證號碼
  rdate: Date  //稅單付訖日期/發票日期/憑證日期
  invoiceAmountBeforeTax: number; //發票稅前金額 = 進口稅/其他費用 + 推貿費
  importtax: number; //進口稅/其他費用
  tradepromotionfee: number; //推貿費
  taxexpense: number;  //營業稅
  totaltaxandfee: number;  //稅費合計
  taxbaseamount: number; //稅基
  expensecode: string;  //報銷代碼
  accountcode: string;  //會計科目
}

export interface InvoiceDetail2{
  rno: string;  //報銷單號
  invcode: string;  //發票代碼
  invno: string;  //發票號碼
  invtype: string;  //發票類型
  invdate: Date;  //發票日期
  amount: number;  //發票金額
  curr: string;  //幣別
  buyertaxid: string;  //買方稅號/統編
  sellertaxid: string;  //賣方稅號/統編
  importtax: number;  //進口稅
  tradepromotionfee: number; //推貿費
  taxbaseamount: number; //稅基
  remark: string;  //備註
}

export interface OverdueChargeAgainstDetail {
  companyCode: string;
  applicantName: string;
  applicantId: string;
  payeeName: string;
  payeeId: string;
  advanceFundRno: string;
  digest: string;
  appliedAmt: number;
  notChargeAgainstAmt: number;
  openDays: number;
  delayTimes: number;
  sceneCode: string;
  sceneName: string;
  file: any;
  disabled: boolean;
}
export interface ExceptionDetail {
  invoiceCode: string;
  invoiceNo: string;
  amount: number;
  taxLoss: number;
  curr: string;
  affordParty: string;
  disabled: boolean;
  // id: number;
  index: string;
  uid: string;
  exTips: string;
  toLocalTaxLoss: number;
  affordPartyValue: string;
  reason: string;
  invdate: Date;
  taxamount: number;
  oamount: number;
  invstat: string;
  abnormalamount: number;
  paymentName: string;
  paymentNo: string;
  collectionName: string;
  collectionNo: string;
  expdesc: string;
  expcode: string;
  invdesc: string;
  invoiceSource: string;
  fileUrl: string;
  taxrate: number;
}

export interface GeneralExpenseInfo {
  advanceRno: string;
  scene: string;
  sceneName: string;
  attribDept: string;
  attribDeptList: any[];
  startingPlace: string;
  cityOnBusiness: string;
  feeDate: string;
  carType: string;
  carTypeName: string;
  kil: number;
  digest: string;
  startingTime: string;
  backTime: string;
  curr: string;
  expenseAmt: number;
  exchangeRate: number;
  toLocalAmt: number;
  attachList: any[];
  fileList: any[];
  invoiceDetailList: ExceptionDetail[];
  fileCategory: string;
  id: number;
  disabled: boolean;
  selfTaxAmt: number;
  actualAmt: number;
  whiteRemark: string;
  taxexpense: number;
}
