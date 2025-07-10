export interface ExceptionDetail {
  invoiceCode: string;
  invoiceNo: string;
  amount: number;
  taxLoss: number;
  curr: string;
  affordParty: string;
  disabled: boolean;
  id: number;
  // index: string;
  // uid: string;
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

export interface ExpenseInfo {
  feeDate: string;
  curr: string;
  appliedAmount: number;
  toLocalCurrAmount: number;
  exchangeRate: number;
  expenseAttribDept: string;
  entertainObject: string;
  accompany: string;
  remark: string;
  invoiceTotalAmount: number;
  entertainDate: string;
  id: number;
  disabled: boolean;
}
