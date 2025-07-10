import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FormDetail, StateInfo, VoucherTypeInfo, FormTypeInfo } from './classes/data-item';
import { DetailTableColumn } from './classes/table-column'
import { format } from 'date-fns';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-form103',
  templateUrl: './form103.component.html',
  styleUrls: ['./form103.component.scss']
})

export class FORM103Component implements OnInit {
  screenHeight: any;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  companyList: any[] = [];
  stateList: any[] = StateInfo;
  voucherTypeList: any[] = VoucherTypeInfo;
  formTypeList: any[] = FormTypeInfo;
  detailListTableColumn = DetailTableColumn;
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
    this.stateList.map(o => o.label = this.translate.instant(o.label));
    this.voucherTypeList.map(o => o.label = this.translate.instant(o.label));
    this.formTypeList.map(o => o.label = this.translate.instant(o.label));
    // this.isFin = this.authService.CheckPermissionByRole('fin');
    this.getEmployeeInfo();
    this.formInitial();
    this.getCompanyData();
    this.queryForm.valueChanges.subscribe(value => {
      this.showTable = false;
    });
  }

  formInitial(): void {
    if (this.companyList.length > 0) {
      this.companyList.map(o => o.checked = this.userInfo.company == o.value);
    }
    this.stateList.map(o => o.checked = false);
    this.voucherTypeList.map(o => o.checked = false);
    this.formTypeList.map(o => o.checked = false);
    this.queryForm = this.fb.group({
      startDate: [null, [this.startDateValidator]],
      endDate: [null, [this.endDateValidator]],
      rno: [null],
      applicantEmplid: [null],
      approval: [{ value: null, disabled: true }],
      state: [this.stateList],
      voucherType: [this.voucherTypeList],
      formType: [this.formTypeList],
      companyCode: [this.companyList],
    });
    let today = new Date();
    let year = new Date(`${today.getFullYear()}-01-01`);
    this.queryForm.controls.startDate.setValue(year);
    this.queryForm.controls.endDate.setValue(today);
    if (this.isFin && this.queryForm.controls.approval.disabled) { this.queryForm.controls.approval.enable(); }
    if (!!this.userInfo?.emplid) {
      this.queryForm.controls.approval.setValue(this.userInfo.emplid);
    }
  }

  ResetQueryParams(): void {
    this.isSpinning = true;
    this.formInitial();
    this.showTable = false;
    this.isSpinning = false;
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
        sdate: paramValue.startDate,
        edate: paramValue.endDate,
        rno: paramValue.rno == null ? null : paramValue.rno.trim(),
        cuser: paramValue.applicantEmplid,
        aemplid: paramValue.approval,
        status: paramValue.state.filter(o => o.checked).map(o => o.value),
        category: paramValue.voucherType.filter(o => o.checked).map(o => o.value),
        formcode: paramValue.formType.filter(o => o.checked).map(o => o.value),
        company: paramValue.companyCode.filter(o => o.checked).map(o => o.value),
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QuerySignedForm, this.queryParam).subscribe((res) => {
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
            appliedDate: !o.cdate ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
            expenseType: o.expname,
            dept: o.expdeptid,
            curr: o.currency,
            actualAmount: o.actamt,
            step: o.stepname,
            paymentDate: o.paymentdate,
            apid: o.apid,
            status: o.status
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
    const url = this.router.serializeUrl(
      this.router.createUrlTree([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form103' } })
    );
    window.open(`#${url}`, '_blank', 'noopener');
  }

}
