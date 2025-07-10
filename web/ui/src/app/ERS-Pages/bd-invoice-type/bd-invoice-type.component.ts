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
import { InvTypeInfo } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bd-invoice-type',
  templateUrl: './bd-invoice-type.component.html',
  styleUrls: ['./bd-invoice-type.component.scss'],
})
export class BdInvoiceTypeComponent implements OnInit {
  navigationSubscription;

  //#region 参数
  nzFilterOption = () => true;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  invTypeList: any[] = [];
  areaList: any[] = [];
  categoryList: any[] = [{ "value": "FP", "label": this.translate.instant('invoice') }, { "value": "SJ", "label": this.translate.instant('receipt') }, { "value": "CP", "label": this.translate.instant('ticket') }];
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  listTableColumn = BDInfoTableColumn;
  listTableData: InvTypeInfo[] = [];
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
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin', 'FinanceAdmin']);
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.queryForm = this.fb.group({
      company: [''],
      invtype: [null],
      category: [null],
      area: [null],
    });
    this.listForm = this.fb.group({
      id: [null],
      company: [null, [Validators.required]],
      invtypecode: [null, [Validators.required]],
      invtype: [null, [Validators.required]],
      category: [null, [Validators.required]],
      area: [null, [Validators.required]],
    });
    this.getEmployeeInfo();
    this.getCompanyData();
    this.getAreaList();
    this.queryForm.valueChanges.subscribe((value) => {
      this.showTable = false;
    });
  }
  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    },
  };
  getAreaList() {
    const queryParam = {
      pageIndex: 1,
      pageSize: 10,
      data: { category: 'area' },
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryDataDictionary,
      queryParam
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        let result: any[] = [];
        res.body.data?.map((o) => {
          result.push({
            value: o.value,
            label: o.name,
          });
        });
        this.areaList = result;
      }
    });
  }
  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
  }

  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
      this.isSpinning = false;
      this.getInvTypeList(res[0]);
    });
  }

  getInvTypeList(companyParam: string) {
    let company = this.queryForm.controls.company.value;
    if (!company) company = companyParam;
    this.Service.doGet(
      URLConst.GetInvTypeByCompany + `?company=${company}`,
      null
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.invTypeList = res.body.data?.map((o) => {
          return { invCode: o.invcode, invType: o.invtype, company: o.company };
        });
      } else {
        this.message.error(res.message);
      }
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
        companyList:
          paramValue.company == ''
            ? this.companyList.filter((o) => o != '')
            : [paramValue.company],
        invtype: paramValue.invtype,
        category: paramValue.category,
        area: paramValue.area,
      },
    };
    this.Service.Post(URLConst.QueryInvType, this.queryParam).subscribe(
      (res) => {
        if (res && res.status === 200 && res.body != null) {
          this.total = res.body.total;
          let result: InvTypeInfo[] = [];
          res.body.data?.map((o) => {
            let categoryLabel = '';
            if (o.category == 'FP') {
              categoryLabel = this.translate.instant('invoice');
            } else if (o.category == 'SJ') {
              categoryLabel = this.translate.instant('receipt');
            } else if (o.category == 'CP') {
              categoryLabel = this.translate.instant('ticket');
            }
            result.push({
              id: o.id,
              company: o.company,
              invtypecode: o.invtypecode,
              invtype: o.invtype,
              category: categoryLabel,
              area: o.area,
              cuser: o.cuser,
              cdate:
                o.cdate == null
                  ? null
                  : format(new Date(o.cdate), 'yyyy/MM/dd'),
              muser: o.muser,
              mdate:
                o.mdate == null
                  ? null
                  : format(new Date(o.mdate), 'yyyy/MM/dd'),
              disabled: false,
            });
          });
          this.listTableData = result;
          this.showTable = true;
          this.isQueryLoading = false;
        }
      }
    );
  }

  addRow(): void {
    this.addloading = true;
    this.listForm.reset();
    if (!this.listForm.controls.company.enabled)
      this.listForm.controls.company.enable();
    if (!this.listForm.controls.invtypecode.enabled)
      this.listForm.controls.invtypecode.enable();
    this.showModal = true;
  }

  editRow(item): void {
    this.isSpinning = true;
    this.editloading = true;
    this.companyList = [];
    this.companyList.push(item.company);
    this.listForm.reset(item);
    if (this.listForm.controls.company.enabled)
      this.listForm.controls.company.disable();
    if (this.listForm.controls.invtypecode.enabled)
      this.listForm.controls.invtypecode.disable();
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
      Id: listFormData.id,
      company: listFormData.company,
      invtypecode: listFormData.invtypecode,
      invtype: listFormData.invtype,
      category: listFormData.category,
      area: listFormData.area,
    };
    if (!this.editloading) this.addItem(params);
    else this.editItem(params);
  }

  addItem(params: any) {
    params.Id = null;
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.AddInvType,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam(true);
          this.showModal = false;
          this.getInvTypeList('');
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
      this.EnvironmentconfigService.authConfig.ersUrl +
      URLConst.MaintainInvType,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam();
          this.getInvTypeList('');
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

  deleteRow(id: string = null): void {
    this.deleteloading = true;
    let params: any = [];
    if (id == null) {
      //多选操作
      params = Array.from(this.setOfCheckedId);
      this.setOfCheckedId.clear();
    } else {
      params.push(id);
      this.setOfCheckedId.delete(id);
    }
    this.Service.Delete(
      this.EnvironmentconfigService.authConfig.ersUrl +
      URLConst.MaintainInvType,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
          this.getInvTypeList('');
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

  ////////带选择框表
  checked = false;
  indeterminate = false;
  listOfCurrentPageData: InvTypeInfo[] = [];
  setOfCheckedId = new Set<string>();
  updateCheckedSet(id: string, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: InvTypeInfo[]): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const listOfEnabledData = this.listOfCurrentPageData.filter(
      ({ disabled }) => !disabled
    );
    this.checked = listOfEnabledData.every(({ id }) =>
      this.setOfCheckedId.has(id)
    );
    this.indeterminate =
      listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) &&
      !this.checked;
  }

  onItemChecked(id: string, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData
      .filter(({ disabled }) => !disabled)
      .forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }
}
