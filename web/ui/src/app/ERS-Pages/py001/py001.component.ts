import { Component, OnInit } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { TableColumnModel } from 'src/app/shared/models';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { format } from 'date-fns';
import { NzModalService } from 'ng-zorro-antd/modal';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { Router } from '@angular/router';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-py001',
  templateUrl: './py001.component.html',
  styleUrls: ['./py001.component.scss']
})
export class Py001Component implements OnInit {
  queryForm: UntypedFormGroup;
  accountListForm: UntypedFormGroup;
  companyList: any[] = [];
  banks: any[] = [];
  isSpinning = false;
  showModal: boolean = false;
  isQueryLoading = false;
  isSaveLoading: boolean = false;
  indeterminate = false;
  indeterminate1 = false;
  checked = false;
  checked1 = false;
  QueryDetailTableData: AccountQueryForm[] = [];
  AccountDetailTableData: AccountForm[] = [];
  QueryDetailListTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('rno'),
      columnKey: 'rno',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('formname'),
      columnKey: 'formname',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.formname.localeCompare(b.formname),
    },
    {
      title: this.translate.instant('cuser'),
      columnKey: 'cuser',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('cname'),
      columnKey: 'cname',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.cname.localeCompare(b.cname),
    },
    {
      title: this.translate.instant('cdate'),
      columnKey: 'cdate',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.cdate.localeCompare(b.cdate),
    },
    {
      title: this.translate.instant('amount'),
      columnKey: 'amount',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.amount.localeCompare(b.amount),
    },
    {
      title: this.translate.instant('currency'),
      columnKey: 'currency',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.currency.localeCompare(b.currency),
    },
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('accountant'),
      columnKey: 'signAccount',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.signAccount.localeCompare(b.signAccount),
    },
    {
      title: this.translate.instant('bank'),
      columnKey: 'bank',
      columnWidth: '80px',
      align: 'left',
      sortFn: (a: AccountQueryForm, b: AccountQueryForm) =>
        a.bank.localeCompare(b.bank),
    },
  ];
  AccountDetailListTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('rno'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'left',
      sortFn: (a: AccountForm, b: AccountForm) =>
        a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('settleAccountDate'),
      columnKey: 'date',
      columnWidth: '',
      align: 'left',
      sortFn: (a: AccountForm, b: AccountForm) =>
        a.date.localeCompare(b.date),
    },
    {
      title: this.translate.instant('signAccount'),
      columnKey: 'signAccount',
      columnWidth: '',
      align: 'left',
      sortFn: (a: AccountForm, b: AccountForm) =>
        a.signAccount.localeCompare(b.signAccount),
    },
  ];
  QueryDetailTotal: number;
  AccountDetailTotal: number;
  pageIndex1: number = 1;
  pageSize1: number = 10;
  pageIndex: number = 1;
  pageSize: number = 10;
  selectSaveAccountRno = new Set<string>();
  selectSaveAccountRno1 = new Set<string>();
  userInfo: any;

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
    public translate: TranslateService,
    private modal: NzModalService,
    private crypto: CryptoService,
    private router: Router,
    private message: NzMessageService,
    // private authService: AuthService,
    private commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    // if (!this.authService.CheckPermissionByRole('fin')) this.router.navigate([`ers/permissiondenied`]);
    this.userInfo = this.commonSrv.getUserInfo;
    this.init();
    this.isSpinning = false;
  }

  init() {
    this.queryForm = this.fb.group({
      accountant: [this.userInfo?.emplid],
      banks: [this.banks],
      companyCode: [this.companyList],
    });
    this.queryForm.controls.banks.setValue('');
    this.accountListForm = this.fb.group({
      rno: [null],
      accountant: [this.userInfo?.emplid],
      startDate: [null, [this.startDateValidator]],
      endDate: [null, [this.endDateValidator]],
    })
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
      if (!!this.accountListForm.controls.endDate.value && new Date(control.value).setHours(0, 0, 0, 0) > (new Date(this.accountListForm.controls.endDate.value)).setHours(0, 0, 0, 0))
        return { startdate: true, error: true };
      if (!this.accountListForm.controls.endDate.pristine) {
        this.accountListForm.controls.endDate!.markAsPristine();
        this.accountListForm.controls.endDate!.updateValueAndValidity();
      }
    }
  };
  endDateValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!!control.value) {
      if (control.value > new Date())
        return { date: true, error: true };
      if (!!this.accountListForm.controls.startDate.value && new Date(control.value).setHours(0, 0, 0, 0) < (new Date(this.accountListForm.controls.startDate.value)).setHours(0, 0, 0, 0))
        return { enddate: true, error: true };
      if (!this.accountListForm.controls.startDate.pristine) {
        this.accountListForm.controls.startDate!.markAsPristine();
        this.accountListForm.controls.startDate!.updateValueAndValidity();;
      }
    }
  };
  getCompanyData() {
    this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
    if (!this.companyList.includes(this.queryForm.controls.companyCode.value)) this.queryForm.controls.companyCode.setValue('');
    this.isSpinning = false;
  }
  ChangeBank() {
    this.GetBanksData(this.queryForm.controls.companyCode.value);
  }
  GetBanksData(company: string) {
    if (company == null || company == '') { return; }
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetBanks + `?company=${company}`, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.banks = res.body.data;
          if (!this.banks.includes(this.queryForm.controls.banks.value))
            this.queryForm.controls.banks.setValue('');
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(res.message ??this.translate.instant('server-error'), { nzDuration: 6000 }); }
      this.isSpinning = false;
    });
  }
  pageIndexChange1(value) {
    this.pageIndex1 = value;
    this.QueryDetailList();
  }

  pageSizeChange1(value) {
    this.pageSize1 = value;
    this.QueryDetailList();
  }
  pageIndexChange(value) {
    this.pageIndex = value;
    this.QueryAccountList();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.QueryAccountList();
  }
  deleteRow(value: string): void {
    this.isSpinning = true;
    var queryParam = [value];
    this.DeleteAccountList(queryParam);
  }
  deleteSelect(): void {
    this.isSpinning = true;
    this.DeleteAccountList(Array.from(this.selectSaveAccountRno));
  }
  DeleteAccountList(queryParam: string[]) {
    this.Service.Delete(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DeleteAccountList, queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          queryParam.map((o) => {
            this.AccountDetailTableData = this.AccountDetailTableData.filter(i => i.rno != o);
          });
          this.message.success(this.translate.instant('tips-delete-success'));
        }
        else {
          this.message.error(res.body.message);
        }
        this.AccountDetailTableData = [...this.AccountDetailTableData];
        this.isSpinning = false;
        this.isSaveLoading = false;
      }
    });
  }

  onItemChecked(rno: string, checked: boolean): void {
    this.updateCheckedSet(rno, checked);
    this.refreshCheckedStatus();
  }
  onItemChecked1(rno: string, checked: boolean): void {
    this.updateCheckedSet1(rno, checked);
    this.refreshCheckedStatus1();
  }
  updateCheckedSet(rno: string, checked: boolean): void {
    if (checked) {
      this.selectSaveAccountRno.add(rno);
    } else {
      this.selectSaveAccountRno.delete(rno);
    }
  }
  updateCheckedSet1(rno: string, checked: boolean): void {
    if (checked) {
      this.selectSaveAccountRno1.add(rno);
    } else {
      this.selectSaveAccountRno1.delete(rno);
    }
  }
  refreshCheckedStatus(): void {
    this.checked = this.AccountDetailTableData.every(({ rno }) => this.selectSaveAccountRno.has(rno));
    this.indeterminate = this.AccountDetailTableData.some(({ rno }) => this.selectSaveAccountRno.has(rno)) && !this.checked;
  }
  refreshCheckedStatus1(): void {
    this.checked1 = this.QueryDetailTableData.every(({ rno }) => this.selectSaveAccountRno1.has(rno));
    this.indeterminate1 = this.QueryDetailTableData.some(({ rno }) => this.selectSaveAccountRno1.has(rno)) && !this.checked1;
  }
  onAllChecked(checked: boolean): void {
    this.AccountDetailTableData.forEach(({ rno }) => this.updateCheckedSet(rno, checked));
    this.refreshCheckedStatus();
  }
  onAllChecked1(checked: boolean): void {
    this.QueryDetailTableData.forEach(({ rno }) => this.updateCheckedSet1(rno, checked));
    this.refreshCheckedStatus1();
  }
  onCurrentPageDataChange(AccountDetailTableData: AccountForm[]): void {
    this.AccountDetailTableData = AccountDetailTableData;
    this.refreshCheckedStatus();
  }
  onCurrentPageDataChange1(QueryDetailTableData: AccountQueryForm[]): void {
    this.QueryDetailTableData = QueryDetailTableData;
    this.refreshCheckedStatus1();
  }
  addItem(): void {
    this.isSpinning = true;
    this.getCompanyData();
    this.QueryDetailTableData = [];
    this.QueryDetailTableData = [...this.QueryDetailTableData];
    this.showModal = true;
  }
  handleCancel(): void {
    // this.QueryAccountList(true);
    this.showModal = false;
  }
  handleOk(): void {
    this.isSaveLoading = true;
    var queryParam = {
      company: this.queryForm.controls.companyCode.value,
      cemplid: this.queryForm.controls.accountant.value,
      bank: this.queryForm.controls.banks.value,
      rno: Array.from(this.selectSaveAccountRno1)
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveAccountList, queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 6000 });
        } else {
          this.modal.success({
            nzTitle: this.translate.instant('submit-successfully'),
            nzContent: `<p>Request NO: ${res.body.data}</p>`,
            nzOnOk: () => this.QueryAccountList(true)
          });
          this.showModal = false;
        }
      }
      else if (res?.status !== 200) {
        this.message.error(res.message, { nzDuration: 6000 })
      }
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }
  QueryDetailList(initial: boolean = false): void {
    if (this.queryForm.controls.companyCode.value == null || this.queryForm.controls.companyCode.value == '') {
      this.message.warning(this.translate.instant('tips.select-company-first'), { nzDuration: 5000 });
      return;
    }
    if (initial) {
      this.selectSaveAccountRno1.clear();
      this.pageIndex1 = 1;
      this.pageSize1 = 10;
    }
    this.isQueryLoading = true;
    var queryParam = {
      pageIndex: this.pageIndex1,
      pageSize: this.pageSize1,
      data: {
        companyList: this.queryForm.controls.companyCode.value == '' ? this.companyList.filter(o => o != '') : [this.queryForm.controls.companyCode.value],
        cemplid: this.queryForm.controls.accountant.value,
        bank: this.queryForm.controls.banks.value,
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCanAccountList, queryParam).subscribe((res) => {
      this.QueryDetailTableData = [];
      if (res && res.status === 200 && res.body != null) {
        this.QueryDetailTotal = res.body.total;
        if (res.body.total == 0) {
          this.message.success(this.translate.instant('noData'), { nzDuration: 5000 });
        } else {
          res.body.data?.map(i => {
            this.QueryDetailTableData.push({
              rno: i.rno,
              formname: i.formname,
              cuser: i.cuser,
              cname: i.cname,
              cdate: format(new Date(i.cdate), "yyyy/MM/dd"),
              amount: i.actamt,
              currency: i.currency,
              company: i.company,
              signAccount: i.cemplid,
              bank: i.bank,
              apid: i.apid
            })
          });
        }

        this.QueryDetailTableData = [...this.QueryDetailTableData];
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }
  QueryAccountList(initial: boolean = false) {
    if (!this.accountListForm.valid) {
      Object.values(this.accountListForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('exist-invalid-field'), { nzDuration: 6000 });
      return;
    }
    this.isQueryLoading = true;
    if (initial) {
      this.selectSaveAccountRno.clear();
      this.pageIndex = 1;
      this.pageSize = 10;
    }
    var queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        carryno: this.accountListForm.controls.rno.value,
        acctant: this.accountListForm.controls.accountant.value,
        startpostdate: this.accountListForm.controls.startDate.value,
        endpostdate: this.accountListForm.controls.endDate.value,
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetAccountList, queryParam).subscribe((res) => {
      this.AccountDetailTableData = [];
      if (res && res.status === 200 && res.body != null) {
        this.AccountDetailTotal = res.body.total;
        if (res.body.total == 0) {
          this.message.success(this.translate.instant('noData'), { nzDuration: 5000 });
        } else {
          res.body.data?.map(i => {
            this.AccountDetailTableData.push({
              rno: i.carryno,
              date: format(new Date(i.postdate), "yyyy/MM/dd"),
              signAccount: i.acctantanme
            })
          });
        }

        this.AccountDetailTableData = [...this.AccountDetailTableData];
        this.isSpinning = false;
        this.isQueryLoading = false;
      }
    });
  }
  DownloadFile(item) {
    this.Service.Download(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DownloadAccountList + `?carryno=${item.rno}`, {}, `${item.rno}_AccountList.xlsx`);
  }
  skipForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    this.router.navigate([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'py001' } })
  }

}

export interface AccountQueryForm {
  rno: string;
  formname: string;
  cuser: string;
  cname: string;
  cdate: string;
  amount: string;
  currency: string;
  company: string;
  signAccount: string;
  bank: string;
  apid: string;
}
export interface AccountForm {
  rno: string;
  date: string;
  signAccount: string;
}