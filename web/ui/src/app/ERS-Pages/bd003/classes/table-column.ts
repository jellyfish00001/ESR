import { TableColumnModel } from 'src/app/shared/models';
import { paperSignerInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'col-company',
    columnKey: 'company',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.company.localeCompare(b.company),
  },
  {
    title: 'company-code',
    columnKey: 'companyCode',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.companyCode.localeCompare(b.companyCode),
  },
  {
    title: 'plant',
    columnKey: 'plant',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.plant.localeCompare(b.plant),
  },
  {
    title: 'employee-id',
    columnKey: 'emplid',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.emplid.localeCompare(b.emplid),
  },
  {
    title: 'name',
    columnKey: 'name',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.name.localeCompare(b.name),
  },
  {
    title: 'create-user',
    columnKey: 'creator',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.creator.localeCompare(b.creator),
  },
  {
    title: 'create-date',
    columnKey: 'createDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.createDate.localeCompare(b.createDate),
  },
  {
    title: 'update-user',
    columnKey: 'updateUser',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.updateUser.localeCompare(b.updateUser),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: paperSignerInfo, b: paperSignerInfo) => a.updateDate.localeCompare(b.updateDate),
  }
];