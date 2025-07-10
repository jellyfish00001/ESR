import { TableColumnModel } from 'src/app/shared/models';
import { SalaryInfo } from './data-item';

export const SalaryTableColumn: TableColumnModel[] = [
  {
    title: 'col-company',
    columnKey: 'company',
    columnWidth: '7%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) =>
      a.company.localeCompare(b.company),
  },
  {
    title: 'reimbursement-scene',
    columnKey: 'sceneName',
    columnWidth: '11%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) =>
      a.sceneName.localeCompare(b.sceneName),
  },
  {
    title: 'required-payment-date',
    columnKey: 'feeDate',
    columnWidth: '8%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) =>
      a.feeDate.localeCompare(b.feeDate),
  },
  {
    title: 'salary-month',
    columnKey: 'salaryMonth',
    columnWidth: '10%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) =>
      a.salaryMonth.localeCompare(b.salaryMonth),
  },
  {
    title: 'bank',
    columnKey: 'bank',
    columnWidth: '10%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.bank.localeCompare(b.bank),
  },
  {
    title: 'request-payment',
    columnKey: 'requestPaymentName',
    columnWidth: '7%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) =>
      a.requestPayment.localeCompare(b.requestPayment),
  },
  {
    title: 'col.currency',
    columnKey: 'curr',
    columnWidth: '5%',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: 'col.applied-amount',
    columnKey: 'appliedAmount',
    columnWidth: '11%',
    align: 'right',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.appliedAmount - b.appliedAmount,
  },
  {
    title: 'col.conversion-to-local-currency',
    columnKey: 'toLocalAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.toLocalAmt - b.toLocalAmt,
  },
  {
    title: 'col.exchange-rate',
    columnKey: 'exchangeRate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: 'digest',
    columnKey: 'digest',
    columnWidth: '',
    align: 'left',
    sortFn: (a: SalaryInfo, b: SalaryInfo) => a.digest.localeCompare(b.digest),
  },
];
