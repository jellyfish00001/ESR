import { MenuModel } from '../models';
export const navigationImage: any[] = [
  {
    title: 'menu.personal-center', //个人中心
    url: 'dailycallspace',
    icon: 'assets/images/menu/personal-center.svg',
    open: false,
    selected: false,
    authID: 'personal-center',
    children: [
      {
        title: 'menu.upload-invoice', //上传发票
        url: 'ers/invoiceupload',
        open: false,
        selected: false,
        authID: 'invoice-upload', //uploadinvoice
      },
      {
        title: 'menu.query-invoice', //查询发票
        url: 'ers/invoicequery',
        open: false,
        selected: false,
        authID: 'invoice-query', //queryinvoice
      },
      {
        title: 'menu.my-invoices', //我的发票
        url: 'ers/my-invoices',
        open: false,
        selected: false,
        authID: 'my-invoices', //invoice
      },
      {
        title: 'menu.proxy',  //代理人
        url: 'ers/proxy',
        // icon: 'assets/images/menu/UserSwitchOutlined.svg',
        open: false,
        selected: false,
        authID: 'proxy',
      },
      {
        title: 'menu.help', //Help
        url: 'ers/help',
        // icon: 'assets/images/menu/QuestionCircleOutlined.svg',
        open: false,
        selected: false,
        authID: 'help', //Help
        openPermission: false,
      },
    ]
  },
  {
    title: 'ers',
    url: 'dailycallspace',
    icon: 'assets/images/menu/FileAddOutlined.svg',
    open: false,
    selected: false,
    authID: 'application', //单据申请
    children: [
      {
        title: 'menu.rq101',  //一般费用报销申请
        url: 'ers/rq101',
        open: false,
        selected: false,
        authID: 'application-general-expenses', //rq101
      },
      {
        title: 'menu.rq201', //交际费报销申请
        url: 'ers/rq201',
        open: false,
        selected: false,
        authID: 'application-entertainment-expense', //rq201
      },
      {
        title: 'menu.rq201a', //交际费报销（餐饮宴客）申请
        url: 'ers/rq201a',
        open: false,
        selected: false,
        authID: 'application-cater-hosting-entertainment-expense', //rq201a
      },
      {
        title: 'menu.rq601', //返台会议申请
        url: 'ers/rq601',
        open: false,
        selected: false,
        authID: 'application-return-taiwan-conference', //rq601
      },
      {
        title: 'menu.rq401', //预支金申请
        url: 'ers/rq401',
        open: false,
        selected: false,
        authID: 'application-cash-advance', //rq401
      },
      {
        title: 'menu.rq401a', //预支金延期冲账申请
        url: 'ers/rq401a',
        open: false,
        selected: false,
        authID: 'application-deferred-debit-of-advance', //rq401a
      },
      {
        title: 'menu.rq501', //批量报销申请
        url: 'ers/rq501',
        open: false,
        selected: false,
        authID: 'application-batch-expenses', //RQ501
        openPermission: true,
      },
      {
        title: 'menu.rq701', //薪资请款申请
        url: 'ers/rq701',
        open: false,
        selected: false,
        authID: 'application-salary', //RQ701
        openPermission: true,
      },
      {
        title: 'menu.rq801', //廠商與總務報銷申請
        url: 'ers/rq801',
        open: false,
        selected: false,
        authID: 'vendor-and-general-affairs', //RQ801
        openPermission: true,
      },

    ]
  },
  {
    title: 'menu.report', //单据查询
    url: 'dailycallspace',
    icon: 'assets/images/menu/FileSearchOutlined.svg',
    open: false,
    selected: false,
    authID: 'repot', //''
    children: [
      {
        title: 'menu.report-applied-form-query', //已申请表单查询
        url: 'ers/report-applied-form-query',
        open: false,
        selected: false,
        authID: 'report-applied-form-query', //form104
      },
      {
        title: 'menu.report-applied-form-query-detail', //已申请表单明细查询
        url: 'ers/report-applied-form-query-detail',
        open: false,
        selected: false,
        authID: 'report-applied-form-query-detail', //form104-detail
      },
      {
        title: 'menu.report-signed-form-query', //已签核表单查询
        url: 'ers/report-signed-form-query',
        open: false,
        selected: false,
        authID: 'report-signed-form-query', //form104
      },
{
        title: 'menu.report-signed-form-query-detail', //已申请表单明细查询
        url: 'ers/report-signed-form-query-detail',
        open: false,
        selected: false,
        authID: 'report-signed-form-query-detail', //form104-detail
      },
      {
        title: 'menu.form105', // 纸本单据查询
        url: 'ers/form105',
        open: false,
        selected: false,
        authID: 'report-paper-form-query', //form105
      },
      {
        title: 'menu.form106', //绩效奖金预算报表
        url: 'ers/form106',
        open: false,
        selected: false,
        authID: 'report-performance-bonus-budget-report', //Report
        openPermission: true,
      },
      {
        title: 'menu.form103', //已签核表单查询
        url: 'ers/form103',
        open: false,
        selected: false,
        authID: 'report-signed-form-query', //form103
        openPermission: false,
      },
      {
        title: 'menu.report-uber-transactional-query', //UBER車資賬單查詢
        url: 'ers/report-uber-transactional-query',
        open: false,
        selected: false,
        authID: 'report-uber-transactional-query',
        openPermission: false,
      },
    ]
  },
  {
    title: 'menu.approval', //待签表单
    url: 'dailycallspace',
    icon: 'assets/images/menu/SolutionOutlined.svg',
    open: false,
    selected: false,
    authID: 'applove', //''
    children: [
      {
        title: 'menu.form101', //待签核单据
        url: 'ers/form101',
        open: false,
        selected: false,
        authID: 'approval-pending-form', //form101
      },
      {
        title: 'menu.form201', //待签收纸本单据
        url: 'ers/form201',
        open: false,
        selected: false,
        authID: 'approval-pending-paper-form', //form201
        openPermission: true,
      },
    ]
  },
  {
    title: 'menu.basedata', //基本资料
    url: 'dailycallspace',
    icon: 'assets/images/menu/DatabaseOutlined.svg',
    open: false,
    selected: false,
    authID: 'base-data', //''
    children: [
      {
        title: 'menu.bd008', //公司别维护
        url: 'ers/bd008',
        open: false,
        selected: false,
        authID: 'bd-company-code', //Company
        openPermission: true,
      },
      {
        title: 'menu.bd-expense-senario', //报销情景维护
        url: 'ers/bd-expense-senario',
        open: false,
        selected: false,
        authID: 'bd-expense-senario', //BDExp
        openPermission: true,
      },
      {
        title: 'menu.bdexpensedept', //归属部门维护
        url: 'ers/expensedept',
        open: false,
        selected: false,
        authID: 'bd-cost-assignment-department', //BDAccount
        openPermission: true,
      },
      {
        title: 'menu.bd-account', //会计科目维护
        url: 'ers/bd-account',
        open: false,
        selected: false,
        authID: 'bd-account', //BDAccount
        openPermission: true,
      },
      {
        title: 'menu.bd-invoice-type', //发票类型维护
        url: 'ers/bd-invoice-type',
        open: false,
        selected: false,
        authID: 'bd-invoice-type',  //BDInvoiceType
        openPermission: true,
      },
      {
        title: 'menu.bd-invoice-rail', //发票字轨维护
        url: 'ers/bd-invoice-rail',
        open: false,
        selected: false,
        authID: 'bd-invoice-rail',  //BdInvoiceRail
      },
      {
        title: 'menu.bd-ticket-rail', //车票字轨维护
        url: 'ers/bd-ticket-rail',
        open: false,
        selected: false,
        authID: 'bd-ticket-rail', //BdTicketRail
      },
      {
        title: 'menu.bd-customer-nickname', //客户昵称维护
        url: 'ers/bd-customer-nickname',
        open: false,
        selected: false,
        authID: 'bd-customer-nickname',  //CustomerNickname
        openPermission: true,
      },
      {
        title: 'menu.companyCategory',
        url: 'ers/bd008',
        open: false,
        selected: false,
        authID: 'CompanyCategoryInfo',
      },
      {
        title: 'menu.supplier', //供应商信息
        url: 'ers/supplier',
        open: false,
        selected: false,
        authID: 'bd-supplier-information', //SupplierInfo
      },

      {
        title: 'menu.bd009', //数据字典
        url: 'ers/bd009',
        open: false,
        selected: false,
        authID: 'bd-data-dictionary', //DataDictionary
      }
    ],
  },
  {
    title: 'menu.approval-setting', //签核设定
    url: 'dailycallspace',
    icon: 'assets/images/menu/AuditOutlined.svg',
    open: false,
    selected: false,
    authID: 'approval-setting', //approval-setting
    children: [
      {
        title: 'menu.bd002', //财务签核人员维护
        url: 'ers/bd002',
        open: false,
        selected: false,
        authID: 'bd-financial-signatory-personnel', //FinReview
        openPermission: true,
      },
      {
        title: 'menu.bd004', //签核主管Auditor维护
        url: 'ers/bd004',
        open: false,
        selected: false,
        authID: 'bd-audit-supervisor', //Auditor
        openPermission: true,
      },
      {
        title: 'menu.bd003', //纸本单签核人维护
        url: 'ers/bd003',
        open: false,
        selected: false,
        authID: 'bd-paper-form-signatory', //BDPaperSign
        openPermission: true,
      },
      {
        title: 'menu.bdsignlevel', //核决权限维护
        url: 'ers/signlevel',
        open: false,
        selected: false,
        authID: 'bd-approval-authority', //BDAccount
        openPermission: true,
      },
    ]
  },
  {
    title: 'menu.finance-operations', //会计作业
    url: 'dailycallspace',
    icon: 'assets/images/menu/会计作业.svg',
    open: false,
    selected: false,
    authID: 'approval-setting', //approval-setting
    children: [
      {
        title: 'menu.finance-advance-payment-clearance', //预支金冲账（缴回）维护
        url: 'ers/finance-advance-payment-clearance',
        open: false,
        selected: false,
        authID: 'finance-advance-payment-clearance', //CashReturn
        openPermission: true,
      },
      {
        title: 'menu.py001', //入账清单维护
        url: 'ers/py001',
        open: false,
        selected: false,
        authID: 'bd-posting-list',//Account
        openPermission: true,
      },
      {
        title: 'menu.finance-payment-list-maintenance', //付款清单维护
        url: 'ers/finance-payment-list-maintenance',
        open: false,
        selected: false,
        authID: 'finance-payment-list-maintenance', //Payment
        openPermission: true,
      },
    ]
  },
  {
    title: 'menu.print', //报销单列印
    url: 'ers/print',
    icon: 'assets/images/menu/PrinterOutlined.svg',
    open: false,
    selected: false,
    authID: 'reimbursement-form-printing', //print
  },

  {
    title: 'menu.permission', //权限管理
    url: 'ers/permission',
    icon: 'assets/images/menu/lock.svg',
    open: false,
    selected: false,
    authID: 'access-management',  //Permission
    openPermission: true,
  },
  {
    title: 'menu.sign-uber-fare', //Uber車資簽核
    url: 'ers/sign-uber-fare',
    icon: 'assets/images/menu/QuestionCircleOutlined.svg',
    open: false,
    selected: false,
    authID: 'sign-uber-fare', //Uber車資簽核
    openPermission: false,
  }
];

