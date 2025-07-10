import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { AuthGuard } from '../core/guard/auth.guard';
import { RQ101Component } from './rq101/rq101.component';
import { RQ201Component } from './rq201/rq201.component';
import { RQ201AComponent } from './rq201a/rq201a.component';
import { FORM103Component } from './form103/form103.component';
import { ReportAppliedFormQueryComponent } from './report-applied-form-query/report-applied-form-query.component';
import { ReportAppliedFormQueryDetailComponent } from './report-applied-form-query-detail/report-applied-form-query-detail.component';
import { ReportSignedFormQueryComponent } from './report-signed-form-query/report-signed-form-query.component';
import { ReportSignedFormQueryDetailComponent } from './report-signed-form-query-detail/report-signed-form-query-detail.component';
import { FORM105Component } from './form105/form105.component';
import { FORM201Component } from './form201/form201.component';
import { RQ204Component } from './rq204/rq204.component';
import { RQ204AComponent } from './rq204a/rq204a.component';
import { RQ401Component } from './rq401/rq401.component';
import { RQ401AComponent } from './rq401a/rq401a.component';
import { RQ404Component } from './rq404/rq404.component';
import { FinanceAdvancePaymentClearanceComponent } from './finance-advance-payment-clearance/finance-advance-payment-clearance.component';
import { Form101Component } from './form101/form101.component';
import { Py001Component } from './py001/py001.component';
import { RQ404AComponent } from './rq404a/rq404a.component';
import { RQ104Component } from './rq104/rq104.component';
import { RQ501Component } from './rq501/rq501.component';
import { RQ504Component } from './rq504/rq504.component';
import { ProxyComponent } from './proxy/proxy.component';
import { BdExpenseSenarioComponent } from './bd-expense-senario/bd-expense-senario.component';
import { PrintComponent } from './print/print.component';
import { BdsignlevelComponent } from './bdsignlevel/bdsignlevel.component';
import { BdexpensedeptComponent } from './bdexpensedept/bdexpensedept.component';
import { BdvirtualdepartmentComponent } from './bdvirtualdepartment/bdvirtualdepartment.component';
import { FinancePaymentListMaintenanceComponent } from './finance-payment-list-maintenance/finance-payment-list-maintenance.component';
import { BdAccountComponent } from './bd-account/bd-account.component';
import { BD008Component } from './bd008/bd008.component';
import { BD002Component } from './bd002/bd002.component';
import { BD003Component } from './bd003/bd003.component';
import { BD004Component } from './bd004/bd004.component';
import { BdInvoiceTypeComponent } from './bd-invoice-type/bd-invoice-type.component';
import { BDCustomerNicknameComponent } from './bd-customer-nickname/bd-customer-nickname.component';
import { Form106Component } from './form106/form106.component';
import { RQ601Component } from './rq601/rq601.component';
import { RQ604Component } from './rq604/rq604.component';
import { PermissionComponent } from './permission/permission.component';
import { PermissionDeniedComponent } from './permissiondenied.component';
import { InvoiceUploadComponent } from './invoiceUpload/invoiceUpload.component';
import { InvoiceQueryComponent } from './invoiceQuery/invoiceQuery.component';
import { MyInvoicesComponent } from './my-invoices/my-invoices.component';
import { HelpComponent } from './help.component';
import { RQ701Component } from './rq701/rq701.component';
import { RQ801Component } from './rq801/rq801.component';
import { RQ704Component } from './rq704/rq704.component';
import { BdInvoiceRailComponent } from './bd-invoice-rail/bd-invoice-rail.component';
import { BdTicketRailComponent } from './bd-ticket-rail/bd-ticket-rail.component';
import { SupplierInfoComponent } from './supplier-info/supplier-info.component';
import { LoginGuard } from '../core/guard/login.guard';
import { BdCompanyCategoryComponent } from './bdCompanyCategory/bdCompanyCategory.component';
import { BD009Component } from './bd009/bd009.component';
import { ReportUberTransactionalQueryComponent } from './report-uber-transactional-query/report-uber-transactional-query.component';
import { SignUberFareComponent } from './sign-uber-fare/sign-uber-fare.component';
import { report } from 'process';

const routes: Routes = [
  {
    path: 'form101', component: Form101Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq101', component: RQ101Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq104', component: RQ104Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq201', component: RQ201Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq201a', component: RQ201AComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq401', component: RQ401Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq401a', component: RQ401AComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq404a', component: RQ404AComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'form103', component: FORM103Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'report-uber-transactional-query', component: ReportUberTransactionalQueryComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'report-applied-form-query', component: ReportAppliedFormQueryComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'report-applied-form-query-detail', component: ReportAppliedFormQueryDetailComponent,
    canActivate: [LoginGuard]
  },
   {
    path: 'report-signed-form-query', component: ReportSignedFormQueryComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'report-signed-form-query-detail', component: ReportSignedFormQueryDetailComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'form105', component: FORM105Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'form106', component: Form106Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'form201', component: FORM201Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq204', component: RQ204Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq204a', component: RQ204AComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq404', component: RQ404Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq501', component: RQ501Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq504', component: RQ504Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'py001', component: Py001Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'finance-payment-list-maintenance', component: FinancePaymentListMaintenanceComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'finance-advance-payment-clearance', component: FinanceAdvancePaymentClearanceComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'proxy', component: ProxyComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'bd-expense-senario', component: BdExpenseSenarioComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'print', component: PrintComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'signlevel',
    component: BdsignlevelComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'expensedept', component: BdexpensedeptComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'virtualdepartment', component: BdvirtualdepartmentComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'bd-account', component: BdAccountComponent,
    canActivate: [LoginGuard]
  },
  // {
  //   path: 'bd008', component: BD008Component,
  //   canActivate: [LoginGuard]
  // },
  {
    path: 'bd002', component: BD002Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'bd003', component: BD003Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'bd004', component: BD004Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'bd-invoice-type',
    component: BdInvoiceTypeComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'bd-customer-nickname',
    component: BDCustomerNicknameComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'rq601', component: RQ601Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq604', component: RQ604Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq701', component: RQ701Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq801', component: RQ801Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'rq704', component: RQ704Component,
    canActivate: [LoginGuard]
  },
  {
    path: 'permission',
    component: PermissionComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'permissiondenied',
    component: PermissionDeniedComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'invoiceupload',
    component: InvoiceUploadComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'invoicequery',
    component: InvoiceQueryComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'my-invoices',
    component: MyInvoicesComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'bd-invoice-rail',
    component: BdInvoiceRailComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'bd-ticket-rail',
    component: BdTicketRailComponent,
    canActivate: [LoginGuard],
  },
  {
    path: 'help', component: HelpComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'supplier', component: SupplierInfoComponent,
    canActivate: [LoginGuard]
  },
  { path: 'bd008', component: BdCompanyCategoryComponent, canActivate: [LoginGuard] },
  { path: 'bd009', component: BD009Component, canActivate: [LoginGuard] },
  { path: 'sign-uber-fare', component: SignUberFareComponent, canActivate: [LoginGuard] },
  {
    path: '',
    redirectTo: 'form101',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ERSPagesRoutingModule { }
