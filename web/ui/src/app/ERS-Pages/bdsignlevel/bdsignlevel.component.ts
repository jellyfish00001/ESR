import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { DomSanitizer } from '@angular/platform-browser';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { TableColumnModel } from 'src/app/shared/models';
import { format } from 'date-fns';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { Observable, Observer, Subscription } from 'rxjs';
@Component({
  selector: 'app-bdsignlevel',
  templateUrl: './bdsignlevel.component.html',
  styleUrls: ['./bdsignlevel.component.scss']
})
export class BdsignlevelComponent implements OnInit {
  isSpinning = true;
  public bpmurl1: any;
  public bpmurl2: any;
  userInfo: any;

  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  total: any;
  queryCompanyList: any[] = [""];
  companyList: any[] = [];
  currList: any[] = [];
  treelevelList: treelevelInfo[]=[];
  signlevelSelection: any[] = [];
  isQueryLoading = false;
  isEdit = false;
  pageIndex: number = 1;
  pageSize: number = 10;
  queryParam: any;
  index: number = 0;
  addloading = false;
  deleteloading = false;
  listTableData: detailInfo[] = [];
  screenWidth: any;
  showModal: boolean = false;
  showTestModal:boolean =false;
  batchUploadModal: boolean = false;
  isSaveLoading: boolean = false;
  batchUploadList: NzUploadFile[] = [];
  showTable = false;

  setOfCheckedId = new Set<string>();
  checked = false;
  indeterminate = false;
  listOfCurrentPageData: any[] = [];

  //表格
  bdaccountInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company'),
      columnKey: 'company',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('approval-item'),
      columnKey: 'item',
      columnWidth: '150',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.item.localeCompare(b.item)
    },
    {
      title: this.translate.instant('approval-level'),
      columnKey: 'signlevelLabel',
      columnWidth: '150',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.signlevel.localeCompare(b.signlevel),
    },
    {
      title: this.translate.instant('approval-amount'),
      columnKey: 'money',
      columnWidth: '150',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.money.localeCompare(b.money),
    },
    {
      title: this.translate.instant('currency'),
      columnKey: 'currency',
      columnWidth: '150',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.currency.localeCompare(b.currency),
    },
    {
      title: this.translate.instant('mdate'),
      columnKey: 'mdate',
      columnWidth: '150',
      align: 'center',
      sortFn: (a: detailInfo, b: detailInfo) =>
        a.mdate.localeCompare(b.mdate),
    }
  ];
  listTableColumn = this.bdaccountInfoTableColumn;
  private langChangeSubscription: Subscription; //訂閱this.translate.currentLang的改變
  currentLang:string = this.translate.currentLang

  constructor(
    private sanitizer: DomSanitizer,
    private EnvironmentconfigService: EnvironmentconfigService,
    private router: Router,
    private authService: AuthService,
    private commonSrv: CommonService,
    private message: NzMessageService,
    public translate: TranslateService,
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    private modal: NzModalService,
  ) {
    this.langChangeSubscription = this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        console.log('Language changed to:', event.lang);
        this.loadContentBasedOnLanguage(event.lang);
      });
  }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']);
    this.screenWidth = window.innerWidth < 700 ? window.innerWidth * 0.9 + 'px' : '700px';
    this.userInfo = this.commonSrv.getUserInfo;
    this.isSpinning = false;
    this.currList = [];
    this.queryForm = this.fb.group({
      companyCode: [""]
    });

    this.listForm = this.fb.group({
      company: [null, [Validators.required]],
      item: [null, [Validators.required]],
      signlevel: [null, [Validators.required]],
      //signlevelLabel: [null],
      money: [null, [Validators.required]],
      currency: [null, [Validators.required]]
    })

    this.getCompanyData();
    this.getCurrency();
    this.loadAndProcessData();
  }

  loadContentBasedOnLanguage(lang: string) {
    this.currentLang = lang;
    this.listTableData.forEach((item) => {
      item.signlevelLabel = this.transSignlevelLable(item.signlevel);
    });
  }

  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
      this.queryCompanyList = res;
      this.isSpinning = false;
    });
  }

  getCurrency() {
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl +
          URLConst.GetCurrencyList,
        null
      ).subscribe((res) => {
        if (res && res.status === 200) {
          this.currList = res.body.map((item) => item.currency);
        } else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
  }

  //取得簽核層級對照表
  async getTreelevelData(){
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryBDTreelevelAll, null).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        let result: treelevelInfo[] = [];
        this.index = 0;
        if (!!res.body.data) {
          res.body.data.map(o => {
            result.push({
              levelname: o.levelname,
              leveltwname: o.leveltwname,
              levelcnname: o.levelcnname,
              levelnum: o.levelnum
            })
          });
        }
        this.treelevelList = result;
      }
    });
  }

  async loadAndProcessData() {
    try {
      await this.getTreelevelData();
    } catch (err) {
      console.error('Error loading data', err);
    }
  }

  queryResultWithCompany(initial: boolean = false){
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
      this.setOfCheckedId.clear();
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        companyList: paramValue.companyCode==''
            ? this.companyList.filter((o) => o != '')
            : [paramValue.companyCode],
        language: this.currentLang
      }
    }

    this.Service.Post(URLConst.QueryBDSignlevelList, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: detailInfo[] = [];
        this.index = 0;
        if (!!res.body.data) {
          res.body.data.forEach(o => {
            result.push({
              id: o.id,
              index: ++this.index,
              company: o.company,
              item: o.item,
              signlevel: o.signlevel,
              signlevelLabel: this.transSignlevelLable(o.signlevel),
              money: o.money,
              currency: o.currency,
              muser: o.muser,
              mdate: !o.mdate ? null : format(new Date(o.mdate), "yyyy/MM/dd HH:mm:ss")
            })
          });
        }
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  addRow() {
    this.showModal = true;
    this.listForm.reset();
    this.listForm.controls.company.enable();
    this.listForm.controls.item.enable();
    this.listForm.controls.signlevel.enable();
    this.treelevelList.forEach((item) => {
      this.signlevelSelection.push({
        label: this.transSignlevelLable(item.levelnum),
        value: item.levelnum
      });
    });
  }

  transSignlevelLable(levelnum){
    const treeLevel = this.treelevelList.find(level => level.levelnum === Number(levelnum));
    let levelName = 'unknowSignLevel';
    if (treeLevel) {
      switch (this.currentLang) {
        case 'zh_TW':
          levelName = treeLevel.leveltwname;
          break;
        case 'zh_CN':
          levelName = treeLevel.levelcnname;
          break;
        case 'en':
          levelName = treeLevel.levelname;
          break;
        default:
          levelName = 'unknowSignLevel';
      }
    }
    return levelName !== "unknowSignLevel" ? levelName+" ("+treeLevel.levelnum+")" : this.translate.instant('unknowSignLevel')+" ("+levelnum+")";
  }

  editRow(item: detailInfo) {
    this.isSpinning = true;
    this.isEdit = true;
    this.listForm.reset(item);
    this.listForm.controls.company.disable();
    this.listForm.controls.item.disable();
    this.listForm.controls.signlevel.disable();
    this.signlevelSelection = []
    this.signlevelSelection.push({
      label: this.transSignlevelLable(this.listForm.get('signlevel').value),
      value: this.listForm.get('signlevel').value
    });
    this.isSpinning = false;
    this.showModal = true;
  }

  deleteRow(item: detailInfo=null) {
    this.isQueryLoading = true;

    //多選
    console.log("my:", item)
    let params: any = [];
    if(item == null){
      params = Array.from(this.setOfCheckedId);
      console.log("params:", params)

      this.Service.Delete(URLConst.DeleteByIdBDSignlevelList, params).subscribe((res) => {
        if (res && res.status === 200 && res.body != null) {
          if (res.body.status == 1) {
            this.message.success(this.translate.instant('operate-successfully'));
            this.queryResultWithCompany();
          }
          else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
        }
        else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
        this.isQueryLoading = false;
      });
      return;
    }


    this.Service.Delete(URLConst.AddEditDeleteQueryBDSignlevelList, item).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithCompany();
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.isQueryLoading = false;
    });
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithCompany();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithCompany();
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
      company: listFormData.company,
      item: listFormData.item,
      signlevel: String(listFormData.signlevel),
      money: listFormData.money,
      currency: listFormData.currency
    }
    console.log("Add/Edit: ", param);
    if (!this.isEdit) this.addItem(param);
    else this.editItem(param);
    this.isEdit = false;
  }

  addItem(params: any) {
    this.Service.Post(URLConst.AddEditDeleteQueryBDSignlevelList, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithCompany(true);
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
    console.log("EditItem: ", params);
    this.Service.Put(URLConst.AddEditDeleteQueryBDSignlevelList, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithCompany();
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

  DownloadFile() {
    let paramValue = this.queryForm.getRawValue();
    this.queryParam.data.language = this.currentLang;
    this.Service.doPost(URLConst.DownloadBDSignlevel, this.queryParam, true).subscribe((res) => {
      this.DownloadFlow(res, `Signlevel.xlsx`);
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
    this.Service.doPost(URLConst.DownloadBDSignlevelTemp, null, true).subscribe((res) => {
      if (res.size > 0) {
        const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const objectUrl = URL.createObjectURL(blob);
        const a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display:none');
        a.setAttribute('href', objectUrl);
        a.setAttribute('download', "upload-signlevel-sample.xlsx");
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
    this.Service.Post(URLConst.BatchUploadBDSignlevel, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithCompany();
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

  onCurrentPageDataChange(listOfCurrentPageData: detailInfo[]): void {
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
}

export interface detailInfo {
  id: string;
  index: number;
  company: string;
  item: string;
  signlevel: string;
  signlevelLabel: string;
  money: string;
  currency: string;
  muser: string;
  mdate: string;
}

export interface treelevelInfo {
  levelname: string;
  leveltwname: string;
  levelcnname: string;
  levelnum: number;
}
