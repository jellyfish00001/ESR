export interface FormDetail {
  companyCode: string;
  rno: string;
  formTypeName: string;
  appliedDept: string;
  applicant: string;
  applicantName: string;
  disabled: boolean;
  id: number;
  appliedDate: string;
  expenseType: string;
  dept: string;
  curr: string;
  actualAmount: string;
  step: string;
  // paymentDate: string;
  payment?: string;
  apid: string;
  status: string;
  rdate?: string; // 憑證日期
  invcode?: string; // 發票編碼
  untaxamount?: string; // 不含稅金額
  taxamount?: string; // 含稅金額
  invno?: string; // 發票號碼
  projectcode?: string; // projectcode
  payeeId?: string; // 收款人ID
  payeename?: string; // 收款人名稱
  // 表單明細：
  digest?: string;
}
