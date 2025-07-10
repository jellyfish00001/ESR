import { TableColumnModel } from 'src/app/shared/models';
import { AuthorityInfo } from './data-item';

export const PermissionTableColumn: TableColumnModel[] = [
  {
    title: 'employee-id',
    columnKey: 'emplid',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuthorityInfo, b: AuthorityInfo) => a.emplid.localeCompare(b.emplid),
  },
  {
    title: 'name',
    columnKey: 'name',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuthorityInfo, b: AuthorityInfo) => a.name.localeCompare(b.name),
  },
  {
    title: 'role',
    columnKey: 'roleName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuthorityInfo, b: AuthorityInfo) => a.roleName.localeCompare(b.roleName),
  },
];