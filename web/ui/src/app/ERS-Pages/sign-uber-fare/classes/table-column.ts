import { TableColumnModel } from 'src/app/shared/models';
import { CashUberDetail } from './data-item';

export const DetailTableColumn: TableColumnModel[] = [
  {
    title: 'travel-date',  // 对应 "Date of Travel"
    columnKey: 'startDate',
    columnWidth: '50px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.startDate.localeCompare(b.startDate),
  },
  {
    title: 'travel-reason',  // 对应 "Reason for Travel"
    columnKey: 'reason',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.reason.localeCompare(b.reason),
  },
  {
    title: 'departure',  // 对应 "Departure"
    columnKey: 'origin',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.origin.localeCompare(b.origin),
  },
  {
    title: 'destination',  // 对应 "Destination"
    columnKey: 'destination',
    columnWidth: '150px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.destination.localeCompare(b.destination),
  },
  {
    title: 'amount',  // 对应 "Amount"
    columnKey: 'amount',
    columnWidth: '50px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.amount - b.amount,
  },
  {
    title: 'expense-accounting-department',  // 对应 "Expense Billing Department"
    columnKey: 'deptId',
    columnWidth: '50px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.  deptId.localeCompare(b.deptId),
  },
  {
    title: 'traveler-id',  // 对应 "Employee ID of Traveler"
    columnKey: 'emplid',
    columnWidth: '50px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.emplid.localeCompare(b.emplid),
  },
  {
    title: 'traveler-name',  // 对应 "Traveler's Name"
    columnKey: 'name',
    columnWidth: '50px',
    align: 'center',
    sortFn: (a: CashUberDetail, b: CashUberDetail) => a.name.localeCompare(b.name),
  },
];


