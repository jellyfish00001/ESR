import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BDInfoTableColumn } from './classes/table-column';
import { NicknameInfo } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bd-customer-nickname',
  templateUrl: './bd-customer-nickname.component.html',
  styleUrls: ['./bd-customer-nickname.component.scss']
})

export class BDCustomerNicknameComponent implements OnInit {
  navigationSubscription;

  //#region 参数
  nzFilterOption = () => true;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  listTableColumn = BDInfoTableColumn;
  listTableData: NicknameInfo[] = [];
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
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']);
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth = window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.queryForm = this.fb.group({
      company: [''],
      name: [null]
    });
    this.listForm = this.fb.group({
      id: [null],
      company: [null, [Validators.required]],
      nickname: [null, [Validators.required]],
      name: [null, [Validators.required]],
      iscarry: [null, [Validators.required]],
    });
    this.getEmployeeInfo();
    this.getCompanyData();

    this.queryForm.valueChanges.subscribe(value => {
      this.showTable = false;
    });
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    }
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
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
    this.isQueryLoading = true;
    let paramValue = this.queryForm.getRawValue();
    if (initial) {
      this.pageIndex = 1;
      this.pageSize = 10;
      this.setOfCheckedId.clear();
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        companyList: paramValue.company == '' ? this.companyList.filter(o => o != '') : [paramValue.company],
        name: paramValue.name
      }
    }
    this.Service.Post(URLConst.QueryBdNickname, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: NicknameInfo[] = [];
        res.body.data?.map(o => {
          result.push({
            id: o.id,
            company: o.company,
            nickname: o.nickname,
            name: o.name,
            iscarry: o.iscarry,
            cuser: o.cuser,
            cdate: o.cdate == null ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
            muser: o.muser,
            mdate: o.mdate == null ? null : format(new Date(o.mdate), "yyyy/MM/dd"),
            disabled: false,
          })
        });
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  addRow(): void {
    this.addloading = true;
    this.listForm.reset({ iscarry: 'N' });
    if (!this.listForm.controls.company.enabled) this.listForm.controls.company.enable();
    this.showModal = true;
  }

  editRow(item): void {
    this.isSpinning = true;
    this.editloading = true;
    this.companyList = [];
    this.companyList.push(item.company);
    this.listForm.reset(item);
    if (this.listForm.controls.company.enabled) this.listForm.controls.company.disable();
    this.isSpinning = false;
    this.showModal = true;
  }

  handleOk(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach(control => {
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
      company: listFormData.company,
      nickname: listFormData.nickname,
      name: listFormData.name,
      iscarry: listFormData.iscarry
    }
    if (!this.editloading) this.addItem(params);
    else this.editItem(params);
  }

  addItem(params: any) {
    params.Id = null;
    this.Service.Post(URLConst.MaintainBdNickname, params).subscribe((res) => {
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
      this.isSaveLoading = false;
      this.isSpinning = false;
    });
  }

  editItem(params: any) {
    this.Service.Put(URLConst.MaintainBdNickname, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
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

  deleteRow(id: string = null): void {
    this.deleteloading = true;
    let params: any = [];
    if (id == null) {   //多选操作
      params = Array.from(this.setOfCheckedId);
      this.setOfCheckedId.clear();
    }
    else {
      params.push(id);
      this.setOfCheckedId.delete(id);
    }
    this.Service.Delete(URLConst.MaintainBdNickname, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.deleteloading = false;
    });
  }

  ////////带选择框表
  checked = false;
  indeterminate = false;
  listOfCurrentPageData: NicknameInfo[] = [];
  setOfCheckedId = new Set<string>();
  updateCheckedSet(id: string, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: NicknameInfo[]): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
    this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
    this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
  }

  onItemChecked(id: string, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }
}