import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { FormDetail } from './classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-report-applied-form-query-detail',
  templateUrl: './report-applied-form-query-detail.component.html',
  styleUrls: ['./report-applied-form-query-detail.component.scss']
})

export class ReportAppliedFormQueryDetailComponent implements OnInit {
  DetailTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company-code'),
      columnKey: 'companyCode',
      columnWidth: '70px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) =>
        a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: this.translate.instant('application-number'),
      columnKey: 'rno',
      columnWidth: '130px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('applied-dept'),
      columnKey: 'appliedDept',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.appliedDept.localeCompare(b.appliedDept),
    },
    {
      title: this.translate.instant('applicant'),
      columnKey: 'applicant',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicant.localeCompare(b.applicant),
    },
    {
      title: this.translate.instant('applicant-name'),
      columnKey: 'applicantName',
      columnWidth: '110px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicantName.localeCompare(b.applicantName),
    },
    {
      title: this.translate.instant('digest'),
      columnKey: 'digest',
      columnWidth: '110px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.formTypeName.localeCompare(b.digest),
    },
    {
      title: this.translate.instant('expense-type'),
      columnKey: 'expenseType',
      columnWidth: '110px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.expenseType.localeCompare(b.expenseType),
    },
     {
      title: this.translate.instant('form.fee-voucher-date'),
      columnKey: 'rdate',//費用/憑證日期
      columnWidth: '90px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.rdate.localeCompare(b.rdate),
    },
      {
      title: this.translate.instant('invoice-no'),
      columnKey: 'invno',//發票號碼
      columnWidth: '70px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.invno.localeCompare(b.invno),
    },
      {
      title: this.translate.instant('col.expense-attribution-department'),
      columnKey: 'dept',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.dept.localeCompare(b.dept),
    },
    {
      title: this.translate.instant('col.currency'),
      columnKey: 'curr',
      columnWidth: '60px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.curr.localeCompare(b.curr),
    },
    {
      title: this.translate.instant('actual-amount'),
      columnKey: 'actualAmount',
      columnWidth: '80px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.actualAmount.localeCompare(b.actualAmount),
    },
    {
      title: this.translate.instant('amount-before-tax'),
      columnKey: 'untaxamount',//發票稅前金額
      columnWidth: '90px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.untaxamount.localeCompare(b.untaxamount),
    },
    {
      title: this.translate.instant('invoice-tax'),
      columnKey: 'taxamount',//發票稅金
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.taxamount.localeCompare(b.taxamount),
    },
             {
      title: this.translate.instant('invoice-code'),
      columnKey: 'invcode',//發票統一編碼
      columnWidth: '90px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.invcode.localeCompare(b.invcode),
    },
    {
      title: this.translate.instant('applied-date'),
      columnKey: 'appliedDate',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.appliedDate.localeCompare(b.appliedDate),
    },
    {
      title: this.translate.instant('form-type-name'),
      columnKey: 'formTypeName',
      columnWidth: '110px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.formTypeName.localeCompare(b.formTypeName),
    },
    {
      title: 'Project Code',
      columnKey: 'projectcode',//project Code
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.projectcode.localeCompare(b.projectcode),
    },
         {
      title: this.translate.instant('payee'),
      columnKey: 'payeeId',//收款人
      columnWidth: '60px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.payeeId.localeCompare(b.payeeId),
    },
             {
      title: this.translate.instant('payee-name'),
      columnKey: 'payeename',//收款人姓名
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.payeename.localeCompare(b.payeename),
    },

    {
      title: this.translate.instant('step'),
      columnKey: 'step',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.step.localeCompare(b.step),
    },
    {
      title: this.translate.instant('payment-date'),
      columnKey: 'payment',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: FormDetail, b: FormDetail) => a.payment.localeCompare(b.payment),
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
  detailTableData: FormDetail[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  showTable = false;
  isQueryLoading = false;
  isFin: boolean = false;



  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    private authService: AuthService,
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
    this.getEmployeeInfo();
    this.formInitial();
    this.getFormCodeList();
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
    this.authService.CheckPermissionByRole(['Admin', 'FinanceAdmin']).subscribe(hasPermission => {
      this.isFin = hasPermission;
      if (hasPermission && this.queryForm.controls.applicantEmplid.disabled) { this.queryForm.controls.applicantEmplid.enable(); }
    });
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
        value: "temporary",
        checked: false
      },
      {
        label: this.translate.instant('signing'),
        value: "pending_approval",
        checked: false
      },
      {
        label: this.translate.instant('approved'),
        value: "approved",
        checked: false
      },
      {
        label: this.translate.instant('reject'),
        value: "rejected",
        checked: false
      },
      {
        label: this.translate.instant('cancel'),
        value: "canceled",
        checked: false
      },
      {
        label: this.translate.instant('posted'),
        value: "posted",
        checked: false
      },
      {
        label: this.translate.instant('confirmed_posted'),
        value: "confirmed_posted",
        checked: false
      },
      {
        label: this.translate.instant('paid'),
        value: "paid",
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
getFormCodeList() {
  this.Service.doGet(
    this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFormTypeList,
    null
  ).subscribe((res) => {
    if (res && res.status === 200 && !!res.body) {
      if (res.body.status == 1) {
        // 假设res.body.data为 [{formname: '一般费用', formcode: 'CASH_1'}, ...]
        this.formTypeList = res.body.data.map(item => ({
          label: item.formname,
          value: item.formcode,
          checked: false
        }));
        // 同步到表单控件
        if (this.queryForm && this.queryForm.controls.formType) {
          this.queryForm.controls.formType.setValue(this.formTypeList);
        }
      } else {
        this.message.error(res.body.message, { nzDuration: 6000 });
      }
    } else {
      this.message.error(
        res.message ?? this.translate.instant('server-error'),
        { nzDuration: 6000 }
      );
    }
  });
}
  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
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
        company: (() => {
          const checkedCompanies = paramValue.companyCode.filter(o => o.checked).map(o => o.value);
          return checkedCompanies.length > 0 ? checkedCompanies : paramValue.companyCode.map(o => o.value);
        })(),
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryAppliedDetailForm, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: FormDetail[] = [];
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
            appliedDate: !o.cdate
              ? null
              : format(new Date(o.cdate), 'yyyy/MM/dd'),
            expenseType: o.expname,
            dept: o.expdeptid,
            curr: o.currency,
            actualAmount: o.actamt ? o.actamt.toLocaleString() : '',
            untaxamount: o.untaxamountDisplay ? o.untaxamountDisplay.toLocaleString() : '',
            taxamount: o.taxamountDisplay ? o.taxamountDisplay.toLocaleString() : '',
            step: o.stepname,
            payment:               
            o.payment == null ? null : format(new Date(o.payment), 'yyyy/MM/dd'),
            apid: o.apid,
            status: o.status,
            payeeId:o.payeeId,
            payeename:o.payeename,
            invno: o.invno,
            invcode: o.invcode,
            projectcode: o.projectcode,
            digest: o.summary,
            rdate: o.rdate ? format(new Date(o.rdate), 'yyyy/MM/dd') : null,
          });
        });
        this.detailTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  download() {
    if (this.queryParam != null) {
      this.Service.doPost(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DownloadAppliedDetailForm, this.queryParam, true).subscribe((res) => {
        this.downloadFlow(res, `${this.translate.instant('DetailReport')}-${format(new Date(), 'yyyyMMdd')}.xlsx`);
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

  ////////带选择框表
  // checked = false;
  // addloading = false;
  // editloading = false;
  // deleteloading = false;
  // indeterminate = false;
  editRow(item): void {
    this.isSpinning = true;
    if (item.applicant != this.userInfo.emplid) {
      this.message.error(this.translate.instant('no-auth'));
      this.isSpinning = false;
      return;
    }
    let editStateList = ["temporary", "rejected", "canceled"];
    if (editStateList.indexOf(item.status) == -1) {
      this.message.error(this.translate.instant('state-error-to-edit'));
      this.isSpinning = false;
      return;
    }
    if (item.status == "rejected") this.enterEditPage(item)
    else this.checkForm(item);
    this.isSpinning = false;
  }

  cancelRow(item): void {
    this.isQueryLoading = true;
    this.isSpinning = true;
    // let cancelStateList = ["T", "R"];
    // if (cancelStateList.indexOf(item.status) == -1) {
    //   this.message.error(this.translate.instant('state-error-to-cancel'));
    //   this.isSpinning = false;
    //   return;
    // }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.CancelSignApi + `?rno=${item.rno}`, {}).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('success-to-cancel-sign'));
          this.queryResultWithParam();
          this.isQueryLoading = false;
          this.isSpinning = false;
        } else {
          this.message.error(res.body.message);
          this.isQueryLoading = false;
          this.isSpinning = false;
        }
      }
    })
    // this.deleteloading = true;

    // this.detailTableData = this.detailTableData.filter(d => d.id != id);
    // this.deleteloading = false;
  }

  checkForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    // this.router.navigate([`pages/ers-pages/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form104' } })
    const url = this.router.serializeUrl(
      this.router.createUrlTree([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form104' } })
    );
    window.open(`#${url}`, '_blank', 'noopener');
  }

  enterEditPage(item) {
    let data = { rno: this.crypto.encrypt(item.rno) };
    let urlDic = {
      ['rq104']: 'rq101',
      ['rq204']: 'rq201',
      ['rq204a']: 'rq201a',
      ['rq404']: 'rq401',
      ['rq404a']: 'rq401a',
      ['rq504']: 'rq501',
      ['rq604']: 'rq601',
      ['rq704']: 'rq701',
    }
    let target = urlDic[item.apid.toLowerCase()];
    const url = this.router.serializeUrl(
      this.router.createUrlTree([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form104' } })
    );
    window.open(`#${url}`, '_blank', 'noopener');
  }

}
