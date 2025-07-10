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
  paymentDate: string;
  apid: string;
  status: string;
}

export const StateInfo = [
  {
    label: 'save-state',
    value: "T",
    checked: false
  },
  {
    label: 'signing',
    value: "P",
    checked: false
  },
  {
    label: 'approved',
    value: "A",
    checked: false
  },
  {
    label: 'reject',
    value: "R",
    checked: false
  },
  {
    label: 'return',
    value: "B",
    checked: false
  },
  {
    label: 'cancel',
    value: "C",
    checked: false
  }
]

export const VoucherTypeInfo = [
  {
    label: 'electronic-invoice',
    value: "N",
    checked: false
  },
  {
    label: 'other-voucher',
    value: "H",
    checked: false
  },
  {
    label: 'many-reimbursement',
    value: "CASH_5",
    checked: false
  }
]

export const FormTypeInfo = [
  {
    label: 'general-expenses',
    value: "CASH_1",
    checked: false
  },
  {
    label: 'entertainment-expenses',
    value: "CASH_2",
    checked: false
  },
  {
    label: 'prepayments',
    value: "CASH_3",
    checked: false
  },
  {
    label: 'batch-reimbursement',
    value: "CASH_4",
    checked: false
  },
  {
    label: 'many-reimbursement',
    value: "CASH_5",
    checked: false
  },
  {
    label: 'delay-advance',
    value: "CASH_3A",
    checked: false
  },
  {
    label: 'return-taiwan',
    value: "CASH_6",
    checked: false
  },
  {
    label: 'salary-request',
    value: "CASH_X",
    checked: false
  }
]
