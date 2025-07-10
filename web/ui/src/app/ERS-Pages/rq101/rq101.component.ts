import { InvoiceDetail } from './../_components/invoices-modal/classes/data-item';
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
import { TableColumnModel } from 'src/app/shared/models';
import {
  OverdueChargeAgainstDetail,
  ExceptionDetail,
  GeneralExpenseInfo,
} from './classes/data-item';
import {
  DetailTableColumn,
  ExpenseTableColumn,
  OvertimeMealExpenseTableColumn,
  DriveFuelExpenseTableColumn,
  chargeAgainstTableColumn,
} from './classes/table-column';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { BehaviorSubject, firstValueFrom, Observable, Observer, Subject } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { catchError, debounceTime, filter, map, switchMap } from 'rxjs/operators';
import { CompanyInfo } from '../bd008/classes/data-item';
import _ from 'lodash';
import { Approver, AssignStep } from 'src/app/shared/models/assign-step';
import { ERSConstants } from 'src/app/common/constant';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-rq101',
  templateUrl: './rq101.component.html',
  styleUrls: ['./rq101.component.scss'],
})
export class RQ101Component implements OnInit, OnDestroy {
  navigationSubscription;

  //#region 参数
  nzFilterOption = (input, option) => {
    if (option.nzLabel.toLowerCase().indexOf(input.toLowerCase()) >= 0) {
      return true;
    }
    let data = this.sceneList.filter((o) => o.expensecode == option.nzValue)[0];
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
  curr: string;
  costdeptid: string;
  isaccount: boolean = false;
  showModal: boolean = false;
  exceptionModal: boolean = false;
  tipModal: boolean = false;
  batchUploadModal: boolean = false;
  isSaveLoading: boolean = false;
  detailListTableColumn = DetailTableColumn;
  listTableColumn = ExpenseTableColumn; // OvertimeMealExpenseTableColumn、DriveFuelExpenseTableColumn
  totalInvoiceDetailList: any[] = [];
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
  fileList: any[] = [];
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
  showChargeAgainst: boolean = false;
  overdueTableColumn = chargeAgainstTableColumn;
  chargeAgainstTableData: OverdueChargeAgainstDetail[] = [];
  chargeAgainstShowTableData: OverdueChargeAgainstDetail[] = [];
  selectedRno: string = null;
  canUpload = true;
  type: string = null;
  sampleUrl: string = null;
  sampleName: string = null;
  applicantInfo: any;
  userInfo: any;
  bpmFlag: boolean = false;
  sceneKeyword: string = '';
  getPJCodeTerms = new Subject<string>();
  attribDeptListControl: Array<{
    index: number;
    deptIdControl: string;
    percentControl: string;
  }> = [];
  userIdList: string[] = [];
  expname: string = '';
  cuser: string;
  taxRate: number = 0.25;
  paymentWayList: any[] = [];
  searchChange$ = new BehaviorSubject('');
  isLoading = false;
  employeeInfoList: Approver[] = [];
  // listOfSelectedValue: any[] = [];
  //#endregion

  onSearch(value: string): void {
    this.isLoading = true;
    this.searchChange$.next(value);
  }
  markAbnormalIdx: number = -1;
  requirePaperAttach: boolean = false;
  requirePaperInvoice: boolean = false;
  requirePaperInvoice2: boolean = false;
  selectedArea: string = '';
  taxLossInfo: string = '';
  selectedVatrate: number = 0;
  pageIndex: number = 1;
  pageSize: number = 10;
  selectedSceneData: any = null;
  descriptionNotice: string = '';
  attachmentNotice: string = '';
  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    // private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    public commonSrv: CommonService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    public domSanitizer: DomSanitizer
  ) {

    const getEmployeeInfoList = (keyword: string): Observable<any> =>
      this.Service
          .doGet(URLConst.QueryEmployeeInfos + `?keyword=${keyword}`, null)
          .pipe(
            // tap(data => console.log(data)),
            catchError((err, caught) => {
              console.log(err);
              throw err;
            }),
            map((res: any) => res.body.data),
            map((list: Approver[]) => list.map((item: any) =>
              {
                return {
                  emplid: item.emplid,
                  name: item.name,
                  nameA: item.nameA,
                  display: _.isEmpty(_.trim(item.nameA)) ? item.name : item.nameA,
                } as Approver;
              })
            ),
          );

    const employeeInfoList$: Observable<Approver[]> = this.searchChange$
      .asObservable()
      .pipe(
        filter((keyword: string) => !_.isEmpty(_.trim(keyword)) && keyword.length >= 2),
        debounceTime(300),
        switchMap(getEmployeeInfoList)
      );

    employeeInfoList$.subscribe(data => {
      this.employeeInfoList = data;
      this.isLoading = false;
    });

    this.navigationSubscription = this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.dataInitial();
      }
    });
  }

  async ngOnInit() {
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.headForm = this.fb.group({
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      applicantDept: [{ value: null, disabled: true }], //申请人部门
      emplid: [null],
      selectedUser: [null],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      payee: [{ value: null, disabled: true }],
      paymentWay: [{ value: null, disabled: false }],
      ext: [null],
      projectCode: [null],
      companyCode: [null, [Validators.required]],
      appliedTotal: [{ value: 0, disabled: true }],
      individualTax: [{ value: 0, disabled: true }],
      actualTotal: [{ value: 0, disabled: true }],
      totalAmount: [{ value: 0, disabled: true }],
      borrowedAmount: [{ value: 0, disabled: true }],
    });
    this.listForm = this.fb.group({
      advanceRno: [null],
      sceneName: [null],
      attribDept: [null],
      attribDeptList: [[]],
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
      attachList: [[]],
      fileList: [[]],
      invoiceDetailList: [[]],
      bpmRno: [null],
      fileCategory: [null],
      fileRequstTips: [null],
      scene: [null],
      id: [this.keyId],
      disabled: [false],
      senarioid: [null],
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
      id: [this.keyId],
      index: [null],
      toLocalTaxLoss: [0],
      affordPartyValue: [null],
      exTips: [null],
    });

    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
    let costDeptInfo = await this.getGetCostDeptid(this.userInfo?.deptid, this.userInfo?.company);
    this.userInfo.costdeptid = costDeptInfo?.body?.data;
    console.log('this.userInfo ',this.userInfo)
    this.getApplicant();
    this.columnValueChange();
    this.getRnoInfo();
    //this.getCompanyData();
    this.getCurrency();

    this.getPJCodeTerms.pipe(debounceTime(1000)).subscribe((term) => {
      this.getProjectCode(term);
    });
    this.getPaymentList();

    //预设收款人为申请人工号
    this.headForm.controls.emplid.setValue(this.userInfo?.emplid);
    let defaultUser: Approver ={ emplid: this.userInfo?.emplid, name: this.userInfo?.cname, nameA: this.userInfo?.ename, display: this.userInfo?.ename };
    this.employeeInfoList.push(defaultUser);
    this.headForm.controls.selectedUser.setValue(this.employeeInfoList[0])
  }

  getApplicant() {
    this.headForm.controls.applicantEmplid.setValue(this.userInfo?.emplid); //申请人
    this.headForm.controls.applicantName.setValue(this.userInfo?.cname); //申请人姓名
    this.headForm.controls.applicantDept.setValue(this.userInfo?.deptid); //申请人部门
    // this.commonSrv
    //   .getEmployeeInfoById(this.userInfo?.emplid)
    //   .subscribe((res) => {
    //     this.headForm.controls.applicantName.setValue(res?.name); //申请人姓名
    //     this.headForm.controls.applicantDept.setValue(res?.deptid); //申请人部门
    //   });
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
        this.taxRate = result[0]?.taxRate;
      }
    });
  }
  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  async columnValueChange() {

    this.headForm.controls.emplid.valueChanges.subscribe((value) => {
      if (
        !!value &&
        this.headForm.controls.emplid.enabled &&
        !this.isSpinning
      ) {
        this.isSpinning = true;
        this.commonSrv.getEmployeeInfoById(value).subscribe(async (res) => {
          this.applicantInfo = res;
          let costDeptInfo = await this.getGetCostDeptid(this.applicantInfo?.deptid, this.applicantInfo?.company);
          if(costDeptInfo) this.applicantInfo.costdeptid = costDeptInfo?.body?.data;
          this.getEmployeeInfo();
        });
        this.getCategoryList(value);
      }
    });
    this.headForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (!!value && this.headForm.controls.companyCode.enabled) {
        if (!!this.headForm.controls.projectCode.value) {
          this.headForm.controls.projectCode.reset();
        }
        this.getSceneList();
        if (!this.isSpinning) {
          this.showChargeAgainstModal();
        }
        this.getTaxRate(value);
      }
    });

    this.detailForm.get('amount').valueChanges.subscribe((value) => {
      // this.detailForm.controls.taxLoss.setValue(
      //   Number((value * this.exchangeRate * 0.25).toFixed(2))
      // );
      this.detailForm.controls.taxLoss.setValue(
        Number((value * this.exchangeRate * this.taxRate).toFixed(2))
      );
    });

    this.listForm.get('curr').valueChanges.subscribe((value) => {
      if (!this.isSaveLoading) {
        if (!!value && value != this.curr) {
          //获取汇率
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
              this.exchangeRate = 0;
            }
            this.afterChangeCurr(value);
          });
        } else if (value == this.curr) {
          this.exchangeRate = 1;
          this.afterChangeCurr(value);
        }
      }
    });

    this.listForm.controls.bpmRno.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'default') {
        if (value != null && value != '' && this.attachList.length == 0)
          this.canUpload = true;
        else this.canUpload = false;
      }
    });

    this.listForm.controls.scene.valueChanges.subscribe( async (value)  => {
      if (!this.isSaveLoading) {
        if (value != null && !!this.sceneList) {
          let selectedScene = this.sceneList.filter((o) => o.expensecode == value)[0];
          this.listForm.controls.senarioid.setValue(selectedScene?.id);
          const res = await this.getSenarioById(selectedScene.id);//this.sceneList.filter((o) => o.expensecode == value)[0];
          let sceneData: any = res?.body?.data;
          this.selectedSceneData = sceneData;
          this.descriptionNotice = sceneData?.descriptionnotice || '';
          this.attachmentNotice = sceneData?.attachmentnotice || '';
          this.expname = "(" + sceneData?.expensecode + ")" + sceneData?.senarioname; // 费用类别
          let typeToNum = !this.type
            ? null
            : this.type == 'default'
            ? 0
            : this.type == 'overtimeMeal'
            ? 1
            : 2;
          if (!sceneData) {
            this.message.error(
              this.translate.instant(
                'Invalid data, please contact the administrator.'
              ),
              { nzDuration: 6000 }
            );
            this.listForm.controls.scene.reset();
          } else if (
            (this.listTableData.length > 1 ||
              (this.listTableData.length == 1 && this.addloading)) &&
            !!this.type &&
            typeToNum != sceneData.extraformcode
          ) {
            this.message.error(
              this.translate.instant('tips.only-one-type-page'),
              { nzDuration: 6000 }
            );
            this.listForm.controls.scene.reset();
          } else {
            this.listForm.controls.sceneName.setValue(sceneData.senarioname);
            let type = sceneData.extraformcode;
            switch (type) {
              case 0: {
                this.type = 'default';
                this.listTableColumn = ExpenseTableColumn;
                // if (this.listForm.controls.curr.value != this.curr) this.listForm.controls.curr.setValue(this.curr);
                if (!this.listForm.controls.curr.enabled)
                  this.listForm.controls.curr.enable({ emitEvent: false });
                if (!this.listForm.controls.expenseAmt.enabled)
                  this.listForm.controls.expenseAmt.enable({
                    emitEvent: false,
                  });
                break;
              }
              case 1: {
                this.type = 'overtimeMeal';
                this.listTableColumn = OvertimeMealExpenseTableColumn;
                this.sampleUrl =
                  '../../../assets/file/overtimemeal-sample.xlsx';
                this.sampleName = this.translate.instant(
                  'sample.overtime-meal'
                );
                if (this.cityList.length == 0) this.getCityList();
                if (!!this.listForm.controls.curr.value){
                  this.listForm.controls.curr.reset();
                }
                if (!this.listForm.controls.curr.disabled){
                  this.listForm.controls.curr.disable({ emitEvent: false });
                }
                if (!this.listForm.controls.expenseAmt.disabled)
                  this.listForm.controls.expenseAmt.disable({
                    emitEvent: false,
                  });
                if (!this.listForm.controls.attachList.value)
                  this.listForm.controls.attachList.reset([]);
                if (!this.listForm.controls.fileList.value)
                  this.listForm.controls.fileList.reset([]);
                if (!this.listForm.controls.invoiceDetailList.value)
                  this.listForm.controls.invoiceDetailList.reset([]);
                break;
              }
              case 2: {
                this.type = 'drive';
                this.listTableColumn = DriveFuelExpenseTableColumn;
                this.sampleUrl = '../../../assets/file/selfdrive-sample.xlsx';
                this.sampleName = this.translate.instant('sample.drive');
                if (this.carTypeList.length == 0) this.getCarType();
                if (this.listForm.controls.curr.value != this.curr){
                  this.listForm.controls.curr.setValue(this.curr);
                }
                if (!this.listForm.controls.curr.disabled){
                  this.listForm.controls.curr.disable({ emitEvent: false });

                }
                if (!this.listForm.controls.expenseAmt.disabled)
                  this.listForm.controls.expenseAmt.disable({
                    emitEvent: false,
                  });
                if (!this.listForm.controls.attachList.value)
                  this.listForm.controls.attachList.reset([]);
                if (!this.listForm.controls.fileList.value)
                  this.listForm.controls.fileList.reset([]);
                if (!this.listForm.controls.invoiceDetailList.value)
                  this.listForm.controls.invoiceDetailList.reset([]);
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
            if (!!sceneData.category) {
              this.listForm.controls.fileCategory.setValue(
                sceneData.category.replace(/\bbpm/gi, '')
              );
              this.bpmFlag =
                sceneData.category.toLowerCase().indexOf('bpm') != -1;
            } else {
              this.bpmFlag = false;
              this.listForm.controls.fileCategory.reset();
            }
            this.listForm.controls.fileRequstTips.setValue(
              sceneData.attachmentnotice//filepoints
            );
            if (sceneData.requirespaperattatchment) {
              this.listForm
                .get('attachList')!
                .setValidators(Validators.required);
              this.listForm.get('attachList')!.markAsDirty();
            } else {
              this.listForm.get('attachList')!.clearValidators();
              this.listForm.get('attachList')!.markAsPristine();
            }
            if (sceneData.requiresinvoice) {
              this.listForm
                .get('invoiceDetailList')!
                .setValidators(Validators.required);
              this.listForm.get('invoiceDetailList')!.markAsDirty();
            } else {
              this.listForm.get('invoiceDetailList')!.clearValidators();
              this.listForm.get('invoiceDetailList')!.markAsPristine();
            }
            //*費用情景若設定需繳交紙本附件，則提示「需繳交紙本附件」
            let scene = this.sceneList?.find(s => s.expensecode === this.listForm.controls.scene.value);
            if (scene) {
              this.getSenarioByExpenseCode(scene.senarioname);
            }
            this.listForm.get('attachList')!.updateValueAndValidity();
            this.listForm.get('invoiceDetailList')!.updateValueAndValidity();
            this.requirePaperInvoiceTips();
          }
        } else {
          this.listForm.controls.fileCategory.reset();
          this.listForm.controls.fileRequstTips.reset();
          this.bpmFlag = false;
        }
      }
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
          if (!!this.listForm.controls.curr.value){
            this.listForm.controls.curr.reset();

          }
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
        }
      }
    });

    this.listForm.controls.startingTime.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'overtimeMeal') {
        if (!!value && !!this.listForm.controls.backTime.value) {
          if (value > this.listForm.controls.backTime.value) {
            this.message.error(
              this.translate.instant('tips.starttime-later-than-backtime'),
              { nzDuration: 6000 }
            );
            if (!!this.listForm.controls.curr.value){
              this.listForm.controls.curr.reset();

            }
            if (!!this.listForm.controls.expenseAmt.value)
              this.listForm.controls.expenseAmt.reset();
          } else if (!!this.listForm.controls.cityOnBusiness.value)
            this.getOverMealExpenseAmt();
        } else {
          if (!!this.listForm.controls.curr.value){
            this.listForm.controls.curr.reset();

          }
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
        }
      }
    });

    this.listForm.controls.backTime.valueChanges.subscribe((value) => {
      if (!this.isSaveLoading && this.type == 'overtimeMeal') {
        if (!!value && !!this.listForm.controls.startingTime.value) {
          if (this.listForm.controls.startingTime.value > value) {
            this.message.error(
              this.translate.instant('tips.starttime-later-than-backtime'),
              { nzDuration: 6000 }
            );
            if (!!this.listForm.controls.curr.value){
              this.listForm.controls.curr.reset();

            }
            if (!!this.listForm.controls.expenseAmt.value)
              this.listForm.controls.expenseAmt.reset();
          } else if (!!this.listForm.controls.cityOnBusiness.value)
            this.getOverMealExpenseAmt();
        } else {
          if (!!this.listForm.controls.curr.value){
            this.listForm.controls.curr.reset();

          }
          if (!!this.listForm.controls.expenseAmt.value)
            this.listForm.controls.expenseAmt.reset();
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
      // this.totalDetailTableData = [];
      // this.detailTableData = [];
      this.detailTableShowData = [];
      this.listTableData = [];
      this.keyId = 0;
      // this.uid = 0;
      this.exInfo = [];
      this.exWarning = [];
      this.exTip = '';
      this.exTotalWarning = [];
      this.tipAffordParty = '';
      this.isSpinning = false;
      this.isAllUpload = true;
      this.canConfirm = this.translate.instant('Confirm');
      this.exchangeRate = 1;
      // this.totalFileList = [];
      this.fileList = [];
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
      this.showPicList = false;
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

  afterChangeCurr(value): void {
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    if (
      !!detailTableData &&
      detailTableData.length > 0 &&
      detailTableData[0].curr != value
    ) {
      detailTableData.map((o) => {
        o.curr = value;
        if (!o.disabled) {
          o.taxLoss = Number(
            (o.amount * this.exchangeRate * this.taxRate).toFixed(2)
          );
          o.toLocalTaxLoss = o.taxLoss;
          o.exTips = this.commonSrv.FormatString(
            this.translate.instant('manual-exception-warning-sample'),
            o.invoiceNo == null ? this.translate.instant('null') : o.invoiceNo,
            o.amount.toLocaleString(),
            o.curr,
            o.reason,
            o.taxLoss.toLocaleString(),
            o.affordParty
          );
        }
      });
    }
  }

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
      this.headForm.controls.borrowedAmount.setValue(headData.amount);
    }
    if (summaryAtmData != null) {
      this.headForm.controls.appliedTotal.setValue(summaryAtmData.amount);
      this.headForm.controls.actualTotal.setValue(summaryAtmData.actamt);
      this.headForm.controls.totalAmount.setValue(summaryAtmData.actamt);
      this.headForm.controls.individualTax.setValue(
        Number((summaryAtmData.amount - summaryAtmData.actamt).toFixed(2))
      );
    }

    if (listData != null) {
      listData.map((o) => {
        let fileList = o.fileList
          .filter((f) => f.ishead == 'N' && f.status != 'F')
          .sort((a, b) => b.item - a.item)
          .map((f) => {
            return {
              id: f.seq,
              uid: f.invoiceid,
              name: f.filename,
              filename: f.tofn,
              status: 'done',
              url: this.commonSrv.changeDomain(f.url),
              type: f.filetype,
              originFileObj: null,
              upload: true,
              invoiceid: f.invoiceid,
              invno: f.invoiceno,
              category: f.category,
            };
          });
        let attachList = o.fileList
          .filter((f) => f.ishead == 'N' && f.status == 'F')
          .sort((a, b) => b.item - a.item)
          .map((f) => {
            return {
              id: f.seq,
              uid: Guid.create().toString(),
              name: f.filename,
              filename: f.tofn,
              status: 'done',
              url: this.commonSrv.changeDomain(f.url),
              type: f.filetype,
              category: f.category,
              source: '',
              originFileObj: null,
            };
          });
        let selfTaxAmt = 0;
        let detailList = o.invList.map((i) => {
          if (i.expcode != null && i.expcode != '') {
            this.exTotalWarning.push(i.expcode);
          }
          if (i.undertaker == 'self') {
            selfTaxAmt += i.taxloss;
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
            uid: i.invoiceid,
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
            invtype: i.invtype,
            invoiceid: i.invoiceid,
            invabnormalreason: i.invabnormalreason,
            fileurl: i.fileurl,
          };
        });
        o.deptList.map((i) => {
          i.amount = Number((o.amount1 * i.percent * 0.01).toFixed(2));
          i.baseamount = Number((o.baseamt * i.percent * 0.01).toFixed(2));
        });
        this.listTableData.push({
          advanceRno: o.advancerno,
          scene: o.expcode,
          sceneName: o.expname,
          attribDept: o.deptList.map((i) => i.deptId).join(','),
          attribDeptList: o.deptList,
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
          attachList: attachList,
          fileList: fileList,
          invoiceDetailList: detailList,
          fileCategory: attachList.length > 0 ? attachList[0].category : null,
          id: o.seq,
          disabled: false,
          selfTaxAmt: selfTaxAmt,
          actualAmt: o.amount,
          whiteRemark: '',
          taxexpense: null,
          senarioid: o.senarioid,
        });
      });

      // 组装文件
      attachmentList.map((o) => {
        this.attachmentList.push({
          uid: Guid.create().toString(),
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
      this.setDetailTableShowData();
    }
    this.isSpinning = false;
    this.listTableData.map((f) => {
      f.fileList.map(async (o) => {
        o.originFileObj = await this.commonSrv.getFileData(
          o.url,
          o.filename,
          o.type
        );
      });
      f.attachList.map(async (o) => {
        o.originFileObj = await this.commonSrv.getFileData(
          o.url,
          o.filename,
          o.type
        );
      });
    });
    this.attachmentList.map(async (o) => {
      o.originFileObj = await this.commonSrv.getFileData(
        o.url,
        o.filename,
        o.type
      );
    });
  }

  handleDeptAdd(index: number = -1): void {
    let idx =
      this.attribDeptListControl.length > 0
        ? this.attribDeptListControl[this.attribDeptListControl.length - 1]
            .index + 1
        : 0;
    if (index != -1) idx = index;
    //TODO:增加判断是否已存在该控制器
    this.attribDeptListControl.push({
      index: idx,
      deptIdControl: `attrib_dept${idx}`,
      percentControl: `attrib_percent${idx}`,
    });
    this.listForm.addControl(
      `attrib_dept${idx}`,
      new UntypedFormControl(null, Validators.required)
    );
    this.listForm.addControl(
      `attrib_percent${idx}`,
      new UntypedFormControl(null, Validators.required)
    );
  }

  handleDeptPop(control: any): void {
    this.listForm.removeControl(control.deptIdControl);
    this.listForm.removeControl(control.percentControl);
    this.attribDeptListControl = this.attribDeptListControl.filter(
      (o) => o.index != control.index
    );
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
    console.log('this.applicantInfo1',this.applicantInfo)
    if (!this.applicantInfo) {
      this.applicantInfo = this.userInfo;
    }
    console.log('this.applicantInfo2',this.applicantInfo)
    this.isaccount = this.applicantInfo?.isaccount;
    this.userIdList = this.commonSrv.getUserInfo?.proxylist ?? [];
    if (this.userIdList.indexOf(this.applicantInfo.emplid) == -1) {
      this.userIdList.push(this.applicantInfo.emplid);
    }
    //this.headForm.controls.companyCode.setValue(this.applicantInfo.company);
    this.headForm.controls.emplid.setValue(this.applicantInfo.emplid);
    if (this.headForm.controls.selectedUser.value) {
      this.headForm.controls.dept.setValue(this.applicantInfo.deptid);
      this.headForm.controls.payee.setValue(this.applicantInfo.cname);
    }
    this.headForm.controls.ext.setValue(this.applicantInfo.phone);
    //this.curr = this.applicantInfo.curr;
    this.headForm.controls.applicantEmplid.setValue(this.userInfo?.emplid); //申请人
    this.headForm.controls.applicantName.setValue(this.userInfo?.cname); //申请人姓名
    this.headForm.controls.applicantDept.setValue(this.userInfo?.deptid); //申请人部门
    // this.commonSrv
    //   .getEmployeeInfoById(this.userInfo?.emplid)
    //   .subscribe((res) => {
    //     console.log('申请人信息', res);
    //     this.headForm.controls.applicantName.setValue(res?.cname); //申请人姓名
    //     this.headForm.controls.applicantDept.setValue(res?.deptid); //申请人部门
    //   });
    // if (
    //   this.detailListTableColumn
    //     .filter((o) => o.columnKey == 'taxLoss')[0]
    //     .title.indexOf('(') == -1
    // )
      // this.detailListTableColumn.filter(
      //   (o) => o.columnKey == 'taxLoss'
      // )[0].title += `(${this.curr})`;
    this.costdeptid = this.applicantInfo.costdeptid;
    if (!this.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      return;
    }
    this.getChargeAgainstTableData();
  }

  getCompanyData() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetCompanyByArea,
      null
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.companyList = res.body.data;
          if (
            !this.companyList.includes(this.headForm.controls.companyCode.value)
          )
            this.headForm.controls.companyCode.setValue('');
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

  inputProjectCode(value) {
    this.getPJCodeTerms.next(value);
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

  getChargeAgainstTableData() {
    this.Service.Post(
      URLConst.GetDefferredData + `/${this.applicantInfo.emplid}`,
      null
    ).subscribe((res) => {
      this.chargeAgainstTableData = [];
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          res.body.data?.map((o) => {
            this.chargeAgainstTableData.push({
              companyCode: o.company,
              applicantName: o.cname,
              applicantId: o.cuser,
              payeeName: o.payeename,
              payeeId: o.payeeid,
              advanceFundRno: o.rno,
              digest: o.remark,
              appliedAmt: o.amount,
              notChargeAgainstAmt: o.actamt,
              openDays: o.opendays,
              delayTimes: o.delay,
              sceneCode: o.expcode,
              sceneName: o.expname,
              file: o.file,
              disabled: false,
            });
          });
          if (this.chargeAgainstTableData.length > 0) {
            this.chargeAgainstTableData = [...this.chargeAgainstTableData];
            if (!this.headForm.controls.rno.value) {
              this.showChargeAgainstModal();
            }
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

  showChargeAgainstModal() {
    if (!this.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      return;
    }
    let rnoList = this.listTableData.map((o) => {
      return o.advanceRno;
    });
    this.chargeAgainstTableData.map((o) => {
      o.disabled =
        o.companyCode !== this.headForm.controls.companyCode.value ||
        rnoList.indexOf(o.advanceFundRno) !== -1;
    });
    this.chargeAgainstShowTableData = this.chargeAgainstTableData;
    this.selectedRno = null;
    if (this.chargeAgainstTableData.length > 0) {
      this.showChargeAgainst = true;
    } else {
      this.message.info(this.translate.instant('no-advance-detail'));
    }
  }

  selectRnoOnChange(rno: string, value) {
    if (value) {
      if (rno != this.selectedRno) this.selectedRno = rno;
    } else {
      if (rno == this.selectedRno) this.selectedRno = null;
    }
  }

  addChargeAgainstItem(): void {
    this.addloading = true;
    if (this.selectedRno == null) {
      this.message.error(this.translate.instant('select-one-item'), {
        nzDuration: 6000,
      });
      this.addloading = false;
      return;
    }
    let advanceFormData = this.chargeAgainstTableData.filter(
      (o) => o.advanceFundRno == this.selectedRno
    )[0];
    if (!!advanceFormData) {
      let sceneData = this.sceneList.filter(
        (o) => o.expensecode == advanceFormData.sceneCode
      )[0];
      if (!sceneData) {
        this.message.error(this.translate.instant('tips.scene-not-existed'), {
          nzDuration: 6000,
        });
        this.addloading = false;
        return;
      }

      this.resetlistFormData(advanceFormData);
      this.attachList = advanceFormData.file.map((o) => {
        return {
          id: this.listForm.controls.id.value,
          uid: Guid.create().toString(),
          name: o.filename,
          filename: o.tofn,
          status: 'done',
          url: o.url,
          type: o.filetype,
          category: o.category,
          originFileObj: null,
        };
      });
      this.attachList.map(async (o) => {
        o.originFileObj = await this.commonSrv.getFileData(
          o.url,
          o.filename,
          o.type
        );
      });
      this.listForm.controls.attachList.setValue(this.attachList);
      this.listForm.controls.advanceRno.disable({ emitEvent: false });
      this.listForm.controls.scene.disable({ emitEvent: false });
      this.listForm.controls.digest.disable({ emitEvent: false });
    }
    this.showChargeAgainst = false;
    this.requirePaperInvoiceTips();
    this.showModal = true;
  }

  getSceneList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.listForm.controls.scenarioid.setValue(''); //清空报销情境ID
    // // 构造 HttpParams
    let params = new HttpParams()
    .set('Companycategory', this.headForm.controls.companyCode.value)
    .set('Keyword', this.listForm.controls.scene.value ? this.listForm.controls.scene.value : null);
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSenarioByExpenseCode,//URLConst.GetSceneList,
      { Companycategory: this.headForm.controls.companyCode.value, Keyword: this.listForm.controls.scene.value?this.listForm.controls.scene.value:'' }
    ).subscribe( async (res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.sceneList = res.body.data;
          console.log('scenelist = ', this.sceneList)
          if (this.listTableData.length > 0) {
            this.bpmFlag =
              this.listTableData[0].fileCategory
                ?.toLowerCase()
                .indexOf('bpm') != -1;
            let sceneData = this.sceneList.filter(
              (o) => o.expensecode == this.listTableData[0].scene
            )[0];
            let res = await this.getSenarioById(sceneData.id);
            let sceneExtra = res?.body.data;
            if (!!sceneExtra) {
              if (sceneExtra.extraformcode == 0) {
                this.type = 'default';
                this.listTableColumn = ExpenseTableColumn;
              } else if (sceneExtra.extraformcode == 1) {
                this.type = 'overtimeMeal';
                this.listTableColumn = OvertimeMealExpenseTableColumn;
              } else {
                this.type = 'drive';
                this.getCarType();
                this.listTableColumn = DriveFuelExpenseTableColumn;
              }
            } else
              this.message.error(this.translate.instant('tips-scene-invalid'), {
                nzDuration: 6000,
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

  resetlistFormData(customData: any = null): void {
    this.keyId++;
    this.expname = '';
    this.attribDeptListControl.map((o) => {
      this.listForm.removeControl(o.deptIdControl);
      this.listForm.removeControl(o.percentControl);
    });
    this.attribDeptListControl = [];
    this.handleDeptAdd();
    this.sceneKeyword = '';
    let deptId = null;
    // if (
    //   this.headForm.controls.companyCode.value == this.applicantInfo.company
    // ) {
      this.getDeptList(this.costdeptid);
      // this.deptList.push(this.costdeptid);
      deptId = this.costdeptid;
    //}
    if (!!customData) {
      this.listForm.reset({
        attrib_dept0: deptId,
        attrib_percent0: 100,
        id: this.keyId,
        curr: this.curr,
        advanceRno: this.selectedRno,
        scene: customData.sceneCode,
        scenarioid: customData?.id,
        digest: customData.digest,
        attribDeptList: [],
        attachList: [],
        fileList: [],
        invoiceDetailList: [],
        disabled: false,
      });
    } else {
      this.listForm.reset({
        attrib_dept0: deptId,
        attrib_percent0: 100,
        id: this.keyId,
        curr: this.curr,
        attribDeptList: [],
        attachList: [],
        fileList: [],
        invoiceDetailList: [],
        disabled: false,
      });
    }
    this.setDetailTableShowData();
    this.isAllUpload = true;
    this.canUpload = false;
    this.fileList = [];
    this.attachList = [];
    this.totalInvoiceDetailList = [];
    if (this.listTableData.length > 0) {
      this.listTableData
        .map((o) => {
          return o.invoiceDetailList;
        })
        .forEach(
          (o) =>
            (this.totalInvoiceDetailList =
              this.totalInvoiceDetailList.concat(o))
        );
    }
  }

  addItem(): void {
    this.addloading = true;
    if (!this.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.addloading = false;
      return;
    }
    this.resetlistFormData();
    this.listForm.controls.advanceRno.enable({ emitEvent: false });
    this.listForm.controls.scene.enable({ emitEvent: false });
    this.listForm.controls.digest.enable({ emitEvent: false });
    this.selectedRno = null;
    this.requirePaperInvoiceTips();
    this.showModal = true;
  }
  checkInvoiceDetail(id: number): void {
    // this.totalFileList.filter(o => o.id == id && o.url == '...' && !o.preview).map(async o => o.preview = await this.commonSrv.getPicBase64(o.originFileObj!));
    // this.fileList = this.totalFileList.filter(o => o.id == id);
    if (this.listTableData.filter((o) => o.id == id).length > 0) {
      this.fileList = this.listTableData.filter((o) => o.id == id)[0].invoiceDetailList;
      this.showPicList = true;
    }
  }

  checkAttachDetail(id: number): void {
    if (this.listTableData.filter((o) => o.id == id).length > 0) {
      this.attachList = this.listTableData.filter(
        (o) => o.id == id
      )[0].attachList;
      this.showPicList = true;
    }
  }

  editRow(id: number): void {
    this.editloading = true;
    // this.detailTableData = [];
    let rowFormData = this.listTableData.filter((d) => d.id == id)[0];
    if (rowFormData != null) {
      // let rowdetailData = this.totalDetailTableData.filter(d => d.id == id);

      // 清空attribController 并根据list数据重新添加
      this.attribDeptListControl.map((o) => {
        this.listForm.removeControl(o.deptIdControl);
        this.listForm.removeControl(o.percentControl);
      });
      this.attribDeptListControl = [];
      let idx = 0;
      rowFormData.attribDeptList.map((o) => {
        this.handleDeptAdd(idx);
        rowFormData[`attrib_dept${idx}`] = o.deptId;
        rowFormData[`attrib_percent${idx}`] = o.percent;
        this.deptList.push(o.deptId);
        idx++;
      });
      this.totalInvoiceDetailList = [];
      if (this.listTableData.length > 0) {
        this.listTableData
          .map((o) => {
            return o.invoiceDetailList;
          })
          .forEach(
            (o) =>
              (this.totalInvoiceDetailList =
                this.totalInvoiceDetailList.concat(o))
          );
      }
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
      this.setDetailTableShowData();
      this.fileList = rowFormData.fileList;
      this.attachList = rowFormData.attachList;
      this.canUpload = false;
      this.requirePaperInvoiceTips();
      this.showModal = true;
    }
    this.editloading = false;
  }

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {
      //多选操作
      // this.totalFileList = this.totalFileList.filter(o => this.setOfCheckedId.has(o.id));
      // this.totalDetailTableData = this.totalDetailTableData.filter(o => !this.setOfCheckedId.has(o.id));
      this.listTableData = this.listTableData.filter(
        (d) => !this.setOfCheckedId.has(d.id)
      );
      this.setOfCheckedId.clear();
    } else {
      // this.totalFileList = this.totalFileList.filter(o => o.id == id);
      // this.totalDetailTableData = this.totalDetailTableData.filter(o => o.id != id);
      this.listTableData = this.listTableData.filter((d) => d.id != id);
      this.setOfCheckedId.delete(id);
    }
    //*費用情景若設定需繳交紙本附件，則提示「需繳交紙本附件」
    this.requirePaperAttach = false;
    this.listTableData.forEach(async (o) => {
      let scene = o.sceneName;
      let sceneInfo = this.sceneList.filter((s) => s.senarioname===scene)[0];
      let res = await this.getSenarioById(sceneInfo?.id);
      let sceneExtra = res?.body.data;
      if (sceneExtra?.requirespaperattatchment) {
        this.requirePaperAttach = true;
        return;
      }
    });
    this.setStatistic();
    this.deleteloading = false;
  }

  setValidatorOfListForm(): void {
    let fileRequired =
      this.listForm.controls.invoiceDetailList.validator != null;
    let attachRequired = this.listForm.controls.attachList.validator != null;
    Object.values(this.listForm.controls).forEach((control) => {
      control.clearValidators();
      control.updateValueAndValidity({ onlySelf: true });
    });
    let exceptList = [
      'exchangeRate',
      'toLocalAmt',
      'attribDept',
      'sceneName',
      'carTypeName',
      'percent',
      'advanceRno',
      'selfTaxAmt',
      'actualAmt',
    ];
    let columnsList = this.listTableColumn
      .filter((o) => exceptList.indexOf(o.columnKey) == -1)
      .map((o) => o.columnKey);
    columnsList.push('scene');
    if (this.type == 'drive') {
      columnsList.push('carType');
    }
    if (fileRequired) {
      columnsList.push('invoiceDetailList');
    }
    if (attachRequired) {
      columnsList.push('attachList');
    }
    columnsList.map((o) => {
      if (o == 'feeDate') {
        this.listForm.get(o)!.setValidators(this.dateValidator);
      } else {
        this.listForm.get(o)!.setValidators(Validators.required);
      }
      this.listForm.get(o)!.markAsDirty();
      this.listForm.get(o)!.updateValueAndValidity();
    });
  }

  async handleOk() {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (this.selectedArea === ERSConstants.Area.TW) {
      if (this.listForm.controls.invoiceDetailList.value.length > 1) {
          this.message.error(this.translate.instant('only-select-one-invoice'), {
            nzDuration: 6000,
          });
          this.isSpinning = false;
          this.isSaveLoading = false;
          return;
        }
    } else if (this.selectedArea !== ERSConstants.Area.TW) {
      let sceneInfo = this.sceneList?.find(s => s.expensecode === this.listForm.controls.scene.value);
      let res = await this.getSenarioById(sceneInfo?.id);
      let sceneExtra = res?.body.data;
      if (sceneExtra?.isvatdeductable && this.listForm.controls.invoiceDetailList.value.length > 1) {
        this.message.error(this.translate.instant('only-select-one-invoice'), {
          nzDuration: 6000,
        });
        this.isSpinning = false;
        this.isSaveLoading = false;
        return;
      }
    }
    if (this.canUpload) {
      this.message.error(this.translate.instant('not-complete-upload'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    if (this.fileList.filter((o) => !o.upload).length > 0) {
      this.message.error(this.translate.instant('upload-invoice-required'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    if (this.type == 'drive') {
      let checkStartPlaceOK = true;
      this.listForm.controls.invoiceDetailList.value.forEach(e => {
        if (e.startingPlace && e.startingPlace !== this.listForm.controls.startingPlace.value) {
          checkStartPlaceOK = false;
        }
      });
      if (!checkStartPlaceOK) {
        this.message.error(this.translate.instant('tips.startplace-not-same'), {
          nzDuration: 6000,
        });
        this.isSpinning = false;
        this.isSaveLoading = false;
        return;
      }
    }
    this.listForm.controls.fileList.setValue(this.fileList);
    this.setValidatorOfListForm(); // 设置验证逻辑
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
    if (
      this.listForm.controls.startingTime.value >
      this.listForm.controls.backTime.value
    ) {
      this.message.error(
        this.translate.instant('tips.starttime-later-than-backtime'),
        { nzDuration: 6000 }
      );
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let percent = 0;
    this.attribDeptListControl.map((o) => {
      percent += this.listForm.get(o.percentControl).value;
    });
    if (percent != 100) {
      this.message.error(this.translate.instant('tips.percent-not-full'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    // if (this.detailTableData.length == 0 && this.fileList.length == 0) {
    //   this.message.error(this.translate.instant('add-invoice-required'));
    //   this.isSpinning = false;
    //   this.isSaveLoading = false;
    //   return;
    // }
    let amount = this.listForm.controls.expenseAmt.value;
    if (amount == 0) {
      this.message.error(this.translate.instant('amount-zero-error'), {
        nzDuration: 6000,
      });
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    let totalExAmount = 0;
    let invoiceExAmount = 0;
    let idx = 0; // 排序文件-item
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    this.listForm.controls.fileList.value.map((o) => {
      o.index = null;
    });
    detailTableData.map((o) => {
      o.index = ++idx;
      if (o.disabled) {
        invoiceExAmount += Number(o.oamount);
        totalExAmount += Number(o.oamount);
        this.listForm.controls.fileList.value.filter(
          (f) => o.uid == f.uid
        )[0].index = idx;
      } else totalExAmount += Number(o.amount);
    });
    // idx = this.listForm.controls.fileList.value.filter(o => !!o.index).map(o => o.index).sort((a,b)=>a.index-b.index);
    this.listForm.controls.fileList.value
      .filter((o) => o.index == null)
      .map((o) => {
        o.index = ++idx;
      });
    idx = 1;
    this.listForm.controls.attachList.value.map((o) => {
      o.index = idx++;
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
      Number(Number(totalExAmount).toFixed(2)) !=
        Number(Number(amount).toFixed(2))
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
    this.setListTableData();
  }

  setListTableData(): void {
    let amount = this.listForm.controls.expenseAmt.value;
    this.listForm.controls.exchangeRate.setValue(this.exchangeRate);
    let baseAmt = amount * this.exchangeRate; //Number((amount * this.exchangeRate).toFixed(2))
    this.listForm.controls.toLocalAmt.setValue(
      Number((amount * this.exchangeRate).toFixed(2))
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

    // 转换费用归属部门百分比
    let attribDeptList = [];
    let attribDept: string = '';
    console.log('this.attribDeptListControl',this.attribDeptListControl)
    this.attribDeptListControl.map((o) => {
      attribDeptList.push({
        deptId: this.listForm.get(o.deptIdControl).value,
        percent: this.listForm.get(o.percentControl).value,
        amount: Number(
          (amount * this.listForm.get(o.percentControl).value * 0.01).toFixed(2)
        ),
        baseamount: Number(
          (baseAmt * this.listForm.get(o.percentControl).value * 0.01).toFixed(
            2
          )
        ),
      });
      attribDept +=
        (attribDept == '' ? '' : ',') +
        this.listForm.get(o.deptIdControl).value;
    });
    let precisionVarianceAtm = 0;
    let precisionVarianceBaseAtm = 0;
    let precisionVariance = 0;
    attribDeptList.map((o) => {
      precisionVarianceAtm += o.amount;
      precisionVarianceBaseAtm += o.baseamount;
    });
    precisionVariance = amount - precisionVarianceAtm;
    if (precisionVariance != 0)
      attribDeptList[0].amount = Number(
        (attribDeptList[0].amount + precisionVariance).toFixed(2)
      );
    precisionVariance = baseAmt - precisionVarianceBaseAtm;
    if (precisionVariance != 0)
      attribDeptList[0].baseamount = Number(
        (attribDeptList[0].baseamount + precisionVariance).toFixed(2)
      );
    this.listForm.controls.attribDeptList.setValue(attribDeptList);
    this.listForm.controls.attribDept.setValue(attribDept);

    let rowId = this.listForm.controls.id.value;
    this.listTableData = this.listTableData.filter((o) => o.id != rowId);
    let data = this.listForm.getRawValue();
    this.attribDeptListControl.map((o) => {
      // 清掉多余属性-避免编辑赋值时产生误差数据覆盖
      delete data[o.deptIdControl];
      delete data[o.percentControl];
    });

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

    // 计算amount2
    let selfAmt = 0;
    data.invoiceDetailList
      .filter((f) => f.affordPartyValue == 'self')
      .forEach((f) => (selfAmt += Number(f.toLocalTaxLoss)));
    data['selfTaxAmt'] = Number(selfAmt.toFixed(2));
    let actualAmt = data.toLocalAmt - selfAmt;
    if (!!data.advanceRno) {
      let notChargeAmt = this.chargeAgainstTableData.filter(
        (f) => f.advanceFundRno == data.advanceRno
      )[0]?.notChargeAgainstAmt;
      actualAmt = actualAmt - notChargeAmt;
      if (actualAmt <= 0) {
        actualAmt = 0;
      }
    }
    data['actualAmt'] = Number(actualAmt.toFixed(2));

    this.listTableData.push(data);
    this.listTableData = [...this.listTableData]; // 刷新表格
    //*費用情景若設定需繳交紙本附件，則提示「需繳交紙本附件」
    this.requirePaperAttach = false;
    this.listTableData.forEach(async (o) => {
      let scene = o.sceneName;
      let sceneInfo = this.sceneList.filter((s) => s.senarioname===scene)[0];
      let res = await this.getSenarioById(sceneInfo?.id);
      let sceneExtra = res?.body.data;
      if (sceneExtra?.requirespaperattatchment) {
        this.requirePaperAttach = true;
        return;
      }
    });
    this.setStatistic();
    this.checkExInvoiceMsg();
    this.showModal = false;
    this.isSaveLoading = false;
    this.addloading = false;
    this.editloading = false;
    this.isSpinning = false;
  }

  setStatistic(): void {
    let appliedTotal = 0;
    let selfTax = 0;
    let actualTotal = 0;
    if (this.listTableData.length > 0) {
      this.headForm.controls.emplid.disable({ emitEvent: false });
      this.headForm.controls.companyCode.disable({ emitEvent: false });
    } else {
      this.headForm.controls.emplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
      this.type = null;
    }
    let totalDetailTableData = [];
    if (this.type == 'overtimeMeal' || this.type == 'drive') {
      this.listTableData.map((o) => {
        appliedTotal += Number(o.toLocalAmt);
        totalDetailTableData = totalDetailTableData.concat(o.invoiceDetailList);
        actualTotal += o.toLocalAmt;
      });
    } else {
      this.listTableData.map((o) => {
        appliedTotal += Number(o.toLocalAmt);
        totalDetailTableData = totalDetailTableData.concat(o.invoiceDetailList);
        actualTotal += o.actualAmt;
      });
    }

    totalDetailTableData
      .filter((o) => o.affordPartyValue == 'self')
      .map((o) => (selfTax += Number(o.toLocalTaxLoss)));
    this.headForm.controls.appliedTotal.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
    this.headForm.controls.totalAmount.setValue(
      appliedTotal.toLocaleString('zh-CN')
    );
    this.headForm.controls.individualTax.setValue(
      selfTax.toLocaleString('zh-CN')
    );
    this.headForm.controls.actualTotal.setValue(
      Number(actualTotal.toFixed(2)).toLocaleString('zh-CN')
    );
    let borrowedAmt = 0;
    let rnoList = this.listTableData.map((o) => {
      return o.advanceRno;
    });
    this.chargeAgainstTableData
      .filter((o) => rnoList.indexOf(o.advanceFundRno) !== -1)
      .map((o) => (borrowedAmt += o.notChargeAgainstAmt));
    this.headForm.controls.borrowedAmount.setValue(borrowedAmt);
    // 异常提示：
    this.exTotalWarning = [];
    let idx = 0;
    totalDetailTableData.map((o) => {
      if (!!o.exTips || !o.disabled) {
        this.exTotalWarning.push(++idx + '. ' + o.exTips);
      }
    });
  }

  handleCancel(): void {
    let scene = this.listTableData.filter(
      (o) => o.id == this.listForm.controls.id.value
    )[0]?.scene;
    if (scene != this.listForm.controls.scene.value) {
      this.listForm.controls.scene.setValue(scene);
    }
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  addExceptionItem(): void {
    // index 与 uid 合用自增长唯一排序值：this.uid  => detailTableData中 index == uid
    this.detailForm.reset({
      disabled: false,
      id: this.keyId,
      index: Guid.create().toString(),
      curr: this.listForm.controls.curr.value,
    });
    this.exceptionModal = true;
  }

  markAsAbnormal(idx: number) {
    let item = this.detailTableShowData[idx];
    this.detailForm.get('invoiceCode').setValue(item.invoiceCode);
    this.detailForm.get('invoiceNo').setValue(item.invoiceNo);
    this.detailForm.get('amount').setValue(item.oamount);
    this.detailForm.get('curr').setValue(item.curr);
    this.detailForm.get('reason').setValue(item.reason ? item.reason : '');
    this.markAbnormalIdx = idx;
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
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    let item = this.detailForm.getRawValue();
    item['oamount'] = item.amount;
    item.amount = item.amount - item.taxLoss;
    // detailTableData.push(item);
    detailTableData[this.markAbnormalIdx].invoiceCode =  item.invoiceCode;
    detailTableData[this.markAbnormalIdx].invoiceNo =  item.invoiceNo;
    detailTableData[this.markAbnormalIdx].oamount = item.oamount;
    detailTableData[this.markAbnormalIdx].curr = item.curr;
    detailTableData[this.markAbnormalIdx].reason = item.reason;
    detailTableData[this.markAbnormalIdx].affordParty = item.affordParty;

    this.listForm.controls.invoiceDetailList.updateValueAndValidity();
    this.setDetailTableShowData();
    this.isSaveLoading = false;
    this.exceptionModal = false;
  }

  handleExCancel(): void {
    this.exceptionModal = false;
  }

  deleteExRow(item: any): void {
    //if (item.data.disabled) {
    if (this.detailTableShowData.length === 1) {
      this.message.warning(this.translate.instant('delete-warning'));
      return;
    }
    let index = item.data.index;
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter((d) => d.index != index);
    this.listForm.controls.invoiceDetailList.setValue(detailTableData);
    this.setDetailTableShowData();
  }

  // getBpmRnoFile() {
  //   if (!this.listForm.controls.fileCategory.value) {
  //     this.message.error(this.translate.instant('input-expense-scene-first'));
  //     return;
  //   }
  //   this.isSaveLoading = true;
  //   let bpmRno = this.listForm.controls.bpmRno.value;
  //   this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetBPMAttachment, { rno: bpmRno }).subscribe(async (res) => {
  //     // this.Service.doPost('http://localhost:5000' + URLConst.GetBPMAttachment, { rno: bpmRno }, true).subscribe((res) => {
  //     if (res && res.status === 200 && res.body.status == 1) {
  //       // let file = new File(res.body.data, bpmRno);
  //       let blob = new Blob([res.body.data], { type: 'application/pdf' });
  //       let file = new File([blob], bpmRno + '.pdf', { type: 'application/pdf' });
  //       // const fileUrl = URL.createObjectURL(blob);
  //       let fileUrl = res.body.data;

  //       // const blob = new Blob([res.body.data], { type: 'application/pdf' });
  //       // const objectUrl = URL.createObjectURL(blob);
  //       // const a = document.createElement('a');
  //       // document.body.appendChild(a);
  //       // a.setAttribute('style', 'display:none');
  //       // a.setAttribute('href', objectUrl);
  //       // a.setAttribute('download', bpmRno + '.xlsx');
  //       // a.click();
  //       // URL.revokeObjectURL(objectUrl);

  //       if (fileUrl != null) {
  //         this.attachList.push(
  //           {
  //             uid: Guid.create().toString(),
  //             id: this.listForm.controls.id.value,
  //             name: bpmRno + '.pdf',
  //             filename: bpmRno + '.pdf',
  //             type: 'application/pdf',
  //             status: 'done',
  //             url: '...',
  //             source: 'bpm',
  //             category: this.listForm.controls.fileCategory.value,
  //             // originFileObj: await this.commonSrv.getFileData(fileUrl, bpmRno + '.pdf', 'application/pdf'),
  //             originFileObj: file,
  //           });
  //         this.listForm.controls.attachList.setValue(this.attachList);
  //         this.attachList = [...this.attachList];
  //       }
  //       this.canUpload = false;
  //     }
  //     else {
  //       let msg = this.translate.instant('operate-failed') + (res.status === 200 ? res.body.message : this.translate.instant('server-error'));
  //       this.message.error(msg);
  //     }
  //     this.isSaveLoading = false;
  //   });
  // }

  getBpmRnoFile() {
    if (!this.listForm.controls.fileCategory.value) {
      this.message.error(this.translate.instant('input-expense-scene-first'), {
        nzDuration: 6000,
      });
      return;
    }
    this.isSaveLoading = true;
    let bpmRno = this.listForm.controls.bpmRno.value;
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
            id: this.listForm.controls.id.value,
            name: bpmRno + '.pdf',
            filename: bpmRno + '.pdf',
            type: 'application/pdf',
            status: 'done',
            url: fileUrl,
            source: 'bpm',
            category: this.listForm.controls.fileCategory.value,
            originFileObj: await this.commonSrv.getFileData(
              fileUrl,
              bpmRno + '.pdf',
              'application/pdf'
            ),
          });
          this.listForm.controls.attachList.setValue(this.attachList);
          this.attachList = [...this.attachList];
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
            item['curr'] = this.listForm.controls.curr.value; // 默认币别与所选币别一致
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

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      // file.uid = (++this.uid).toString();
      let uploadedFile;
      if (this.showModal) {
        uploadedFile = this.fileList.filter(
          (o) => o.originFileObj?.name == file.name
        );
      } else {
        uploadedFile = this.attachmentList.filter(
          (o) => o.originFileObj?.name == file.name
        );
      }
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

  beforeAttachUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      let upload = !(this.batchUploadList.length > 0 && this.batchUploadModal);
      if (!upload) {
        this.message.error(this.translate.instant('can-upload-only-one-item'), {
          nzDuration: 6000,
        });
      } else {
        let uploadedFile;
        if (!this.batchUploadModal)
          uploadedFile = this.attachList.filter(
            (o) => o.originFileObj.name == file.name
          );
        else
          uploadedFile = this.batchUploadList.filter(
            (o) => o.originFileObj.name == file.name
          );
        upload = uploadedFile.length == 0;
        if (!upload) {
          this.message.error(
            this.commonSrv.FormatString(
              this.translate.instant('has-been-uploaded-that'),
              uploadedFile[0].originFileObj.name,
              uploadedFile[0].name
            ),
            { nzDuration: 6000 }
          );
        }
      }
      // upload = !((this.batchUploadList.length > 0 && this.batchUploadModal) || (this.attachList.length > 0 && !this.batchUploadModal));
      observer.next(upload);
      observer.complete();
    });
  };

  removeFile = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      let detailTableData = this.listForm.controls.invoiceDetailList.value;
      detailTableData = detailTableData.filter((o) => o.uid != file.uid);
      this.listForm.controls.invoiceDetailList.setValue(detailTableData);
      this.setDetailTableShowData();
      observer.next(true);
      observer.complete();
    });
  };

  removeAttach = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      this.listForm.controls.bpmRno.reset();
      this.listForm.controls.attachList.setValue(this.attachList);
      observer.next(true);
      observer.complete();
    });
  };

  handleAttachChange(info: NzUploadChangeParam): void {
    let attachList = [...info.fileList];
    attachList = attachList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      if (!this.batchUploadModal) {
        file.id = this.listForm.controls.id.value;
        file.source = 'upload';
        file.category = this.listForm.controls.fileCategory.value;
      }
      return file;
    });
    if (!this.batchUploadModal) {
      this.attachList = attachList;
      this.listForm.controls.attachList.setValue(this.attachList);
      this.canUpload = false;
    } else this.batchUploadList = attachList;
  }

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      if (!file.url) file.url = '...';
      if (this.showModal) {
        file.id = this.listForm.controls.id.value;
        file.upload = file.upload ? true : false;
        file.url = file.upload ? (!file.url ? '...' : file.url) : null;
      }
      return file;
    });
    if (this.showModal) {
      this.exWarning = [];
      this.fileList = fileList;
      this.isAllUpload = this.fileList.filter((o) => !o.upload).length == 0;
    } else this.attachmentList = fileList;
    this.canUpload = false;
  }

  clickBatchUpload(): void {
    if (this.userInfo.isMobile) {
      this.message.info(this.translate.instant('only-pc'), {
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
      this.type == 'overtimeMeal'
        ? URLConst.UploadOvertmeMeal
        : URLConst.UploadDrive;
    let sceneData = this.sceneList.filter(
      (w) => w.expensecode == this.listForm.controls.scene.value
    )[0];
    let selfAmt = 0;
    let data = this.listForm.getRawValue();
    data.invoiceDetailList
      .filter((f) => f.affordPartyValue == 'self')
      .forEach((f) => (selfAmt += Number(f.toLocalTaxLoss)));
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + url,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        let dataList = res.body.data?.map((o) => {
          // let seq = this.
          return {
            advanceRno: null,
            scene: sceneData?.expensecode,
            scenarioid: sceneData?.id,
            sceneName: sceneData?.senarioname,
            attribDept: o.deptid,
            attribDeptList: [
              {
                deptId: o.deptid,
                percent: 100,
                amount: o.amount == null ? o.total : o.amount,
                baseamount: o.baseamt == null ? o.total : o.baseamt,
              },
            ],
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
            attachList: [],
            fileList: [],
            invoiceDetailList: [],
            fileCategory: null,
            id: ++this.keyId,
            disabled: false,
            actualAmt: (o.amount == null ? o.total : o.amount) - selfAmt, //目前一般费用报销大量上传（误餐/自驾）没有个人承担税金，实际报销金额（扣除个人承担税金）= 计算后金额
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
    console.log('invoicelist = ',this.listForm.controls.invoiceDetailList.value)
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter((o) => !o.disabled);
    this.fileList = [];
    value.forEach((o) => {
      let item = o;
      // item['id'] = this.listForm.controls.id.value;
      detailTableData.push(item);
      this.fileList.push({
        id: o.id,
        uid: o.invoiceid,
        url: o.fileurl,
        name: o.invtype + o.oamount,
        upload: true,
      });
    });
    this.listForm.controls.invoiceDetailList.setValue(detailTableData);
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
      let rowId = this.listForm.controls.id.value;
      // this.detailTableData = this.detailTableData.filter(o => !o.disabled || this.fileList.filter(p => p.upload).map(t => t.uid).indexOf(o.uid) != -1);
      let detailTableData = this.listForm.controls.invoiceDetailList.value;
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
          id: rowId,
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
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    // if (!!detailTableData)
    //   this.detailTableShowData = detailTableData.filter(
    //     (o) => o.exTips != null && o.exTips != ''
    //   );
    this.detailTableShowData = [...detailTableData]; // 刷新表格
  }

  handleTipCancel(): void {
    this.exInfo = [];
    this.fileList = this.fileList.filter((o) => o.upload);
    let detailTableData = this.listForm.controls.invoiceDetailList.value;
    detailTableData = detailTableData.filter(
      (o) =>
        !o.disabled ||
        this.fileList.map((t) => t.uid).indexOf(o.uid.toString()) != -1
    );
    this.listForm.controls.invoiceDetailList.setValue(detailTableData);
    this.setDetailTableShowData();
    this.tipModal = false;
  }

  checkParam(): boolean {
    if (!this.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
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
    return true;
  }

  submit(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;

    let formData = this.SetParam();
    // 提交表單
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq101,
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
          nzOnOk: () => this.router.navigateByUrl(`ers/rq101`), // reset form data
        });
      } else {
        this.message.error(
          this.translate.instant('submit-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error')),
          { nzDuration: 6000 }
        );
      }
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq101,
      formData
    ).subscribe((res) => {
      // this.Service.Post('http://localhost:5000' + URLConst.SaveRq101, formData).subscribe((res) => {
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

  SetParam() : FormData {
    const formData = new FormData();
    //#region 组装数据
    let rno = this.headForm.controls.rno.value;
    let headData = {
      rno: rno,
      cname: this.headForm.controls.applicantName.value,
      deptid: this.headForm.controls.dept.value,
      ext: this.headForm.controls.ext.value,
      company: this.headForm.controls.companyCode.value,
      projectcode: this.headForm.controls.projectCode.value,
      currency: this.curr,
      amount: this.headForm.controls.borrowedAmount.value,
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payee.value,
      paymentway: this.headForm.controls.paymentWay.value,
      //apid: 'RQ101'
    };
    formData.append('head', JSON.stringify(headData));

    let sceneDataArr:any = [];
    this.listTableData.map( async (o) => {
      let sceneData = this.sceneList.filter((i) => i.expensecode == o.scene)[0];
      let res = await this.getSenarioById(sceneData.id);
      let sceneExtra = res?.body?.data;
      sceneDataArr.push(sceneExtra);
    })


    let totalDetailTableData = [];
    let totalFileList = [];
    let totalAttachList = [];
    let listData = this.listTableData.map((o) => {
      // let invList = o.invoiceDetailList;
      // invList.map(o => {o.index = })
      totalDetailTableData = totalDetailTableData.concat(o.invoiceDetailList);
      totalFileList = totalFileList.concat(o.fileList);
      totalAttachList = totalAttachList.concat(o.attachList);
      let sceneData = this.sceneList.filter((i) => i.expensecode == o.scene)[0];
      let sceneExtra = sceneDataArr.filter((i) => i.id == sceneData.id)[0];

      //可抵扣税额
      let taxDeductible = 0;
      // let sceneInfo = this.sceneList?.find(s => s.expcode === o.scene);
      if (sceneExtra?.isvatdeductible && o.invoiceDetailList.length === 1) {
        let taxamount = o.invoiceDetailList[0].taxamount;
        let reason = o.invoiceDetailList[0].reason;
        if (taxamount && !reason) {
          let expenseAmt =o.expenseAmt;
          let oamount = o.invoiceDetailList[0].oamount
          let invTaxRate = o.invoiceDetailList[0].taxrate;
          //当申请金额小于发票金额时，计算部分金额可扣税额
          if (oamount > expenseAmt) {
            if (this.selectedVatrate > 0)
              taxDeductible = expenseAmt / (1 + this.selectedVatrate) * this.selectedVatrate;
            if ((this.selectedVatrate == 0 || this.selectedVatrate == null) && invTaxRate > 0)
              taxDeductible = expenseAmt / (1 + invTaxRate) * invTaxRate;
            if ((this.selectedVatrate == 0 || this.selectedVatrate == null) && (invTaxRate == 0 || invTaxRate == null))
              taxDeductible = taxamount;
          } else {
            taxDeductible = taxamount;
          }
        }
      }
      console.log('o===',o)
      return {
        rno: rno,
        seq: o.id,
        advancerno: o.advanceRno,
        expcode: o.scene,
        expname: o.sceneName,
        deptList: o.attribDeptList,
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
        acctcode: sceneExtra?.accountcode,
        acctname: sceneExtra?.accountname,
        amount: o.actualAmt,
        taxexpense: taxDeductible,//可抵扣税额
        senarioid: o.senarioid,
      };
    });

    formData.append('detail', JSON.stringify(listData));
    let detailData = totalDetailTableData.map((o) => {
      return {
        seq: o.id,
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
        invoiceid: o.invoiceid,
        invabnormalreason: o.invabnormalreason,
        sellerTaxId: o.collectionNo,
        filepath: o.filepath,
        source: o.source,
      };
    });
    formData.append('inv', JSON.stringify(detailData));

    // let indx = 0;
    let invoiceFileData = totalFileList.map((o) => {
      return {
        rno: rno,
        seq: o.id,
        item: o.index,
        filetype: o.type,
        filename: o.name,
        ishead: 'N',
        key: o.uid,
      };
    });
    // indx = 0;
    let attachFileData = totalAttachList.map((o) => {
      return {
        rno: rno,
        seq: o.id,
        item: o.index,
        filetype: o.type,
        filename: o.name,
        category: o.category,
        ishead: 'N',
        status: 'F',
        key: o.uid,
      };
    });
    // indx = 0;
    let indx = 0;
    let attachmentData = this.attachmentList.map((o) => {
      return {
        rno: rno,
        seq: 0,
        item: ++indx,
        filetype: o.type,
        filename: o.name,
        ishead: 'Y',
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
      amount: this.headForm.controls.appliedTotal.value,
      actamt: this.headForm.controls.actualTotal.value,
    };
    formData.append('amount', JSON.stringify(amountData));

    // totalFileList.forEach((file: any) => {
    //   // let key = invoiceFileData.filter(o => o.seq == file.id && o.item == Number(file.uid)).map(o => o.key)[0];
    //   formData.append(file.uid, file.originFileObj);
    // });
    totalAttachList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
  }

  compareFn = (o1: Approver, o2: Approver): boolean => o1 && o2 && o1.emplid === o2.emplid && o1.name === o2.name && o1.nameA === o2.nameA && o1.display === o2.display;

  selectedUser(event: any) {
    this.isSpinning = false;
    this.headForm.controls.emplid.setValue(event?.emplid);
    //this.columnValueChange();
  }

  getPaymentList() {
    const queryParam = {
      pageIndex: 1,
      pageSize: 10,
      data: {category: 'payment_method'},
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryDataDictionary,
      queryParam
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.paymentWayList = res.body.data;
      }
    });
  }

  getCategoryList(userId: string) {
    this.curr = '';
    this.selectedArea = '';
    this.selectedVatrate = 0;
    this.companyList = [];
    const queryParam = {
      userId: userId,
    };
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryCategoryListByUserId,{userId: userId}).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          res.body.data?.map((o) => {
            this.companyList.push({
              companyCategory:o.companyCategory,
              primary:o.primary,
              baseCurrency: o.baseCurrency,
              area: o.area,
              vatrate: o.vatrate,
          });
          });
          if(this.companyList.length > 0){
            if(this.companyList[0].primary){
              this.headForm.controls.companyCode.setValue(this.companyList[0].companyCategory);
              this.curr = this.companyList[0].baseCurrency;
              this.selectedArea = this.companyList[0].area;
              this.selectedVatrate = this.companyList[0].vatrate;
            }
          }
        }
    });
  }

  updateSelectedArea() {
    this.selectedArea = '';
    if (this.headForm.controls.companyCode.value) {
      this.companyList.forEach((o) => {
        if (o.companyCategory == this.headForm.controls.companyCode.value) {
          this.selectedArea = o.area;
          return;
        }
      });
    }
  }

  requirePaperInvoiceTips() {
    this.requirePaperInvoice = false
    if(this.selectedArea === ERSConstants.Area.TW){
      this.requirePaperInvoice = false;
    } else if(this.selectedArea !== ERSConstants.Area.CN && this.selectedArea !== ERSConstants.Area.TW){
      //非大陆台湾的地区必须显示要上缴纸质发票
      this.requirePaperInvoice = true;
    } else if (this.selectedArea === ERSConstants.Area.CN) {
      if (this.listForm.controls.invoiceDetailList.validator !== null) {
        if (this.listForm.controls.invoiceDetailList.value.length > 0) {
          this.listForm.controls.invoiceDetailList.value.forEach(e => {
            //判断选中的发票的来源，全部是发票池的就隐藏提示信息
            if (e.source !== ERSConstants.InvoiceSource.InvoicePool) {
              this.requirePaperInvoice = true;
              return;
            }
          });
        } else {
          this.requirePaperInvoice = true;
        }
      }
    }
  }

  requirePaperInvoiceTips2() {
    this.requirePaperInvoice2 = false
    if(this.selectedArea === ERSConstants.Area.TW){
      this.requirePaperInvoice2 = false;
    } else if(this.selectedArea !== 'CN' && this.selectedArea !== ERSConstants.Area.TW){
      //非大陆台湾的地区必须显示要上缴纸质发票
      this.requirePaperInvoice2 = true;
      if (this.listForm.controls.invoiceDetailList.validator !== null) {
        if (this.listForm.controls.invoiceDetailList.value.length > 0) {
          this.listForm.controls.invoiceDetailList.value.forEach(e => {
            //判断选中的发票的来源，全部是发票池的就隐藏提示信息
            if (e.source !== ERSConstants.InvoiceSource.InvoicePool) {
              this.requirePaperInvoice2 = true;
              return;
            }
          });
        } else {
          this.requirePaperInvoice2 = true;
        }
      }
    }
  }

  //组装异常发票税损信息
  checkExInvoiceMsg() {
    this.taxLossInfo = '';
    if (this.listTableData.length > 0) {
      this.listTableData.forEach(e => {
        e.invoiceDetailList.forEach(i => {
          if (this.selectedArea === ERSConstants.Area.TW && i.reason) {
            this.taxLossInfo += this.commonSrv.FormatString(
              this.translate.instant('exInvoiceTips2'),
              i.oamount?.toString()
            ) + '<br>';
          }
          if (this.selectedArea !== ERSConstants.Area.TW && i.reason) {
            this.taxLossInfo += this.commonSrv.FormatString(
              this.translate.instant('exInvoiceTips1'),
              i.amount?.toString(), i.taxamount?.toString(), i.affordParty
            ) + '<br>';
          }
        })
      });
    }
  }

  getSenarioByExpenseCode(expenseCode: string) {
    // 构造 HttpParams
  let params = new HttpParams()
  .set('pageIndex', this.pageIndex.toString())
  .set('pageSize', this.pageSize.toString());
  // 展平嵌套对象
  //Object.keys(queryParam.data).forEach((key) => {
    params = params.set(`data.ExpenseCode`, expenseCode);
  //});
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSenarioByExpenseCode, {params}
    ).subscribe((res) => {
      this.requirePaperAttach = false;
      if (res && res.status === 200 && res.body != null) {
        console.log('getSenarioByExpenseCode = ', res.body.data);
        let senario = res.body.data;
        if (senario) {
          if (senario?.requiresinvoice === 'Y') {
            this.requirePaperAttach = true;
          }
        }
      }
    });
  }

  // async getSenarioById(id: string) {
  //   let senario: any = null;
  //   this.Service.doGet(
  //   this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSenarioById + id,null
  //   ).subscribe((res) => {
  //     this.requirePaperAttach = false;
  //     if (res && res.status === 200 && res.body != null) {
  //       console.log('getSenarioById = ', res.body.data);
  //       console.log('1------------------')
  //       senario = res.body.data;

  //       // if (senario) {
  //       //   if (senario?.requiresinvoice) {
  //       //     this.requirePaperAttach = true;
  //       //   }
  //       // }
  //     }
  //   });
  //   return senario;
  // }

  async getSenarioById(id: string): Promise<any> {
    return firstValueFrom(
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSenarioById + id,
        null
      )
    );
  }

  async getGetCostDeptid(deptid: string, company: string) : Promise<any> {
    // 构造 HttpParams
  let params = new HttpParams()
  .set('deptid', deptid)
  .set('company', company);
  return firstValueFrom(
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCostDeptid + '?deptid=' + deptid + '&company=' + company,
      null
    )
  );
  // this.Service.doGet(
  //     this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCostDeptid, {params}
  //   ).subscribe((res) => {
  //     this.requirePaperAttach = false;
  //     if (res && res.status === 200 && res.body != null) {
  //       console.log('GetCostDeptid = ', res.body.data);
  //       let senario = res.body.data;
  //       if (senario) {
  //         if (senario?.requiresinvoice === 'Y') {
  //           this.requirePaperAttach = true;
  //         }
  //       }
  //     }
  //   });
  }
}
