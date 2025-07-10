export interface ReturnTaiwanInfo {
  feeDate: string;
  feeType: string;
  feeTypeName: string;
  digest: string;
  expenseAttribDept: string;
  curr: string;
  appliedAmount: number;
  exchangeRate: number;
  toLocalCurrAmount: number;
  remark: string;
  id: number;
  disabled: boolean;
  whiteRemark: string;
  selfTaxAmt: number;
  actualAmt: number;
}

export interface ExceptionDetail {
  invoiceCode: string;
  invoiceNo: string;
  amount: number;
  taxLoss: number;
  curr: string;
  affordParty: string;
  disabled: boolean;
  id: number;
  index: number;
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
  invtype: string;
  invoiceid: string;
  invabnormalreason: string;
  fileurl: string;
}