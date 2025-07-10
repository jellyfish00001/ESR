export interface ExpensesInfo {
  id: number;
  feeDate: string;
  entertainObject: string;
  accompany: string;
  remark: string;
  expenseAttribDept: string;
  curr: string;
  appliedAmount: number;
  toLocalCurrAmount: number;
  exchangeRate: number;
  selfAffordAmt: number;
  companyAffordAmt: number;
  whiteRemark: string;
  selfTaxAmt: number;
  actualAmt: number;
}

export interface SignLogList {
  step: string;
  signUser: string;
  signDate: string;
  status: string;
  remark: string;
}
export interface SignList {
  step: string;
  signUser: string;
  status: string;
  company: string;
}

export interface ApprovalParams {
  company: string;
  rno: string;
  remark: string;
  inviteMethod: number;
  inviteUser: string;
  paperSign: boolean;
  toEmplid1: string;
  toEmplid2: string;
  toEmplid3: string;
  detail: any[];
}

export interface Accountant {
  Id: string;
  companyCode: string;
  plant: string;
  rv1: string;
  rv2: string;
  rv3: string;
}
