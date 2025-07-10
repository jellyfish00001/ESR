import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ERSPagesRoutingModule } from './ERSPages-routing.module';
import { RQ101Component } from './rq101/rq101.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';   //多语言组件引用
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { RQ201Component } from './rq201/rq201.component';
import { ShareModule } from 'src/app/shared/share-module.module';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { PdfJsViewerModule } from 'ng2-pdfjs-viewer';
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
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { NoteTableTemplateComponent } from './rq201a/note-table/note-table.component';
import { FileListModalComponent } from './_components/file-list-modal/file-list-modal.component';
import { SignOffTemplateComponent } from './_components/sign-off/sign-off.component';
import { InvoicesModalComponent } from './_components/invoices-modal/invoices-modal.component';
import { RQ201AFormComponent } from './rq201a/rq201a-form/rq201a-form.component';
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
import { BdsignlevelComponent } from './bdsignlevel/bdsignlevel.component';
import { PrintComponent } from './print/print.component';
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
import { InvoiceFormFieldsComponent } from './my-invoices/invoice-form-fields/invoice-form-fields.component';
import { HelpComponent } from './help.component';
import { RQ701Component } from './rq701/rq701.component';
import { RQ704Component } from './rq704/rq704.component';
import { BdInvoiceRailComponent } from './bd-invoice-rail/bd-invoice-rail.component';
import { BdTicketRailComponent } from './bd-ticket-rail/bd-ticket-rail.component';
import { SupplierInfoComponent } from './supplier-info/supplier-info.component';
import { BdexpensedeptComponent } from './bdexpensedept/bdexpensedept.component';
import { BdvirtualdepartmentComponent } from './bdvirtualdepartment/bdvirtualdepartment.component';
import { BdCompanyCategoryComponent } from './bdCompanyCategory/bdCompanyCategory.component';
import { BD009Component } from './bd009/bd009.component';
import { ReportUberTransactionalQueryComponent } from './report-uber-transactional-query/report-uber-transactional-query.component';
import { SignUberFareComponent } from './sign-uber-fare/sign-uber-fare.component';
import { RQ801Component } from './rq801/rq801.component';

@NgModule({
  declarations: [
    RQ101Component,
    RQ104Component,
    RQ201Component,
    RQ201AComponent,
    FORM103Component,
    ReportAppliedFormQueryComponent,
    ReportAppliedFormQueryDetailComponent,
    ReportSignedFormQueryComponent,
    ReportSignedFormQueryDetailComponent,
    FORM105Component,
    FORM201Component,
    RQ204Component,
    RQ204AComponent,
    NoteTableTemplateComponent,
    FileListModalComponent,
    SignOffTemplateComponent,
    InvoicesModalComponent,
    RQ201AFormComponent,
    RQ401Component,
    RQ401AComponent,
    RQ404Component,
    RQ501Component,
    RQ504Component,
    FinanceAdvancePaymentClearanceComponent,
    Form101Component,
    Py001Component,
    RQ404AComponent,
    ProxyComponent,
    BdExpenseSenarioComponent,
    BdsignlevelComponent,
    PrintComponent,
    FinancePaymentListMaintenanceComponent,
    BdAccountComponent,
    BD008Component,
    BD002Component,
    BD003Component,
    BD004Component,
    BdInvoiceTypeComponent,
    BDCustomerNicknameComponent,
    Form106Component,
    RQ601Component,
    RQ604Component,
    PermissionComponent,
    PermissionDeniedComponent,
    InvoiceUploadComponent,
    InvoiceQueryComponent,
    MyInvoicesComponent,
    InvoiceFormFieldsComponent,
    HelpComponent,
    RQ701Component,
    RQ704Component,
    BdInvoiceRailComponent,
    BdTicketRailComponent,
    SupplierInfoComponent,
    BdexpensedeptComponent,
    BdCompanyCategoryComponent,
    BD009Component,
    ReportUberTransactionalQueryComponent,
    SignUberFareComponent,
    RQ801Component,
    BdvirtualdepartmentComponent,
  ],
  imports: [
    NgZorroAntdModule,
    ShareModule,
    CommonModule,
    ERSPagesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    NzFormModule,
    NzSelectModule,
    NzSpaceModule,
    PdfJsViewerModule,
  ],
  providers: [{ provide: CryptoService }, { provide: CommonService }],
})
export class ERSPagesModule {}
