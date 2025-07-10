import { TableColumnModel } from 'src/app/shared/models';
import { ReturnTaiwanInfo ,ExceptionDetail} from './data-item';

export const ReturnTaiwanTableColumn: TableColumnModel[] = [
  {
    title: 'col.datetime',
    columnKey: 'feeDate',
    columnWidth: '7%',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.feeDate.localeCompare(b.feeDate),
  },
  {
    title: 'expname',
    columnKey: 'feeTypeName',
    columnWidth: '7%',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.feeTypeName.localeCompare(b.feeTypeName),
  },
  {
    title: 'digest',
    columnKey: 'digest',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.digest.localeCompare(b.digest),
  },
  {
    title: 'col.expense-attribution-department',
    columnKey: 'expenseAttribDept',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.expenseAttribDept.localeCompare(b.expenseAttribDept),
  },
  {
    title: 'col.currency',
    columnKey: 'curr',
    columnWidth: '5%',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: 'col.applied-amount',
    columnKey: 'appliedAmount',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.appliedAmount - b.appliedAmount,
  },
  {
    title: 'col.exchange-rate',
    columnKey: 'exchangeRate',
    columnWidth: '5%',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: 'col.conversion-to-local-currency',
    columnKey: 'toLocalCurrAmount',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.toLocalCurrAmount - b.toLocalCurrAmount,
  },
  {
    title: 'col.remark',
    columnKey: 'remark',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.remark.localeCompare(b.remark),
  },
  {
    title: ('individual-responsibility-for-taxes'),
    columnKey: 'selfTaxAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.selfTaxAmt - b.selfTaxAmt,
  },
  {
    title: ('actual-reimbursable-amount'),
    columnKey: 'actualAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ReturnTaiwanInfo, b: ReturnTaiwanInfo) => a.actualAmt - b.actualAmt,
  },
];

export const DetailTableColumn: TableColumnModel[] = [
  {
    title: 'invoice-code',
    columnKey: 'invoiceCode',
    columnWidth: '20%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
      a.invoiceCode.localeCompare(b.invoiceCode),
  },
  {
    title: 'invoice-no',
    columnKey: 'invoiceNo',
    columnWidth: '15%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.invoiceNo.localeCompare(b.invoiceNo),
  },
  {
    title: 'exception-expense-amount',
    columnKey: 'oamount',
    columnWidth: '',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.oamount - b.oamount,
  },
  {
    title: 'tax-loss',
    columnKey: 'taxLoss',
    columnWidth: '23%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.taxLoss - b.taxLoss,
  },
  {
    title: 'afford-party',
    columnKey: 'affordParty',
    columnWidth: '13%',
    align: 'right',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.affordParty.localeCompare(b.affordParty),
  }
];