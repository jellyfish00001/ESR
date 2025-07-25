import { TableColumnModel } from 'src/app/shared/models';
import { OverdueChargeAgainstDetail } from '../../rq401/classes/data-item';
import { ExceptionDetail, GeneralExpenseInfo } from './data-item';

export const DetailTableColumn: TableColumnModel[] = [
  {
    title: ('col.invoice-file'),
    columnKey: 'fileurl',
    columnWidth: '20%',
    align: 'right',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.fileUrl.localeCompare(b.fileUrl),
  },
  {
    title: ('invoiceSource'),
    columnKey: 'source',
    columnWidth: '20%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
      a.invoiceSource.localeCompare(b.invoiceSource),
  },
  // {
  //   title: ('invoice-code'),
  //   columnKey: 'invoiceCode',
  //   columnWidth: '20%',
  //   align: 'left',
  //   sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
  //     a.invoiceCode.localeCompare(b.invoiceCode),
  // },
  // {
  //   title: ('invoice-no'),
  //   columnKey: 'invoiceNo',
  //   columnWidth: '20%',
  //   align: 'left',
  //   sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.invoiceNo.localeCompare(b.invoiceNo),
  // },
  // {
  //   title: ('invoice-amount'),
  //   columnKey: 'oamount',
  //   columnWidth: '20%',
  //   align: 'left',
  //   sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.oamount - b.oamount,
  // },
  {
    title: ('causes-of-exceptions'),
    columnKey: 'reason',
    columnWidth: '23%',
    align: 'left',
    sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.reason.localeCompare(b.reason),
  },
  // {
  //   title: ('tax-loss'),
  //   columnKey: 'taxLoss',
  //   columnWidth: '23%',
  //   align: 'left',
  //   sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.taxLoss - b.taxLoss,
  // },
  // {
  //   title: ('afford-party'),
  //   columnKey: 'affordParty',
  //   columnWidth: '20%',
  //   align: 'right',
  //   sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.affordParty.localeCompare(b.affordParty),
  // },

];

export const ExpenseTableColumn: TableColumnModel[] = [
  {
    title: ('expname'),
    columnKey: 'sceneName',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.scene.localeCompare(b.scene),
  },
  {
    title: ('date-of-expense'),
    columnKey: 'feeDate',
    columnWidth: '120px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.feeDate.localeCompare(b.feeDate),
  },
  {
    title: ('col.currency'),
    columnKey: 'curr',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: ('col.expense-attribution-department'),
    columnKey: 'attribDept',
    columnWidth: '120px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.attribDept.localeCompare(b.attribDept),
  },
  {
    title: ('percent'),
    columnKey: 'percent',
    columnWidth: '110px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('reimbursement-amount'),
    columnKey: 'expenseAmt',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.expenseAmt - b.expenseAmt,
  },
  {
    title: ('col.conversion-to-local-currency'),
    columnKey: 'toLocalAmt',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.toLocalAmt - b.toLocalAmt,
  },
  {
    title: ('col.exchange-rate'),
    columnKey: 'exchangeRate',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: ('digest'),
    columnKey: 'digest',
    columnWidth: '80px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.digest.localeCompare(b.digest),
  },
  {
    title: ('individual-responsibility-for-taxes'),
    columnKey: 'selfTaxAmt',
    columnWidth: '120px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.selfTaxAmt - b.selfTaxAmt,
  },
  {
    title: ('actual-reimbursable-amount'),
    columnKey: 'actualAmt',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.actualAmt - b.actualAmt,
  },
  {
    title: ('advance-fund-no'),
    columnKey: 'advanceRno',
    columnWidth: '120px',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.advanceRno.localeCompare(b.advanceRno),
  },
];

export const OvertimeMealExpenseTableColumn: TableColumnModel[] = [
  {
    title: ('expname'),
    columnKey: 'sceneName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.scene.localeCompare(b.scene),
  },
  {
    title: ('col.expense-attribution-department'),
    columnKey: 'attribDept',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.attribDept.localeCompare(b.attribDept),
  },
  {
    title: ('percent'),
    columnKey: 'percent',
    columnWidth: '',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('city-on-business'),
    columnKey: 'cityOnBusiness',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.cityOnBusiness.localeCompare(b.cityOnBusiness),
  },
  {
    title: ('date-of-expense'),
    columnKey: 'feeDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.feeDate.localeCompare(b.feeDate),
  },
  {
    title: ('start-time'),
    columnKey: 'startingTime',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.startingTime.localeCompare(b.startingTime),
  },
  {
    title: ('back-time'),
    columnKey: 'backTime',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.backTime.localeCompare(b.backTime),
  },
  {
    title: ('col.currency'),
    columnKey: 'curr',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: ('reimbursement-amount'),
    columnKey: 'expenseAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.expenseAmt - b.expenseAmt,
  },
  {
    title: ('col.exchange-rate'),
    columnKey: 'exchangeRate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: ('col.conversion-to-local-currency'),
    columnKey: 'toLocalAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.toLocalAmt - b.toLocalAmt,
  },
  {
    title: ('digest'),
    columnKey: 'digest',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.digest.localeCompare(b.digest),
  },
  {
    title: ('advance-fund-no'),
    columnKey: 'advanceRno',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.advanceRno.localeCompare(b.advanceRno),
  },
];

export const DriveFuelExpenseTableColumn: TableColumnModel[] = [
  {
    title: ('expname'),
    columnKey: 'sceneName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.scene.localeCompare(b.scene),
  },
  {
    title: ('col.expense-attribution-department'),
    columnKey: 'attribDept',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.attribDept.localeCompare(b.attribDept),
  },
  {
    title: ('percent'),
    columnKey: 'percent',
    columnWidth: '',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('starting-place'),
    columnKey: 'startingPlace',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.startingPlace.localeCompare(b.startingPlace),
  },
  {
    title: ('date-of-expense'),
    columnKey: 'feeDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.feeDate.localeCompare(b.feeDate),
  },
  {
    title: ('vehicle-type'),
    columnKey: 'carTypeName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.carTypeName.localeCompare(b.carTypeName),
  },
  {
    title: ('kil'),
    columnKey: 'kil',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.kil - b.kil,
  },
  {
    title: ('col.currency'),
    columnKey: 'curr',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: ('reimbursement-amount'),
    columnKey: 'expenseAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.expenseAmt - b.expenseAmt,
  },
  {
    title: ('col.exchange-rate'),
    columnKey: 'exchangeRate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.exchangeRate - b.exchangeRate,
  },
  {
    title: ('col.conversion-to-local-currency'),
    columnKey: 'toLocalAmt',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.toLocalAmt - b.toLocalAmt,
  },
  {
    title: ('digest'),
    columnKey: 'digest',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.digest.localeCompare(b.digest),
  },
  {
    title: ('advance-fund-no'),
    columnKey: 'advanceRno',
    columnWidth: '',
    align: 'center',
    sortFn: (a: GeneralExpenseInfo, b: GeneralExpenseInfo) => a.advanceRno.localeCompare(b.advanceRno),
  },
];

export const chargeAgainstTableColumn: TableColumnModel[] = [
  {
    title: ('company-code'),
    columnKey: 'companyCode',
    columnWidth: '70px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
      a.companyCode.localeCompare(b.companyCode),
  },
  {
    title: ('applicant-name'),
    columnKey: 'applicantName',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.applicantName.localeCompare(b.applicantName),
  },
  {
    title: ('applicant-emplid'),
    columnKey: 'applicantId',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.applicantId.localeCompare(b.applicantId),
  },
  {
    title: ('payee'),
    columnKey: 'payeeName',
    columnWidth: '85px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.payeeName.localeCompare(b.payeeName),
  },
  {
    title: ('receiver-emplid'),
    columnKey: 'payeeId',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.payeeId.localeCompare(b.payeeId),
  },
  {
    title: ('advance-fund-no'),
    columnKey: 'advanceFundRno',
    columnWidth: '115px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.advanceFundRno.localeCompare(b.advanceFundRno),
  },
  {
    title: ('digest'),
    columnKey: 'digest',
    columnWidth: '100px',
    align: 'left',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.digest.localeCompare(b.digest),
  },
  {
    title: ('col.applied-amount'),
    columnKey: 'appliedAmt',
    columnWidth: '85px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.appliedAmt - b.appliedAmt,
  },
  {
    title: ('not-charge-against-amount'),
    columnKey: 'notChargeAgainstAmt',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.notChargeAgainstAmt - b.notChargeAgainstAmt,
  },
  {
    title: ('open-days'),
    columnKey: 'openDays',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.openDays - b.openDays,
  },
  {
    title: ('delay-times'),
    columnKey: 'delayTimes',
    columnWidth: '85px',
    align: 'center',
    sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.delayTimes - b.delayTimes,
  }
];
