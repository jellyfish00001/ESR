import { TableColumnModel } from 'src/app/shared/models';
import { InvTypeInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'company-code',
    columnKey: 'company',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) =>
      a.company.localeCompare(b.company),
  },
  {
    title: 'invoice-code',
    columnKey: 'invtypecode',
    columnWidth: '10%',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) =>
      a.invtypecode.localeCompare(b.invtypecode),
  },
  {
    title: 'col-category',
    columnKey: 'category',
    columnWidth: '',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) =>
      a.invtype.localeCompare(b.category),
  },
  {
    title: 'exp-invoicecategory',
    columnKey: 'invtype',
    columnWidth: '',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) =>
      a.invtype.localeCompare(b.invtype),
  },
  {
    title: 'area',
    columnKey: 'area',
    columnWidth: '',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) => a.invtype.localeCompare(b.area),
  },
  {
    title: 'create-user',
    columnKey: 'cuser',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) => a.cuser.localeCompare(b.cuser),
  },
  {
    title: 'create-date',
    columnKey: 'cdate',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) => a.cdate.localeCompare(b.cdate),
  },
  {
    title: 'update-user',
    columnKey: 'muser',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) => a.muser.localeCompare(b.muser),
  },
  {
    title: 'update-date',
    columnKey: 'mdate',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: InvTypeInfo, b: InvTypeInfo) => a.mdate.localeCompare(b.mdate),
  },
];
