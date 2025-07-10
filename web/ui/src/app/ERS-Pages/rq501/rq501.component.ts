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
import { ExceptionDetail, GeneralExpenseInfo } from './classes/data-item';
import {
  DetailTableColumn,
  ExpenseTableColumn,
  OvertimeMealExpenseTableColumn,
  DriveFuelExpenseTableColumn,
} from './classes/table-column';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CompanyInfo } from '../bd008/classes/data-item';

@Component({
  selector: 'app-rq501',
  templateUrl: './rq501.component.html',
  styleUrls: ['./rq501.component.scss'],
})
export class RQ501Component implements OnInit, OnDestroy {
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
  radioParam1 = 'self-' + this.translate.instant('individual-afford');
  radioParam2 = 'company-' + this.translate.instant('company-afford');
  headForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  detailForm: UntypedFormGroup;
  companyList: any[] = [];
  projectCodeList: any[] = [];
  currList: any[] = [];
  deptList: any[] = [];
  sceneList: any[];
  cityList: any[] = [];
  carTypeList: any[] = [];
  payeeList: any[] = [];
  curr: string;
  costdeptid: string;
  isaccount: boolean = false;
  showModal: boolean = false;
  exceptionModal: boolean = false;
  tipModal: boolean = false;
  batchUploadModal: boolean = false;
  isSaveLoading: boolean = false;
  detailListTableColumn = DetailTableColumn;
  listTableColumn = ExpenseTableColumn;
  overtimeMealListTableColumn = OvertimeMealExpenseTableColumn;
  driveFuelListTableColumn = DriveFuelExpenseTableColumn;
  detailTableShowData: ExceptionDetail[] = [];
  listTableData: GeneralExpenseInfo[] = [];
  listOfAction = ['Delete'];
  keyId: number = 0;
  exInfo: any[] = [];
  exWarning: string[] = [];
  exTip: string = '';
  exTotalWarning: any[] = [];
  tipAffordParty = '';
  isSpinning = false;
  isAllUpload = true;
  canConfirm = this.translate.instant('Confirm');
  exchangeRate = 1;
  fileList: NzUploadFile[] = [];
  attachList: NzUploadFile[] = [];
  attachmentList: NzUploadFile[] = [];
  batchUploadList: NzUploadFile[] = [];
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
  canUpload = false;
  type: string = null;
  sampleUrl: string = null;
  sampleName: string = null;
  userInfo: any;
  bpmFlag: boolean = false;
  nameList: any[] = [];
  sceneKeyword: string = '';
  expname: string = '';
  taxRate: number = 0.25;
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
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.headForm = this.fb.group({
      emplid: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      payee: [{ value: null, disabled: true }],
      ext: [null],
      projectCode: [null],
      companyCode: [Validators.required],
      appliedTotal: [{ value: 0, disabled: true }],
      individualTax: [{ value: 0, disabled: true }],
      actualTotal: [{ value: 0, disabled: true }],
      totalAmount: [{ value: 0, disabled: true }],
      invoiceDetailList: [[]],
      attachList: [[]],
      fileList: [[]],
      bpmRno: [null],
      fileCategory: [null],
      fileRequstTips: [null],
      scene: [null],
    });
    this.listForm = this.fb.group({
      attribDept: [null],
      payeeId: [null],
      payeeName: [null],
      payeeDeptid: [null],
      bankName: [null],
      startingPlace: [null],
      cityOnBusiness: [null],
      feeDate: [null],
      carType: [null],
      carTypeName: [null],
      kil: [null],
      digest: [null],
      startingTime: [null],
      backTime: [null],
      curr: [null],
      expenseAmt: [null],
      exchangeRate: [null],
      toLocalAmt: [null],
      id: [this.keyId],
      disabled: [false],
    });

    this.detailForm = this.fb.group({
      invoiceCode: [null],
      invoiceNo: [null],
      amount: [0, [Validators.required]],
      curr: [{ value: null, disabled: true }],
      reason: [null, [Validators.required]],
      taxLoss: [{ value: 0, disabled: true }],
      affordParty: [null, [Validators.required]],
      disabled: [false],
      // id: [this.keyId],
      index: [null],
      toLocalTaxLoss: [0],
      affordPartyValue: [null],
      exTips: [null],
    });

    this.columnValueChange();
    this.getEmployeeInfo();
    this.getCompanyData();
    this.getCurrency();
    this.getRnoInfo();
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }
  getTaxRate(value) {
    const queryParam = {
      pageIndex: 1,
      pageSize: 10,
      data: [value],
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.OperateBd008,
      queryParam
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        let result: CompanyInfo[] = [];
        res.body.data?.map((o) => {
          result.push({
            id: o.id,
            companyCode: o.company,
            company: o.companycode,
            sapCompanyCode: o.companysap,
            companyDesc: o.companydesc,
            abbr: o.stwit,
            curr: o.basecurr,
            taxpayerNo: o.identificationcode,
            taxRate: o.taxcode,
            area: o.area,
            timezone: o.timezone,
            timezoneName: null,
            creator: o.cuser,
            createDate:
              o.cdate == null ? null : format(new Date(o.cdate), 'yyyy/MM/dd'),
            updateUser: o.muser,
            updateDate:
              o.mdate == null ? null : format(new Date(o.mdate), 'yyyy/MM/dd'),
            disabled: false,
          });
        });
        this.taxRate = result[0].taxRate;
      }
    });
  }
  columnValueChange() {
    this.headForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (!!value && this.headForm.controls.companyCode.enabled) {
        if (!!this.headForm.controls.projectCode.value) {
          this.headForm.controls.projectCode.reset();
        }
        this.getSceneList();
      }
      this.getTaxRate(value);
    });

    this.detailForm.get('curr').valueChanges.subscribe((value) => {
      let amount = this.detailForm.controls.amount.value;
      if (!!value && value != this.curr) {
        //手动新增异常报销Modal获取汇率
        const params = {
          ccurfrom: value,
          ccurto: this.curr,
        };
        this.Service.doGet(
          this.EnvironmentconfigService.authConfig.ersUrl +
            URLConst.GetExchangeRate,
          params
        ).subscribe((res) => {
          if (res && res.status === 200) {
            this.exchangeRate = res.body;
          } else {
            this.message.error(res.message, { nzDuration: 6000 });
            this.exchangeRate = 0;
          }
          this.detailForm.controls.taxLoss.setValue(
            Number((amount * this.exchangeRate * this.taxRate).toFixed(2))
          );
        });
      } else if (value == this.curr) {
        this.exchangeRate = 1;
        this.detailForm.controls.taxLoss.setValue(
          Number((amount * this.exchangeRate * this.taxRate).toFixed(2))
        );
      }
    });

    this.detailForm.get('amount').valueChanges.subscribe((value) => {
      this.detailForm.controls.taxLoss.setValue(
        (value * this.exchangeRate * this.taxRate).toFixed(2)
      );
    });

    this.listForm.get('curr').valueChanges.subscribe((value) => {
      if (!this.isSaveLoading) {
        let exchangeRate = 1;
        if (!!value && value != this.curr) {
          //手动单笔新增default报销明细获取汇率
          const params = {
            ccurfrom: value,
            ccurto: this.curr,
          };
          this.Service.doGet(
            this.EnvironmentconfigService.authConfig.ersUrl +
              URLConst.GetExchangeRate,
            params
          ).subscribe((res) => {
            if (res && res.status === 200) {
              exchangeRate = res.body;
            } else {
              this.message.error(res.message, { nzDuration: 6000 });
              exchangeRate = 0;
            }
            this.listForm.controls.exchangeRate.setValue(exchangeRate);
          });
        } else if (value == this.curr) {
          exchangeRate = 1;
          this.listForm.controls.exchangeRate.setValue(exchangeRate);
        }
      }
    });

    this.headForm.controls.bpmRno.valueChanges.subscribe((value) => {
      if (value != null && value != '' && this.attachList.length == 0)
        this.canUpload = true;
      else this.canUpload = false;
    });

    this.headForm.controls.scene.valueChanges.subscribe((value) => {
      // if (!this.isSaveLoading) {
      if (value != null && !!this.sceneList) {
        if (this.headForm.controls.companyCode.enabled)
          this.headForm.controls.companyCode.disable();
        let sceneData = this.sceneList.filter((o) => o.expcode == value)[0];
        this.expname = sceneData?.expname;
        // let typeToNum = !this.type ? null : (this.type == "default" ? 0 : (this.type == "overtimeMeal" ? 1 : 2));
        if (!!sceneData && (this.listTableData.length == 0 || !this.type)) {
          // this.headForm.controls.sceneName.setValue(sceneData.expname);
          let type = sceneData.expcategory;
          switch (type) {
            case 0: {
              this.type = 'default';
              this.listTableColumn = ExpenseTableColumn;
              this.sampleUrl = '../../../assets/file/default-batch-sample.xlsx';
              this.sampleName = this.translate.instant('sample.batch.default');
              if (this.listForm.controls.curr.value != this.curr)
                this.listForm.controls.curr.setValue(this.curr);
              if (!this.listForm.controls.curr.enabled)
                this.listForm.controls.curr.enable();
              if (!this.listForm.controls.expenseAmt.enabled)
                this.listForm.controls.expenseAmt.enable();
              break;
            }
            case 1: {
              this.type = 'overtimeMeal';
              this.listTableColumn = OvertimeMealExpenseTableColumn;
              this.sampleUrl =
                '../../../assets/file/overtimemeal-batch-sample.xlsx';
              this.sampleName = this.translate.instant(
                'sample.batch.overtime-meal'
              );
              if (this.cityList.length == 0) this.getCityList();
              if (!!this.listForm.controls.curr.value)
                this.listForm.controls.curr.reset();
              if (!this.listForm.controls.curr.disabled)
                this.listForm.controls.curr.disable();
              if (!this.listForm.controls.expenseAmt.disabled)
                this.listForm.controls.expenseAmt.disable();
              break;
            }
            case 2: {
              this.type = 'drive';
              this.listTableColumn = DriveFuelExpenseTableColumn;
              this.sampleUrl =
                '../../../assets/file/selfdrive-batch-sample.xlsx';
              this.sampleName = this.translate.instant('sample.batch.drive');
              if (this.carTypeList.length == 0) this.getCarType();
              if (this.listForm.controls.curr.value != this.curr)
                this.listForm.controls.curr.setValue(this.curr);
              if (!this.listForm.controls.curr.disabled)
                this.listForm.controls.curr.disable();
              if (!this.listForm.controls.expenseAmt.disabled)
                this.listForm.controls.expenseAmt.disable();
              break;
            }
            default:
              this.type = null;
              this.listForm.controls.scene.reset();
              this.message.error(
                'Invalid data, please contact the administrator.',
                { nzDuration: 6000 }
              );
              break;
          }
          this.sceneKeyword = sceneData.keyword;
          if (!!sceneData.filecategory) {
            this.headForm.controls.fileCategory.setValue(
              sceneData.filecategory.replace(/\bbpm/gi, '')
            );
            this.bpmFlag =
              sceneData.filecategory.toLowerCase().indexOf('bpm') != -1;
          } else {
            this.bpmFlag = false;
            this.headForm.controls.fileCategory.reset();
          }
          this.headForm.controls.fileRequstTips.setValue(sceneData.filepoints);
          if (sceneData.isupload == 'Y') {
            this.headForm.get('attachList')!.setValidators(Validators.required);
            this.headForm.get('attachList')!.markAsDirty();
          } else {
            this.headForm.get('attachList')!.clearValidators();
            this.headForm.get('attachList')!.markAsPristine();
          }
          if (sceneData.isinvoice == 'Y') {
            this.headForm
              .get('invoiceDetailList')!
              .setValidators(Validators.required);
            this.headForm.get('invoiceDetailList')!.markAsDirty();
          } else {
            this.headForm.get('invoiceDetailList')!.clearValidators();
            this.headForm.get('invoiceDetailList')!.markAsPristine();
          }
          this.headForm.get('attachList')!.updateValueAndValidity();
          this.headForm.get('invoiceDetailList')!.updateValueAndValidity();
        } else if (!sceneData) {
          this.message.error(
            this.translate.instant(
              'Invalid data, please contact the administrator.'
            ),
            { nzDuration: 6000 }
          );
          this.headForm.controls.scene.reset();
        }
      } else {
        this.expname = '';
        this.headForm.controls.fileCategory.reset();
        this.bpmFlag = false;
        this.type = null;
        this.headForm.controls.fileRequstTips.reset();
        if (!this.headForm.controls.companyCode.enabled)
          this.headForm.controls.companyCode.enable();
      }
      // }
    });

    this.listForm.controls.carType.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'drive') {
        if (!!value && !!this.listForm.controls.kil.value) {
          let amount = this.carTypeList.filter((o) => o.type == value)[0]
            ?.amount;
          if (!!amount)
            this.listForm.controls.expenseAmt.setValue(
              Number((this.listForm.controls.kil.value * amount).toFixed(2))
            );
        } else if (!!this.listForm.controls.expenseAmt.value)
          this.listForm.controls.expenseAmt.reset();
      }
    });

    this.listForm.controls.kil.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'drive') {
        if (!!value && !!this.listForm.controls.carType.value) {
          let amount = this.carTypeList.filter(
            (o) => o.type == this.listForm.controls.carType.value
          )[0]?.amount;
          if (!!amount)
            this.listForm.controls.expenseAmt.setValue(
              Number((value * amount).toFixed(2))
            );
        } else if (!!this.listForm.controls.expenseAmt.value)
          this.listForm.controls.expenseAmt.reset();
      }
    });

    this.listForm.controls.cityOnBusiness.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'overtimeMeal') {
        if (
          !!value &&
          !!this.listForm.controls.startingTime.value &&
          !!this.listForm.controls.backTime.value
        )
          this.getOverMealExpenseAmt();
        else {
          if (!!this.listForm.controls.curr.value)
            this.listForm.controls.curr.reset();
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
        }
      }
    });

    this.listForm.controls.startingTime.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'overtimeMeal') {
        if (
          !!value &&
          !!this.listForm.controls.cityOnBusiness.value &&
          !!this.listForm.controls.backTime.value
        )
          this.getOverMealExpenseAmt();
        else {
          if (!!this.listForm.controls.curr.value)
            this.listForm.controls.curr.reset();
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
        }
      }
    });

    this.listForm.controls.backTime.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'overtimeMeal') {
        if (
          !!value &&
          !!this.listForm.controls.cityOnBusiness.value &&
          !!this.listForm.controls.startingTime.value
        )
          this.getOverMealExpenseAmt();
        else {
          if (!!this.listForm.controls.curr.value)
            this.listForm.controls.curr.reset();
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
        }
      }
    });

    this.listForm.controls.payeeId.valueChanges.subscribe((value) => {
      if (!!value && !!this.payeeList && this.payeeList.length > 0) {
        let payeeInfo = this.payeeList.filter((o) => o.payeeid == value)[0];
        if (!!payeeInfo) {
          this.listForm.controls.payeeName.setValue(payeeInfo.payeename);
          this.listForm.controls.payeeDeptid.setValue(payeeInfo.payeedept);
          this.listForm.controls.bankName.setValue(payeeInfo.bank);
        }
      }
    });
  }

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyList = [];
      this.projectCodeList = [];
      this.currList = [];
      this.carTypeList = [];
      this.deptList = [];
      this.sceneList = [];
      this.curr = null;
      this.costdeptid = null;
      this.isaccount = false;
      this.showModal = false;
      this.exceptionModal = false;
      this.tipModal = false;
      this.isSaveLoading = false;
      this.detailTableShowData = [];
      this.listTableData = [];
      this.keyId = 0;
      this.exInfo = [];
      this.exWarning = [];
      this.exTip = '';
      this.exTotalWarning = [];
      this.tipAffordParty = '';
      this.isSpinning = false;
      this.isAllUpload = true;
      this.canConfirm = this.translate.instant('Confirm');
      this.exchangeRate = 1;
      this.fileList = [];
      this.attachmentList = [];
      this.attachList = [];
      this.type = null;
      this.previewImage = '';
      this.previewVisible = false;
      this.checked = false;
      this.addloading = false;
      this.editloading = false;
      this.deleteloading = false;
      this.indeterminate = false;
      this.listOfCurrentPageData = [];
      this.setOfCheckedId = new Set<number>();
      this.showPicList = false;
      this.expname = '';
      this.ngOnInit();
    }
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-future-date'),
    },
  };

  dateValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { error: true, required: true };
    } else if (control.value > new Date()) {
      return { date: true, error: true };
    }
  };

  getRnoInfo() {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
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
              this.assembleFromData(res.body.data);
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
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    let allFileList = value.file;
    let invoiceDetailList = value.invList;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.headForm.controls.emplid.setValue(headData.payeeId);
      this.headForm.controls.dept.setValue(headData.deptid);
      this.headForm.controls.payee.setValue(headData.payeename);
      this.headForm.controls.ext.setValue(headData.ext);
      if (this.headForm.controls.companyCode.value != headData.company) {
        this.headForm.controls.companyCode.setValue(headData.company);
      }
      if (headData.projectcode != null) {
        this.projectCodeList.push(headData.projectcode);
        this.headForm.controls.projectCode.setValue(headData.projectcode);
      }
    }
    if (summaryAtmData != null) {
      this.headForm.controls.appliedTotal.setValue(summaryAtmData.amount);
      this.headForm.controls.actualTotal.setValue(summaryAtmData.actamt);
      this.headForm.controls.totalAmount.setValue(summaryAtmData.actamt);
      this.headForm.controls.individualTax.setValue(
        Number((summaryAtmData.amount - summaryAtmData.actamt).toFixed(2))
      );
    }

    // 组装文件
    allFileList
      .sort((a, b) => b.item - a.item)
      .map((o) => {
        if (o.status == 'I') {
          this.fileList.push({
            id: o.seq,
            uid: o.item.toString(),
            name: o.filename,
            filename: o.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(o.url),
            type: o.filetype,
            originFileObj: null,
            upload: true,
          });
        } else if (o.status == 'F') {
          this.attachList.push({
            id: o.seq,
            uid: Guid.create().toString(),
            name: o.filename,
            filename: o.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(o.url),
            type: o.filetype,
            category: o.category,
            source: '',
            originFileObj: null,
          });
        } else {
          this.attachmentList.push({
            uid: Guid.create().toString(),
            name: o.filename,
            filename: o.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(o.url),
            type: o.filetype,
          });
        }
      });
    let fileCategory =
      this.attachList.length > 0 ? this.attachList[0].category : null;
    if (!!fileCategory) {
      this.bpmFlag = fileCategory.toLowerCase().indexOf('bpm') != -1;
      this.headForm.controls.fileCategory.setValue(
        fileCategory.replace(/\bbpm/gi, '')
      );
    }
    let index = 1;
    let detailList = invoiceDetailList.map((i) => {
      if (i.expcode != null && i.expcode != '') {
        this.exTotalWarning.push(index++ + '. ' + i.expcode);
      }
      return {
        invoiceCode: i.invcode,
        invoiceNo: i.invno,
        amount: i.amount,
        taxLoss: i.taxloss,
        curr: i.curr,
        affordParty:
          i.undertaker == 'self'
            ? this.translate.instant('individual-afford')
            : i.undertaker == 'company'
            ? this.translate.instant('company-afford')
            : '',
        disabled: i.abnormal == 'N',
        id: i.seq,
        index: i.item,
        uid: i.item,
        exTips: i.expcode,
        toLocalTaxLoss: i.taxloss,
        affordPartyValue: i.undertaker,
        reason: i.reason,
        invdate: i.invdate,
        taxamount: i.taxamount,
        oamount: i.oamount,
        invstat: i.invstat,
        abnormalamount: i.abnormalamount,
        paymentName: i.paymentName,
        paymentNo: i.paymentNo,
        collectionName: i.collectionName,
        collectionNo: i.collectionNo,
        expdesc: i.expdesc,
        expcode: i.expcode,
        invdesc: i.invdesc,
        underwriter: i.underwriter,
        invtype: i.invtype,
        invoiceid: i.invoiceid,
        invabnormalreason: i.invabnormalreason,
        fileurl: i.fileurl,
        baseamt: i.baseamt,
      };
    });
    this.headForm.controls.invoiceDetailList.setValue(detailList);

    if (listData != null) {
      listData.map((o) => {
        this.listTableData.push({
          attribDept: o.deptid,
          payeeId: o.payeeid,
          payeeName: o.payeename,
          payeeDeptid: o.payeedeptid,
          bankName: o.bank,
          startingPlace: o.location,
          cityOnBusiness: o.city,
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          carType: o.cartype,
          carTypeName: null,
          kil: o.journey,
          digest: o.summary,
          startingTime: format(new Date(o.gotime), 'HH:mm'),
          backTime: format(new Date(o.backtime), 'HH:mm'),
          curr: o.currency,
          expenseAmt: o.amount1,
          exchangeRate: o.rate,
          toLocalAmt: o.baseamt,
          id: o.seq,
          disabled: false,
          whiteRemark: '',
          taxexpense: null,
        });
      });
      this.keyId = this.listTableData.sort((a, b) => b.id - a.id)[0].id;
      this.headForm.controls.scene.setValue(listData[0].expcode);
      if (this.listTableData.length > 0) {
        let nameList = this.listTableData.map((o) => {
          return { emplid: o.payeeId, label: o.payeeId + '/' + o.payeeName };
        });
        this.nameList = nameList.reduce((accumalator, current) => {
          if (
            !accumalator.some(
              (item) =>
                item.emplid === current.emplid && item.label === current.label
            )
          ) {
            accumalator.push(current);
          }
          return accumalator;
        }, []);
      }

      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
      this.fileList = [...this.fileList];
      this.attachList = [...this.attachList];
      this.attachmentList = [...this.attachmentList];
      this.setDetailTableShowData();
    }
    this.isSpinning = false;
    this.fileList.map(async (o) => {
      o.originFileObj = await this.commonSrv.getFileData(
        o.url,
        o.filename,
        o.type
      );
    });
    this.attachList.map(async (o) => {
      o.originFileObj = await this.commonSrv.getFileData(
        o.url,
        o.filename,
        o.type
      );
    });
    this.attachmentList.map(async (o) => {
      o.originFileObj = await this.commonSrv.getFileData(
        o.url,
        o.filename,
        o.type
      );
    });
    this.headForm.controls.fileList.setValue(this.fileList);
    this.headForm.controls.attachList.setValue(this.attachList);
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

  filters: UploadFilter[] = [
    {
      name: 'type',
      fn: (fileList: NzUploadFile[]) => {
        const filterFiles = fileList.filter(
          (w) =>
            ~['image/jpeg'].indexOf(w.type) ||
            ~['image/png'].indexOf(w.type) ||
            ~['image/bmp'].indexOf(w.type) ||
            ~['application/pdf'].indexOf(w.type)
        );
        if (filterFiles.length !== fileList.length) {
          this.message.error(this.translate.instant('file-format-erro-inv'), {
            nzDuration: 6000,
          });
          return filterFiles;
        }
        return fileList;
      },
    },
  ];

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

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
    this.headForm.controls.companyCode.setValue(this.userInfo.company);
    this.headForm.controls.emplid.setValue(this.userInfo.emplid);
    this.headForm.controls.dept.setValue(this.userInfo.deptid);
    this.headForm.controls.payee.setValue(this.userInfo.cname);
    this.headForm.controls.ext.setValue(this.userInfo.phone);
    this.curr = this.userInfo.curr;
    if (
      this.detailListTableColumn
        .filter((o) => o.columnKey == 'taxLoss')[0]
        .title.indexOf('(') == -1
    )
      this.detailListTableColumn.filter(
        (o) => o.columnKey == 'taxLoss'
      )[0].title =
        this.translate.instant(
          this.detailListTableColumn.filter((o) => o.columnKey == 'taxLoss')[0]
            .title
        ) + `(${this.curr})`;
    this.costdeptid = this.userInfo.costdeptid;
    this.isaccount = this.userInfo.isaccount;
    if (!this.userInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.headForm.controls.scene.disable();
      return;
    }
  }

  getCompanyData() {
    this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
    if (this.companyList.length == 1)
      this.headForm.controls.companyCode.setValue(this.companyList[0]);
    else {
      if (!this.companyList.includes(this.headForm.controls.companyCode.value))
        this.headForm.controls.companyCode.setValue('');
    }
  }

  //TODO:优化分隔符隐患
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

  getDeptList(value) {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    if (value.length > 0) {
      const params = {
        deptid: value,
        company: this.headForm.controls.companyCode.value,
      };
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetDeptList,
        params
      ).subscribe((res) => {
        if (res && res.status === 200) {
          this.deptList = res.body.map(
            (item) => `${item.deptid} : ${item.descr}`
          );
          if (!this.showModal) {
            this.listForm.controls.attribDept.setValue(
              this.deptList[0].split(' : ')[0]
            );
          }
        } else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
    } else this.deptList = [];
  }

  getSceneList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSceneList,
      { company: this.headForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.sceneList = res.body.data;
          if (this.listTableData.length > 0) {
            let scene = this.headForm.controls.scene.value;
            this.headForm.controls.scene.setValue(scene);
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
      this.isSpinning = false;
    });
  }

  getCityList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCityList,
      { company: this.headForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.cityList = res.body.data;
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

  getPayeeList(value) {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetPayeeInfo +
        `?keyword=${value}`,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.payeeList = res.body.data;
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

  getCarType() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCarTypeList,
      { company: this.headForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.carTypeList = res.body.data;
          if (this.listTableData.length > 0) {
            this.listTableData
              .filter((o) => o.carTypeName == null)
              .map((o) => {
                o.carTypeName = this.carTypeList.filter(
                  (f) => f.type == o.carType
                )[0]?.name;
              });
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

  getOverMealExpenseAmt() {
    if (this.isSaveLoading) return;
    if (
      this.listForm.controls.startingTime.value >
      this.listForm.controls.backTime.value
    ) {
      this.message.error(
        this.translate.instant('tips.starttime-later-than-backtime'),
        { nzDuration: 6000 }
      );
      if (!!this.listForm.controls.curr.value)
        this.listForm.controls.curr.reset();
      if (!!this.listForm.controls.expenseAmt.value)
        this.listForm.controls.expenseAmt.reset();
      return;
    }
    let gotimeDate = format(
      this.listForm.controls.startingTime.value,
      'yyyy-MM-dd'
    );
    let gotimespan = format(
      this.listForm.controls.startingTime.value,
      'HH:mm:ss'
    );
    let gotime = gotimeDate + 'T' + gotimespan;
    let backtimeDate = format(
      this.listForm.controls.backTime.value,
      'yyyy-MM-dd'
    );
    let backtimespan = format(
      this.listForm.controls.backTime.value,
      'HH:mm:ss'
    );
    let backtime = backtimeDate + 'T' + backtimespan;
    let param = {
      city: this.listForm.controls.cityOnBusiness.value,
      gotime: gotime,
      backtime: backtime,
      company: this.headForm.controls.companyCode.value,
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetOverMealExpenseAmt,
      param
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.listForm.controls.curr.setValue(res.body.data.currency);
          this.listForm.controls.expenseAmt.setValue(res.body.data.amount);
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

  ////////带选择框表
  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: GeneralExpenseInfo[] = [];
  setOfCheckedId = new Set<number>();
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: GeneralExpenseInfo[]): void {
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

  addItem(): void {
    if (!this.userInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      return;
    }
    if (!this.headForm.controls.scene.value) {
      this.message.error(this.translate.instant('tips.select-expense-scene'), {
        nzDuration: 6000,
      });
      return;
    }
    this.addloading = true;
    this.keyId++;
    let deptId = null;
    if (this.headForm.controls.companyCode.value == this.userInfo.company) {
      this.getDeptList(this.costdeptid);
      // this.deptList.push(this.costdeptid);
      deptId = this.costdeptid;
    }
    this.listForm.reset({
      attribDept: deptId,
      id: this.keyId,
      curr: this.curr,
      exchangeRate: 1,
      disabled: false,
    });
    this.listForm.controls.digest.enable();
    this.showModal = true;
  }

  editRow(id: number): void {
    this.editloading = true;
    let rowFormData = this.listTableData.filter((d) => d.id == id)[0];
    if (rowFormData != null) {
      this.deptList.push(rowFormData.attribDept);
      this.payeeList = [];
      this.payeeList.push({
        payeeid: rowFormData.payeeId,
        payeename: rowFormData.payeeName,
        payeedept: rowFormData.payeeDeptid,
        bank: rowFormData.bankName,
      });
      this.listForm.reset(rowFormData);
      this.listForm.controls.startingTime.setValue(
        new Date(
          rowFormData.startingTime == null
            ? null
            : new Date().toLocaleDateString() + ' ' + rowFormData.startingTime
        )
      );
      this.listForm.controls.backTime.setValue(
        new Date(
          rowFormData.backTime == null
            ? null
            : new Date().toLocaleDateString() + ' ' + rowFormData.backTime
        )
      );
      this.showModal = true;
    }
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

  setValidatorOfListForm(): void {
    Object.values(this.listForm.controls).forEach((control) => {
      control.clearValidators();
      control.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    });
    let exceptList = [
      'payeeDeptid',
      'bankName',
      'payeeName',
      'exchangeRate',
      'toLocalAmt',
      'attribDept',
      'sceneName',
      'carTypeName',
    ];
    let columnsList = this.listTableColumn
      .filter((o) => exceptList.indexOf(o.columnKey) == -1)
      .map((o) => o.columnKey);
    if (this.type == 'drive') columnsList.push('carType');
    columnsList.map((o) => {
      if (o == 'feeDate')
        this.listForm.get(o)!.setValidators(this.dateValidator);
      else this.listForm.get(o)!.setValidators(Validators.required);
      this.listForm.get(o)!.markAsDirty();
      this.listForm.get(o)!.updateValueAndValidity();
    });
    this.listForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
  }

  handleOk(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    this.setValidatorOfListForm(); // 设置验证逻辑
    if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true, emitEvent: false });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let amount = this.listForm.controls.expenseAmt.value;
    if (amount == 0) {
      this.message.error(this.translate.instant('amount-zero-error'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    this.setListTableData();
  }

  setListTableData(): void {
    let amount = this.listForm.controls.expenseAmt.value;
    let exchangeRate = this.listForm.controls.exchangeRate.value;
    this.listForm.controls.toLocalAmt.setValue(
      Number((amount * exchangeRate).toFixed(2))
    );

    // 转换日期
    let feeType = typeof this.listForm.controls.feeDate.value;
    if (this.listForm.controls.feeDate.value != null && feeType != 'string') {
      let feeDate =
        this.listForm.controls.feeDate.value == null
          ? null
          : format(this.listForm.controls.feeDate.value, 'yyyy/MM/dd');
      this.listForm.controls.feeDate.setValue(feeDate);
    }

    let rowId = this.listForm.controls.id.value;
    this.listTableData = this.listTableData.filter((o) => o.id != rowId);
    let data = this.listForm.getRawValue();

    if (
      !!this.listForm.controls.startingTime.value &&
      !!this.listForm.controls.backTime.value
    ) {
      // 转换时间格式-timepick组件不允许赋值为string类型
      data.startingTime = format(
        this.listForm.controls.startingTime.value,
        'HH:mm'
      );
      data.backTime = format(this.listForm.controls.backTime.value, 'HH:mm');
    }

    if (!!this.listForm.controls.carType.value) {
      let carTypeName = this.carTypeList.filter(
        (o) => o.type == this.listForm.controls.carType.value
      )[0].name;
      data.carTypeName = carTypeName;
    }

    this.listTableData.push(data);
    this.listTableData = [...this.listTableData]; // 刷新表格

    this.setStatistic();
    this.showModal = false;
    this.isSaveLoading = false;
    this.addloading = false;
    this.editloading = false;
    this.isSpinning = false;
  }

  setStatistic(): void {
    let appliedTotal = 0;
    this.listTableData.map((o) => {
      appliedTotal += Number(o.toLocalAmt);
    });
    let selfTax = 0;
    let invoiceDetailList = this.headForm.controls.invoiceDetailList.value;
    invoiceDetailList
      .filter((o) => o.affordPartyValue == 'self')
      .map((o) => (selfTax += Number(o.toLocalTaxLoss)));
    this.headForm.controls.appliedTotal.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
    this.headForm.controls.actualTotal.setValue(
      Number((appliedTotal - selfTax).toFixed(2)).toLocaleString('zh-CN')
    );
    this.headForm.controls.totalAmount.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
    if (this.listTableData.length > 0) {
      this.headForm.controls.scene.disable();
      let nameList = this.listTableData.map((o) => {
        return { emplid: o.payeeId, label: o.payeeId + '/' + o.payeeName };
      });
      this.nameList = nameList.reduce((accumalator, current) => {
        if (
          !accumalator.some(
            (item) =>
              item.emplid === current.emplid && item.label === current.label
          )
        ) {
          accumalator.push(current);
        }
        return accumalator;
      }, []);
      let emplidList = this.nameList.map((o) => {
        return o.emplid;
      });
      let detailTableData = this.headForm.controls.invoiceDetailList.value; // 使异常报销明细中失效的underwriter重置
      detailTableData
        .filter(
          (o) =>
            o.affordPartyValue == 'self' &&
            !!o.underwriter &&
            emplidList.indexOf(o.underwriter) == -1
        )
        .map((o) => (o.underwriter = null));
      this.headForm.controls.invoiceDetailList.setValue(detailTableData);
      this.setDetailTableShowData();
    } else this.nameList = [];
    if (
      !this.headForm.controls.scene.enabled &&
      this.fileList.length == 0 &&
      this.headForm.controls.invoiceDetailList.value.length == 0 &&
      this.listTableData.length == 0 &&
      this.attachList.length == 0
    )
      this.headForm.controls.scene.enable();
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  exSetStatistic(): void {
    let appliedTotal = 0;
    this.listTableData.map((o) => {
      appliedTotal += Number(o.toLocalAmt);
    });
    let selfTax = 0;
    let invoiceDetailList = this.headForm.controls.invoiceDetailList.value;
    invoiceDetailList
      .filter((o) => o.affordPartyValue == 'self')
      .map((o) => (selfTax += Number(o.toLocalTaxLoss)));
    this.headForm.controls.individualTax.setValue(selfTax);
    this.headForm.controls.actualTotal.setValue(
      Number((appliedTotal - selfTax).toFixed(2))
    );
    // 异常提示：
    this.exTotalWarning = [];
    let idx = 0;
    invoiceDetailList.map((o) => {
      if (!!o.exTips || !o.disabled) {
        this.exTotalWarning.push(++idx + '. ' + o.exTips);
      }
    });
    if (
      this.headForm.controls.scene.enabled &&
      (this.fileList.length > 0 ||
        invoiceDetailList.length > 0 ||
        this.listTableData.length > 0 ||
        this.attachList.length > 0)
    )
      this.headForm.controls.scene.disable();
    if (
      !this.headForm.controls.scene.enabled &&
      this.fileList.length == 0 &&
      invoiceDetailList.length == 0 &&
      this.listTableData.length == 0 &&
      this.attachList.length == 0
    )
      this.headForm.controls.scene.enable();
  }

  addExceptionItem(): void {
    this.detailForm.reset({
      disabled: false,
      index: Guid.create().toString(),
      curr: this.curr,
    });
    this.exceptionModal = true;
  }

  handleExOk(): void {
    this.isSaveLoading = true;
    if (!this.detailForm.valid) {
      Object.values(this.detailForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'), {
        nzDuration: 6000,
      });
      this.isSaveLoading = false;
      return;
    }
    let affordParty = this.detailForm.controls.affordParty.value;
    affordParty = affordParty.split('-');
    if (affordParty.length > 1)
      this.detailForm.controls.affordParty.setValue(affordParty[1]);
    this.detailForm.controls.affordPartyValue.setValue(affordParty[0]);
    // 增加异常提示
    let detailFormData = this.detailForm.getRawValue();
    detailFormData['invoiceNo'] =
      detailFormData['invoiceNo'] == null
        ? this.translate.instant('null')
        : detailFormData['invoiceNo'];
    let exTips = this.commonSrv.FormatString(
      this.translate.instant('manual-exception-warning-sample'),
      detailFormData['invoiceNo'],
      detailFormData['amount'],
      detailFormData['curr'],
      detailFormData['reason'],
      detailFormData['taxLoss'],
      detailFormData['affordParty']
    );
    this.detailForm.controls.exTips.setValue(exTips);

    let taxLoss = this.detailForm.controls.taxLoss.value;
    this.detailForm.controls.toLocalTaxLoss.setValue(taxLoss);
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    let item = this.detailForm.getRawValue();
    item['oamount'] = item.amount;
    detailTableData.push(item);
    this.headForm.controls.invoiceDetailList.updateValueAndValidity();
    this.exSetStatistic();
    this.setDetailTableShowData();
    this.isSaveLoading = false;
    this.exceptionModal = false;
  }

  handleExCancel(): void {
    this.exceptionModal = false;
  }

  SelectUnderwriter(value: string, index: string): void {
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    detailTableData
      .filter((d) => d.index == index)
      .map((o) => (o.underwriter = value));
    this.headForm.controls.invoiceDetailList.setValue(detailTableData);
    this.setDetailTableShowData();
  }

  deleteExRow(item: any): void {
    if (item.disabled) {
      this.message.warning(this.translate.instant('delete-warning'));
      return;
    }
    let index = item.index;
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter((d) => d.index != index);
    this.headForm.controls.invoiceDetailList.setValue(detailTableData);
    this.exSetStatistic();
    this.setDetailTableShowData();
  }

  getBpmRnoFile() {
    if (!this.headForm.controls.fileCategory.value) {
      this.message.error(this.translate.instant('input-expense-scene-first'), {
        nzDuration: 6000,
      });
      return;
    }
    this.isSaveLoading = true;
    let bpmRno = this.headForm.controls.bpmRno.value;
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetBPMAttachment,
      { rno: bpmRno }
    ).subscribe(async (res) => {
      if (res && res.status === 200 && res.body.status == 1) {
        let fileUrl = res.body.data;
        if (fileUrl != null) {
          this.attachList.push({
            uid: Guid.create().toString(),
            name: bpmRno + '.pdf',
            filename: bpmRno + '.pdf',
            type: 'application/pdf',
            status: 'done',
            url: fileUrl,
            source: 'bpm',
            category: this.listForm.controls.fileCategory.value,
            originFileObj: await this.commonSrv.getFileData(
              res.body.data,
              bpmRno + '.pdf',
              'application/pdf'
            ),
          });
          this.headForm.controls.attachList.setValue(this.attachList);
          this.attachList = [...this.attachList];
          if (
            this.headForm.controls.scene.enabled &&
            this.attachList.length > 0
          )
            this.headForm.controls.scene.disable();
        }
        this.canUpload = false;
      } else {
        let msg =
          this.translate.instant('operate-failed') +
          (res.status === 200
            ? res.body.message
            : this.translate.instant('server-error'));
        this.message.error(msg, { nzDuration: 6000 });
      }
      this.isSaveLoading = false;
    });
  }

  uploadInvoice(): void {
    this.isSaveLoading = true;
    this.isSpinning = true;
    const formData = new FormData();
    const uidDictionary: { [key: string]: string } = {};
    let idx = -1;
    this.fileList
      .filter((o) => !o.upload)
      .forEach((file: any) => {
        uidDictionary[(++idx).toString()] = file.uid;
        formData.append(idx.toString(), file.originFileObj);
      });
    // this.Service.Post('http://localhost:5000' + URLConst.PostFileToRead, formData).subscribe((res) => {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.PostFileToRead,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.exInfo = res.body.list; // 读出来的发票信息
        this.exInfo.forEach((o) => {
          o.item = uidDictionary[o.item];
        });
        this.exWarning = [];
        let exInfoWarning = this.exInfo.filter(
          (o) => o['expcode'] != '' && o['expcode'] != null
        ); // 异常发票信息
        if (this.exInfo.length > 0) {
          this.exInfo.forEach((item) => {
            // 改文件名
            item['curr'] = this.curr; // 默认币别与本位币别一致
            var aList = document
              .getElementById('invoiceUpload')
              .getElementsByClassName('ant-upload-list-item-name');
            for (var i = 0; i < aList.length; i++) {
              let title = this.fileList.filter((o) => o.uid == item.item)[0]
                .name;
              if (aList[i].getAttribute('title') == title) {
                aList[i].innerHTML = item.invdesc;
                aList[i].setAttribute('title', item.invdesc);
              }
            }
            this.fileList.filter((o) => o.uid == item.item)[0].name =
              item.invdesc;
            this.fileList.filter((o) => o.uid == item.item)[0].filename =
              item.invdesc;
          });
          if (exInfoWarning.length > 0) {
            // 展示异常弹窗
            // this.exInfo.map(o => this.exWarning += this.commonSrv.FormatString(this.translate.instant('exception-warning'), o['invno'], o['expdesc']) + '<br>');
            let canExpenseList = exInfoWarning.filter((o) => o.paymentStat); // 若异常报销发票都为无法请款发票，则先存正常及未识别出来的文件及数据
            if (canExpenseList.length == 0) {
              this.canConfirm = null;
              this.saveExInfo();
            } else this.canConfirm = this.translate.instant('confirm');

            exInfoWarning.map((o) => {
              this.exWarning.push(o['expcode']);
            });
            this.exTip =
              this.canConfirm != null
                ? this.commonSrv.FormatString(
                    this.translate.instant('exception-tip'),
                    res.body.amount
                  )
                : '';
            this.tipAffordParty = '';
            this.tipModal = true;
          } else this.saveExInfo(); // 全部无异常
        } else {
          this.fileList.map((o) => {
            o.upload = true;
            o.url = !o.url ? '...' : o.url;
          }); // 全部读不出
          this.isAllUpload = true;
        }
      } else {
        this.message.error('Failed to upload, please try it again.', {
          nzDuration: 6000,
        });
      }
      this.fileList = [...this.fileList];
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    // 上传bpm & 批量文件
    return new Observable((observer: Observer<boolean>) => {
      let uploadedFile;
      if (!this.batchUploadModal)
        uploadedFile = this.attachList.filter(
          (o) => o.originFileObj.name == file.name
        );
      else
        uploadedFile = this.batchUploadList.filter(
          (o) => o.originFileObj.name == file.name
        );
      let upload = uploadedFile.length == 0;
      if (!upload)
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('has-been-uploaded-that'),
            uploadedFile[0].originFileObj.name,
            uploadedFile[0].name
          ),
          { nzDuration: 6000 }
        );

      // upload = !((this.batchUploadList.length > 0 && this.batchUploadModal) || (this.attachList.length > 0 && !this.batchUploadModal));
      upload = !(this.batchUploadList.length > 0 && this.batchUploadModal);
      if (!upload)
        this.message.error(this.translate.instant('can-upload-only-one-item'), {
          nzDuration: 6000,
        });
      observer.next(upload);
      observer.complete();
    });
  };

  beforeInvoiceUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let uploadedFile = this.fileList.filter(
        (o) => o.originFileObj.name == file.name
      );
      let upload = uploadedFile.length == 0;
      if (!upload)
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('has-been-uploaded-that'),
            uploadedFile[0].originFileObj.name,
            uploadedFile[0].name
          ),
          { nzDuration: 6000 }
        );
      observer.next(upload);
      observer.complete();
    });
  };

  beforeAttachmentUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let uploadedFile = this.attachmentList.filter(
        (o) => o.originFileObj.name == file.name
      );
      let upload = uploadedFile.length == 0;
      if (!upload)
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('has-been-uploaded-that'),
            uploadedFile[0].originFileObj.name,
            uploadedFile[0].name
          ),
          { nzDuration: 6000 }
        );
      observer.next(upload);
      observer.complete();
    });
  };

  removeFile = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      let detailTableData = this.headForm.controls.invoiceDetailList.value;
      detailTableData = detailTableData.filter((o) => o.uid != file.uid);
      this.headForm.controls.invoiceDetailList.setValue(detailTableData);
      this.exSetStatistic();
      this.setDetailTableShowData();
      observer.next(true);
      observer.complete();
    });
  };

  removeAttach = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      this.headForm.controls.bpmRno.reset();
      this.headForm.controls.attachList.setValue(this.attachList);
      observer.next(true);
      observer.complete();
    });
  };

  handleAttachChange(info: NzUploadChangeParam): void {
    // BPM & 大量上传文件
    let attachList = [...info.fileList];
    attachList = attachList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      if (!this.batchUploadModal) {
        file.source = 'upload';
        file.category = this.headForm.controls.fileCategory.value;
      }
      return file;
    });
    if (!this.batchUploadModal) {
      this.attachList = attachList;
      this.headForm.controls.attachList.setValue(this.attachList);
      this.canUpload = false;
      if (this.attachList.length > 0) this.headForm.controls.scene.disable();
      if (
        !this.headForm.controls.scene.enabled &&
        this.fileList.length == 0 &&
        this.headForm.controls.invoiceDetailList.value.length == 0 &&
        this.listTableData.length == 0 &&
        this.attachList.length == 0
      )
        this.headForm.controls.scene.enable();
    } else this.batchUploadList = attachList;
  }

  handleInvoiceFileChange(info: NzUploadChangeParam): void {
    // 发票
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      if (!file.url) file.url = '...';
      // file.id = this.listForm.controls.id.value;
      file.upload = file.upload ? true : false;
      file.url = file.upload ? (!file.url ? '...' : file.url) : null;
      return file;
    });
    this.exWarning = [];
    this.fileList = fileList;
    this.isAllUpload = this.fileList.filter((o) => !o.upload).length == 0;
    if (this.fileList.length > 0) this.headForm.controls.scene.disable();
    if (
      !this.headForm.controls.scene.enabled &&
      this.fileList.length == 0 &&
      this.headForm.controls.invoiceDetailList.value.length == 0 &&
      this.listTableData.length == 0 &&
      this.attachList.length == 0
    )
      this.headForm.controls.scene.enable();
  }

  handleAttachmentChange(info: NzUploadChangeParam): void {
    // 附件
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      if (!file.url) file.url = '...';
      return file;
    });
    this.attachmentList = fileList;
  }

  clickBatchUpload(): void {
    if (this.userInfo.isMobile) {
      this.message.info(this.translate.instant('only-pc'), {
        nzDuration: 6000,
      });
      return;
    }
    if (!this.userInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      return;
    }
    if (this.type == null) {
      this.message.error(this.translate.instant('tips.select-expense-scene'), {
        nzDuration: 6000,
      });
      return;
    }
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
    let url =
      this.type == 'default'
        ? URLConst.BatchUploadDefault
        : this.type == 'overtimeMeal'
        ? URLConst.BatchUploadOvertmeMeal
        : URLConst.BatchUploadDrive;
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + url,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        let dataList = res.body.data?.map((o) => {
          // let seq = this.
          return {
            attribDept: o.deptid,
            startingPlace: o.departureplace,
            cityOnBusiness: o.businesscity,
            feeDate: format(new Date(o.cdate), 'yyyy/MM/dd'),
            carType: o.vehiclevalue,
            carTypeName: o.vehicletype,
            kil: o.kilometers,
            digest: o.summary,
            startingTime:
              o.departuretime == null
                ? null
                : format(new Date(o.departuretime), 'HH:mm'),
            backTime:
              o.backtime == null ? null : format(new Date(o.backtime), 'HH:mm'),
            curr: o.currency == null ? this.curr : o.currency,
            expenseAmt: o.amount == null ? o.total : o.amount,
            exchangeRate: o.rate,
            toLocalAmt: o.baseamt == null ? o.total : o.baseamt,
            id: ++this.keyId,
            disabled: false,
            payeeId: o.payeeid,
            payeeName: o.payeename,
            payeeDeptid: o.payeedept,
            bankName: o.bank,
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

  handleTipOk(): void {
    this.isSaveLoading = true;
    if (this.tipAffordParty == '' && this.canConfirm != null) {
      this.message.error(
        this.translate.instant('afford-party-cannot-be-null'),
        { nzDuration: 6000 }
      );
      this.isSaveLoading = false;
      return;
    }
    this.saveExInfo();
    this.isSaveLoading = false;
    this.tipModal = false;
  }

  addInvoice(value): void {
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter((o) => !o.disabled);
    this.fileList = [];
    value.forEach((o) => {
      if (!o.baseamt) {
        this.message.error(
          `Invalid invoice: Invoice no.${o.invoiceNo}, please contact the administrator.`,
          { nzDuration: 6000 }
        );
      }
      let item = o;
      detailTableData.push(item);
      this.fileList.push({
        id: o.id,
        uid: o.invoiceid,
        url: o.fileurl,
        name: o.invtype + o.oamount,
        upload: true,
      });
    });
    this.headForm.controls.invoiceDetailList.setValue(detailTableData);
    this.exSetStatistic();
    this.setDetailTableShowData();
    this.isAllUpload = true;
  }

  saveExInfo() {
    //删除无法请款的文件及item
    let cannotExpenseUid = this.exInfo
      .filter((o) => !o.paymentStat)
      .map((o) => o.item);
    if (cannotExpenseUid.length > 0) {
      this.fileList = this.fileList.filter(
        (o) => cannotExpenseUid.indexOf(o.uid) == -1
      );
    }
    this.exInfo = this.exInfo.filter((o) => o.paymentStat);
    if (this.exInfo.length > 0) {
      let detailTableData = this.headForm.controls.invoiceDetailList.value;
      this.exInfo.forEach((item) => {
        detailTableData.push({
          invoiceCode: item['invcode'],
          invoiceNo: item['invno'],
          amount: item['amount'],
          taxLoss: item['taxloss'],
          curr: item['curr'],
          affordParty:
            item['expcode'] != '' && item['expcode'] != null
              ? this.tipAffordParty.split('-')[1]
              : null,
          disabled: true,
          // id: rowId,
          index: item['item'],
          uid: item['item'],
          exTips: item['expcode'],
          toLocalTaxLoss: item['taxloss'],
          affordPartyValue:
            item['expcode'] != '' && item['expcode'] != null
              ? this.tipAffordParty.split('-')[0]
              : null,
          reason: item['reason'],
          invdate: item['invdate'],
          taxamount: item['taxamount'],
          oamount: item['oamount'],
          invstat: item['invstat'],
          abnormalamount: item['abnormalamount'],
          paymentName: item['paymentName'],
          paymentNo: item['paymentNo'],
          collectionName: item['collectionName'],
          collectionNo: item['collectionNo'],
          expdesc: item['expdesc'],
          expcode: item['expcode'],
          invdesc: item['invdesc'],
        });
      });
      // this.detailTableData = [...this.detailTableData];   // 刷新表格
      this.exSetStatistic();
      this.setDetailTableShowData();
    }
    this.fileList.map((o) => {
      o.upload = true;
      o.url = !o.url ? '...' : o.url;
    });
    this.isAllUpload = true;
    this.fileList = [...this.fileList];
  }

  setDetailTableShowData() {
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    if (!!detailTableData)
      this.detailTableShowData = detailTableData.filter(
        (o) => o.exTips != null && o.exTips != ''
      );
    this.detailTableShowData = [...this.detailTableShowData]; // 刷新表格
  }

  handleTipCancel(): void {
    this.exInfo = [];
    this.fileList = this.fileList.filter((o) => o.upload);
    let detailTableData = this.headForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter(
      (o) =>
        !o.disabled ||
        this.fileList.map((t) => t.uid).indexOf(o.uid.toString()) != -1
    );
    this.setDetailTableShowData();
    this.tipModal = false;
  }

  checkParam(): boolean {
    if (!this.userInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.canUpload) {
      this.message.error(this.translate.instant('not-complete-upload'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.fileList.filter((o) => !o.upload).length > 0) {
      this.message.error(this.translate.instant('upload-invoice-required'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    let fileRequired =
      this.headForm.controls.invoiceDetailList.validator != null;
    if (
      fileRequired &&
      this.headForm.controls.invoiceDetailList.value.length == 0 &&
      this.fileList.length == 0
    ) {
      this.message.error(this.translate.instant('add-invoice-required'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    let attachRequired = this.headForm.controls.attachList.validator != null;
    if (attachRequired && this.attachList.length == 0) {
      let msg = this.bpmFlag
        ? this.translate.instant('tips.bpm-attach-required')
        : this.commonSrv.FormatString(
            this.translate.instant('tips.attach-required'),
            this.headForm.controls.fileCategory.value
          );
      this.message.error(msg, { nzDuration: 6000 });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
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
    if (
      this.headForm.controls.invoiceDetailList.value.length > 0 &&
      this.headForm.controls.invoiceDetailList.value.filter(
        (o) => o.affordPartyValue == 'self' && !o.underwriter
      ).length > 0
    ) {
      this.message.error(this.translate.instant('select-underwriter'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    let amount: number = this.headForm.controls.appliedTotal.value
      .toString()
      ?.replace(/,/g, '');
    let invoiceExAmount = 0;
    let totalExAmount = 0;
    this.headForm.controls.invoiceDetailList.value.map((o) => {
      if (o.disabled) {
        invoiceExAmount += Number(o.baseamt);
        totalExAmount += Number(o.baseamt);
      } else totalExAmount += Number(o.amount);
    });
    if (invoiceExAmount > 0) {
      if (
        Number(Number(totalExAmount).toFixed(2)) <
        Number(Number(amount).toFixed(2))
      ) {
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('amount-error-ex-ap'),
            totalExAmount.toLocaleString(),
            amount.toLocaleString()
          ),
          { nzDuration: 6000 }
        );
        this.isSpinning = false;
        this.isSaveLoading = false;
        return;
      }
    } else if (
      totalExAmount != 0 &&
      Number(totalExAmount).toFixed(2) != Number(amount).toFixed(2)
    ) {
      this.message.error(
        this.commonSrv.FormatString(
          this.translate.instant('amount-not-equal-ex-ap'),
          totalExAmount.toLocaleString(),
          amount.toLocaleString()
        ),
        { nzDuration: 6000 }
      );
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq501,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(this.translate.instant('submit-successfully'));
        // let tips = res.body.data.stat ? this.translate.instant('required-send-fin') + (res.body.message == null ? '' : res.body.message) : this.translate.instant('no-required-send-fin')
        this.commonSrv.SendMobileSignXMLData(res.body.data.rno);
        let tips = res.body.message;
        this.modal.info({
          nzTitle: this.translate.instant('tips'),
          nzContent: `<p>${tips}</p><p>Request NO: ${res.body.data.rno}</p>`,
          nzOnOk: () => this.router.navigateByUrl(`ers/rq501`), // reset form data
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq501,
      formData
    ).subscribe((res) => {
      // this.Service.Post('http://localhost:5000' + URLConst.SaveRq501, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(
          this.translate.instant('save-successfully') +
            `Request NO: ${res.body.data.rno}`
        );
        this.headForm.controls.rno.setValue(res.body.data.rno);
      } else
        this.message.error(
          this.translate.instant('save-failed') +
            (res.status === 200
              ? res.body.message ?? res.body.data?.message
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
      // amount: this.headForm.controls.appliedTotal.value,
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payee.value,
    };
    formData.append('head', JSON.stringify(headData));

    let sceneData = this.sceneList.filter(
      (o) => o.expcode == this.headForm.controls.scene.value
    )[0];
    let listData = this.listTableData.map((o) => {
      let selfAmt = 0;
      this.headForm.controls.invoiceDetailList.value
        ?.filter((f) => f.id == o.id && f.affordPartyValue == 'self')
        .forEach((f) => (selfAmt += Number(f.toLocalTaxLoss)));
      let amount = Number((o.toLocalAmt - selfAmt).toFixed(2));
      return {
        rno: rno,
        seq: o.id,
        expcode: sceneData?.expcode,
        expname: sceneData?.expname,
        deptid: o.attribDept,
        payeeid: o.payeeId,
        payeename: o.payeeName,
        payeedeptid: o.payeeDeptid,
        bank: o.bankName,
        location: o.startingPlace,
        city: o.cityOnBusiness,
        rdate: o.feeDate,
        cartype: o.carType == null ? 0 : o.carType,
        journey: o.kil,
        summary: o.digest,
        gotime: o.startingTime,
        backtime: o.backTime,
        currency: o.curr,
        basecurr: this.curr,
        amount1: o.expenseAmt,
        rate: o.exchangeRate,
        baseamt: o.toLocalAmt,
        acctcode: sceneData?.acctcode,
        acctname: sceneData?.acctname,
        amount: amount,
      };
    });
    formData.append('detail', JSON.stringify(listData));

    let indx = 0;
    this.fileList.map((o) => {
      o.index = null;
    });
    let detailData = this.headForm.controls.invoiceDetailList.value.map((o) => {
      o.index = ++indx;
      if (o.disabled)
        this.fileList.filter((f) => o.uid == f.uid)[0].index = indx;
      return {
        seq: 1,
        item: Number(o.index),
        invcode: o.invoiceCode,
        invno: o.invoiceNo,
        amount: o.amount,
        taxloss: Number(o.taxLoss),
        curr: o.curr,
        undertaker: o.affordPartyValue,
        invdate: o.invdate,
        taxamount: o.taxamount,
        oamount: o.oamount,
        invstat: o.invstat,
        abnormalamount: o.abnormalamount,
        paymentName: o.paymentName,
        paymentNo: o.paymentNo,
        collectionName: o.collectionName,
        collectionNo: o.collectionNo,
        expdesc: o.expdesc,
        expcode: o.exTips,
        invdesc: o.invdesc,
        invtype: o.invtype,
        reason: o.reason,
        abnormal: o.disabled ? 'N' : 'Y',
        underwriter: o.underwriter,
        invoiceid: o.invoiceid,
        invabnormalreason: o.invabnormalreason,
      };
    });
    this.fileList
      .filter((o) => o.index == null)
      .map((o) => {
        o.index = ++indx;
      });
    formData.append('inv', JSON.stringify(detailData));
    let invoiceFileData = this.fileList.map((o) => {
      return {
        rno: rno,
        seq: 1,
        item: o.index,
        filetype: o.type,
        filename: o.name,
        ishead: 'Y',
        status: 'I',
        key: o.uid,
      };
    });
    indx = 0;
    let attachFileData = this.attachList.map((o) => {
      return {
        rno: rno,
        seq: 1,
        item: ++indx,
        filetype: o.type,
        filename: o.name,
        category: o.category,
        ishead: 'Y',
        status: 'F',
        key: o.uid,
      };
    });
    indx = 0;
    let attachmentData = this.attachmentList.map((o) => {
      return {
        rno: rno,
        seq: 0,
        item: ++indx,
        filetype: o.type,
        filename: o.name,
        ishead: 'Y',
        status: null,
        key: o.uid,
      };
    });
    // formData.append('file', JSON.stringify(invoiceFileData.concat(attachmentData.concat(attachFileData))));
    formData.append(
      'file',
      JSON.stringify(attachmentData.concat(attachFileData))
    );

    let amountData = {
      rno: rno,
      currency: this.curr,
      amount: this.headForm.controls.appliedTotal.value
        .toString()
        ?.replace(/,/g, ''),
      actamt: this.headForm.controls.actualTotal.value,
    };
    formData.append('amount', JSON.stringify(amountData));

    // this.fileList.forEach((file: any) => {
    //   formData.append(file.uid, file.originFileObj);
    // });
    this.attachList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
  }
}
