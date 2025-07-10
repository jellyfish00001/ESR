import { VenderInfo } from './../supplier-info/classes/data-item';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators, UntypedFormArray } from '@angular/forms';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TableColumnModel } from 'src/app/shared/models';
import { format } from 'date-fns';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { Observable, Observer, Subscription, firstValueFrom } from 'rxjs';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ExcelTableColumn, InvoiceTableColumn } from './classes/table-column';
import { Cash5ExcelDto } from './classes/data-item';
import { InvoiceDetail } from './../_components/invoices-modal/classes/data-item';

@Component({
  selector: 'app-rq801',
  templateUrl: './rq801.component.html',
  styleUrls: ['./rq801.component.scss']
})
export class RQ801Component implements OnInit, OnDestroy{
  navigationSubscription;
  constructor(
        private EnvironmentconfigService: EnvironmentconfigService,
        private authService: AuthService,
        private commonSrv: CommonService,
        private message: NzMessageService,
        public translate: TranslateService,
        private fb: UntypedFormBuilder,
        private Service: WebApiService,
        private router: Router,
  ) {
    this.navigationSubscription = this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.dataInitial();
      }
    });
  }

  userInfo: any;

  screenWidth: any;
  isSpinning = false;
  isFirstLoading: boolean = true;
  addloading:boolean = false;
  isQueryLoading: boolean = false;
  isSaveLoading: boolean = false;

  headForm: UntypedFormGroup; //使用者輸入
  excelData: Cash5ExcelDto[]=[]; //excel通過檢查回傳
  invoiceData: InvoiceDetail[] = []; //發票資料
  attachfileData: any[] = []; //附件資料

  detailForm: UntypedFormGroup;

  excelColumn = ExcelTableColumn;
  invoiceColumn = InvoiceTableColumn

  fileList: any[] = []; //發票檔案
  attachmentList: NzUploadFile[] = [];

  batchUploadModal: boolean = false;
  batchUploadList: NzUploadFile[] = [];
  previewImage: string | undefined = ''; //預覽檔案
  previewVisible = false;
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  

  companyList: any[] = [];
  selectedCompany: string = '';
  vendorList: any[] = [];
  selectedVendor:string = '';
  projectCodeList: any[] = [];

  ngOnInit(): void {
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth = window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.userInfo = this.commonSrv.getUserInfo;
    
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }

    this.headForm = this.fb.group({
      rno: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      applicantDept: [{ value: null, disabled: true }],
      applicantExt: [{ value: null, disabled: true }],
      projectCode: [ null],
      companyCode: [null, [Validators.required]],
      reimbursemenType: [null, [Validators.required]],
      requestAmount: [null, [Validators.required]],
      vendor: [ null],
      paymentDate: [ null],
    });

    // 監聽報銷類型的變化
    this.headForm.get('reimbursemenType').valueChanges.subscribe(value => {
      this.updateValidators(value);
    });
    this.updateValidators(this.headForm.get('reimbursemenType').value);

    //this.excelForm = this.fb.group({
    //  items: this.fb.array([
    //    this.fb.group({
    //       senarioname: [null], // 費用類別
    //       deptid: [null], // 費用歸屬部門
    //       agentemplid: [null], // 承辦人
    //       unifycode: [null], // 廠商統一編號
    //       billnoandsummary: [null], // 提單號碼/費用摘要
    //       reportno: [null], // 報單號碼
    //       invoice: [null], // 稅單號碼/發票號碼/憑證號碼
    //       rdate: [null], // 稅單付訖日期/發票日期/憑證日期
    //       importtax: [null], // 進口稅/其他費用
    //       tradepromotionfee: [null], // 推貿費
    //       taxexpense: [null], // 營業稅
    //       totaltaxandfee: [null], // 稅費合計
    //       taxbaseamount: [null] // 稅基
    //    })
    //  ])
    //});

    this.excelColumn = ExcelTableColumn;

    this.getApplicant();
    this.getCompanyData();
    this.getVendorData();
  }

  updateValidators(reimbursemenType: string) {
    const vendorControl = this.headForm.get('vendor');  //廠商
    const paymentDateControl = this.headForm.get('paymentDate'); //付款日期

    if (reimbursemenType === '2') {
      // 當 reimbursemenType 為 '2' 時，添加必填驗證
      vendorControl.setValidators([Validators.required]);
      paymentDateControl.setValidators([Validators.required]);
    } else {
      // 當 reimbursemenType 不為 '2' 時，移除驗證並清空值
      vendorControl.setValidators(null);
      paymentDateControl.setValidators(null);
      vendorControl.setValue(null);
      paymentDateControl.setValue(null);
    }

    // 更新控制項的驗證狀態
    vendorControl.updateValueAndValidity();
    paymentDateControl.updateValueAndValidity();
  }

  ngOnDestroy(): void {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  getApplicant() {
    this.headForm.controls.applicantEmplid.setValue(this.userInfo?.emplid); //申请人
    this.headForm.controls.applicantName.setValue(this.userInfo?.cname); //申请人姓名
    this.headForm.controls.applicantDept.setValue(this.userInfo?.deptid); //申请人部门
    this.headForm.controls.applicantExt.setValue(this.userInfo?.phone); //申请人分机
  }

  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
      this.isSpinning = false;
    });
  }

  getVendorData() {
    if (this.vendorList.length == 0) {
      this.Service.doGet(URLConst.GetAllVendors, null).subscribe((res) => {
        if (res && res.status === 200) {

          this.vendorList = res.body.data?.map(o => { return { unifyCode: o.unifyCode, vendorName: o.venderName, vendorCode: o.venderCode } });
        }
        else { this.message.error(res.message); }
      });
    }
  }

  updateSelectedVendor() {
    this.selectedVendor = '';
    if (this.headForm.controls.vendor.value) {
      this.vendorList.forEach((o) => {
        if (o.VendorCode == this.headForm.controls.vendor.value) {
          this.selectedVendor = o.VendorCode;
          return;
        }
      });
    }
  }

  updateSelectedArea() {
    this.selectedCompany = '';
    if (this.headForm.controls.companyCode.value) {
      this.companyList.forEach((o) => {
        if (o == this.headForm.controls.companyCode.value) {
          this.selectedCompany = o;
          return;
        }
      });
    }
  }

  uploadFile(){
    if(this.checkInput()){
      this.batchUploadList = [];
      this.batchUploadModal = true;
    }
  }

  checkInput():boolean{
    if(!this.headForm.controls.companyCode.value){
      this.message.error(this.translate.instant('tips.select-company-first'));
      return false;
    }

    if(!this.headForm.controls.reimbursemenType.value){
      this.message.error(this.translate.instant('NeedReimbursementType'));
      return false;
    }

    if(this.headForm.controls.reimbursemenType.value == 2) {
      if(!this.headForm.controls.vendor.value){
        this.message.error(this.translate.instant('NeedVandor'));
        return false;
      }
      if(!this.headForm.controls.paymentDate.value){
        this.message.error(this.translate.instant('NeedRDate'));
        return false;
      }
    }

    if(!this.headForm.controls.requestAmount.value){
      this.message.error(this.translate.instant('NeedPayment'));
      return false
    }

    return true;
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
    if (file.type.indexOf('image') !== -1) {
      if (!file.preview && file.url == '...') {
        file.preview = await this.commonSrv.getPicBase64(file.originFileObj!);
      }
      this.previewImage = file.url == '...' ? file.preview : file.url;
      this.previewVisible = true;
    } else if (file.type.indexOf('application/pdf') !== -1) {
      file.safeUrl = this.commonSrv.getFileUrl(file.originFileObj);
      this.frameSrc = file.safeUrl;
      this.drawerVisible = true;
    } else {
      const objectUrl = URL.createObjectURL(file.originFileObj);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.setAttribute('style', 'display:none');
      a.setAttribute('href', objectUrl);
      a.setAttribute('download', file.name);
      a.click();
      URL.revokeObjectURL(objectUrl);
    }
  };

  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {   // 上传批量文件
    return new Observable((observer: Observer<boolean>) => {
      // file.uid = (++this.uid).toString();
      let uploadedFile;
      
      uploadedFile = this.attachmentList.filter(
        (o) => o.originFileObj?.name == file.name
      );
      
      let upload = uploadedFile.length == 0;
      if (!upload) {
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('has-been-uploaded-that'),
            uploadedFile[0].originFileObj?.name,
            uploadedFile[0].name
          ),
          { nzDuration: 6000 }
        );
      }
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

  handleChange(info: NzUploadChangeParam): void {
      let fileList = [...info.fileList];
      fileList = fileList.map((file) => {
        file.status = 'done';
        if (!file.url) file.url = '...';
        return file;
      });
      this.attachmentList = fileList;
    }

  async handleBatchUpload() {
    this.addloading = true;
    const formData = new FormData();
    this.batchUploadList.forEach((file: any) => {
      formData.append('excelFile', file.originFileObj);
      formData.append('company', this.headForm.controls.companyCode.value);
      formData.append('totalAmount', this.headForm.controls.requestAmount.value);
      formData.append('userId', this.headForm.controls.applicantEmplid.value);
    });
    //上傳大量報銷Excel
    const res = await firstValueFrom(this.Service.Post(URLConst.BatchUploadCash5Excel, formData));
    if (res && res.status === 200 && res.body != null) {
      if (res.body.status == 1) {
        this.excelData = res.body.data?.map(o => ({
          ...o,
          disabled: false
        })) ?? [];
        this.message.success(this.translate.instant('operate-successfully'));
        this.batchUploadModal = false;
      }
      else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
    }
    else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
    this.addloading = false;
    this.isQueryLoading = false;
    this.isSpinning = false;
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-future-date'),
    },
  };

  addInvoice(value): void {
    console.log('invoicelist = ',this.invoiceData);
    value.forEach((o) => {
      this.invoiceData.push(o);
    });
    console.log("InvoiceData", this.invoiceData)
    //this.setDetailTableShowData();
  }

  checkParam(): boolean {
    //檢查銀行帳號
    if (!this.userInfo?.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }

    //檢查申請者手動輸入資訊
    if (!this.headForm.valid) {
      Object.values(this.headForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }

    //檢查是否有上傳大量報銷資料
    if (this.excelData.length == 0) {
      var errormsg = this.commonSrv.FormatString(
        this.translate.instant('tips.attach-required'),//請上傳{0}檔案
        this.translate.instant('many-reimbursement')//大量報銷
      )

      this.message.error(errormsg, {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }

    //檢查發票資料
    if (this.invoiceData.length == 0) {
      this.message.error(this.translate.instant('add-invoice-required'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }


    return true;
  }
  
  save(){
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;

    this.isSpinning = false;
    this.isSaveLoading = false;
  }

  submit(){

  }

  //get items(): UntypedFormArray {
  //  return this.excelForm.get('items') as UntypedFormArray;
  //}

  //private populateFormArray(dataList: any[]) {
  //  this.items.clear();
  //  dataList.forEach((data) => {
  //    this.items.push(this.fb.group({
  //      senarioname: [data.senarioname ?? null],
  //      deptid: [data.deptid ?? null],
  //      agentemplid: [data.agentemplid ?? null],
  //      unifycode: [data.unifycode ?? null],
  //      billnoandsummary: [data.billnoandsummary ?? null],
  //      reportno: [data.reportno ?? null],
  //      invoice: [data.invoice ?? null],
  //      rdate: [data.rdate ? new Date(data.rdate) : null],
  //      importtax: [data.importtax ?? null],
  //      tradepromotionfee: [data.tradepromotionfee ?? null],
  //      taxexpense: [data.taxexpense ?? null],
  //      totaltaxandfee: [data.totaltaxandfee ?? null],
  //      taxbaseamount: [data.taxbaseamount ?? null]
  //    }));
  //  });
  //}

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyList = [];
      this.ngOnInit();
    }
  }

}
