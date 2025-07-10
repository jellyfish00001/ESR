import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { ExceptionDetail } from './classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-form105',
  templateUrl: './form105.component.html',
  styleUrls: ['./form105.component.scss']
})

export class FORM105Component implements OnInit {
  DetailTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company-code'),
      columnKey: 'companyCode',
      columnWidth: '70px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
        a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: this.translate.instant('application-number'),
      columnKey: 'rno',
      columnWidth: '120px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('form-type-name'),
      columnKey: 'formTypeName',
      columnWidth: '110px',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.formTypeName.localeCompare(b.formTypeName),
    },
    {
      title: this.translate.instant('applied-dept'),
      columnKey: 'appliedDept',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.appliedDept.localeCompare(b.appliedDept),
    },
    {
      title: this.translate.instant('applicant'),
      columnKey: 'applicant',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.applicant.localeCompare(b.applicant),
    },
    {
      title: this.translate.instant('applicant-name'),
      columnKey: 'applicantName',
      columnWidth: '110px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.applicantName.localeCompare(b.applicantName),
    },
    {
      title: this.translate.instant('applied-date'),
      columnKey: 'appliedDate',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.appliedDate.localeCompare(b.appliedDate),
    },
    {
      title: this.translate.instant('expense-type'),
      columnKey: 'expenseType',
      columnWidth: '140px',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.expenseType.localeCompare(b.expenseType),
    },
    {
      title: this.translate.instant('col.expense-attribution-department'),
      columnKey: 'dept',
      columnWidth: '120px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.dept.localeCompare(b.dept),
    },
    {
      title: this.translate.instant('col.currency'),
      columnKey: 'curr',
      columnWidth: '60px',
      align: 'center',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.curr.localeCompare(b.curr),
    },
    {
      title: this.translate.instant('actual-amount'),
      columnKey: 'actualAmount',
      columnWidth: '110px',
      align: 'right',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.actualAmount.localeCompare(b.actualAmount),
    },
    {
      title: this.translate.instant('step'),
      columnKey: 'step',
      columnWidth: '150px',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.step.localeCompare(b.step),
    },
  ];

  // rno: string;
  screenHeight: any;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  companyList: any[] = [];
  stateList: any[] = [];
  voucherTypeList: any[] = [];
  formTypeList: any[] = [];
  detailListTableColumn = this.DetailTableColumn;
  detailTableData: ExceptionDetail[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  showTable = false;
  isQueryLoading = false;
  totalFileList: any[] = [];
  fileList: any[] = [];
  isFin: boolean = false;



  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    // private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    private commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    // this.isFin = this.authService.CheckPermissionByRole('fin');
    this.getEmployeeInfo();
    this.formInitial();
    this.getCompanyData();
    this.queryForm.valueChanges.subscribe(value => {
      this.showTable = false;
    });
  }

  formInitial(): void {
    this.dataInit();
    if (this.companyList.length > 0)
      this.companyList.map(o => o.checked = this.userInfo.company == o.value);
    this.queryForm = this.fb.group({
      startDate: [null, [this.startDateValidator]],
      endDate: [null, [this.endDateValidator]],
      rno: [null],
      applicantEmplid: [{ value: null, disabled: true }],
      state: [this.stateList],
      voucherType: [this.voucherTypeList],
      formType: [this.formTypeList],
      companyCode: [this.companyList],
    });
    let today = new Date();
    let year = new Date(`${today.getFullYear()}-01-01`);
    this.queryForm.controls.startDate.setValue(year);
    this.queryForm.controls.endDate.setValue(today);
    this.queryForm.controls.applicantEmplid.setValue(this.userInfo?.emplid);
    if (this.isFin && this.queryForm.controls.applicantEmplid.disabled) { this.queryForm.controls.applicantEmplid.enable(); }
  }

  ResetQueryParams(): void {
    this.isSpinning = true;
    this.formInitial();
    this.showTable = false;
    this.isSpinning = false;
  }

  dataInit(): void {
    this.stateList = [
      {
        label: this.translate.instant('save-state'),
        value: "T",
        checked: false
      },
      {
        label: this.translate.instant('signing'),
        value: "P",
        checked: false
      },
      {
        label: this.translate.instant('approved'),
        value: "A",
        checked: false
      },
      {
        label: this.translate.instant('reject'),
        value: "R",
        checked: false
      },
      {
        label: this.translate.instant('return'),
        value: "B",
        checked: false
      },
      {
        label: this.translate.instant('cancel'),
        value: "C",
        checked: false
      },
    ];
    this.voucherTypeList = [
      {
        label: this.translate.instant('electronic-invoice'),
        value: "N",
        checked: false
      },
      {
        label: this.translate.instant('other-voucher'),
        value: "H",
        checked: false
      },
      {
        label: this.translate.instant('many-reimbursement'),
        value: "CASH_5",
        checked: false
      },
    ];
    this.formTypeList = [
      {
        label: this.translate.instant('general-expenses'),
        value: "CASH_1",
        checked: false
      },
      {
        label: this.translate.instant('entertainment-expenses'),
        value: "CASH_2",
        checked: false
      },
      {
        label: this.translate.instant('prepayments'),
        value: "CASH_3",
        checked: false
      },
      {
        label: this.translate.instant('batch-reimbursement'),
        value: "CASH_4",
        checked: false
      },
      {
        label: this.translate.instant('many-reimbursement'),
        value: "CASH_5",
        checked: false
      },
      {
        label: this.translate.instant('return-taiwan'),
        value: "CASH_6",
        checked: false
      },
      {
        label: this.translate.instant('salary-request'),
        value: "CASH_X",
        checked: false
      }
    ]
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-future-date'),
      startdate: this.translate.instant('can-not-later-than-end-date'),
      enddate: this.translate.instant('can-not-earlier-than-start-date'),
    }
  };


  startDateValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!!control.value) {
      if (control.value > new Date())
        return { date: true, error: true };
      if (!!this.queryForm.controls.endDate.value && new Date(control.value).setHours(0, 0, 0, 0) > (new Date(this.queryForm.controls.endDate.value)).setHours(0, 0, 0, 0))
        return { startdate: true, error: true };
      if (!this.queryForm.controls.endDate.pristine) {
        this.queryForm.controls.endDate!.markAsPristine();
        this.queryForm.controls.endDate!.updateValueAndValidity();
      }
    }
  };

  endDateValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!!control.value) {
      if (control.value > new Date())
        return { date: true, error: true };
      if (!!this.queryForm.controls.startDate.value && new Date(control.value).setHours(0, 0, 0, 0) < (new Date(this.queryForm.controls.startDate.value)).setHours(0, 0, 0, 0))
        return { enddate: true, error: true };
      if (!this.queryForm.controls.startDate.pristine) {
        this.queryForm.controls.startDate!.markAsPristine();
        this.queryForm.controls.startDate!.updateValueAndValidity();;
      }
    }
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.userInfo = { emplid: localStorage.getItem('userId') }
    }
  }


  getCompanyData() {
    this.commonSrv.getApplyQueryFormCompanys().subscribe(res => {
      this.companyList = res;
      this.companyList = this.companyList.map(o => { return { label: o, value: o, checked: this.userInfo.company == o } });
      this.queryForm.controls.companyCode.setValue(this.companyList);
      this.isSpinning = false;
    });
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithParam();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithParam();
  }

  queryResultWithParam(initial: boolean = false) {
    if (!this.queryForm.valid) {
      Object.values(this.queryForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('exist-invalid-field'));
      return;
    }
    this.isQueryLoading = true;
    let paramValue = this.queryForm.getRawValue();
    if (initial) {
      this.pageIndex = 1;
      this.pageSize = 10;
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        startdate: paramValue.startDate,
        enddate: paramValue.endDate,
        rno: paramValue.rno == null ? null : paramValue.rno.trim(),
        cemplid: paramValue.applicantEmplid,
        status: paramValue.state.filter(o => o.checked).map(o => o.value),
        category: paramValue.voucherType.filter(o => o.checked).map(o => o.value),
        formcode: paramValue.formType.filter(o => o.checked).map(o => o.value),
        company: paramValue.companyCode.filter(o => o.checked).map(o => o.value),
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryPaperForm, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: ExceptionDetail[] = [];
        res.body.data?.map(o => {
          result.push({
            companyCode: o.company,
            rno: o.rno,
            formTypeName: o.formname,
            appliedDept: o.deptid,
            applicant: o.cemplid,
            applicantName: o.cname,
            disabled: false,
            id: 0,
            appliedDate: !o.cdate ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
            expenseType: o.expname,
            dept: o.expdeptid,
            curr: o.currency,
            actualAmount: o.actamt,
            step: o.stepname,
            paymentDate: o.paymentdate,
            apid: o.apid,
            status: o.status
          });
          this.totalFileList = this.totalFileList.concat(o.invlist);
        });
        this.detailTableData = result;
        this.showTable = true;
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.isQueryLoading = false;
    });
  }

  checkInvoiceDetail(rno: string): void {
    // this.totalFileList.filter(o => o.id == id).map(async o => o.preview = await this.commonSrv.getPicBase64(o.originFileObj!));
    this.fileList = this.totalFileList.filter(o => o.rno == rno);
  }

  download() {
    if (this.queryParam != null) {
      this.Service.doPost(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DownloadPaperForm, this.queryParam, true).subscribe((res) => {
        this.downloadFlow(res, `${this.translate.instant('Report')}-${format(new Date(), 'yyyyMMdd')}.xlsx`);
      });
    }
  }

  downloadFlow(flow, name) {
    if (flow.size > 0) {
      const blob = new Blob([flow], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const objectUrl = URL.createObjectURL(blob);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.setAttribute('style', 'display:none');
      a.setAttribute('href', objectUrl);
      a.setAttribute('download', name);
      a.click();
      URL.revokeObjectURL(objectUrl);
    }
  }

  editRow(item): void {
    this.isSpinning = true;
    if (item.applicant != this.userInfo.emplid) {
      this.message.error(this.translate.instant('no-auth'));
      this.isSpinning = false;
      return;
    }
    let editStateList = ["T", "R", "C"];
    if (editStateList.indexOf(item.status) == -1) {
      this.message.error(this.translate.instant('state-error-to-edit'));
      this.isSpinning = false;
      return;
    }
    this.checkForm(item);
    this.isSpinning = false;
  }

  cancelRow(item): void {
    // this.deleteloading = true;

    // this.detailTableData = this.detailTableData.filter(d => d.id != id);
    // this.deleteloading = false;
  }

  openPicFile(preview: any, name: any): void {
    const img = new window.Image();
    img.src = preview;
    const newWin = window.open('');
    newWin.document.write(img.outerHTML);
    newWin.document.title = name;
    newWin.document.close();
  }

  checkForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    this.router.navigate([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form105' } })
  }

}
