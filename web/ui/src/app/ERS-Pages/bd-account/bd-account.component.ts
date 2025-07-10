import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService } from '@ngx-translate/core';
import { format } from 'date-fns';
import { TableColumnModel } from 'src/app/shared/models';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';


@Component({
  selector: 'app-bd-account',
  templateUrl: './bd-account.component.html',
  styleUrls: ['./bd-account.component.scss']
})
export class BdAccountComponent implements OnInit {
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  isSpinning = false;
  total: any;
  companyList: any[] = [];
  isQueryLoading = false;
  isEdit = false;
  pageIndex: number = 1;
  pageSize: number = 10;
  queryParam: any;
  index: number = 0;
  addloading = false;
  listTableData: detailInfo[] = [];
  showModal: boolean = false;
  showTable = false;
  userInfo: any;
  //表格
  bdaccountInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('accounting-subject-code'),
      columnKey: 'acctcode',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.acctcode.localeCompare(b.acctcode),
    },
    {
      title: this.translate.instant('accounting-subject-name'),
      columnKey: 'acctname',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.acctname.localeCompare(b.acctname)
    },
    {
      title: this.translate.instant('accounting-cuser'),
      columnKey: 'cuser',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('accounting-cdate'),
      columnKey: 'cdate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.cdate.localeCompare(b.cdate),
    },
    {
      title: this.translate.instant('muser'),
      columnKey: 'muser',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.muser.localeCompare(b.muser),
    },
    {
      title: this.translate.instant('mdate'),
      columnKey: 'mdate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.mdate.localeCompare(b.mdate),
    },
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.company.localeCompare(b.company),
    }
  ];
  listTableColumn = this.bdaccountInfoTableColumn;

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    public translate: TranslateService,
    private message: NzMessageService,
    private authService: AuthService,
    private router: Router,
    private commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin', 'FinanceAdmin']);
    this.userInfo = this.commonSrv.getUserInfo;
    this.isSpinning = true;
    this.queryForm = this.fb.group({
      acctcode: [null],
      acctname: [null],
      companyCode: ['']
    });
    this.listForm = this.fb.group({
      company: [null, [Validators.required]],
      acctcode: [null, [Validators.required]],
      acctname: [null, [Validators.required]]
    })
    this.getCompanyData();
  }

  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
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
        acctcode: paramValue.acctcode,
        acctname: paramValue.acctname,
        companyList: paramValue.companyCode == '' ? this.companyList.filter(o => o != '') : [paramValue.companyCode],
      }
    }
    this.Service.Post(URLConst.QueryBDAccountList, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: detailInfo[] = [];
        this.index = 0;
        if (!!res.body.data) {
          res.body.data.map(o => {
            result.push({
              index: ++this.index,
              acctcode: o.acctcode,
              acctname: o.acctname,
              company: o.company,
              cuser: o.cuser,
              cdate: !o.cdate ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
              muser: o.muser,
              mdate: !o.mdate ? null : format(new Date(o.mdate), "yyyy/MM/dd")
            })
          });
        }
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.isEdit = false;
  }

  handleOk(): void {
    this.isSpinning = true;
    this.isQueryLoading = true;
    if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-inform'));
      this.isSpinning = false;
      this.isQueryLoading = false;
      return;
    }
    let listFormData = this.listForm.getRawValue();
    var param = {
      acctcode: listFormData.acctcode,
      acctname: listFormData.acctname,
      company: listFormData.company,
    }
    if (!this.isEdit) this.addItem(param);
    else this.editItem(param);
    this.isEdit = false;
  }

  addItem(params: any) {
    this.Service.Post(URLConst.AddEditDeleteBDAccount, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam(true);
          this.showModal = false;
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
    })
  }

  editItem(params: any) {
    this.Service.Put(URLConst.AddEditDeleteBDAccount, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
      this.showModal = false;
    })
  }

  addRow() {
    this.showModal = true;
    this.listForm.reset();
    this.listForm.controls.company.enable();
    this.listForm.controls.acctcode.enable();

  }

  deleteRow(item: detailInfo) {
    this.isQueryLoading = true;
    this.Service.Delete(URLConst.AddEditDeleteBDAccount, item).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.isQueryLoading = false;
    });
  }

  editRow(item: detailInfo) {
    this.isSpinning = true;
    this.isEdit = true;
    this.listForm.reset(item);
    this.listForm.controls.company.disable();
    this.listForm.controls.acctcode.disable();
    this.isSpinning = false;
    this.showModal = true;
  }

  DownloadFile() {
    let paramValue = this.queryForm.getRawValue();
    // var data = {
    //   acctcode: paramValue.acctcode,
    //   acctname: paramValue.acctname,
    //   company: paramValue.companyCode,
    // };
    // this.Service.Download(URLConst.DownloadBDAccount, data, `BD05.xlsx`);
    this.Service.doPost(URLConst.DownloadBDAccount, this.queryParam, true).subscribe((res) => {
      this.DownloadFlow(res, `bd-account.xlsx`);
    });
  }

  DownloadFlow(flow, name) {
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
}

export interface detailInfo {
  index: number;
  acctcode: string;
  acctname: string;
  cuser: string;
  cdate: string;
  muser: string;
  mdate: string;
  company: string;
}
