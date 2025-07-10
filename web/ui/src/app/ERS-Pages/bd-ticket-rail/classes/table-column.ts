import { TableColumnModel } from 'src/app/shared/models';
import { BdTicketRail } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'company-code',
    columnKey: 'company',
    columnWidth: '75px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.company.localeCompare(b.company),
  },
  {
    title: 'ticketrail',
    columnKey: 'ticketrail',
    columnWidth: '60px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.ticketrail.localeCompare(b.ticketrail),
  },
  {
    title: 'voucheryear',
    columnKey: 'voucheryear',
    columnWidth: '60px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.ticketrail.localeCompare(b.voucheryear),
  },
  {
    title: 'vouchermonth',
    columnKey: 'vouchermonth',
    columnWidth: '60px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.ticketrail.localeCompare(b.vouchermonth),
  },
  {
    title: 'currentNumber',
    columnKey: 'currentnumber',
    columnWidth: '60px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.ticketrail.localeCompare(b.currentnumber),
  },
  {
    title: 'create-user',
    columnKey: 'creator',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.creator.localeCompare(b.creator),
  },
  {
    title: 'create-date',
    columnKey: 'createDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.createDate.localeCompare(b.createDate),
  },
  {
    title: 'update-user',
    columnKey: 'updateUser',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.updateUser.localeCompare(b.updateUser),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: BdTicketRail, b: BdTicketRail) =>
      a.updateDate.localeCompare(b.updateDate),
  },
];
