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
}

export interface AdvanceFundInfo {
  id: number;
  advanceSceneName: string;
  advanceScene: string;
  requiredPaymentDate: string;
  digest: string;
  advanceDate: string;
  requestPaymentName: string;
  requestPayment: string;
  curr: string;
  appliedAmt: number;
  toLocalAmt: number;
  exchangeRate: number;
  remark: string;
  bpmRno: string;
  fileCategory: string;
  fileList: any[];
  disabled: boolean;
}
