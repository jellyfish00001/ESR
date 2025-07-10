export interface AccountingDetail {
  sequenceNo: number;
  documentDate: string;
  postingDate: string;
  company: string;
  currency: string;
  exchangeRate: number;
  reference: string;
  documentHeaderText: string;
  documentType: string;
  postingKey: string;
  accountNumber: number;
  specialGLIndicator: string;
  amountInDocumentCurrency: number;
  amountInLocalCurrency: number;
  paymentTerm: string;
  paymentMethod: string;
  baseLineDate: number;
  taxCode: number;
  taxBaseAmount: number;
  LCTaxBaseAmount: number;
  withholdingTaxType: string;
  withholdingTaxCode: string;
  withholdingTaxBaseAmount: string;
  withholdingtaxamount: number;
  costCenter: string;
  order: string;
  lineText: string; // Line Text
  assignmentNumber: string; // Assignment number
  profitCenter: string; // Profit Center
  partnerProfitCenter: string; // Partner Profit Center
  customerCode: string; // Customer Code (ie. Bill-to Party)
  plant: string; // Plant
  businessType: string; // Business Type
  endCustomer: string; // End customer
  materialDivision: string; // Material Division
  salesDivision: string; // Sales Division
  reference1: string; // Reference 1
  reference2: string; // Reference 2
  reference3: string; // Reference 3
  unifyCode: string; // UNIFYCODE
  certificate: string; // CERTIFICATE
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
  accountingRemarks: string;
  taxexpense: number;
  invoiceNo:string;
  companyCode: string;
  cashDetailId: string;
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
