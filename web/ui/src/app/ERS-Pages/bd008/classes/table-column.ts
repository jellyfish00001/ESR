import { TableColumnModel } from 'src/app/shared/models';
import { CompanyInfo } from './data-item';

export const BDInfoTableColumn: TableColumnModel[] = [
  {
    title: 'company-code',
    columnKey: 'companyCode',
    columnWidth: '75px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.companyCode.localeCompare(b.companyCode),
  },
  {
    title: 'col-company',
    columnKey: 'company',
    columnWidth: '70px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.company.localeCompare(b.company),
  },
  {
    title: 'sap-company-code',
    columnKey: 'sapCompanyCode',
    columnWidth: '105px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.sapCompanyCode.localeCompare(b.sapCompanyCode),
  },
  {
    title: 'company-description',
    columnKey: 'companyDesc',
    columnWidth: '200px',
    align: 'left',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.companyDesc.localeCompare(b.companyDesc),
  },
  {
    title: 'col-abbr',
    columnKey: 'abbr',
    columnWidth: '70px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.abbr.localeCompare(b.abbr),
  },
  {
    title: 'to-local-currency',
    columnKey: 'curr',
    columnWidth: '75px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.curr.localeCompare(b.curr),
  },
  {
    title: 'taxpayer-no',
    columnKey: 'taxpayerNo',
    columnWidth: '175px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.taxpayerNo.localeCompare(b.taxpayerNo),
  },
  {
    title: 'tax-rate',
    columnKey: 'taxRate',
    columnWidth: '110px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.taxRate - b.taxRate,
  },
  {
    title: 'column-location',
    columnKey: 'area',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.area.localeCompare(b.area),
  },
  {
    title: 'timezone',
    columnKey: 'timezoneName',
    columnWidth: '80px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.timezone - b.timezone,
  },
  {
    title: 'create-user',
    columnKey: 'creator',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.creator.localeCompare(b.creator),
  },
  {
    title: 'create-date',
    columnKey: 'createDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.createDate.localeCompare(b.createDate),
  },
  {
    title: 'update-user',
    columnKey: 'updateUser',
    columnWidth: '100px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.updateUser.localeCompare(b.updateUser),
  },
  {
    title: 'update-date',
    columnKey: 'updateDate',
    columnWidth: '90px',
    align: 'center',
    sortFn: (a: CompanyInfo, b: CompanyInfo) => a.updateDate.localeCompare(b.updateDate),
  },

];