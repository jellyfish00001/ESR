import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { OverdueChargeAgainstDetail, AdvanceFundInfo } from './classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Item } from 'angular2-multiselect-dropdown';

@Component({
  selector: 'app-rq401',
  templateUrl: './rq401.component.html',
  styleUrls: ['./rq401.component.scss']
})

export class RQ401Component implements OnInit, OnDestroy {
  navigationSubscription;
  chargeAgainstTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('company-code'),
      columnKey: 'companyCode',
      columnWidth: '70px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: this.translate.instant('applicant-name'),
      columnKey: 'applicantName',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.applicantName.localeCompare(b.applicantName),
    },
    {
      title: this.translate.instant('applicant-emplid'),
      columnKey: 'applicantId',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.applicantId.localeCompare(b.applicantId),
    },
    {
      title: this.translate.instant('payee'),
      columnKey: 'payeeName',
      columnWidth: '85px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.payeeName.localeCompare(b.payeeName),
    },
    {
      title: this.translate.instant('receiver-emplid'),
      columnKey: 'payeeId',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.payeeId.localeCompare(b.payeeId),
    },
    {
      title: this.translate.instant('advance-fund-no'),
      columnKey: 'advanceFundRno',
      columnWidth: '115px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.advanceFundRno.localeCompare(b.advanceFundRno),
    },
    {
      title: this.translate.instant('digest'),
      columnKey: 'digest',
      columnWidth: '100px',
      align: 'left',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.digest.localeCompare(b.digest),
    },
    {
      title: this.translate.instant('col.applied-amount'),
      columnKey: 'appliedAmt',
      columnWidth: '85px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.appliedAmt - b.appliedAmt,
    },
    {
      title: this.translate.instant('not-charge-against-amount'),
      columnKey: 'notChargeAgainstAmt',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.notChargeAgainstAmt - b.notChargeAgainstAmt,
    },
    {
      title: this.translate.instant('open-days'),
      columnKey: 'openDays',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.openDays - b.openDays,
    },
    {
      title: this.translate.instant('delay-times'),
      columnKey: 'delayTimes',
      columnWidth: '85px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) => a.delayTimes - b.delayTimes,
    }
  ];

  AdvanceTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('expname'),
      columnKey: 'advanceSceneName',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) =>
        a.advanceSceneName.localeCompare(b.advanceSceneName),
    },
    {
      title: this.translate.instant('required-payment-date'),
      columnKey: 'requiredPaymentDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.requiredPaymentDate > b.requiredPaymentDate,
    },
    {
      title: this.translate.instant('digest'),
      columnKey: 'digest',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.digest.localeCompare(b.digest),
    },
    {
      title: this.translate.instant('advance-charge-against-date'),
      columnKey: 'advanceDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.advanceDate > b.advanceDate,
    },
    {
      title: this.translate.instant('request-payment'),
      columnKey: 'requestPaymentName',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.requestPaymentName.localeCompare(b.requestPaymentName),
    },
    {
      title: this.translate.instant('col.currency'),
      columnKey: 'curr',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.curr.localeCompare(b.curr),
    },
    {
      title: this.translate.instant('col.applied-amount'),
      columnKey: 'appliedAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.appliedAmt - b.appliedAmt,
    },
    {
      title: this.translate.instant('col.conversion-to-local-currency'),
      columnKey: 'toLocalAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.toLocalAmt - b.toLocalAmt,
    },
    {
      title: this.translate.instant('col.exchange-rate'),
      columnKey: 'exchangeRate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.exchangeRate - b.exchangeRate,
    },
    {
      title: this.translate.instant('col.remark'),
      columnKey: 'remark',
      columnWidth: '',
      align: 'center',
      sortFn: (a: AdvanceFundInfo, b: AdvanceFundInfo) => a.remark.localeCompare(b.remark),
    },
  ];

  //#region 参数
  nzFilterOption = (input, option) => {
    if (option.nzLabel.toLowerCase().indexOf(input.toLowerCase()) >= 0) { return true; }
    let data = this.advanceSceneList.filter(o => o.expcode == option.nzValue)[0];
    let keyword = data?.keyword;
    if (!!keyword && keyword.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    let expname = data?.expname;
    if (!!expname && expname.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    return false;
  }
  screenWidth: any;
  headForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  projectCodeList: any[] = [];
  currList: any[] = [];
  advanceSceneList: any[] = [];
  paymentList: any[] = [];
  curr: string;
  showModal: boolean = false;
  isSaveLoading: boolean = false;
  showChargeAgainst: boolean = false;
  overdueTableColumn = this.chargeAgainstTableColumn;
  listTableColumn = this.AdvanceTableColumn;
  chargeAgainstTableData: OverdueChargeAgainstDetail[] = [];
  listTableData: AdvanceFundInfo[] = [];
  keyId: number = 0;
  isSpinning = false;
  canUpload = true;
  exchangeRate = 1;
  fileList: NzUploadFile[] = [];
  attachmentList: NzUploadFile[] = [];
  previewImage: string | undefined = '';
  previewVisible = false;
  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
  showPicList: boolean = false;
  isFirstLoading: boolean = true;
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  bpmFlag: boolean = false;
  applicantInfo: any;
  userInfo: any;
  sceneKeyword: string = '';
  userIdList: string[] = [];
  expname: string = '';
  cuser: string;
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
    public domSanitizer: DomSanitizer,
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
    this.screenWidth = window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.headForm = this.fb.group({
      applicantEmplid: [null],
      applicantName: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      ext: [null],
      projectCode: [null],
      companyCode: [null, [Validators.required]],
      payeeEmplid: [{ value: null, disabled: true }],
      payeeName: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      actualAmt: [{ value: 0, disabled: true }],
      totalCost: [{ value: 0, disabled: true }],
    });
    this.listForm = this.fb.group({
      id: [this.keyId],
      advanceScene: [null, [Validators.required]],
      requiredPaymentDate: [null, [this.dateValidator]],
      digest: [null, [Validators.required]],
      advanceDate: [null, [this.dateMonthValidator]],
      requestPayment: [null, [Validators.required]],
      curr: [null, [Validators.required]],
      appliedAmt: [null, [Validators.required]],
      toLocalAmt: [null],
      exchangeRate: [null],
      remark: [null],
      bpmRno: [null],
      fileCategory: [null],
      fileRequstTips: [null],
      fileList: [[]],
      disabled: [false],
    });

    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) { this.message.error('Can not get user information. Please refresh the page...', { nzDuration: 6000 }); }
    this.columnSubscribe();
    this.getRnoInfo();
    this.getCompanyData();
    this.getPaymentList();
    this.getCurrency();
  }

  columnSubscribe() {
    this.headForm.controls.applicantEmplid.valueChanges.subscribe(value => {
      if (!!value && this.headForm.controls.applicantEmplid.enabled && !this.isSpinning) {
        this.commonSrv.getEmployeeInfoById(value).subscribe(res => {
          this.isSpinning = true;
          this.applicantInfo = res;
          this.getEmployeeInfo();
        });
      }
    });
    this.headForm.controls.companyCode.valueChanges.subscribe(value => {
      if (!!value && this.headForm.controls.companyCode.enabled) {
        this.headForm.controls.projectCode.reset();
        this.getAllAdvanceSceneList();
      }
    });
    this.listForm.controls.advanceScene.valueChanges.subscribe(value => {
      if (value != null) {
        let sceneData = this.advanceSceneList.filter(o => o.expcode == value)[0];
        if (!sceneData) {
          this.message.error(this.translate.instant('Invalid data, please contact the administrator.'), { nzDuration: 6000 });
          this.listForm.controls.advanceScene.reset();
        }
        else {
          if (sceneData.datelevel == "Y") {
            this.listForm.controls.advanceDate.setValidators(this.dateMonthValidator)
            this.listForm.controls.advanceDate.markAsDirty();
          } else {
            this.listForm.controls.advanceDate.setValidators(Validators.required)
            this.listForm.controls.advanceDate.markAsDirty();
          }
          this.listForm.controls.advanceDate.updateValueAndValidity();
          this.expname = sceneData?.expname;
          this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFileRequest, { expcode: value, company: this.headForm.controls.companyCode.value, category: 2 }).subscribe((res) => {
            if (res && res.status === 200 && !!res.body && res.body.status == 1 && res.body.data?.length > 0) {
              let result = res.body.data[0];
              this.sceneKeyword = result.keyword;
              if (!!result.filecategory) {
                this.listForm.controls.fileCategory.setValue(result.filecategory.replace(/\bbpm/gi, ''));
                this.bpmFlag = result.filecategory.toLowerCase().indexOf('bpm') != -1;
              } else {
                this.bpmFlag = false;
                this.listForm.controls.fileCategory.reset();
              }
              this.listForm.controls.fileRequstTips.setValue(result.filepoints);
              if (result.isupload == "Y") {
                this.listForm.get('fileList')!.setValidators(Validators.required);
                this.listForm.get('fileList')!.markAsDirty();
              }
              else {
                this.listForm.get('fileList')!.clearValidators();
                this.listForm.get('fileList')!.markAsPristine();
              }
              this.listForm.get('fileList')!.updateValueAndValidity();
            }
            else {
              this.message.error(res.status === 200 && !!res.body ? res.body.message : res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 });
              this.listForm.controls.fileCategory.reset();
              this.listForm.controls.fileRequstTips.reset();
              this.bpmFlag = false;
            }
          });
        }
      }
      else {
        this.expname = '';
        this.listForm.controls.fileCategory.reset();
        this.listForm.controls.fileRequstTips.reset();
        this.bpmFlag = false;
      }
    });
    this.listForm.controls.requiredPaymentDate.valueChanges.subscribe(value => {
      if (value != null && !!this.listForm && !!this.listForm.controls.advanceDate.value) {
        this.listForm.get('advanceDate')!.markAsDirty();
        this.listForm.get('advanceDate')!.updateValueAndValidity();
      }
    });
    this.listForm.controls.bpmRno.valueChanges.subscribe(value => {
      if (value != null && value != '' && this.fileList.length == 0) this.canUpload = true;
      else this.canUpload = false;
    });
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyList = [];
      this.projectCodeList = [];
      this.currList = [];
      this.paymentList = [];
      this.curr = null;
      this.listTableData = [];
      this.keyId = 0;
      this.isSpinning = false;
      this.exchangeRate = 1;
      this.attachmentList = [];
      this.checked = false;
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
      overrange: this.translate.instant('can-not-later-a-month'),
      early: this.translate.instant('tips.cannot-earlier-than-payment')
    }
  };

  dateValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!!control.value) {
      if (control.value < (new Date()).setHours(0, 0, 0, 0)) {
        return { date: true, error: true };
      }
      if (!!this.listForm.controls.advanceDate.value) { this.listForm.controls.advanceDate!.updateValueAndValidity(); }
    }
    else { return { error: true, required: true }; }
  };

  dateMonthValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
    let requiredPaymentDate;
    if (this.listForm != null && this.listForm.controls.requiredPaymentDate.value != null) {
      requiredPaymentDate = new Date(this.listForm.controls.requiredPaymentDate.value);
    }
    if (!control.value) {
      return { error: true, required: true };
    }
    else if (new Date(control.value).setHours(0, 0, 0, 0) < (new Date()).setHours(0, 0, 0, 0)) {
      return { date: true, error: true };
    }
    else if (!!requiredPaymentDate && (new Date(control.value).setHours(0, 0, 0, 0) < new Date(requiredPaymentDate).setHours(0, 0, 0, 0))) {
      return { early: true, error: true };
    }
    else if (!!requiredPaymentDate && (new Date(control.value).setHours(0, 0, 0, 0) > new Date(requiredPaymentDate.setMonth((requiredPaymentDate).getMonth() + 1)).setHours(0, 0, 0, 0))) {
      return { overrange: true, error: true };
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
          this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFormData, { rno: rno }).subscribe(res => {
            if (res != null && res.status === 200 && !!res.body) {
              if (res.body.status == 1) {
                this.isSpinning = true;
                this.assembleFromData(res.body.data);
                this.isSpinning = false;
              }
              else { this.message.error(res.body.message, { nzDuration: 6000 }); }
            }
            else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
          });
        }
      }
      else { this.getEmployeeInfo(); }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    let attachmentList = value.file;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.applicantInfo = { company: headData.company, emplid: headData.cuser, deptid: headData.deptid, cname: headData.cname, phone: headData.ext, curr: headData.currency, costdeptid: headData.costdeptid, isaccount: true };
      this.cuser = headData.cuser;
      this.getEmployeeInfo();
      this.headForm.controls.payeeEmplid.setValue(headData.payeeId);
      this.headForm.controls.payeeName.setValue(headData.payeename);
      if (headData.projectcode != null) {
        this.projectCodeList.push(headData.projectcode);
        this.headForm.controls.projectCode.setValue(headData.projectcode);
      }
    }
    if (summaryAtmData != null) {
      this.headForm.controls.totalCost.setValue(summaryAtmData.amount);
    }

    if (listData != null) {
      listData.map(o => {
        let fileList = o.fileList.sort((a, b) => b.item - a.item).map(f => {
          return {
            id: f.seq,
            uid: Guid.create().toString(),
            name: f.filename,
            status: 'done',
            url: this.commonSrv.changeDomain(f.url),
            type: f.filetype,
            category: f.category,
            source: '', //TODO:获取标记
            // originFileObj: await this.commonSrv.getFileData(f.url, f.filename, f.filetype),
            originFileObj: null,
          }
        });
        this.listTableData.push({
          id: o.seq,
          advanceSceneName: o.expname,
          advanceScene: o.expcode,
          requiredPaymentDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          digest: o.summary,
          advanceDate: format(new Date(o.revsdate), 'yyyy/MM/dd'),
          requestPaymentName: o.payname,
          requestPayment: o.paytyp,
          curr: o.currency,
          appliedAmt: o.amount1,
          toLocalAmt: o.baseamt,
          exchangeRate: o.rate,
          remark: o.remarks,
          bpmRno: null,
          fileCategory: o.fileList.length > 0 ? o.fileList[0].category : null,
          fileList: fileList,
          disabled: false,
        });
      });

      // 组装文件
      attachmentList.map(o => {
        this.attachmentList.push({
          uid: o.item.toString(),
          name: o.filename,
          status: 'done',
          url: this.commonSrv.changeDomain(o.url),
          type: o.filetype,
        });
      });
      // this.listTableData.map(o => {
      //   if (o.fileCategory == null) {
      //     this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFileRequest, { expcode: o.advanceScene, company: this.headForm.controls.companyCode.value, category: 2 }).subscribe((res) => {
      //       if (res && res.status === 200 && !!res.body) {
      //         if (res.body.status == 1 && res.body.data?.length > 0) {
      //           let result = res.body.data[0];
      //           o.fileCategory = result.filecategory;
      //         }
      //         else { this.message.error("Exist invalid scene in details.", { nzDuration: 6000 }); }
      //       }
      //       else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
      //     });
      //   }
      // });
      this.keyId = this.listTableData.sort((a, b) => b.id - a.id)[0].id;
      if (this.listTableData.length > 0) {
        this.headForm.controls.applicantEmplid.disable({ emitEvent: false });
        this.headForm.controls.companyCode.disable({ emitEvent: false });
      }
      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
      this.attachmentList = [...this.attachmentList];
    }
    this.listTableData.map(f => {
      f.fileList.map(async o => { o.originFileObj = await this.commonSrv.getFileData(o.url, o.name, o.type); })
    });
    this.attachmentList.map(async o => {
      o.originFileObj = await this.commonSrv.getFileData(o.url, o.name, o.type);
    });
  }

  getChargeAgainstTableData() {
    this.isSpinning = true;
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
      this.isSpinning = false;
      return;
    }
    this.Service.Post(URLConst.GetChargeAgainstData + `/${this.applicantInfo.emplid}`, null).subscribe((res) => {
      this.chargeAgainstTableData = [];
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          res.body.data?.map(o => {
            this.chargeAgainstTableData.push({
              companyCode: o.company,
              applicantName: o.username,
              applicantId: o.user,
              payeeName: o.payee,
              payeeId: o.payeeuser,
              advanceFundRno: o.rno,
              digest: o.remark,
              appliedAmt: o.amount,
              notChargeAgainstAmt: o.actamt,
              openDays: o.opendays,//Math.ceil((Number(new Date()) - Number(new Date(o.cdate))) / (1000 * 3600 * 24)),
              delayTimes: o.delay,
            });
          });
          if (this.chargeAgainstTableData.length > 0) {
            this.chargeAgainstTableData = [...this.chargeAgainstTableData];
            if (!this.headForm.controls.rno.value)
              this.showChargeAgainst = true
          };
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
      this.isSpinning = false;
    });
  }

  showChargeAgainstModal() {
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
      return;
    }
    if (this.chargeAgainstTableData.length > 0) this.showChargeAgainst = true;
    else this.message.info(this.translate.instant('no-charge-against-data'));
  }

  getAllAdvanceSceneList() {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetAllAdvanceSceneList, { company: this.headForm.controls.companyCode.value }).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.advanceSceneList = res.body.data;
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
    });
  }

  getAdvanceSceneList(value) {
    if (value.length > 0) {
      this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetAdvanceSceneList, { input: value, company: this.headForm.controls.companyCode.value }).subscribe((res) => {
        if (res && res.status === 200 && !!res.body) {
          if (res.body.status == 1) {
            this.advanceSceneList = res.body.data;
            if (!this.showModal) { this.listForm.controls.advanceScene.setValue(this.advanceSceneList[0].expcode); }
          }
          else { this.message.error(res.body.message, { nzDuration: 6000 }); }
        }
        else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
      });
    }
  }

  handlePreview = async (file: NzUploadFile): Promise<void> => {
    if (file.type.indexOf('image') !== -1) {
      if (!file.preview && file.url == '...') { file.preview = await this.commonSrv.getPicBase64(file.originFileObj!); }
      this.previewImage = file.url == '...' ? file.preview : file.url;
      this.previewVisible = true;
    }
    else if (file.type.indexOf('application/pdf') !== -1) {
      file.safeUrl = this.commonSrv.getFileUrl(file.originFileObj);
      this.frameSrc = file.safeUrl;
      this.drawerVisible = true;
    }
    else {
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

  filters: UploadFilter[] = [
    {
      name: 'type',
      fn: (fileList: NzUploadFile[]) => {
        const filterFiles = fileList.filter(w =>
          ~['image/jpeg'].indexOf(w.type) ||
          ~['image/png'].indexOf(w.type) ||
          ~['image/bmp'].indexOf(w.type) ||
          ~['application/pdf'].indexOf(w.type));
        if (filterFiles.length !== fileList.length) {
          this.message.error(this.translate.instant('file-format-erro-inv'), { nzDuration: 6000 });
          return filterFiles;
        }
        return fileList;
      }
    }
  ];

  getEmployeeInfo() {
    if (!this.applicantInfo) {
      this.applicantInfo = this.userInfo;
    }
    this.userIdList = this.commonSrv.getUserInfo?.proxylist;
    if (this.userIdList.indexOf(this.applicantInfo.emplid) == -1) { this.userIdList.push(this.applicantInfo.emplid); }
    this.headForm.controls.companyCode.setValue(this.applicantInfo.company);
    this.headForm.controls.applicantEmplid.setValue(this.applicantInfo.emplid);
    this.headForm.controls.applicantName.setValue(this.applicantInfo.cname);
    this.headForm.controls.dept.setValue(this.applicantInfo.deptid);
    this.headForm.controls.ext.setValue(this.applicantInfo.phone);
    this.headForm.controls.payeeEmplid.setValue(this.applicantInfo.emplid);
    this.headForm.controls.payeeName.setValue(this.applicantInfo.cname);
    this.curr = this.applicantInfo.curr;
    this.listForm.controls.curr.setValue(this.curr);   // 首次加載頁面
    this.getChargeAgainstTableData();
  }

  getCompanyData() {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCompanyByArea, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1){this.companyList = res.body.data;
        if (!this.companyList.includes(this.headForm.controls.companyCode.value))
          this.headForm.controls.companyCode.setValue('');}
          else { this.message.error(res.body.message, { nzDuration: 6000 }); }
        }
        else { this.message.error(res.message ??this.translate.instant('server-error'), { nzDuration: 6000 }); }
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
        company: this.headForm.controls.companyCode.value
      };
      this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetProjectCode, params).subscribe((res) => {
        if (res && res.status === 200) {
          this.projectCodeList = res.body.map(item => `${item.code}-${item.description}`);
        }
        else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
    }
    else
      this.projectCodeList = [];
  }

  getPaymentList() {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetPaymentList, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.paymentList = res.body.data;
          // this.paymentList = res.body.map(item => item.payType + '-' + item.payName);}
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
    });
  }

  getCurrency() {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCurrencyList, null).subscribe((res) => {
      if (res && res.status === 200) {
        this.currList = res.body.map(item => item.currency);
      }
      else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  checkFileDetail(id: number): void {
    if (this.listTableData.filter(o => o.id == id).length > 0) {
      this.fileList = this.listTableData.filter(o => o.id == id)[0].fileList;
    }
  }

  addItem(): void {
    this.addloading = true;
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
      this.addloading = false;
      return;
    }
    if (this.chargeAgainstTableData.length > 0) {
      this.message.error(this.translate.instant('exist-not-charge-against-data'), { nzDuration: 6000 });
      this.addloading = false;
      return;
    }
    if (this.listTableData.length > 0) {
      this.message.error(this.translate.instant('can-add-only-one-item'), { nzDuration: 6000 });
      this.addloading = false;
      return;
    }
    this.keyId++;
    this.expname = '';
    this.bpmFlag = false;
    this.listForm.reset({ disabled: false, id: this.keyId, curr: this.curr, fileList: [], requestPayment: 'T' });
    this.canUpload = false;
    this.fileList = [];
    // this.advanceSceneList = [];
    this.showModal = true;
  }

  editRow(id: number): void {
    this.editloading = true;
    this.bpmFlag = false;
    let rowFormData = this.listTableData.filter(d => d.id == id)[0];
    this.listForm.reset(rowFormData);
    // this.advanceSceneList = [];
    // this.advanceSceneList.push({ expcode: rowFormData.advanceScene, expname: rowFormData.advanceSceneName });
    this.canUpload = false;
    this.fileList = rowFormData.fileList;
    this.showModal = true;
  }

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {   //多选操作
      this.listTableData = this.listTableData.filter(d => !this.setOfCheckedId.has(d.id));
      this.setOfCheckedId.clear();
    }
    else {
      this.listTableData = this.listTableData.filter(d => d.id != id);
      this.setOfCheckedId.delete(id);
    }
    if (this.listTableData.length > 0) {
      this.headForm.controls.applicantEmplid.disable({ emitEvent: false });
      this.headForm.controls.companyCode.disable({ emitEvent: false });
    } else {
      this.headForm.controls.applicantEmplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
    }
    this.deleteloading = false;
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
      this.message.error(this.translate.instant('fill-in-form'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    if (this.canUpload) {
      this.message.error(this.translate.instant('not-complete-upload'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let amount = this.listForm.controls.appliedAmt.value;
    if (amount == 0) {
      this.message.error(this.translate.instant('amount-zero-error'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    this.exchangeRate = 1;
    if (this.curr != this.listForm.controls.curr.value) {
      this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetExchangeRate, { ccurfrom: this.listForm.controls.curr.value, ccurto: this.curr }).subscribe((res) => {
        if (res && res.status === 200) {
          this.exchangeRate = res.body;
          this.setListTableData();
        } else {
          this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'), { nzDuration: 6000 });
          this.exchangeRate = 0;
        }
      });
    }
    else this.setListTableData();
  }

  setListTableData(): void {
    let amount = this.listForm.controls.appliedAmt.value;
    this.listForm.controls.exchangeRate.setValue(this.exchangeRate);
    this.listForm.controls.toLocalAmt.setValue(Number((amount * this.exchangeRate).toFixed(2)));

    let requiredPaymentDateType = typeof this.listForm.controls.requiredPaymentDate.value;    //转换日期格式
    let advanceDateType = typeof this.listForm.controls.advanceDate.value;
    if (this.listForm.controls.requiredPaymentDate.value != null && requiredPaymentDateType != "string") {
      let requiredPaymentDate = this.listForm.controls.requiredPaymentDate.value == null ? null : format(this.listForm.controls.requiredPaymentDate.value, 'yyyy/MM/dd');
      this.listForm.controls.requiredPaymentDate.setValue(requiredPaymentDate);
    }
    if (this.listForm.controls.advanceDate.value != null && advanceDateType != "string") {
      let advanceDate = this.listForm.controls.advanceDate.value == null ? null : format(this.listForm.controls.advanceDate.value, 'yyyy/MM/dd');
      this.listForm.controls.advanceDate.setValue(advanceDate);
    }
    let rowId = this.listForm.controls.id.value;
    this.listTableData = this.listTableData.filter(o => o.id != rowId);
    let formData = this.listForm.getRawValue();
    formData.advanceSceneName = this.advanceSceneList.filter(t => t.expcode == formData.advanceScene).map(t => t.expname)[0];
    formData.requestPaymentName = this.paymentList.filter(t => t.payType == formData.requestPayment).map(t => t.payName)[0];
    this.listTableData.push(formData);

    let total = 0;   // 计算费用总计
    this.listTableData.map(item => total += Number(item.toLocalAmt));
    this.headForm.controls.totalCost.setValue(total);

    this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
    this.listTableData = [...this.listTableData];   // 刷新表格

    if (this.listTableData.length > 0) {
      {
        this.headForm.controls.applicantEmplid.disable({ emitEvent: false });
        this.headForm.controls.companyCode.disable({ emitEvent: false });
      }
    } else {
      this.headForm.controls.applicantEmplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
    }

    this.showModal = false;
    this.isSaveLoading = false;
    this.addloading = false;
    this.editloading = false;
    this.isSpinning = false;
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  ////////带选择框表
  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: AdvanceFundInfo[] = [];
  setOfCheckedId = new Set<number>();
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: AdvanceFundInfo[]): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
    this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
    this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
  }

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let uploadedFile;
      if (this.showModal) uploadedFile = this.fileList.filter(o => o.originFileObj.name == file.name);
      else uploadedFile = this.attachmentList.filter(o => o.originFileObj.name == file.name);
      let upload = uploadedFile.length == 0;
      if (!upload) this.message.error(this.commonSrv.FormatString(this.translate.instant('has-been-uploaded-that'), uploadedFile[0].originFileObj.name, uploadedFile[0].name), { nzDuration: 6000 });
      // if (this.showModal && this.fileList.length > 0) {
      //   upload = false;
      //   this.message.error(this.translate.instant('can-upload-only-one-item'), { nzDuration: 6000 });
      // }
      observer.next(upload);
      observer.complete();
    });
  };

  removeFile = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      this.listForm.controls.bpmRno.reset();
      this.listForm.controls.fileList.setValue(this.fileList);
      observer.next(true);
      observer.complete();
    });
  }

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map(file => {
      file.status = "done";
      file.url = !file.url ? '...' : file.url;
      if (this.showModal) {
        file.id = this.listForm.controls.id.value;
        file.source = 'upload';
        file.category = this.listForm.controls.fileCategory.value;
      }
      return file;
    });
    if (this.showModal) {
      this.fileList = fileList;
      this.listForm.controls.fileList.setValue(this.fileList);
      this.canUpload = false;
    }
    else this.attachmentList = fileList;
  }

  getBpmRnoFile() {
    if (!this.listForm.controls.fileCategory.value) {
      this.message.error(this.translate.instant('input-advance-scene-first'), { nzDuration: 6000 });
      return;
    }
    this.isSaveLoading = true;
    let bpmRno = this.listForm.controls.bpmRno.value;
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetBPMAttachment, { rno: bpmRno }).subscribe(async (res) => {
      if (res && res.status === 200 && res.body.status == 1) {
        let fileUrl = res.body.data;
        if (fileUrl != null) {
          this.fileList.push(
            {
              uid: Guid.create().toString(),
              id: this.listForm.controls.id.value,
              name: bpmRno + '.pdf',
              filename: bpmRno + '.pdf',
              type: 'application/pdf',
              status: 'done',
              url: fileUrl,
              source: 'bpm',
              category: this.listForm.controls.fileCategory.value,
              originFileObj: await this.commonSrv.getFileData(res.body.data, bpmRno + '.pdf', 'application/pdf'),
            });
          this.listForm.controls.fileList.setValue(this.fileList);
          this.fileList = [...this.fileList];
        }
        this.canUpload = false;
      }
      else {
        let msg = this.translate.instant('operate-failed') + (res.status === 200 ? res.body.message : this.translate.instant('server-error'));
        this.message.error(msg, { nzDuration: 6000 });
      }
      this.isSaveLoading = false;
    });
  }

  checkParam(): boolean {
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.chargeAgainstTableData.length > 0) {
      this.message.error(this.translate.instant('exist-not-charge-against-data'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (!this.headForm.valid) {
      Object.values(this.headForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.listTableData.length == 0) {
      this.message.error(this.translate.instant('fill-in-detail'), { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    return true;
  }

  commit(sendType: string): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;

    this.Service.Post(URLConst.ChangePayeeTips + `?amount=${this.headForm.controls.totalCost.value}&company=${this.headForm.controls.companyCode.value}`, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body && !!res.body.data) {
        if (res.body.data.ischange && res.body.data.changeid != this.headForm.controls.payeeEmplid.value) {
          let tips = res.body.message
          this.modal.info({
            nzTitle: this.translate.instant('tips'),
            nzContent: `<p>${tips}</p>`,
            nzOnOk: () => this.sendForm(sendType, true, { payeeEmplid: res.body.data.changeid, payeeName: res.body.data.changename }),
            nzOnCancel: () => { this.isSpinning = false; this.isSaveLoading = false; }
          });
        }
        else this.sendForm(sendType, res.body.data.ischange);
      }
      else if (res.status !== 200) {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  sendForm(sendType: string, isChange: boolean, payeeInfo: any = null): void {
    let formData = this.SetParam();
    if (sendType == 'save') {  //暫存表單
      this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq401, formData).subscribe((res) => {
        // this.Service.Post('http://localhost:5000' + URLConst.SaveRq401, formData).subscribe((res) => {
        if (res && res.status === 200 && res.body.status === 1) {
          this.message.success(this.translate.instant('save-successfully') + `Request NO: ${res.body.data.rno}`);
          this.headForm.controls.rno.setValue(res.body.data.rno);
          if (!this.cuser) { this.cuser = this.userInfo?.emplid; }
          if (isChange && !!payeeInfo) {
            this.headForm.controls.payeeEmplid.setValue(payeeInfo.payeeEmplid);
            this.headForm.controls.payeeName.setValue(payeeInfo.payeeName);
          } else if (!isChange) {
            this.headForm.controls.payeeEmplid.setValue(this.headForm.controls.applicantEmplid.value);
            this.headForm.controls.payeeName.setValue(this.headForm.controls.applicantName.value);
          }
        }
        else this.message.error(this.translate.instant('save-failed') + (res.status === 200 ? res.body.message : this.translate.instant('server-error')), { nzDuration: 6000 });
        this.isSpinning = false;
        this.isSaveLoading = false;
      });
    }
    else if (sendType == 'submit') {   // 提交表單
      this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq401, formData).subscribe((res) => {
        if (res && res.status === 200 && res.body.status === 1) {
          this.message.success(this.translate.instant('submit-successfully'));
          // let tips = res.body.data.stat ? this.translate.instant('required-send-fin') + (res.body.message == null ? '' : res.body.message) : this.translate.instant('no-required-send-fin')
          this.commonSrv.SendMobileSignXMLData(res.body.data.rno);
          let tips = res.body.message;
          this.modal.info({
            nzTitle: this.translate.instant('tips'),
            nzContent: `<p>${tips}</p><p>Request NO: ${res.body.data.rno}</p>`,
            nzOnOk: () => this.router.navigateByUrl(`ers/rq401`)   // reset form data
          });
        }
        else
          this.message.error(this.translate.instant('submit-failed') + (res.status === 200 ? res.body.message : this.translate.instant('server-error')), { nzDuration: 6000 });
        this.isSpinning = false;
        this.isSaveLoading = false;
      });
    }
  }

  SetParam(): FormData {
    const formData = new FormData();
    //#region 组装数据
    let rno = this.headForm.controls.rno.value
    let headData = {
      rno: rno,
      cuser: this.headForm.controls.applicantEmplid.value,
      cname: this.headForm.controls.applicantName.value,
      deptid: this.headForm.controls.dept.value,
      ext: this.headForm.controls.ext.value,
      company: this.headForm.controls.companyCode.value,
      projectcode: this.headForm.controls.projectCode.value,
      payeeId: this.headForm.controls.applicantEmplid.value,
      payeename: this.headForm.controls.applicantName.value,
      currency: this.curr,
    }
    formData.append('head', JSON.stringify(headData));

    let fileList = [];
    let listData = this.listTableData.map(o => {
      fileList = fileList.concat(o.fileList);
      let sceneData = this.advanceSceneList.filter(i => i.expcode == o.advanceScene)[0];
      return {
        rno: rno,
        seq: o.id,
        expname: o.advanceSceneName,
        expcode: o.advanceScene,
        rdate: o.requiredPaymentDate,
        summary: o.digest,
        revsdate: o.advanceDate,
        payname: o.requestPaymentName,
        paytyp: o.requestPayment,
        currency: o.curr,
        amount1: o.appliedAmt,
        baseamt: o.toLocalAmt,
        rate: o.exchangeRate,
        remarks: o.remark,
        basecurr: this.curr,
        deptid: this.headForm.controls.dept.value,
        acctcode: sceneData?.acctcode,
        acctname: sceneData?.acctname,
      }
    });
    formData.append('detail', JSON.stringify(listData))

    let indx = 0;
    let detailFileData = fileList.map(o => {
      return {
        rno: rno,
        seq: o.id,
        item: indx++,
        category: o.category,
        filetype: o.type,
        filename: o.name,
        ishead: "N",
        key: o.uid,
      }
    });
    indx = 0;
    let attachmentData = this.attachmentList.map(o => {
      return {
        rno: rno,
        seq: 0,
        item: indx++,
        category: null,
        filetype: o.type,
        filename: o.name,
        ishead: "Y",
        key: o.uid,
      }
    });
    formData.append('file', JSON.stringify(detailFileData.concat(attachmentData)))

    let amountData = {
      rno: rno,
      currency: this.curr,
      amount: this.headForm.controls.totalCost.value,
      actamt: this.headForm.controls.totalCost.value,
    }
    formData.append('amount', JSON.stringify(amountData));

    fileList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
  }

}
