import { TableColumnModel } from 'src/app/shared/models';
import { VenderInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'unifyCode',
    columnKey: 'UnifyCode',
    columnWidth: '',
    align: 'center',
    sortFn: (a: VenderInfo, b: VenderInfo) => a.UnifyCode.localeCompare(b.UnifyCode),
  },
  {
    title: 'venderCode',
    columnKey: 'VenderCode',
    columnWidth: '',
    align: 'center',
    sortFn: (a: VenderInfo, b: VenderInfo) => a.VenderCode.localeCompare(b.VenderCode),
  },
  {
    title: 'venderName',
    columnKey: 'VenderName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: VenderInfo, b: VenderInfo) => a.VenderName.localeCompare(b.VenderName),
  },
  // {
  //   title: 'whether-carry-nickname',
  //   columnKey: 'iscarry',
  //   columnWidth: '',
  //   align: 'center',
  //   sortFn: (a: SupplierInfo, b: SupplierInfo) => a.iscarry.localeCompare(b.iscarry),
  // },
  // {
  //   title: 'create-user',
  //   columnKey: 'cuser',
  //   columnWidth: '100px',
  //   align: 'center',
  //   sortFn: (a: SupplierInfo, b: SupplierInfo) => a.cuser.localeCompare(b.cuser),
  // },
  // {
  //   title: 'create-date',
  //   columnKey: 'cdate',
  //   columnWidth: '100px',
  //   align: 'center',
  //   sortFn: (a: SupplierInfo, b: SupplierInfo) => a.cdate.localeCompare(b.cdate),
  // },
  // {
  //   title: 'update-user',
  //   columnKey: 'muser',
  //   columnWidth: '100px',
  //   align: 'center',
  //   sortFn: (a: SupplierInfo, b: SupplierInfo) => a.muser.localeCompare(b.muser),
  // },
  // {
  //   title: 'update-date',
  //   columnKey: 'mdate',
  //   columnWidth: '100px',
  //   align: 'center',
  //   sortFn: (a: SupplierInfo, b: SupplierInfo) => a.mdate.localeCompare(b.mdate),
  // },
];