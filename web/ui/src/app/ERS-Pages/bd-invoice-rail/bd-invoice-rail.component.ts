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
import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BDInfoTableColumn } from './classes/table-column';
import { BdInvoiceRail } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';
import {
  NzShowUploadList,
  NzUploadChangeParam,
  NzUploadFile,
  UploadFilter,
} from 'ng-zorro-antd/upload';
import { Observable, Observer } from 'rxjs';
@Component({
  selector: 'app-bd-invoice-rail',
  templateUrl: './bd-invoice-rail.component.html',
  styleUrls: ['./bd-invoice-rail.component.scss'],
})
export class BdInvoiceRailComponent implements OnInit {
  navigationSubscription;

  //#region 参数
  nzFilterOption = () => true;
  screenWidth: any;
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  currList: any[] = [];
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  listTableColumn = BDInfoTableColumn;
  listTableData: BdInvoiceRail[] = [];
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
  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
  attachmentList = [];
  sampleName: string = this.translate.instant('payment-upload-sample');
  sampleUrl: string = '../../../assets/file/payment.xlsx';
  batchUploadList: NzUploadFile[] = [];
  batchUploadModal: boolean = false;
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
  ) {}

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']);
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.queryForm = this.fb.group({
      invoicerail: null,
      year: null,
      month: null,
      invoicetype: null,
    });
    this.listForm = this.fb.group({
      id: [null],
      invoicerail: [null, [Validators.required]],
      year: [null, [Validators.required]],
      month: [1, [Validators.required]],
      invoicetype: [null, [Validators.required]],
    });
    this.getEmployeeInfo();

    this.queryForm.valueChanges.subscribe((value) => {
      this.showTable = false;
    });
    this.isSpinning = false;
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    },
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithParam();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithParam();
  }
  filters: UploadFilter[] = [
    {
      name: 'type',
      fn: (fileList: NzUploadFile[]) => {
        const filterFiles = fileList.filter(
          (w) =>
            ~['application/vnd.ms-excel'].indexOf(w.type) ||
            ~[
              'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            ].indexOf(w.type)
        );
        if (filterFiles.length !== fileList.length) {
          this.message.error(this.translate.instant('file-format-erro-excel'));
          return filterFiles;
        }
        return fileList;
      },
    },
  ];
  handleFileChange(info: NzUploadChangeParam): void {
    // 批量上传
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.batchUploadList = fileList;
  }
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
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    // 上传批量文件
    return new Observable((observer: Observer<boolean>) => {
      let upload = !(this.batchUploadList.length > 0);
      if (!upload)
        this.message.error(this.translate.instant('can-upload-only-one-item'));
      observer.next(upload);
      observer.complete();
    });
  };
  handleBatchUpload() {
    this.addloading = true;
    const formData = new FormData();
    this.batchUploadList.forEach((file: any) => {
      formData.append('excelFile', file.originFileObj);
    });
    this.Service.Post(URLConst.BatchUploadBDInvoiceRail, formData).subscribe(
      (res) => {
        console.log(res, 'res');
        if (res && res.status === 200 && res.body != null) {
          if (res.body.status == 1) {
            let successList = res.body.data;
            let tips = this.commonSrv.FormatString(
              this.translate.instant('tips-success-upload-info'),
              successList.length.toString()
            );
            this.message.success(
              this.translate.instant('save-successfully') + tips
            );
            if (successList.length > 0) {
              this.queryResultWithParam(true);
            }
            this.batchUploadModal = false;
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
        this.isQueryLoading = false;
        this.isSpinning = false;
      }
    );
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
        invoicerail: paramValue.invoicerail,
        year: paramValue.year && paramValue.year != '' ? paramValue.year : 0,
        month:
          paramValue.month && paramValue.month != '' ? paramValue.month : 0,
        invoicetype: paramValue.invoicetype,
      },
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.QueryBDInvoiceRail,
      this.queryParam
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: BdInvoiceRail[] = [];
        res.body.data?.map((o) => {
          result.push({
            id: o.id,
            qi: o.qi,
            invoicerail: o.invoicerail,
            year: o.year,
            month: o.month,
            formatcode: o.formatcode,
            invoicetype: o.invoicetype,
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

  clickBatchUpload(): void {
    this.batchUploadList = [];
    this.batchUploadModal = true;
  }
  beforeAttachUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let upload = this.attachmentList.length > 0;
      if (upload) {
        this.message.error(this.translate.instant('can-upload-only-one-item'));
      }
      observer.next(!upload);
      observer.complete();
    });
  };
  handleAttachChange(info: NzUploadChangeParam): void {
    let attachList = [...info.fileList];
    if (attachList.length > 1) return;
    attachList = attachList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    // this.uploadForm.controls.attachmentList.setValue(attachList);
    this.attachmentList = [...attachList];
  }
  addRow(): void {
    this.addloading = true;
    // this.listForm.reset({ curr: this.userInfo.curr });
    // if (!this.listForm.controls.companyCode.enabled)
    //   this.listForm.controls.companyCode.enable();
    this.showModal = true;
  }

  editRow(item): void {
    this.isSpinning = true;
    this.editloading = true;
    this.listForm.reset(item);
    // if (this.listForm.controls.companyCode.enabled)
    //   this.listForm.controls.companyCode.disable();
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
      company: listFormData.companyCode,
      companycode: listFormData.company,
      companysap: listFormData.sapCompanyCode,
      companydesc: listFormData.companyDesc,
      stwit: listFormData.abbr,
      basecurr: listFormData.curr,
      identificationcode: listFormData.taxpayerNo,
      taxcode: listFormData.taxRate,
      area: listFormData.area,
      timezone: listFormData.timezone,
    };
    if (!this.editloading) this.addItem(params);
    else this.editItem(params);
  }

  addItem(params: any) {
    params.Id = null;
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.AddBd008,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          if (this.companyList.indexOf(params.company) == -1) {
            this.companyList.push(params.company);
            this.companyList = this.companyList.sort((a, b) =>
              a.localeCompare(b)
            );
          }
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.OperateBd008,
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

  deleteRow(id: string = null): void {
    this.deleteloading = true;
    let params: any = [];
    if (id == null) {
      //多选操作
      params = this.listTableData.filter((d) => this.setOfCheckedId.has(d.id));
      this.setOfCheckedId.clear();
    } else {
      params = this.listTableData.filter((d) => d.id == id);
      this.setOfCheckedId.delete(id);
    }
    this.Service.Delete(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.OperateBd008,
      params
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
          // this.getCompanyData();
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
  listOfCurrentPageData: BdInvoiceRail[] = [];
  setOfCheckedId = new Set<string>();
  updateCheckedSet(id: string, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: BdInvoiceRail[]): void {
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
