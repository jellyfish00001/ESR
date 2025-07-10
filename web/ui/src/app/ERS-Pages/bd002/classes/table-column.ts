import { TableColumnModel } from 'src/app/shared/models';
import { AccountantInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'col.category',
    columnKey: 'category',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.category.localeCompare(b.category),
  },
  {
    title: 'step',
    columnKey: 'signStep',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.signStep.localeCompare(b.signStep),
  },
  {
    title: 'company-code',
    columnKey: 'company',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.company.localeCompare(b.company),
  },
  {
    title: 'plant',
    columnKey: 'plant',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.plant.localeCompare(b.plant),
  },
  {
    title: 'approver',
    columnKey: 'approver',
    columnWidth: '250px',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.approver.localeCompare(b.approver),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '',
    align: 'center',
    sortFn: (a: AccountantInfo, b: AccountantInfo) => a.updateDate.localeCompare(b.updateDate),
  },

];
