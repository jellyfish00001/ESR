import { TableColumnModel } from 'src/app/shared/models';
import { FormDetail } from './data-item';

export const DetailTableColumn: TableColumnModel[] = [
    {
      title: 'company-code',
      columnKey: 'companyCode',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) =>
        a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: 'application-number',
      columnKey: 'rno',
      columnWidth: '140px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.rno.localeCompare(b.rno),
    },
    {
      title: 'form-type-name',
      columnKey: 'formTypeName',
      columnWidth: '130px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.formTypeName.localeCompare(b.formTypeName),
    },
    {
      title: 'applied-dept',
      columnKey: 'appliedDept',
      columnWidth: '',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.appliedDept.localeCompare(b.appliedDept),
    },
    {
      title: 'applicant',
      columnKey: 'applicant',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicant.localeCompare(b.applicant),
    },
    {
      title: 'applicant-name',
      columnKey: 'applicantName',
      columnWidth: '',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicantName.localeCompare(b.applicantName),
    },
    {
      title: 'applied-date',
      columnKey: 'appliedDate',
      columnWidth: '105px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.appliedDate.localeCompare(b.appliedDate),
    },
    {
      title: 'expense-type',
      columnKey: 'expenseType',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.expenseType.localeCompare(b.expenseType),
    },
    {
      title:'col.expense-attribution-department',
      columnKey: 'dept',
      columnWidth: '125px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.dept.localeCompare(b.dept),
    },
    {
      title: 'col.currency',
      columnKey: 'curr',
      columnWidth: '70px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.curr.localeCompare(b.curr),
    },
    {
      title: 'actual-amount',
      columnKey: 'actualAmount',
      columnWidth: '',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.actualAmount.localeCompare(b.actualAmount),
    },
    {
      title: 'step',
      columnKey: 'step',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.step.localeCompare(b.step),
    },
    {
      title: 'payment-date',
      columnKey: 'paymentDate',
      columnWidth: '105px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.paymentDate.localeCompare(b.paymentDate),
    }
];
