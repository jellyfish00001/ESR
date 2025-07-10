import { TableColumnModel } from 'src/app/shared/models';
import { NicknameInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'company-code',
    columnKey: 'company',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.company.localeCompare(b.company),
  },
  {
    title: 'col-customer-nickname',
    columnKey: 'nickname',
    columnWidth: '',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.nickname.localeCompare(b.nickname),
  },
  {
    title: 'col-customer-name',
    columnKey: 'name',
    columnWidth: '',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.name.localeCompare(b.name),
  },
  {
    title: 'whether-carry-nickname',
    columnKey: 'iscarry',
    columnWidth: '',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.iscarry.localeCompare(b.iscarry),
  },
  {
    title: 'create-user',
    columnKey: 'cuser',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.cuser.localeCompare(b.cuser),
  },
  {
    title: 'create-date',
    columnKey: 'cdate',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.cdate.localeCompare(b.cdate),
  },
  {
    title: 'update-user',
    columnKey: 'muser',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.muser.localeCompare(b.muser),
  },
  {
    title: 'update-date',
    columnKey: 'mdate',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: NicknameInfo, b: NicknameInfo) => a.mdate.localeCompare(b.mdate),
  },
];