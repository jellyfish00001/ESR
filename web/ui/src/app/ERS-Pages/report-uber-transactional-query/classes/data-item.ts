export interface FormDetail {
  tripId?: string;
  transactionTimestamp?: string;
  requestDateUtc?: string;
  requestTimeUtc?: string;
  requestDate?: string;
  requestTime?: string;
  dropOffDateUtc?: string;
  dropOffTimeUtc?: string;
  dropOffDate?: string;
  dropOffTime?: string;
  requestTimezoneOffsetFromUtc?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  employeeId?: string;
  service?: string;
  city?: string;
  distance?: number;
  duration?: number;
  pickupAddress?: string;
  dropOffAddress?: string;
  expenseCode?: string;
  expenseMemo?: string;
  invoices?: string;
  program?: string;
  group: string;
  paymentMethod?: string;
  transactionType?: string;
  fareInLocalCurrency?: number;
  taxesInLocalCurrency?: number;
  tipInLocalCurrency?: number;
  transactionAmountInLocalCurrency?: number;
  localCurrencyCode?: string;
  fareInHomeCurrency?: number;
  taxesInHomeCurrency?: number;
  tipInHomeCurrency?: number;
  transactionAmountInHomeCurrency?: number;
  estimatedServiceAndTechnologyFee?: number;
  rno?: string;
  signStatus?: string;
  fileName?: string;
  company?: string;
}

export const StateInfo = [
  {
    label: 'pending',
    value: "N",
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
]



