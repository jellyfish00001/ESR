import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FormDetail, StateInfo } from './classes/data-item';
import { DetailTableColumn } from './classes/table-column'
import { format } from 'date-fns';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-report-uber-transactional-query',
  templateUrl: './report-uber-transactional-query.component.html',
  styleUrls: ['./report-uber-transactional-query.component.scss']
})

export class ReportUberTransactionalQueryComponent implements OnInit {
  screenHeight: any;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  companyList: any[] = [];
  stateList: any[] = StateInfo;
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
    this.queryForm = this.fb.group({
      startDate: [null, [this.startDateValidator]],
      endDate: [null, [this.endDateValidator]],
      rno: [null],
      applicantEmplid: [null],
      approval: [{ value: null, disabled: true }],
      state: [this.stateList],
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
        startDate: paramValue.startDate,
        endDate: paramValue.endDate,
        rno: paramValue.rno == null ? null : paramValue.rno.trim(),
        signStatus: paramValue.state.filter(o => o.checked).map(o => o.value),
        cuser: paramValue.applicantEmplid,
        company: paramValue.companyCode.filter(o => o.checked).map(o => o.value),
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetUberTransactional, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: FormDetail[] = [];
        res.body.data?.map(o => {
          result.push({
            tripId: o.tripId, // TripId
            transactionTimestamp: o.transactionTimestamp, // 交易时间戳
            requestDateUtc: o.requestDateUtc ? format(new Date(o.requestDateUtc), "yyyy/MM/dd") : '', // 预约日期（UTC）
            requestTimeUtc: o.requestTimeUtc, // 预约时间（UTC）
            requestDate: o.requestDate ? format(new Date(o.requestDate), "yyyy/MM/dd") : '', // 预约日期（本地）
            requestTime: o.requestTime, // 预约时间（本地）
            dropOffDateUtc: o.dropOffDateUtc ? format(new Date(o.requestDate), "yyyy/MM/dd") : '', // 下车日期（UTC）
            dropOffTimeUtc: o.dropOffTimeUtc, // 下车时间（UTC）
            dropOffDate: o.dropOffDate ? format(new Date(o.dropOffDate), "yyyy/MM/dd") : '', // 下车日期（本地）
            dropOffTime: o.dropOffTime, // 下车时间（本地）
            requestTimezoneOffsetFromUtc: o.requestTimezoneOffsetFromUtc, // 时区偏移
            firstName: o.firstName, // 名字
            lastName: o.lastName, // 姓
            email: o.email, // Email
            employeeId: o.employeeId, // EmployeeId
            service: o.service, // 服务
            city: o.city, // City
            distance: o.distance, // 距离
            duration: o.duration, // 总时间
            pickupAddress: o.pickupAddress, // PickupAddress
            dropOffAddress: o.dropOffAddress, // DropOffAddress
            expenseCode: o.expenseCode, // ExpenseCode
            expenseMemo: o.expenseMemo, // 费用备忘录
            invoices: o.invoices, // 发票
            program: o.program, // 方案
            group: o.group, // Group
            paymentMethod: o.paymentMethod, // 付款方式
            transactionType: o.transactionType, // 交易类型
            fareInLocalCurrency: o.fareInLocalCurrency, // 票价（本地币）
            taxesInLocalCurrency: o.taxesInLocalCurrency, // 税费（本地币）
            tipInLocalCurrency: o.tipInLocalCurrency, // 小费（本地币）
            transactionAmountInLocalCurrency: o.transactionAmountInLocalCurrency, // 交易金额（本地币）
            localCurrencyCode: o.localCurrencyCode, // 本地货币代码
            fareInHomeCurrency: o.fareInHomeCurrency, // 票价（本币）
            taxesInHomeCurrency: o.taxesInHomeCurrency, // 税费（本币）
            tipInHomeCurrency: o.tipInHomeCurrency, // 小费（本币）
            transactionAmountInHomeCurrency: o.transactionAmountInHomeCurrency, // 交易金额（本币）
            estimatedServiceAndTechnologyFee: o.estimatedServiceAndTechnologyFee, // 预估服务费
            rno: o.rno, // rno
            signStatus: o.signStatus, // SignStatus
            fileName: o.fileName, // 文件名
            company: o.company, // 公司别
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
      this.Service.doPost(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DownloadUberTransactional, this.queryParam, true).subscribe((res) => {
        this.downloadFlow(res, `${this.translate.instant('UberTransactional')}-${format(new Date(), 'yyyyMMdd')}.xlsx`);
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
