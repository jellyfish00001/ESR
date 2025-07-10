import { TableColumnModel } from 'src/app/shared/models';
import { ExpenseInfo, ExceptionDetail } from './data-item';

export const DetailTableColumn: TableColumnModel[] = [
  {
    title: ('col.invoice-file'),
    columnKey: 'fileurl',
    columnWidth: '20%',
    align: 'right',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.fileurl.localeCompare(b.fileurl),
  },
  {
    title: ('invoiceSource'),
    columnKey: 'source',
    columnWidth: '20%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
      a.invoiceSource.localeCompare(b.invoiceSource),
  },
  {
    title: ('causes-of-exceptions'),
    columnKey: 'reason',
    columnWidth: '23%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.reason.localeCompare(b.reason),
  },
];

export const ExpenseTableColumn: TableColumnModel[] = [
  {
    title: 'col.datetime',
    columnKey: 'feeDate',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) =>
      a.feeDate > b.feeDate,
  },
  {
    title: 'col.currency',
    columnKey: 'curr',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: 'col.applied-amount',
    columnKey: 'appliedAmount',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.appliedAmount - b.appliedAmount,
  },
  {
    title: 'col.conversion-to-local-currency',
    columnKey: 'toLocalCurrAmount',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.toLocalCurrAmount - b.toLocalCurrAmount,
  },
  {
    title: 'col.exchange-rate',
    columnKey: 'exchangeRate',
    columnWidth: '5%',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: 'col.expense-attribution-department',
    columnKey: 'expenseAttribDept',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.expenseAttribDept.localeCompare(b.expenseAttribDept),
  },
  {
    title: 'col.host-object',
    columnKey: 'entertainObject',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.entertainObject.localeCompare(b.entertainObject),
  },
  {
    title: 'col.company-escort',
    columnKey: 'accompany',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.accompany.localeCompare(b.accompany),
  },
  {
    title: 'col.remark',
    columnKey: 'remark',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.remark.localeCompare(b.remark),
  },
  {
    title: 'col.total-invoice-value',
    columnKey: 'invoiceTotalAmount',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.invoiceTotalAmount - b.invoiceTotalAmount,
  },
  {
    title: 'col.entertain-date',
    columnKey: 'entertainDate',
    columnWidth: '7%',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.entertainDate > b.entertainDate,
  },
  {
    title: ('individual-responsibility-for-taxes'),
    columnKey: 'selfTaxAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.selfTaxAmt - b.selfTaxAmt,
  },
  {
    title: ('actual-reimbursable-amount'),
    columnKey: 'actualAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.actualAmt - b.actualAmt,
  },
  {
    title: ('unifyCode'),
    columnKey: 'unifycode',
    columnWidth: '',
    align: 'center',
    sortFn: (a: ExpenseInfo, b: ExpenseInfo) => a.unifyCode > b.unifyCode,
  },
];