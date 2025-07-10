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
  underwriter: string;
}

export interface GeneralExpenseInfo {
  attribDept: string;
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
  payeeId: string;
  payeeName: string;
  payeeDeptid: string;
  bankName: string;
  id: number;
  disabled: boolean;
  whiteRemark: string;
  taxexpense: number;
}
