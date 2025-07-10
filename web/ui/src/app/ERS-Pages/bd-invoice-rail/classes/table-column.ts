import { TableColumnModel } from 'src/app/shared/models';
import { BdInvoiceRail } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'qi',
    columnKey: 'qi',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) => a.qi.localeCompare(b.qi),
  },
  {
    title: 'invoicerail',
    columnKey: 'invoicerail',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.invoicerail.localeCompare(b.invoicerail),
  },
  {
    title: 'year',
    columnKey: 'year',
    columnWidth: '70px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) => a.year - b.year,
  },
  {
    title: 'month',
    columnKey: 'month',
    columnWidth: '70px',
    align: 'left',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) => a.month - b.month,
  },
  {
    title: 'formatcode',
    columnKey: 'formatcode',
    columnWidth: '70px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) => a.formatcode - b.formatcode,
  },
  {
    title: 'exp-invoicecategory',
    columnKey: 'invoicetype',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.invoicetype.localeCompare(b.invoicetype),
  },
  {
    title: 'create-user',
    columnKey: 'creator',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.creator.localeCompare(b.creator),
  },
  {
    title: 'create-date',
    columnKey: 'createDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.createDate.localeCompare(b.createDate),
  },
  {
    title: 'update-user',
    columnKey: 'updateUser',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.updateUser.localeCompare(b.updateUser),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: BdInvoiceRail, b: BdInvoiceRail) =>
      a.updateDate.localeCompare(b.updateDate),
  },
];
