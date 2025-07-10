import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  UntypedFormBuilder,
  FormControl,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TableColumnModel } from 'src/app/shared/models';
import { ChargeAgainstAdvanceFundInfo } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-finance-advance-payment-clearance',
  templateUrl: './finance-advance-payment-clearance.component.html',
  styleUrls: ['./finance-advance-payment-clearance.component.scss'],
})
export class FinanceAdvancePaymentClearanceComponent implements OnInit {
  navigationSubscription;
  bdInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company-code'),
      columnKey: 'companyCode',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: this.translate.instant('advance-fund-no'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('charge-against-amount'),
      columnKey: 'amount',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.amount - b.amount,
    },
    {
      title: this.translate.instant('create-user'),
      columnKey: 'creator',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.creator.localeCompare(b.creator),
    },
    {
      title: this.translate.instant('create-date'),
      columnKey: 'createDate',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.createDate.localeCompare(b.createDate),
    },
    {
      title: this.translate.instant('update-user'),
      columnKey: 'updateUser',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.updateUser.localeCompare(b.updateUser),
    },
    {
      title: this.translate.instant('update-date'),
      columnKey: 'updateDate',
      columnWidth: '',
      align: 'center',
      sortFn: (
        a: ChargeAgainstAdvanceFundInfo,
        b: ChargeAgainstAdvanceFundInfo
      ) => a.updateDate.localeCompare(b.updateDate),
    },
  ];

  //#region 参数
  nzFilterOption = () => true;
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  queryCompanyList: any[] = [];
  companyList: any[] = [];
  rnoList: any[] = [];
  curr: string;
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  listTableColumn = this.bdInfoTableColumn;
  listTableData: ChargeAgainstAdvanceFundInfo[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  showTable = false;
  isQueryLoading = false;
  isFirstLoading: boolean = true;
  addloading = false;
  editloading = false;
  deleteloading = false;
  //#endregion

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private commonSrv: CommonService,
  ) {}

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']); //'fin'
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.queryForm = this.fb.group({
      companyCode: [''],
      rno: [null],
    });
    this.listForm = this.fb.group({
      id: [null],
      companyCode: [null, [Validators.required]],
      rno: [null, [Validators.required]],
      amount: [null, [Validators.required]],
    });
    this.getEmployeeInfo();
    this.getCompanyData();
    this.queryForm.valueChanges.subscribe((value) => {
      this.showTable = false;
    });
    this.listForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (!!value && this.addloading) {
        if (!!this.listForm.controls.rno.value)
          this.listForm.controls.rno.reset();
        this.getRnoList('');
      }
    });
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-past-date'),
      overrange: this.translate.instant('can-not-earlier-a-month'),
    },
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    this.curr = this.userInfo.curr;
  }

  getCompanyData() {
    this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
      this.isSpinning = false;
    });
  }

  getRnoList(value) {
    if (!this.listForm.controls.companyCode.value)
      this.message.error(this.translate.instant('tips.select-company-first'));
    else {
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl +
          URLConst.GetAdvanceRnoList,
        {
          word: value.trim(),
          company: this.listForm.controls.companyCode.value,
        }
      ).subscribe((res) => {
        if (res && res.status === 200 && !!res.body) {
          if (res.body.status == 1) {
            this.rnoList = res.body.data;
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
      Object.values(this.queryForm.controls).forEach((control) => {
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
    let rnoList = [];
    if (paramValue.rno != null && paramValue.rno.trim() != '') {
      rnoList.push(paramValue.rno.trim());
      this.queryForm.controls.rno.reset(paramValue.rno.trim());
    } else this.queryForm.controls.rno.reset();
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        company:
          paramValue.companyCode == ''
            ? this.companyList.filter((o) => o != '')
            : [paramValue.companyCode],
        rno: rnoList,
      },
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryBd001,
      this.queryParam
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: ChargeAgainstAdvanceFundInfo[] = [];
        res.body.data?.map((o) => {
          result.push({
            id: o.id,
            companyCode: o.company,
            rno: o.rno,
            amount: o.amount,
            creator: o.cuser,
            createDate:
              o.cdate == null ? null : format(new Date(o.cdate), 'yyyy/MM/dd'),
            updateUser: o.muser,
            updateDate:
              o.mdate == null ? null : format(new Date(o.mdate), 'yyyy/MM/dd'),
            disabled: false,
          });
        });
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  addRow(): void {
    this.addloading = true;
    this.listForm.reset();
    this.listForm.controls.companyCode.enable();
    this.listForm.controls.rno.enable();
    this.rnoList = [];
    this.showModal = true;
  }

  editRow(item): void {
    this.isSpinning = true;
    this.editloading = true;
    this.rnoList = [];
    this.listForm.reset(item);
    this.rnoList.push(item.rno);
    this.listForm.controls.companyCode.disable();
    this.listForm.controls.rno.disable();
    this.isSpinning = false;
    this.showModal = true;
  }

  handleOk(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let listFormData = this.listForm.getRawValue();
    let params = {
      id: listFormData.id,
      company: listFormData.companyCode,
      rno: listFormData.rno,
      amount: listFormData.amount,
      cuser: this.userInfo.emplid,
      cdate: new Date(),
      mdate: new Date(),
    };
    if (!this.editloading) this.addItem(params);
    else this.editItem(params);
  }

  addItem(params: any) {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.AddBd001,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam(true);
          this.showModal = false;
        } else
          this.message.error(
            this.translate.instant('save-failed') +
              (res.body.message == null ? '' : res.body.message)
          );
      } else
        this.message.error(
          this.translate.instant('operate-failed') +
            this.translate.instant('server-error')
        );
      this.addloading = false;
      this.isSaveLoading = false;
      this.isSpinning = false;
    });
  }

  editItem(params: any) {
    this.Service.Put(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.EditBd001,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam();
        } else
          this.message.error(
            this.translate.instant('save-failed') +
              (res.body.message == null ? '' : res.body.message)
          );
      } else
        this.message.error(
          this.translate.instant('operate-failed') +
            this.translate.instant('server-error')
        );
      this.editloading = false;
      this.isSaveLoading = false;
      this.isSpinning = false;
      this.showModal = false;
    });
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  deleteRow(params: any): void {
    this.deleteloading = true;
    this.Service.Delete(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.DeleteBd001,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
        } else
          this.message.error(
            this.translate.instant('operate-failed') +
              (res.body.message == null ? '' : res.body.message)
          );
      } else
        this.message.error(
          this.translate.instant('operate-failed') +
            this.translate.instant('server-error')
        );
      this.deleteloading = false;
    });
  }
}
