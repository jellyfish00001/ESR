import { Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { TableColumnModel } from 'src/app/shared/models';
import { Observable, Observer, Subscription } from 'rxjs';


@Component({
  selector: 'app-bdexpensedept',
  templateUrl: './bdexpensedept.component.html',
  styleUrls: ['./bdexpensedept.component.scss']
})
export class BdexpensedeptComponent {
  //頁面狀態
  isSpinning = false;
  showModal: boolean = false;
  isQueryLoading = false;
  isSaveLoading: boolean = false;
  addloading = false;
  deleteloading = false;
  listOfCurrentPageData: any[] = [];
  setOfCheckedId = new Set<string>();
  showTable = false;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  checked = false;
  indeterminate = false;
  screenWidth: any;

  //資料和form相關
  userInfo: any;
  queryParam: any;
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  listTableData: ExpenseDeptInfo[] = [];
  batchUploadList: NzUploadFile[] = [];
  batchUploadModal: boolean = false;

  //表格設定
  bdaccountInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '30%',
      align: 'center',
      sortFn: (a: ExpenseDeptInfo, b: ExpenseDeptInfo) =>
        a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('dept-code'),
      columnKey: 'deptid',
      columnWidth: '30%',
      align: 'center',
      sortFn: (a: ExpenseDeptInfo, b: ExpenseDeptInfo) =>
        a.deptid.localeCompare(b.deptid)
    },
    {
      title: this.translate.instant('isVirtualDept'),
      columnKey: 'isvirtualdept',
      columnWidth: '30%',
      align: 'center',
      sortFn: (a: ExpenseDeptInfo, b: ExpenseDeptInfo) =>
        a.isvirtualdept.localeCompare(b.isvirtualdept),
    }
  ];
  listTableColumn = this.bdaccountInfoTableColumn;

  constructor(
      private fb: UntypedFormBuilder,
      private Service: WebApiService,
      private EnvironmentconfigService: EnvironmentconfigService,
      public translate: TranslateService,
      private modal: NzModalService,
      private crypto: CryptoService,
      private router: Router,
      private message: NzMessageService,
      private authService: AuthService,
      private commonSrv: CommonService,
    ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']);
    this.isSpinning = true;
    this.userInfo = this.commonSrv.getUserInfo;
    this.screenWidth = window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.init();
    this.isSpinning = false;
  }

  init() {
    this.queryForm = this.fb.group({
      company: [null],
      deptid: [null]
    });

    this.listForm = this.fb.group({
      company: [null, [Validators.required]],
      deptid: [null, [Validators.required]],
      isvirtualdept: ['N']
    })
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
        company: paramValue.company,
        deptid: paramValue.deptid,
      }
    }

    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryBDExpenseDeptList, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          this.total = res.body.total;
          let result: ExpenseDeptInfo[] = [];
          res.body.data?.map(o => {
              result.push({
                  id: o.id,
                  company: o.company,
                  deptid: o.deptid,
                  isvirtualdept: o.isvirtualdept,
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
    this.listForm.reset();
    this.listForm.get('isvirtualdept')?.setValue('N');//預設為N
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
    if (listFormData.company == 0 && (listFormData.company == null || listFormData.company == '')){
        this.message.error('Please input company');
        this.isSpinning = false;
        this.isSaveLoading = false;
        return;
    }

    if (listFormData.deptid == 0 && (listFormData.deptid == null || listFormData.deptid == '')){
      this.message.error('Please input department');
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
  }

    let params = {
        company: listFormData.company,
        deptid: listFormData.deptid,
        isvirtualdept: listFormData.isvirtualdept==null ? 'N' : listFormData.isvirtualdept,
    };

    this.addItem(params);
  }

  addItem(params: any) {
    params.Id = null;
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.AddEditDeleteQueryBDExpenseDeptList, params).subscribe((res) => {
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

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
  }

  deleteRow(id: string=null): void {
    this.isQueryLoading = true;

    let params: any = [];
    if (id == null) {   //多选操作
        params = Array.from(this.setOfCheckedId);
        this.setOfCheckedId.clear();
    }
    else {
        params.push(id);
        this.setOfCheckedId.delete(id);
    }
    this.Service.Delete(URLConst.AddEditDeleteQueryBDExpenseDeptList, params).subscribe((res) => {
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

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithParam();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithParam();
  }

  onCurrentPageDataChange(listOfCurrentPageData: ExpenseDeptInfo[]): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
      const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
      this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
      this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
  }

  updateCheckedSet(id: string, checked: boolean): void {
    if (checked) {
        this.setOfCheckedId.add(id);
    } else {
        this.setOfCheckedId.delete(id);
    }
  }

  onItemChecked(id: string, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }

  DownloadFile() {
    let paramValue = this.queryForm.getRawValue();
        this.Service.doPost(URLConst.DownloadBDExpenseDept, this.queryParam, true).subscribe((res) => {
      this.DownloadFlow(res, `ExpenseDept.xlsx`);
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
    DownloadTemplate() {
    this.Service.doPost(URLConst.DownloadBDExpenseDeptTemp, null, true).subscribe((res) => {
      if (res.size > 0) {
        const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const objectUrl = URL.createObjectURL(blob);
        const a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display:none');
        a.setAttribute('href', objectUrl);
        a.setAttribute('download', "upload-expensedept-sample.xlsx");
        a.click();
        URL.revokeObjectURL(objectUrl);
      }
    });
  }
    UploadFile() {
    this.batchUploadList = [];
    this.batchUploadModal = true;
  }
  filters: UploadFilter[] = [
    {
      name: 'type',
      fn: (fileList: NzUploadFile[]) => {
        const filterFiles = fileList.filter(w =>
          ~['application/vnd.ms-excel'].indexOf(w.type) ||
          ~['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'].indexOf(w.type));
        if (filterFiles.length !== fileList.length) {
          this.message.error(this.translate.instant('file-format-erro-excel'));
          return filterFiles;
        }
        return fileList;
      }
    }
  ];
    handlePreview = async (file: NzUploadFile): Promise<void> => {
    const objectUrl = URL.createObjectURL(file.originFileObj);
    const a = document.createElement('a');
    document.body.appendChild(a);
    a.setAttribute('style', 'display:none');
    a.setAttribute('href', objectUrl);
    a.setAttribute('download', file.name);
    a.click();
    URL.revokeObjectURL(objectUrl);
  };
    clickBatchUpload(): void {
    this.batchUploadList = [];
    this.batchUploadModal = true;
  }
    beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {   // 上传批量文件
    return new Observable((observer: Observer<boolean>) => {
      let upload = !(this.batchUploadList.length > 0);
      if (!upload) this.message.error(this.translate.instant('can-upload-only-one-item'));
      observer.next(upload);
      observer.complete();
    });
  };
    uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
    handleFileChange(info: NzUploadChangeParam): void {   // 批量上传
    let fileList = [...info.fileList];
    fileList = fileList.map(file => {
      file.status = "done";
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.batchUploadList = fileList;
  }
    handleBatchUpload() {
    this.addloading = true;
    const formData = new FormData();
    this.batchUploadList.forEach((file: any) => { formData.append('excelFile', file.originFileObj); });
    this.Service.Post(URLConst.BatchUploadBDExpenseDept, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
          this.batchUploadModal = false;
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
    });
  }
}

export interface ExpenseDeptInfo {
  id: string;
  company: string;
  deptid: string;
  isvirtualdept: string;
  disabled: boolean;
}

export interface ProcessFlowDto{
  rno: string;
  approverEmplid: string;
  inviteEmplid?: string;
  inviteMethod?: number;
  comment: string;

}
