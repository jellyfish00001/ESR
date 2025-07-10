import { TableColumnModel } from 'src/app/shared/models';
import { InvoiceInfo } from './data-item';

export const DetailTableColumn: TableColumnModel[] = [
  {
    title: ('purchaser-name'),
    columnKey: 'buyername',
    columnWidth: '185px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) =>
      a.buyername.localeCompare(b.buyername),
  },
  {
    title: ('seller-name'),
    columnKey: 'sellername',
    columnWidth: '215px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.sellername.localeCompare(b.sellername),
  },
  {
    title: ('seller-tax-number'),
    columnKey: 'sellertaxid',
    columnWidth: '175px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.sellertaxid.localeCompare(b.sellertaxid),
  },
  {
    title: ('invoice-code'),
    columnKey: 'invcode',
    columnWidth: '110px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.invcode.localeCompare(b.invcode),
  },
  {
    title: ('invoice-no'),
    columnKey: 'invno',
    columnWidth: '175px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.invno.localeCompare(b.invno),
  },
  {
    title: ('exp-invoicecategory'),
    columnKey: 'invtype',
    columnWidth: '160px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.invtype.localeCompare(b.invtype),
  },
  {
    title: ('invoice-date'),
    columnKey: 'invdate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.invdate.localeCompare(b.invdate),
  },
  {
    title: ('price-excluding-tax'),
    columnKey: 'untaxamount',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.untaxamount - b.untaxamount,
  },
  {
    title: ('tax-amount'),
    columnKey: 'taxamount',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.taxamount - b.taxamount,
  },
  {
    title: ('total-amount-including-tax'),
    columnKey: 'amount',
    columnWidth: '105px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.amount - b.amount,
  },
  {
    title: ('col.remark'),
    columnKey: 'remark',
    columnWidth: '100px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.remark.localeCompare(b.remark),
  },
  {
    title: ('invoice-request-status'),
    columnKey: 'paytype',
    columnWidth: '115px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.paytype.localeCompare(b.paytype),
  },
  {
    title: ('causes-of-exceptions'),
    columnKey: 'abnormalreason',
    columnWidth: '160px',
    align: 'left',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.abnormalreason.localeCompare(b.abnormalreason),
  },
  {
    title: ('ers-reimbursement-number'),
    columnKey: 'rno',
    columnWidth: '120px',
    align: 'center',
    sortFn: (a: InvoiceInfo, b: InvoiceInfo) => a.rno.localeCompare(b.rno),
  }
];
