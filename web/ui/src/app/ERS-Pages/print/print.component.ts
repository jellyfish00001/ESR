import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CommonService } from 'src/app/shared/service/common.service';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { format } from 'date-fns';
import { TableColumnModel } from 'src/app/shared/models';
import { Router } from '@angular/router';
// import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-print',
  templateUrl: './print.component.html',
  styleUrls: ['./print.component.scss']
})
export class PrintComponent implements OnInit {
  isSpinning = false;
  companyList: any[] = [];
  userInfo: any;
  queryForm: UntypedFormGroup;
  stateList: any[] = [];
  voucherTypeList: any[] = [];
  formTypeList: any[] = [];
  detailTableData: PrintInfo[] = [];
  showTable = false;
  isQueryLoading = false;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  queryParam: any;
  DetailTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('rno'),
      columnKey: 'rno',
      columnWidth: '130px',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('formcode'),
      columnKey: 'formcode',
      columnWidth: '120px',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.formcode.localeCompare(b.formcode),
    },
    {
      title: this.translate.instant('formname'),
      columnKey: 'formname',
      columnWidth: '',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.formname.localeCompare(b.formname),
    },
    {
      title: this.translate.instant('cuser'),
      columnKey: 'cuser',
      columnWidth: '120px',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('cname'),
      columnKey: 'cname',
      columnWidth: '',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.cname.localeCompare(b.cname),
    },
    {
      title: this.translate.instant('cdate'),
      columnKey: 'cdate',
      columnWidth: '110px',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.cdate.localeCompare(b.cdate),
    },
    {
      title: this.translate.instant('stepname'),
      columnKey: 'stepname',
      columnWidth: '',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.stepname.localeCompare(b.stepname),
    },
    {
      title: this.translate.instant('payment'),
      columnKey: 'payment',
      columnWidth: '',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.payment.localeCompare(b.payment),
    },
    {
      title: this.translate.instant('company-code'),
      columnKey: 'company',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: PrintInfo, b: PrintInfo) =>
        a.company.localeCompare(b.company),
    },
  ];
  detailListTableColumn = this.DetailTableColumn;
  setOfCheckedRno = new Set<string>();
  indeterminate = false;
  checked = false;
  isFin: boolean = false;

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private crypto: CryptoService,
    private commonSrv: CommonService,
    private router: Router,
    // private authService: AuthService,
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

  print() {
    this.isSpinning = true;
    this.isQueryLoading = true;
    this.Service.Post(URLConst.Printing, Array.from(this.setOfCheckedRno)).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        var wind = window.open('');
        wind.document.body.innerHTML = res.body.data;
        wind.print();
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }

  refreshCheckedStatus(): void {
    this.checked = this.detailTableData.every(({ rno }) => this.setOfCheckedRno.has(rno));
    this.indeterminate = this.detailTableData.some(({ rno }) => this.setOfCheckedRno.has(rno)) && !this.checked;
  }

  onItemChecked(rno: string, checked: boolean): void {
    this.updateCheckedSet(rno, checked);
    this.refreshCheckedStatus();
  }
  updateCheckedSet(rno: string, checked: boolean): void {
    if (checked) {
      this.setOfCheckedRno.add(rno);
    } else {
      this.setOfCheckedRno.delete(rno);
    }
  }

  onAllChecked(checked: boolean): void {
    this.detailTableData.forEach(({ rno }) => this.updateCheckedSet(rno, checked));
    this.refreshCheckedStatus();
  }

  ResetQueryParams(): void {
    this.isSpinning = true;
    this.formInitial();
    this.showTable = false;
    this.isSpinning = false;
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
      this.setOfCheckedRno.clear();
      this.checked = false;
      this.indeterminate = false;
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        startdate: paramValue.startDate,
        enddate: paramValue.endDate,
        rno: paramValue.rno == null ? null : paramValue.rno.trim(),
        cuser: paramValue.applicantEmplid,
        status: paramValue.state.filter(o => o.checked).map(o => o.value),
        category: paramValue.voucherType.filter(o => o.checked).map(o => o.value),
        formcode: paramValue.formType.filter(o => o.checked).map(o => o.value),
        company: paramValue.companyCode.filter(o => o.checked).map(o => o.value),
        aemplid: paramValue.approval
      }
    }
    this.Service.Post(URLConst.PrintQuery, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: PrintInfo[] = [];
        res.body.data?.map(o => {
          result.push({
            rno: o.rno,
            formcode: o.formcode,
            cuser: o.cuser,
            cname: o.cname,
            cdate: !o.cdate ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
            step: o.step,
            stepname: o.stepname,
            status: o.status,
            apid: o.apid,
            formname: o.formname,
            company: o.company,
            payment: o.payment,
          })
        });
        this.detailTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  checkForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    // this.router.navigate([`pages/ers-pages/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'print' } })
    const url = this.router.serializeUrl(
      this.router.createUrlTree([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'print' } })
    );
    window.open(`#/${url}`, '_blank', 'noopener');
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
      approval: [null],
      state: [this.stateList],
      voucherType: [this.voucherTypeList],
      formType: [this.formTypeList],
      companyCode: [this.companyList],
    });
    let today = new Date();
    let year = new Date(`${today.getFullYear()}-01-01`);
    this.queryForm.controls.startDate.setValue(year);
    this.queryForm.controls.endDate.setValue(today);
    if (!!this.userInfo?.emplid) { this.queryForm.controls.applicantEmplid.setValue(this.userInfo.emplid); }
    if (this.isFin && this.queryForm.controls.applicantEmplid.disabled) this.queryForm.controls.applicantEmplid.enable();
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
      // {
      //   label: this.translate.instant('delay-advance'),
      //   value: "CASH_3A",
      //   checked: false
      // },
    ]
  }
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
}

export interface PrintInfo {
  rno: string;
  formcode: string;
  cuser: string;
  cname: string;
  cdate: string;
  step: number;
  stepname: string;
  status: string;
  apid: string;
  formname: string;
  company: string;
  payment: string;
}