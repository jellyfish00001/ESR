import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { TableColumnModel } from 'src/app/shared/models';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { format } from 'date-fns';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-finance-payment-list-maintenance',
  templateUrl: './finance-payment-list-maintenance.component.html',
  styleUrls: ['./finance-payment-list-maintenance.component.scss']
})
export class FinancePaymentListMaintenanceComponent implements OnInit {
  isSpinning = false;
  radioValue = 'A'
  ConfirmDetailTableData: ConfirmForm[] = [];
  ConfirmDetailTotal: number;
  DetailTotal: number;
  ConfirmDetailListTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('payment-no'),
      columnKey: 'no',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.no.localeCompare(b.no),
    },
    {
      title: this.translate.instant('sysno'),
      columnKey: 'sysno',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.sysno.localeCompare(b.sysno),
    },
    {
      title: this.translate.instant('amt'),
      columnKey: 'amt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.amt.localeCompare(b.amt),
    },
    {
      title: this.translate.instant('bank'),
      columnKey: 'bank',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.bank.localeCompare(b.bank),
    },
    {
      title: this.translate.instant('payment'),
      columnKey: 'payment',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.payment.localeCompare(b.payment),
    },
    {
      title: this.translate.instant('payment-cuser'),
      columnKey: 'cuser',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('identification'),
      columnKey: 'identification',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ConfirmForm, b: ConfirmForm) =>
        a.identification.localeCompare(b.identification),
    },
  ];
  isQueryLoading = false;
  pageIndex: number = 1;
  pageSize: number = 10;
  ConfirmChecked = false;
  selectSaveConfirmRno = new Set<string>();
  indeterminate = false;
  indeterminateDetail = false;
  showModal = false;
  queryCompanyList: any[] = [];
  companyList: any[] = [];
  uploadForm: UntypedFormGroup;
  queryForm: UntypedFormGroup;
  banks: any[] = [];
  QueryDetailListTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('payment-no'),
      columnKey: 'no',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.no.localeCompare(b.no),
    },
    {
      title: this.translate.instant('sysno'),
      columnKey: 'sysno',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.sysno.localeCompare(b.sysno),
    },
    {
      title: this.translate.instant('amt'),
      columnKey: 'amt',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.amt.localeCompare(b.amt),
    },
    {
      title: this.translate.instant('bank'),
      columnKey: 'bank',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.bank.localeCompare(b.bank),
    },
    {
      title: this.translate.instant('payment'),
      columnKey: 'payment',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.payment.localeCompare(b.payment),
    },
    {
      title: this.translate.instant('payment-cuser'),
      columnKey: 'cuser',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('identification'),
      columnKey: 'identification',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: DetailForm, b: DetailForm) =>
        a.identification.localeCompare(b.identification),
    },
  ];
  QueryDetailTableData: DetailForm[] = [];
  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
  attachmentList = [];
  sampleName: string = this.translate.instant('payment-upload-sample');
  sampleUrl: string = "../../../assets/file/payment.xlsx";

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
    public translate: TranslateService,
    private modal: NzModalService,
    private crypto: CryptoService,
    private router: Router,
    private message: NzMessageService,
    private commonSrv: CommonService,
    private authService: AuthService,
  ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin', 'FinanceAdmin']); //'fin'
    this.isSpinning = true;
    this.queryForm = this.fb.group({
      company: ['', [Validators.required]],
      sysno: [null],
      bank: [null],
      cuser: [null],
      paymentdate: [null],
      identification: [null],
    });
    this.uploadForm = this.fb.group({
      company: [null, [Validators.required]],
      paymentdate: [null],
      identification: [null, [Validators.required]],
      attachmentList: [null, [Validators.required]]
    })
    this.QueryConfirmDetailList(true);
    this.getCompanyData();
  }
  handleOk(): void {
    if (this.uploadForm.controls.company.value == null || this.uploadForm.controls.company.value == '') {
      this.message.warning('company must select!', { nzDuration: 5000 });
      return;
    }
    if (this.uploadForm.controls.identification.value == null || this.uploadForm.controls.identification.value == '') {
      this.message.warning('identification must input!', { nzDuration: 5000 });
      return;
    }
    if (this.attachmentList.length == 0) {
      this.message.warning('file must upload!', { nzDuration: 5000 });
      return;
    }
    const formData = new FormData();
    formData.append("company", this.uploadForm.controls.company.value);
    formData.append("paymentdate", this.uploadForm.controls.paymentdate.value ? format(new Date(this.uploadForm.controls.paymentdate.value), 'yyyy/MM/dd') : '');
    formData.append("identification", this.uploadForm.controls.identification.value);
    formData.append(this.attachmentList[0].uid, this.attachmentList[0].originFileObj);
    this.Service.Post(URLConst.SaveConfirmPaymentList, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 5000 });
        } else {
          this.showModal = false;
          this.modal.success({
            nzTitle: this.translate.instant('operate-successfully'),
            nzContent: `<p>Request NO: ${res.body.data}</p>`,
            nzOnOk: () => this.QueryConfirmDetailList(true)
          });
        }
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }

  beforeAttachUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let upload = this.attachmentList.length > 0;
      if (upload) {
        this.message.error(this.translate.instant('can-upload-only-one-item'));
      }
      observer.next(!upload);
      observer.complete();
    });
  };
  handleAttachChange(info: NzUploadChangeParam): void {
    let attachList = [...info.fileList];
    if (attachList.length > 1) return;
    attachList = attachList.map(file => {
      file.status = "done";
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.uploadForm.controls.attachmentList.setValue(attachList);
    this.attachmentList = [...attachList];
  }

  pageIndexChangeDetail(value) {
    this.pageIndex = value;
    this.QueryDetailList();
  }
  pageSizeChangeDetail(value) {
    this.pageSize = value;
    this.QueryDetailList();
  }
  QueryDetailList(): void {
    if (this.queryForm.controls.company.value == null) {
      this.message.warning('company must select!', { nzDuration: 5000 });
      return;
    }
    this.isQueryLoading = true;
    var queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        companyList: this.queryForm.controls.company.value == '' ? this.companyList.filter(o => o != '') : [this.queryForm.controls.company.value],
        paymentdate: this.queryForm.controls.paymentdate.value,
        sysno: this.queryForm.controls.sysno.value,
        bank: this.queryForm.controls.bank.value,
        cuser: this.queryForm.controls.cuser.value,
        identification: this.queryForm.controls.identification.value,
      }
    }
    this.Service.Post(URLConst.QueryPaymentList, queryParam).subscribe((res) => {
      this.QueryDetailTableData = [];
      if (res && res.status === 200 && res.body != null) {
        this.DetailTotal = res.body.total;
        if (res.body.total == 0) {
          this.message.success(this.translate.instant('noData'), { nzDuration: 5000 });
        } else {
          res.body.data?.map(i => {
            this.QueryDetailTableData.push({
              no: i.no,
              sysno: i.sysno,
              cuser: i.cuser,
              amt: i.amt,
              bank: i.bank,
              payment: format(new Date(i.payment), "yyyy/MM/dd"),
              company: i.company,
              identification: i.identification
            })
          });
        }

        this.QueryDetailTableData = [...this.QueryDetailTableData];
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }

  ChangeBank() {
    this.GetBanksData(this.queryForm.controls.company.value);
  }
  GetBanksData(company: string) {
    if (company == null || company == '') { return; }
    this.Service.doGet(URLConst.GetBanks + `?company=${company}`, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.banks = res.body.data;
          if (!this.banks.includes(this.queryForm.controls.bank.value))
            this.queryForm.controls.bank.setValue('');
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 }); }
      this.isSpinning = false;
    });
  }

  ChangeApplication(item) {
    this.radioValue = item;
    this.pageIndex = 1;
    this.pageSize = 10;
    this.indeterminate = false;
    this.selectSaveConfirmRno.clear();
  }

  onItemChecked(sysno: string, checked: boolean): void {
    this.updateCheckedSet(sysno, checked);
    this.refreshCheckedStatus();
  }
  updateCheckedSet(sysno: string, checked: boolean): void {
    if (checked) {
      this.selectSaveConfirmRno.add(sysno);
    } else {
      this.selectSaveConfirmRno.delete(sysno);
    }
  }
  refreshCheckedStatus(): void {
    this.ConfirmChecked = this.ConfirmDetailTableData.every(({ sysno }) => this.selectSaveConfirmRno.has(sysno));
    this.indeterminate = this.ConfirmDetailTableData.some(({ sysno }) => this.selectSaveConfirmRno.has(sysno)) && !this.ConfirmChecked;
  }
  onAllChecked(checked: boolean): void {
    this.ConfirmDetailTableData.forEach(({ sysno }) => this.updateCheckedSet(sysno, checked));
    this.refreshCheckedStatus();
  }
  onCurrentPageDataChange(ConfirmDetailTableData: ConfirmForm[]): void {
    this.ConfirmDetailTableData = ConfirmDetailTableData;
    this.refreshCheckedStatus();
  }
  pageIndexChange(value) {
    this.pageIndex = value;
    this.QueryConfirmDetailList();
  }
  pageSizeChange(value) {
    this.pageSize = value;
    this.QueryConfirmDetailList();
  }

  QueryConfirmDetailList(initial: boolean = false): void {
    this.isQueryLoading = true;
    var queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    }
    if (initial) {
      queryParam.pageIndex = 1;
      queryParam.pageSize = 10;
    }
    this.Service.Post(URLConst.GetConfirmPaymentList, queryParam).subscribe((res) => {
      this.ConfirmDetailTableData = [];
      if (res && res.status === 200 && res.body != null) {
        this.ConfirmDetailTotal = res.body.total;
        if (res.body.total == 0) {
          this.message.success(this.translate.instant('noData'), { nzDuration: 6000 });
        } else {
          res.body.data?.map(i => {
            this.ConfirmDetailTableData.push({
              no: i.no,
              sysno: i.sysno,
              cuser: i.cuser,
              amt: i.amt,
              bank: i.bank,
              payment: format(new Date(i.payment), "yyyy/MM/dd"),
              company: i.company,
              identification: i.identification
            })
          });
        }
        this.ConfirmDetailTableData = [...this.ConfirmDetailTableData];
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    }, (error) => {
      // 捕获 HTTP 403 错误
      if (error && error.status === 403) {
        this.message.error(this.translate.instant('system.permission-denied'), { nzDuration: 6000 });
      } else {
        this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 });
      }
      this.isSpinning = false;
      this.isQueryLoading = false;
    });
  }
  handleCancel(): void {
    this.showModal = false;
  }

  // DownloadFile(item) {
  //   this.Service.Download(URLConst.DownloadConfirmPaymentByRno, item, `${item.bank}_PaymentList.xlsx`);
  // }
  DownloadSysno(item) {
    let fileName = item.bank + this.translate.instant('petty-cash-payment-list') + '.xlsx';
    this.Service.Download(URLConst.DownloadSysnoPayment, item, fileName);
  }
  DownloadNo(item) {
    this.Service.Download(URLConst.DownloadNoPayment, item, `${item.no}.xlsx`);
  }
  deleteRow(value: string): void {
    this.isSpinning = true;
    var queryParam = [value];
    this.DeleteConfirmList(queryParam);
  }
  SaveSelect(): void {
    this.isQueryLoading = true;
    var queryParam = this.ConfirmDetailTableData.filter(i => this.selectSaveConfirmRno.has(i.sysno));
    this.Service.Post(URLConst.UpdatePaymentStatusList, queryParam).subscribe((res) => {
      if (res && res.status === 200) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 5000 });
        } else {
          this.message.success(this.translate.instant('save-successfully'), { nzDuration: 5000 });
        }
        this.QueryConfirmDetailList();
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }
  deleteSelect(): void {
    this.isSpinning = true;
    this.DeleteConfirmList(Array.from(this.selectSaveConfirmRno));
  }
  DeleteConfirmList(queryParam: string[]) {
    this.Service.Delete(URLConst.DeleteConfirmPaymentList, queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('tips-delete-success'), { nzDuration: 5000 });
          queryParam.map((o) => {
            this.ConfirmDetailTableData = this.ConfirmDetailTableData.filter(i => i.sysno != o);
          });
        }
        else {
          this.message.error(res.body.message, { nzDuration: 5000 });
        }
        this.ConfirmDetailTableData = [...this.ConfirmDetailTableData];
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }
  addItem(): void {
    this.uploadForm.reset();
    this.attachmentList = [];
    this.showModal = true;
  }
  getCompanyData() {
    this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
    this.queryCompanyList = this.commonSrv.getCompanyAddOptionsByPermission;
  }

}

export interface ConfirmForm {
  no: string;
  sysno: string;
  cuser: string;
  amt: string;
  bank: string;
  payment: string;
  company: string;
  identification: string;
}

export interface DetailForm {
  no: string;
  sysno: string;
  cuser: string;
  amt: string;
  bank: string;
  payment: string;
  company: string;
  identification: string;
}
