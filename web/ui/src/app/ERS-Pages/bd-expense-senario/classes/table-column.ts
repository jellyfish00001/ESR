import { TableColumnModel } from 'src/app/shared/models';
import { DetailInfo } from './data-item';
import _ from 'lodash';

export const InfoTableColumn: TableColumnModel[] = [
    {
      title: ('companyCategory'),
      columnKey: 'companycategory',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.companycategory.localeCompare(b.companycategory),
    },
    {
      title: ('col-category'),
      columnKey: 'category',
      columnWidth: '70px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.category.localeCompare(b.category),
    },
    {
      title: ('expense-category'),
      columnKey: 'expcode',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.expname.localeCompare(b.expname),
    },
    {
      title: ('reimbursement-scene'),
      columnKey: 'senarioname',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.senarioname.localeCompare(b.senarioname),
    },
    {
      title: ('col-keyword'),
      columnKey: 'keyword',
      columnWidth: '120px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.keyword.localeCompare(b.keyword),
    },
    {
      title: ('accounting-subject'),
      columnKey: 'acctcode',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.acctcode.localeCompare(b.acctcode),
    },
    {
      title: ('auditlevelcode'),
      columnKey: 'auditlevelcode',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.auditlevelcode.localeCompare(b.auditlevelcode),
    },
    {
      title: ('exp-descriptionnotice'),
      columnKey: 'descriptionnotice',
      columnWidth: '170px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.descriptionnotice.localeCompare(b.descriptionnotice),
    },
    {
      title: ('exp-attachmentnotice'),
      columnKey: 'attachmentnotice',
      columnWidth: '170px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.attachmentnotice.localeCompare(b.attachmentnotice),
    },
    {
      title: ('requirespaperattachment'),
      columnKey: 'requirespaperattachment',
      columnWidth: '170px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        !_.isEmpty(a.requirespaperattachment) && !_.isEmpty(b.requirespaperattachment) && a.requirespaperattachment.localeCompare(b.requirespaperattachment),
    },
    {
      title: ('exp-isinvoice'),
      columnKey: 'requiresinvoice',
      columnWidth: '150px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        !_.isEmpty(a.requiresinvoice) && !_.isEmpty(b.requiresinvoice) && a.requiresinvoice.localeCompare(b.requiresinvoice),
    },
    {
      title: ('exp-canbypassfinanceapproval'),
      columnKey: 'canbypassfinanceapproval',
      columnWidth: '150px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        !_.isEmpty(a.canbypassfinanceapproval) && !_.isEmpty(b.canbypassfinanceapproval) && a.canbypassfinanceapproval.localeCompare(b.canbypassfinanceapproval),
    },
    {
      title: ('exp-requiresattachment'),
      columnKey: 'requiresattachment',
      columnWidth: '150px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        !_.isEmpty(a.requiresattachment) && !_.isEmpty(b.requiresattachment) && a.requiresattachment.localeCompare(b.requiresattachment),
    },
    {
      title: ('exp-isdeduction'),
      columnKey: 'isvatdeductable',
      columnWidth: '150px',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        !_.isEmpty(a.isvatdeductable) && !_.isEmpty(b.isvatdeductable) && a.isvatdeductable.localeCompare(b.isvatdeductable),
    },
    // {
    //   title: ('cuser'),
    //   columnKey: 'cuser',
    //   columnWidth: '110px',
    //   align: 'center',
    //   sortFn: (a: DetailInfo, b: DetailInfo) =>
    //     a.cuser.localeCompare(b.cuser),
    // },
    // {
    //   title: ('cdate'),
    //   columnKey: 'cdate',
    //   columnWidth: '100px',
    //   align: 'center',
    //   sortFn: (a: DetailInfo, b: DetailInfo) =>
    //     a.cdate.localeCompare(b.cdate),
    // },
    // {
    //   title: ('muser'),
    //   columnKey: 'muser',
    //   columnWidth: '100px',
    //   align: 'center',
    //   sortFn: (a: DetailInfo, b: DetailInfo) =>
    //   !_.isEmpty(a.muser) && !_.isEmpty(b.muser) && a.muser.localeCompare(b.muser),
    // },
    // {
    //   title: ('mdate'),
    //   columnKey: 'mdate',
    //   columnWidth: '100px',
    //   align: 'center',
    //   sortFn: (a: DetailInfo, b: DetailInfo) =>
    //   !_.isEmpty(a.mdate) && !_.isEmpty(b.mdate) && a.mdate.localeCompare(b.mdate),
    // }
  ];
