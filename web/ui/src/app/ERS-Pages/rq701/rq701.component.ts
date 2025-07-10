import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  UntypedFormBuilder,
  UntypedFormControl,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import {
  NzShowUploadList,
  NzUploadChangeParam,
  NzUploadFile,
  UploadFilter,
} from 'ng-zorro-antd/upload';
import { SalaryInfo } from './classes/data-item';
import { SalaryTableColumn } from './classes/table-column';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { get } from 'http';

@Component({
  selector: 'app-rq701',
  templateUrl: './rq701.component.html',
  styleUrls: ['./rq701.component.scss'],
})
export class RQ701Component implements OnInit, OnDestroy {
  navigationSubscription;
  //#region 参数
  nzFilterOption = (input, option) => {
    if (option.nzLabel.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    let data = this.sceneList.filter((o) => o.expcode == option.nzValue)[0];
    let keyword = data?.keyword;
    if (!!keyword && keyword.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    let expname = data?.expname;
    if (!!expname && expname.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    return false;
  };
  screenWidth: any;
  headForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyCodeList: any[] = [];
  companyList: any[] = [];
  bankList: any[] = [];
  paymentList: any[] = [];
  projectCodeList: any[] = [];
  sceneList: any[] = [];
  curr: string;
  isaccount: boolean = false;
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  listTableColumn = SalaryTableColumn;
  listTableData: SalaryInfo[] = [];
  listOfAction = ['Delete'];
  keyId: number = 0;
  uid: number = 0;
  isSpinning = false;
  attachmentList: NzUploadFile[] = [];
  previewImage: string | undefined = '';
  previewVisible = false;
  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
  isFirstLoading: boolean = true;
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  applicantInfo: any;
  userInfo: any;
  sceneKeyword: string = '';
  userIdList: string[] = [];
  expname: string = '';
  cuser: string;
  batchUploadModal: boolean = false;
  batchUploadList: NzUploadFile[] = [];
  sampleUrl: string = null;
  sampleName: string = null;
  currList: any[] = [];
  exchangeRate = 1;
  //#endregion

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    // private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private commonSrv: CommonService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    public domSanitizer: DomSanitizer
  ) {
    this.navigationSubscription = this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.dataInitial();
      }
    });
  }

  ngOnInit(): void {
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.sampleUrl = '../../../assets/file/salary-upload-sample.xlsx';
    this.sampleName = this.translate.instant('sample.batch.salary');
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.headForm = this.fb.group({
      emplid: [null],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      payee: [{ value: null, disabled: true }],
      ext: [null],
      projectCode: [null],
      companyCode: [[null, Validators.required]],
      appliedTotal: [{ value: 0, disabled: true }],
      actualTotal: [{ value: 0, disabled: true }],
      totalAmount: [{ value: 0, disabled: true }],
    });
    this.listForm = this.fb.group({
      company: [null, [Validators.required]],
      sceneCode: [null, [Validators.required]],
      feeDate: [null, [this.dateValidator]],
      salaryMonth: [null, [Validators.required]],
      bank: [null, [Validators.required]],
      requestPayment: [null, [Validators.required]],
      curr: [{ value: null }, [Validators.required]],
      appliedAmount: [null, [Validators.required]],
      toLocalAmt: [null],
      exchangeRate: [null],
      digest: [null],
      disabled: [false],
      id: [this.keyId],
    });

    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
    this.columnSubscribe();
    this.getRnoInfo();
    this.getCompanyData();
    this.getCurrency();
    this.getPaymentList();
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
  columnSubscribe() {
    this.headForm.controls.emplid.valueChanges.subscribe((value) => {
      if (
        !!value &&
        this.headForm.controls.emplid.enabled &&
        !this.isSpinning
      ) {
        this.isSpinning = true;
        this.commonSrv.getEmployeeInfoById(value).subscribe((res) => {
          this.applicantInfo = res;
          this.getEmployeeInfo();
          this.isSpinning = false;
        });
      }
    });
    this.headForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (!!value && this.headForm.controls.companyCode.enabled) {
        this.headForm.controls.projectCode.reset();
        this.getSceneList();
        this.getCompanyList();
        this.getBankList();
      }
    });
    this.listForm.controls.sceneCode.valueChanges.subscribe((value) => {
      if (!!value) {
        let data = this.sceneList.filter((o) => o.expcode == value)[0];
        this.expname = data?.expname;
        if (!!data) {
          this.sceneKeyword = data.keyword;
        } else {
          this.sceneKeyword = '';
        }
      } else {
        this.expname = '';
      }
    });
    this.headForm.controls.actualTotal.valueChanges.subscribe((value) => {
      if (!!value) {
        let appliedTotal = Number(this.headForm.controls.appliedTotal.value);
        if (value > appliedTotal) {
          this.message.error(
            this.translate.instant('can-not-exceed-applied-amount')
          );
          this.headForm.controls.actualTotal.reset(appliedTotal);
        } else {
          this.headForm.controls.totalAmount.reset(value);
        }
      }
    });
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  excelFilters: UploadFilter[] = [
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
          this.message.error(this.translate.instant('file-format-erro-excel'), {
            nzDuration: 6000,
          });
          return filterFiles;
        }
        return fileList;
      },
    },
  ];

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyCodeList = [];
      this.projectCodeList = [];
      this.companyList = [];
      this.bankList = [];
      this.paymentList = [];
      this.sceneList = [];
      this.curr = null;
      this.isaccount = false;
      this.showModal = false;
      this.isSaveLoading = false;
      this.listTableData = [];
      this.keyId = 0;
      this.uid = 0;
      this.exchangeRate = 1;
      this.isSpinning = false;
      this.attachmentList = [];
      this.previewImage = '';
      this.previewVisible = false;
      this.checked = false;
      this.addloading = false;
      this.editloading = false;
      this.deleteloading = false;
      this.indeterminate = false;
      this.listOfCurrentPageData = [];
      this.setOfCheckedId = new Set<number>();
      this.ngOnInit();
    }
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-past-date'),
    },
  };

  dateValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { error: true, required: true };
    } else if (control.value < new Date().setHours(0, 0, 0, 0)) {
      return { date: true, error: true };
    }
  };

  getRnoInfo() {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        if (!!data.rno) {
          let rno: string = this.crypto.decrypt(data.rno);
          this.headForm.controls.rno.setValue(rno);
          // 获取单据信息
          this.Service.Post(
            this.EnvironmentconfigService.authConfig.ersUrl +
              URLConst.GetFormData,
            { rno: rno }
          ).subscribe((res) => {
            if (res != null && res.status === 200 && !!res.body) {
              if (res.body.status == 1) {
                this.isSpinning = true;
                this.assembleFromData(res.body.data);
                this.isSpinning = false;
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
      } else {
        this.getEmployeeInfo();
      }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    let attachmentList = value.file;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.applicantInfo = {
        company: headData.company,
        emplid: headData.payeeId,
        deptid: headData.deptid,
        cname: headData.payeename,
        phone: headData.ext,
        curr: headData.currency,
        costdeptid: headData.costdeptid,
        isaccount: true,
      };
      this.cuser = headData.cuser;
      this.getEmployeeInfo();
      if (headData.projectcode != null) {
        this.projectCodeList.push(headData.projectcode);
        this.headForm.controls.projectCode.setValue(headData.projectcode);
      }
    }
    if (summaryAtmData != null) {
      this.headForm.controls.appliedTotal.setValue(summaryAtmData.amount);
      this.headForm.controls.actualTotal.setValue(summaryAtmData.actamt);
      this.headForm.controls.totalAmount.setValue(summaryAtmData.actamt);
    }

    if (listData != null) {
      listData.map((o) => {
        let selfTaxAmt = 0;
        o.invList
          .filter((i) => i.undertaker == 'self')
          .map((i) => {
            selfTaxAmt += i.taxloss;
          });
        this.listTableData.push({
          id: o.seq,
          sceneCode: o.expcode,
          sceneName: o.expname,
          company: o.companycode,
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          salaryMonth: o.summary,
          curr: o.currency,
          appliedAmount: o.amount1,
          toLocalAmt: o.baseamt,
          exchangeRate: o.rate,
          bank: o.bank,
          digest: o.remarks,
          requestPayment: o.paytyp,
          requestPaymentName: o.payname,
          disabled: false,
        });
      });

      // 组装文件
      attachmentList.map((o) => {
        this.attachmentList.push({
          uid: o.item.toString(),
          name: o.filename,
          filename: o.tofn,
          status: 'done',
          url: this.commonSrv.changeDomain(o.url),
          type: o.filetype,
        });
      });
      this.keyId = this.listTableData.sort((a, b) => b.id - a.id)[0].id;

      if (this.listTableData.length > 0) {
        this.headForm.controls.emplid.disable({ emitEvent: false });
        this.headForm.controls.companyCode.disable({ emitEvent: false });
      }
      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
      this.attachmentList = [...this.attachmentList];
    }
    this.isSpinning = false;
    this.attachmentList.map(async (o) => {
      o.originFileObj = await this.commonSrv.getFileData(
        o.url,
        o.filename,
        o.type
      );
    });
  }

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

  getEmployeeInfo() {
    if (!this.applicantInfo) {
      this.applicantInfo = this.userInfo;
    }
    this.userIdList = this.commonSrv.getUserInfo?.proxylist;
    if (this.userIdList.indexOf(this.applicantInfo.emplid) == -1) {
      this.userIdList.push(this.applicantInfo.emplid);
    }
    this.headForm.controls.companyCode.setValue(this.applicantInfo.company);
    this.headForm.controls.emplid.setValue(this.applicantInfo.emplid);
    this.headForm.controls.dept.setValue(this.applicantInfo.deptid);
    this.headForm.controls.payee.setValue(this.applicantInfo.cname);
    this.headForm.controls.ext.setValue(this.applicantInfo.phone);
    this.curr = this.applicantInfo.curr;
    this.isaccount = this.applicantInfo.isaccount;
    this.resetFormData(true); // 首次加載頁面
  }

  getCompanyData() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetCompanyByArea,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.companyCodeList = res.body.data;
          if (
            !this.companyCodeList.includes(
              this.headForm.controls.companyCode.value
            )
          ) {
            this.headForm.controls.companyCode.setValue('');
          } else {
            this.getCompanyList();
          }
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

  getCompanyList() {
    this.Service.Post(
      URLConst.GetCompanyList +
        `?company=${this.headForm.controls.companyCode.value}`,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.companyList = res.body.data;
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

  getProjectCode(value) {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    if (value.length > 0) {
      const params = {
        code: value,
        company: this.headForm.controls.companyCode.value,
      };
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl +
          URLConst.GetProjectCode,
        params
      ).subscribe((res) => {
        if (res && res.status === 200) {
          this.projectCodeList = res.body.map(
            (item) => `${item.code}-${item.description}`
          );
        } else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
    } else this.projectCodeList = [];
  }

  getPaymentList() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetPaymentList,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.paymentList = res.body.data; // payType,payName
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

  resetFormData(first: boolean = false) {
    this.expname = '';
    if (first) {
      this.listForm.controls.curr.setValue(this.curr);
    } else {
      this.keyId++;
      this.listForm.reset({ disabled: false, id: this.keyId, curr: this.curr });
    }
  }

  getSceneList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.Service.doGet(
      URLConst.GetSalarySceneList +
        `?company=${this.headForm.controls.companyCode.value}`,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.sceneList = res.body.data;
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

  getBankList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.Service.doGet(
      URLConst.GetBankList +
        `?company=${this.headForm.controls.companyCode.value}`,
      null
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.bankList = res.body.data;
      } else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
      this.isSpinning = false;
    });
  }

  ////////带选择框表
  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: SalaryInfo[] = [];
  setOfCheckedId = new Set<number>();
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: SalaryInfo[]): void {
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

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData
      .filter(({ disabled }) => !disabled)
      .forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }

  clickBatchUpload(): void {
    if (this.userInfo.isMobile) {
      this.message.info(this.translate.instant('only-pc'), {
        nzDuration: 6000,
      });
      return;
    }
    // if (!this.userInfo.isaccount) { this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 }); return; }
    this.batchUploadList = [];
    this.batchUploadModal = true;
  }

  handleBatchUpload(): void {
    this.isSaveLoading = true;
    this.isSpinning = true;
    const formData = new FormData();
    this.batchUploadList
      .filter((o) => !o.upload)
      .forEach((file: any) => {
        formData.append('excelFile', file.originFileObj);
      });
    formData.append('company', this.headForm.controls.companyCode.value);
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.BatchUploadSalary,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        let dataList = res.body.data?.map((o) => {
          // let seq = this.
          return {
            company: o.companycode,
            sceneCode: o.expinfo?.expcode,
            sceneName: o.expinfo?.description,
            feeDate: format(new Date(o.reqdate), 'yyyy/MM/dd'),
            salaryMonth: o.salarydate,
            bank: o.bank,
            requestPayment: o.payway?.paytype,
            requestPaymentName: o.payway?.payname,
            curr: o.currency,
            appliedAmount: o.amount,
            digest: o.remarks,
            id: ++this.keyId,
            disabled: false,
          };
        });
        if (dataList.length > 0) {
          this.listTableData = this.listTableData.concat(dataList);
          this.listTableData = [...this.listTableData];
          this.setStatistic();
        }
        this.addloading = false;
        this.editloading = false;
        this.batchUploadModal = false;
        this.showModal = false;
      } else
        this.message.error(
          this.translate.instant('operate-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error')),
          { nzDuration: 6000 }
        );
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  addItem(): void {
    this.addloading = true;
    // if (!this.isaccount) {
    //   this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
    //   this.addloading = false;
    //   return;
    // }
    this.resetFormData();
    this.showModal = true;
  }

  editRow(id: number): void {
    this.editloading = true;
    let rowFormData = this.listTableData.filter((d) => d.id == id)[0];
    this.listForm.reset(rowFormData);
    this.showModal = true;
    this.editloading = false;
  }

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {
      //多选操作
      this.listTableData = this.listTableData.filter(
        (d) => !this.setOfCheckedId.has(d.id)
      );
      this.setOfCheckedId.clear();
    } else {
      this.listTableData = this.listTableData.filter((d) => d.id != id);
      this.setOfCheckedId.delete(id);
    }
    this.setStatistic();
    this.deleteloading = false;
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
      this.message.error(this.translate.instant('fill-in-form'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let amount = this.listForm.controls.appliedAmount.value;
    if (amount == 0) {
      this.message.error(this.translate.instant('amount-zero-error'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    // this.exchangeRate = 1;
    // if (this.curr != this.listForm.controls.curr.value) {
    //   this.Service.doGet(
    //     this.EnvironmentconfigService.authConfig.ersUrl +
    //       URLConst.GetExchangeRate,
    //     { ccurfrom: this.listForm.controls.curr.value, ccurto: this.curr }
    //   ).subscribe((res) => {
    //     if (res && res.status === 200) {
    //       this.exchangeRate = res.body;
    //       this.setListTableData();
    //     } else {
    //       this.message.error(
    //         this.translate.instant('operate-failed') +
    //           this.translate.instant('server-error'),
    //         { nzDuration: 6000 }
    //       );
    //       this.exchangeRate = 0;
    //     }
    //   });
    // } else this.setListTableData();
    this.setListTableData();
  }

  setListTableData(): void {
    // let amount = this.listForm.controls.appliedAmt.value;
    // this.listForm.controls.exchangeRate.setValue(this.exchangeRate);
    // this.listForm.controls.toLocalAmt.setValue(
    //   Number((amount * this.exchangeRate).toFixed(2))
    // );
    let feeDType = typeof this.listForm.controls.feeDate.value;
    if (this.listForm.controls.feeDate.value != null && feeDType != 'string') {
      let feeDate =
        this.listForm.controls.feeDate.value == null
          ? null
          : format(this.listForm.controls.feeDate.value, 'yyyy/MM/dd');
      this.listForm.controls.feeDate.setValue(feeDate);
    }
    let sceneData = this.sceneList.filter(
      (o) => o.expcode == this.listForm.controls.sceneCode.value
    )[0];
    let sceneName = sceneData.description; // 存报销情景
    let data = this.listForm.getRawValue();
    data['sceneName'] = sceneName;
    data['requestPaymentName'] = this.paymentList
      .filter((t) => t.payType == data.requestPayment)
      .map((t) => t.payName)[0];

    let rowId = this.listForm.controls.id.value;
    this.listTableData = this.listTableData.filter((o) => o.id != rowId);
    // 增加摘要组装
    let param = {
      summary: data.salaryMonth,
      keyword: sceneData.keyword,
      payname: data.requestPayment == 'T' ? data.bank : data.requestPaymentName,
      companycode: data.company,
    };
    this.Service.Post(URLConst.AssembleDigest, param).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          data.digest = res.body.data;
          this.listTableData.push(data);
          this.listTableData = [...this.listTableData]; // 刷新表格
          this.setStatistic();
          this.showModal = false;
          this.isSaveLoading = false;
          this.addloading = false;
          this.editloading = false;
          this.isSpinning = false;
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

  setStatistic(): void {
    if (this.listTableData.length > 0) {
      this.headForm.controls.emplid.disable({ emitEvent: false });
      this.headForm.controls.companyCode.disable({ emitEvent: false });
    } else {
      this.headForm.controls.emplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
    }
    let appliedTotal = 0;
    this.listTableData.map((o) => {
      appliedTotal += Number(o.appliedAmount);
    });
    this.headForm.controls.appliedTotal.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
    this.headForm.controls.actualTotal.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    // 上传bpm & 批量文件
    return new Observable((observer: Observer<boolean>) => {
      let uploadedFile;
      if (!this.batchUploadModal) {
        uploadedFile = this.attachmentList.filter(
          (o) => o.originFileObj.name == file.name
        );
      } else {
        uploadedFile = this.batchUploadList.filter(
          (o) => o.originFileObj.name == file.name
        );
      }
      let upload = uploadedFile.length == 0;
      if (!upload) {
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('has-been-uploaded-that'),
            uploadedFile[0].originFileObj.name,
            uploadedFile[0].name
          ),
          { nzDuration: 6000 }
        );
      } else {
        upload = !(this.batchUploadList.length > 0 && this.batchUploadModal);
        if (!upload) {
          this.message.error(
            this.translate.instant('can-upload-only-one-item'),
            { nzDuration: 6000 }
          );
        }
      }
      observer.next(upload);
      observer.complete();
    });
  };

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    if (this.batchUploadModal) {
      this.batchUploadList = fileList;
    } else {
      this.attachmentList = fileList;
    }
  }

  checkParam(): boolean {
    // if (!this.isaccount) {
    //   this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
    //   this.isSpinning = false;
    //   this.isSaveLoading = false;
    //   return false;
    // }
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
    if (this.listTableData.length == 0) {
      this.message.error(this.translate.instant('fill-in-detail'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.attachmentList.length == 0) {
      this.message.error(this.translate.instant('required-for-attachment'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    return true;
  }

  submit(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;

    let formData = this.SetParam();
    // 提交表單
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq701,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(this.translate.instant('submit-successfully'));
        this.commonSrv.SendMobileSignXMLData(res.body.data.rno);
        this.modal.info({
          nzTitle: this.translate.instant('tips'),
          nzContent: `<p>Request NO: ${res.body.data.rno}</p>`,
          nzOnOk: () => this.router.navigateByUrl(`ers/rq701`), // reset form data
        });
      } else
        this.message.error(
          this.translate.instant('submit-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error')),
          { nzDuration: 6000 }
        );
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  save(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;
    let formData = this.SetParam();
    //暫存表單
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq701,
      formData
    ).subscribe((res) => {
      // this.Service.Post('http://localhost:5000' + URLConst.SaveRq701, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(
          this.translate.instant('save-successfully') +
            `Request NO: ${res.body.data.rno}`
        );
        this.headForm.controls.rno.setValue(res.body.data.rno);
        if (!this.cuser) {
          this.cuser = this.userInfo?.emplid;
        }
      } else
        this.message.error(
          this.translate.instant('save-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error')),
          { nzDuration: 6000 }
        );
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  SetParam(): FormData {
    const formData = new FormData();
    //#region 组装数据
    let rno = this.headForm.controls.rno.value;
    let headData = {
      rno: rno,
      cname: this.headForm.controls.payee.value,
      deptid: this.headForm.controls.dept.value,
      ext: this.headForm.controls.ext.value,
      company: this.headForm.controls.companyCode.value,
      projectcode: this.headForm.controls.projectCode.value,
      currency: this.curr,
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payee.value,
    };
    formData.append('head', JSON.stringify(headData));

    let listData = this.listTableData.map((o) => {
      let sceneData = this.sceneList.filter((i) => i.expcode == o.sceneCode)[0];
      return {
        rno: rno,
        seq: o.id,
        expcode: o.sceneCode,
        expname: o.sceneName,
        companycode: o.company,
        rdate: o.feeDate,
        summary: o.salaryMonth,
        currency: o.curr,
        basecurr: this.curr,
        amount1: o.appliedAmount,
        rate: 1,
        baseamt: o.appliedAmount,
        amount: o.appliedAmount,
        acctcode: sceneData?.acctcode,
        acctname: sceneData?.acctname,
        bank: o.bank,
        remarks: o.digest,
        paytyp: o.requestPayment,
        payname: o.requestPaymentName,
      };
    });
    formData.append('detail', JSON.stringify(listData));
    formData.append('inv', JSON.stringify([]));

    let indx = 0;
    let attachmentData = this.attachmentList.map((o) => {
      return {
        rno: rno,
        seq: 0,
        item: indx++,
        filetype: o.type,
        filename: o.name,
        ishead: 'Y',
        key: o.uid,
      };
    });
    formData.append('file', JSON.stringify(attachmentData));

    let amountData = {
      rno: rno,
      currency: this.curr,
      amount: this.headForm.controls.appliedTotal.value,
      actamt: this.headForm.controls.actualTotal.value,
    };
    formData.append('amount', JSON.stringify(amountData));

    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
  }
}
