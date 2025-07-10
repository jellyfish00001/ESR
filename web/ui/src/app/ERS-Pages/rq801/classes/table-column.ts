import { TableColumnModel } from 'src/app/shared/models';
import { OverdueChargeAgainstDetail } from '../../rq401/classes/data-item';
import { ExceptionDetail, GeneralExpenseInfo } from './data-item';

export const InvoiceTableColumn: TableColumnModel[] = [
  {
    title: ('invoice-type'),  //發票類型
    columnKey: 'invtype',
    columnWidth: '40px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('invoice-date'), //發票日期
    columnKey: 'invdate',
    columnWidth: '30px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('invoice-amount'), //發票金額
    columnKey: 'amount',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('currency'), //幣別
    columnKey: 'curr',
    columnWidth: '20px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('buyer_tax_no'), //買方稅號/統編
    columnKey: 'buyertaxid',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('sales_tax_no'), //賣方稅號/統編
    columnKey: 'sellertaxid',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('import-tax'), //進口稅​
    columnKey: 'importtax',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('tradePromotionFee'), //推廣貿易服務費​
    columnKey: 'tradepromotionfee',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('businessTax'), //營業稅
    columnKey: 'businessTax',
    columnWidth: '30px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('businessTaxBase'), //營業稅稅基
    columnKey: 'businessTaxBase',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  },
  {
    title: ('col.remark'), //備註
    columnKey: 'remark',
    columnWidth: '50px',
    align: 'center',
    sortFn: null
  }
];

export const ExcelTableColumn: TableColumnModel[] = [
  {
    title: ('reimbursement-scene'),  //報銷情景
    columnKey: 'senarioname',
    columnWidth: '60px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('expense-category'),  //費用類別
    columnKey: 'expensecode',
    columnWidth: '60px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('accounting-subject'),  //會計科目
    columnKey: 'accountcode',
    columnWidth: '60px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('col.expense-attribution-department'),  //費用歸屬部門
    columnKey: 'deptid',
    columnWidth: '50px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('digest'),  //提單號碼/費用摘要
    columnKey: 'billnoandsummary',
    columnWidth: '90px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('vendor-unifyCode'), //廠商統一編號
    columnKey: 'unifycode',
    columnWidth: '50px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('reportNumber'),  //報單號碼
    columnKey: 'reportno',
    columnWidth: '60px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('taxInvoiceOrVoucherNumber'),  //稅單號碼/發票號碼/憑證號碼
    columnKey: 'invoice',
    columnWidth: '80px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('paymentOrInvoiceOrVoucherDate'),  //稅單付訖日期/發票日期/憑證日期
    columnKey: 'rdate',
    columnWidth: '80px',
    align: 'center',
    sortFn:null,
  },
  {
    title: ('invoiceAmountBeforeTax'),  //發票稅前金額 = 進口稅/其他費用 + 推貿費
    columnKey: 'invoiceAmountBeforeTax',
    columnWidth: '60px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('businessTax'),  //營業稅
    columnKey: 'taxexpense',
    columnWidth: '30px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('totalTaxAndFees'),  //稅費合計
    columnKey: 'totaltaxandfee',
    columnWidth: '40px',
    align: 'center',
    sortFn: null,
  },
  {
    title: ('taxbase'),  //稅基
    columnKey: 'taxbaseamount',
    columnWidth: '30px',
    align: 'center',
    sortFn: null,
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
