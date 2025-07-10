import { TableColumnModel } from 'src/app/shared/models';
import { AuditorInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'company-code',
    columnKey: 'company',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.company.localeCompare(b.company),
  },
  {
    title: 'form-type',
    columnKey: 'formName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.formName.localeCompare(b.formName),
  },
  {
    title: 'employee-id',
    columnKey: 'emplid',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.emplid.localeCompare(b.emplid),
  },
  {
    title: 'name',
    columnKey: 'name',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.name.localeCompare(b.name),
  },
  {
    title: 'dept-code',
    columnKey: 'deptid',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.deptid.localeCompare(b.deptid),
  },
  {
    title: 'audit-emplid',
    columnKey: 'auditEmplid',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.auditEmplid.localeCompare(b.auditEmplid),
  },
  {
    title: 'audit-name',
    columnKey: 'auditName',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.auditName.localeCompare(b.auditName),
  },
  {
    title: 'start-date',
    columnKey: 'startDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.startDate.localeCompare(b.startDate),
  },
  {
    title: 'end-date',
    columnKey: 'endDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.endDate.localeCompare(b.endDate),
  },
  {
    title: 'update-user',
    columnKey: 'updateUser',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.updateUser.localeCompare(b.updateUser),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AuditorInfo, b: AuditorInfo) => a.updateDate.localeCompare(b.updateDate),
  },

];