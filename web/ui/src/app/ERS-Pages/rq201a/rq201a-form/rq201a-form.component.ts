import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
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
import { EnvironmentconfigService } from '../../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import {
  NzShowUploadList,
  NzUploadChangeParam,
  NzUploadFile,
  UploadFilter,
} from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { ExceptionDetail, ExpenseInfo } from '../classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { BehaviorSubject, catchError, debounceTime, filter, firstValueFrom, map, Observable, Observer, switchMap } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CompanyInfo } from '../../bd008/classes/data-item';
import { Approver } from 'src/app/shared/models/assign-step';
import _ from 'lodash';
import { HttpParams } from '@angular/common/http';
import { ERSConstants } from 'src/app/common/constant';


@Component({
  selector: 'app-rq201a-form',
  templateUrl: './rq201a-form.component.html',
  styleUrls: ['./rq201a-form.component.scss'],
})
export class RQ201AFormComponent implements OnInit, OnDestroy {
  // @memberof RQ201AFormComponent
  @Input() auditMode: boolean = false;
  @Input() showPicList: boolean = false;
  @Output() getExTotalTips = new EventEmitter();
  @Output() getData = new EventEmitter();
  @Output() getFileData = new EventEmitter();
  navigationSubscription;
  DetailTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('invoice-code'),
      columnKey: 'invoiceCode',
      columnWidth: '20%',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
        a.invoiceCode.localeCompare(b.invoiceCode),
    },
    {
      title: this.translate.instant('invoice-no'),
      columnKey: 'invoiceNo',
      columnWidth: '15%',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
        a.invoiceNo.localeCompare(b.invoiceNo),
    },
    {
      title: this.translate.instant('exception-expense-amount'),
      columnKey: 'oamount',
      columnWidth: '',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.oamount - b.oamount,
    },
    {
      title: this.translate.instant('tax-loss'),
      columnKey: 'taxLoss',
      columnWidth: '14%',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) => a.taxLoss - b.taxLoss,
    },
    {
      title: this.translate.instant('col.currency'),
      columnKey: 'curr',
      columnWidth: '9%',
      align: 'left',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
        a.curr.localeCompare(b.curr),
    },
    {
      title: this.translate.instant('afford-party'),
      columnKey: 'affordParty',
      columnWidth: '13%',
      align: 'right',
      sortFn: (a: ExceptionDetail, b: ExceptionDetail) =>
        a.affordParty.localeCompare(b.affordParty),
    },
  ];

  //#region  参数
  nzFilterOption = (input, option) => {
    let name = this.customerList.filter((o) => o.nickname == option.nzValue)[0]
      ?.name;
    if (!!name)
      return (
        name.toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
        option.nzValue.toLowerCase().indexOf(input.toLowerCase()) >= 0
      );
    else return false;
  };
  formId: string;
  screenHeight: any;
  screenWidth: any;
  radioParam1 = 'self-' + this.translate.instant('individual-afford');
  radioParam2 = 'company-' + this.translate.instant('company-afford');
  headForm: UntypedFormGroup;
  detailForm: UntypedFormGroup;
  companyList: any[] = [];
  projectCodeList: any[] = [];
  currList: any[] = [];
  deptList: any[] = [];
  customerList: any[] = [];
  comTopList: any[] = [];
  curr: string;
  costdeptid: any = { deptId: null, deptName: null, deptLabel: null };
  exceptionModal: boolean = false;
  tipModal: boolean = false;
  isSaveLoading: boolean = false;
  detailListTableColumn = this.DetailTableColumn;
  detailTableData: ExceptionDetail[] = [];
  detailTableShowData: ExceptionDetail[] = [];
  keyId: number = 0; // TODO:待将list的id与detail的id以seq&item的逻辑拆分开来
  exInfo: any[] = [];
  exWarning: string[] = [];
  exTip: string = '';
  tipAffordParty = '';
  isSpinning = false;
  isAllUpload = true;
  canConfirm = this.translate.instant('confirm');
  invoiceParty: ExceptionDetail;
  timeDifferModal: boolean = false;
  isReasonRequired: boolean = false;
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
  isFirstLoading: boolean = true;
  isAuditMode: boolean;
  exTotalWarning: any[] = [];
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  isAssemble: boolean = false;
  applicantInfo: any;
  userInfo: any;
  userIdList: string[] = [];
  cuser: string;
  amountList: any = {
    insideSupervisor: 50,
    insideElse: 50,
    outsideSupervisor: 300,
    outsideElse: 160,
  };
  taxRate: number = 0.25;
  isLoading: boolean = false;
  searchChange$ = new BehaviorSubject('');
  onSearch(value: string): void {
    this.isLoading = true;
    this.searchChange$.next(value);
  }
  employeeInfoList: Approver[] = [];
  selectedArea: string = '';
  selectedVatrate: number = 0;
  paymentWayList: any[] = [];
  //#endregion

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
                  phone: item.phone,
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

    // this.navigationSubscription = this.router.events.subscribe((event: any) => {
    //   if (event instanceof NavigationEnd) {
    //     this.dataInitial();
    //   }
    // });

  }

  ngOnInit(): void {
    this.dataInitial();
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.isAuditMode = this.auditMode;
    this.headForm = this.fb.group({
      emplid: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: false }],
      payee: [{ value: null, disabled: false }],
      ext: [{ value: null, disabled: this.isAuditMode }],
      projectCode: [{ value: null, disabled: this.isAuditMode }],
      companyCode: [
        { value: null, disabled: this.isAuditMode },
        Validators.required,
      ],
      isPriorApproval: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      noApprovalReason: [{ value: null, disabled: this.isAuditMode }],
      banquetTimespan: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      customerName: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      restaurantName: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      attribDept: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      banquetDate: [
        { value: null, disabled: this.isAuditMode },
        [this.dateValidator],
      ],
      curr: [{ value: this.curr, disabled: false }, [Validators.required]],
      cusTop: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      cusOtherMember: [{ value: null, disabled: this.isAuditMode }],
      cusAllPeopleNum: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      comTop: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      comTopCategory: [{ value: null, disabled: true }, [Validators.required]],
      comOtherMember: [{ value: null, disabled: this.isAuditMode }],
      comAllPeopleNum: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      isAccordPeopleRule: [
        { value: null, disabled: true },
        [Validators.required],
      ],
      totalBudget: [{ value: null, disabled: true }, [Validators.required]],
      actualAmt: [
        { value: null, disabled: this.isAuditMode },
        [Validators.required],
      ],
      isAccordExpenseRule: [
        { value: null, disabled: true },
        [Validators.required],
      ],
      exceededAmt: [{ value: null, disabled: true }, [Validators.required]],
      handleWay: [{ value: null, disabled: this.isAuditMode }],
      actualExpenseAmt: [
        { value: null, disabled: true },
        [Validators.required],
      ],
      timeDifferReason: [
        { value: null, disabled: this.isAuditMode },
        [this.reasonValidator],
      ],
      comTopBudget: [{ value: null, disabled: this.isAuditMode }],
      selectedUser: [null],
      applicantName: [{ value: null, disabled: true }],
      paymentWay: [null, [Validators.required]],
      payeename: [{ value: null, disabled: true }],
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
    this.monitorFormField();
    this.getRnoInfo();
    //this.getCompanyData();
    this.getCurrency();
    this.getMealCost(this.headForm.controls.companyCode.value);
    this.getPaymentList();
    //预设收款人为申请人工号
    this.headForm.controls.payee.setValue(this.userInfo?.emplid);
    this.headForm.controls.payeename.setValue(this.userInfo?.cname);
    let defaultUser: Approver ={ emplid: this.userInfo?.emplid, name: this.userInfo?.cname, nameA: this.userInfo?.ename, display: this.userInfo?.ename };
    this.employeeInfoList.push(defaultUser);
    this.headForm.controls.selectedUser.setValue(this.employeeInfoList[0])
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  getMealCost(company: string) {
    if (company || company != '') {
      this.Service.doGet(URLConst.getMealCost, { company: company }).subscribe(
        (res) => {
          if (res && res.status === 200 && !!res.body && res.body.status == 1) {
            var temp = res.body.data;
            this.amountList.insideSupervisor = temp.filter(
              (i) => i.categoryid == 1 && i.area == 'in'
            )[0]?.budget;
            this.amountList.outsideSupervisor = temp.filter(
              (i) => i.categoryid == 1 && i.area == 'out'
            )[0]?.budget;
            this.amountList.insideElse = temp.filter(
              (i) => i.categoryid == 2 && i.area == 'in'
            )[0]?.budget;
            this.amountList.outsideElse = temp.filter(
              (i) => i.categoryid == 2 && i.area == 'out'
            )[0]?.budget;
          }
        }
      );
    }
  }

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyList = [];
      this.projectCodeList = [];
      this.currList = [];
      this.deptList = [];
      this.comTopList = [];
      this.curr = null;
      this.costdeptid = { deptId: null, deptName: null, deptLabel: null };
      this.exceptionModal = false;
      this.tipModal = false;
      this.isSaveLoading = false;
      this.detailTableData = [];
      this.detailTableShowData = [];
      this.keyId = 0;
      this.exInfo = [];
      this.exWarning = [];
      this.exTip = '';
      this.tipAffordParty = '';
      this.isSpinning = false;
      this.isAllUpload = true;
      this.canConfirm = this.translate.instant('confirm');
      this.invoiceParty = null;
      this.timeDifferModal = false;
      this.isReasonRequired = false;
      this.exchangeRate = 1;
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
      //this.ngOnInit();
    }
  }

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
          this.message.error(this.translate.instant('file-format-erro-inv'));
          return filterFiles;
        }
        return fileList;
      },
    },
  ];

  getRnoInfo() {
    this.isAssemble = true;
    this.actRoute.queryParams.subscribe((res) => {
      if (this.isAssemble) {
        if (res && res.data && JSON.parse(res.data)) {
          let data = JSON.parse(res.data);
          if (!!data.user) {
            this.commonSrv.getEmployeeInfoById(data.user).subscribe((res) => {
              this.isAssemble = true;
              this.applicantInfo = res;
              if (!!data.newCompany) {
                this.headForm.controls.companyCode.setValue(data.newCompany);
              }
              this.getEmployeeInfo();
              this.isAssemble = false;
            });
          }
          if (!!data.rno) {
            let rno: string = this.crypto.decrypt(data.rno);
            this.headForm.controls.rno.setValue(rno);
            if (!data.user && !data.newCompany) {
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
                this.isAssemble = false;
              });
            }
          } else {
            this.isAssemble = false;
          }
        } else {
          this.getEmployeeInfo();
          this.isAssemble = false;
        }
      }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let infoData = value.detail;
    let attachmentList = value.file;
    let summaryAtmData = value.amount;
    if (headData != null) {
      // this.userIdList = this.userInfo?.proxylist;
      // if (this.userIdList.indexOf(headData.payeeId) == -1) { this.userIdList.push(headData.payeeId); }
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
      this.headForm.controls.payee.setValue(headData.payeeId);
      this.headForm.controls.payeename.setValue(headData.payeename);
      this.headForm.controls.dept.setValue(headData.deptid);
      this.headForm.controls.ext.setValue(headData.ext);
      this.headForm.controls.companyCode.setValue(headData.company);
      this.headForm.controls.isPriorApproval.setValue(headData.whetherapprove);
      this.headForm.controls.noApprovalReason.setValue(headData.approvereason);
      if (headData.projectcode != null) {
        this.projectCodeList.push(headData.projectcode);
        this.headForm.controls.projectCode.setValue(headData.projectcode);
      }
    }
    let assignment = '';
    if (infoData != null) {
      let exInfoList: any = [];
      let invoiceFileList: any = [];
      infoData.map((o) => {
        // if (o.object != null)
        //   this.customerList.push(o.object);
        if (o.deptid != null) this.deptList.push(o.deptid);
        if (o.keep != null)
          this.commonSrv
            .getManagerList(o.keep)
            .subscribe((res) => (this.comTopList = res));
        this.headForm.controls.rno.setValue(o.rno);
        this.headForm.controls.banquetTimespan.setValue(o.flag.toString());
        this.headForm.controls.banquetDate.setValue(o.rdate);
        this.headForm.controls.curr.setValue(o.currency);
        this.headForm.controls.totalBudget.setValue(o.amount1);
        this.headForm.controls.attribDept.setValue(o.deptid);
        this.headForm.controls.customerName.setValue(o.object);
        this.headForm.controls.comTop.setValue(o.keep);
        this.headForm.controls.actualAmt.setValue(o.actualexpense);
        this.headForm.controls.cusAllPeopleNum.setValue(o.objectsum);
        this.headForm.controls.restaurantName.setValue(o.treataddress);
        this.headForm.controls.cusOtherMember.setValue(o.otherobject);
        this.headForm.controls.comTopCategory.setValue(o.keepcategory);
        this.headForm.controls.comOtherMember.setValue(o.otherkeep);
        this.headForm.controls.comAllPeopleNum.setValue(o.keepsum);
        this.headForm.controls.isAccordPeopleRule.setValue(o.isaccordnumber);
        this.headForm.controls.timeDifferReason.setValue(o.datediffreason);
        this.headForm.controls.actualExpenseAmt.setValue(o.paymentexpense);
        this.headForm.controls.isAccordExpenseRule.setValue(o.isaccordcost);
        this.headForm.controls.exceededAmt.setValue(o.overbudget);
        this.headForm.controls.handleWay.setValue(o.processmethod);
        this.headForm.controls.cusTop.setValue(o.custsuperme);
        assignment = o.assignment;
        if (!!o.datediffreason) this.isReasonRequired = true;
        exInfoList = exInfoList.concat(o.invList);
        invoiceFileList = invoiceFileList.concat(o.fileList);
      });

      exInfoList.map((o) => {
        if (o.expcode != null && o.expcode != '') {
          this.exTotalWarning.push(o.expcode);
        }
        this.detailTableData.push({
          invoiceCode: o.invcode,
          invoiceNo: o.invno,
          amount: o.amount,
          taxLoss: o.taxloss,
          curr: o.curr,
          affordParty:
            o.undertaker == 'self'
              ? this.translate.instant('individual-afford')
              : o.undertaker == 'company'
              ? this.translate.instant('company-afford')
              : '',
          disabled: o.abnormal == 'N',
          id: o.seq, // seq == item (201a)
          exTips: o.expcode,
          toLocalTaxLoss: o.taxloss,
          affordPartyValue: o.undertaker,
          reason: o.reason,
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
          expcode: o.expcode,
          invdesc: o.invdesc,
          invtype: o.invtype,
          invoiceid: o.invoiceid,
          invabnormalreason: o.invabnormalreason,
          fileurl: o.fileurl,
        });
      });

      // 组装文件
      invoiceFileList.map((o) => {
        this.fileList.push({
          id: o.item,
          uid: Guid.create().toString(),
          name: o.filename,
          filename: o.tofn,
          status: 'done',
          url: this.commonSrv.changeDomain(o.url),
          upload: true,
          type: o.filetype,
          category: o.category,
          invno: o.invoiceno,
        });
      });
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
      if (this.detailTableData.length > 0)
        this.keyId = this.detailTableData.sort((a, b) => b.id - a.id)[0].id;
      this.setDetailTableShowData();
      this.fileList = [...this.fileList];
      this.attachmentList = [...this.attachmentList];
    }
    this.isSpinning = false;
    this.fileList.map(async (o) => {
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
    let individualAffordAmt = 0;
    let companyAffordAmt = 0;
    this.detailTableData.map((o) => {
      if (o.affordPartyValue == 'self')
        individualAffordAmt += Number(o.toLocalTaxLoss);
      else if (o.affordPartyValue == 'company')
        companyAffordAmt += Number(o.toLocalTaxLoss);
    });
    this.getFileData.emit(this.fileList);
    this.getData.emit([
      {
        selfAffordAmt: individualAffordAmt,
        companyAffordAmt: companyAffordAmt,
        whiteRemark: assignment,
        company: this.headForm.controls.companyCode.value,
      },
    ]);
    this.getExTotalTips.emit(this.exTotalWarning);
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
            taxRate: o.incomeTaxRate?o.incomeTaxRate:null,
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
  monitorFormField() {
    this.headForm.controls.payee.valueChanges.subscribe((value) => {
      if (
        !!value &&
        this.headForm.controls.payee.enabled &&
        !this.isAssemble &&
        !this.isAuditMode
      ) {
        // let rno = this.headForm.controls.rno.value;
        // let data = {
        //   rno: !!rno ? this.crypto.encrypt(rno) : null,
        //   user: value,
        //   newCompany: this.headForm.controls.companyCode.value,
        // };
        // this.router.navigate([`ers/rq201a`], {
        //   queryParams: { data: JSON.stringify(data) },
        // });
        this.isSpinning = true;
        this.commonSrv.getEmployeeInfoById(value).subscribe(async (res) => {
          this.applicantInfo = res;
          this.costdeptid = { deptId: null, deptName: null, deptLabel: null };
          let costDeptInfo = await this.getGetCostDeptid(this.applicantInfo?.deptid, this.applicantInfo?.company);
          if(costDeptInfo) this.applicantInfo.costdeptid = costDeptInfo?.body?.data;
          this.costdeptid.deptId = this.applicantInfo.costdeptid;
          this.getEmployeeInfo();
          this.getCategoryList(value);
          this.isSpinning = false;
        });


      }
    });
    this.headForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (value && !this.isAuditMode && !this.isAssemble) {
        // this.headForm.controls.projectCode.reset();
        // this.headForm.controls.attribDept.reset();
        // this.headForm.controls.customerName.reset();
        // this.headForm.controls.comTop.reset();
        let rno = this.headForm.controls.rno.value;
        let data = {
          rno: !!rno ? this.crypto.encrypt(rno) : null,
          user: this.headForm.controls.emplid.value,
          newCompany: value,
        };
        this.router.navigate([`ers/rq201a`], {
          queryParams: { data: JSON.stringify(data) },
        });
      } else if (!!value) {
        this.getAllCustomerList();
      }
      this.getTaxRate(value);
      if(this.costdeptid?.deptId) this.getDeptList(this.costdeptid.deptId);
    });
    this.detailForm.get('amount').valueChanges.subscribe((value) => {
      this.detailForm.controls.taxLoss.setValue(
        Number((value * this.taxRate * this.exchangeRate).toFixed(2))
      );
    });
    this.headForm.controls.isPriorApproval.valueChanges.subscribe((value) => {
      if (value == 'N') {
        this.headForm
          .get('noApprovalReason')!
          .setValidators(Validators.required);
        this.headForm.get('noApprovalReason')!.markAsDirty();
      } else {
        this.headForm.get('noApprovalReason')!.clearValidators();
        this.headForm.get('noApprovalReason')!.markAsPristine();
      }
      this.headForm.get('noApprovalReason')!.updateValueAndValidity();
    });
    this.headForm.controls.banquetDate.valueChanges.subscribe((value) => {
      this.isReasonRequired = false;
      if (
        value != null &&
        this.invoiceParty != null &&
        this.invoiceParty.invdate.toString().substring(0, 10) !=
          format(value, 'yyyy-MM-dd')
      ) {
        this.timeDifferModal = true;
      }
    });
    this.detailForm.controls.curr.valueChanges.subscribe((value) => {
      let amount = this.detailForm.controls.amount.value;
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
            this.message.error(res.message, { nzDuration: 6000 });
            this.exchangeRate = 0;
          }
          this.detailForm.controls.taxLoss.setValue(
            Number((amount * this.taxRate * this.exchangeRate).toFixed(2))
          );
        });
      } else if (value != this.curr) {
        this.exchangeRate = 1;
        this.detailForm.controls.taxLoss.setValue(
          Number((amount * this.taxRate * this.exchangeRate).toFixed(2))
        );
      } else if (!value && !!this.detailForm.controls.taxLoss.value) {
        this.detailForm.controls.taxLoss.reset();
      }
    });
    this.headForm.controls.banquetTimespan.valueChanges.subscribe((value) => {
      if (!!value && !!this.headForm.controls.comTop.value) {
        let comTop = this.headForm.controls.comTop.value;
        this.Service.doGet(
          this.EnvironmentconfigService.authConfig.ersUrl +
            URLConst.GetManagerCategory,
          {
            userId: comTop,
            category: value,
            company: this.headForm.controls.companyCode.value,
          }
        ).subscribe((res) => {
          if (res && res.status === 200) {
            let category =
              res.body.data?.categoryid + '-' + res.body.data?.categoryname;
            this.headForm.controls.comTopCategory.setValue(category);
            this.headForm.controls.comTopBudget.setValue(res.body.data?.budget);
            if (
              this.headForm.controls.cusAllPeopleNum.value != null &&
              this.headForm.controls.comAllPeopleNum.value != null
            ) {
              this.getBudget();
            }
          } else {
            this.message.error(res.message, { nzDuration: 6000 });
          }
        });
      }
    });
    this.headForm.controls.comTop.valueChanges.subscribe((value) => {
      let banquetTimespan = this.headForm.controls.banquetTimespan.value;
      if (value != null) {
        if (banquetTimespan == null) {
          this.message.error(
            this.translate.instant('tips.choose-banquet-timespan')
          );
          this.headForm.controls.comTop.reset();
        } else {
          this.Service.doGet(
            this.EnvironmentconfigService.authConfig.ersUrl +
              URLConst.GetManagerCategory,
            {
              userId: value,
              category: banquetTimespan,
              company: this.headForm.controls.companyCode.value,
            }
          ).subscribe((res) => {
            if (res && res.status === 200) {
              if (res.body.status == 2) {
                this.message.error(res.body.message);
                this.headForm.controls.comTop.reset();
              } else {
                let category =
                  res.body.data?.categoryid + '-' + res.body.data?.categoryname;
                this.headForm.controls.comTopCategory.setValue(category);
                this.headForm.controls.comTopBudget.setValue(
                  res.body.data?.budget
                );
                if (
                  this.headForm.controls.cusAllPeopleNum.value != null &&
                  this.headForm.controls.comAllPeopleNum.value != null
                ) {
                  this.getBudget();
                }
              }
            } else {
              this.message.error(res.message, { nzDuration: 6000 });
            }
          });
        }
      } else {
        this.headForm.controls.comTopCategory.reset();
        this.headForm.controls.comTopBudget.reset();
        if (
          this.headForm != null &&
          this.headForm.controls.totalBudget.value != null
        ) {
          this.headForm.controls.totalBudget.reset();
          this.headForm.controls.isAccordPeopleRule.reset();
        }
      }
    });
    this.headForm.controls.cusAllPeopleNum.valueChanges.subscribe((value) => {
      if (value != null && value != '') {
        if (
          this.headForm.controls.comAllPeopleNum.value != null &&
          this.headForm.controls.comTopBudget.value != null
        ) {
          this.getBudget();
        }
      } else if (
        this.headForm != null &&
        this.headForm.controls.totalBudget.value != null
      ) {
        this.headForm.controls.totalBudget.reset();
        this.headForm.controls.isAccordPeopleRule.reset();
      }
    });
    this.headForm.controls.comAllPeopleNum.valueChanges.subscribe((value) => {
      if (value != null && value != '') {
        if (
          this.headForm.controls.cusAllPeopleNum.value != null &&
          this.headForm.controls.comTopBudget.value != null
        ) {
          this.getBudget();
        }
      } else if (
        this.headForm != null &&
        this.headForm.controls.totalBudget.value != null
      ) {
        this.headForm.controls.totalBudget.reset();
        this.headForm.controls.isAccordPeopleRule.reset();
      }
    });
    this.headForm.controls.totalBudget.valueChanges.subscribe((value) => {
      if (value != null) {
        if (
          this.headForm.controls.actualAmt.value != null &&
          this.headForm.controls.actualAmt.value != ''
        ) {
          if (this.headForm.controls.actualAmt.value > value) {
            this.headForm.controls.exceededAmt.setValue(
              Number(
                (this.headForm.controls.actualAmt.value - value).toFixed(2)
              )
            );
            this.headForm.controls.isAccordExpenseRule.setValue('N');
            this.headForm.get('handleWay')!.setValidators(Validators.required);
            this.headForm.get('handleWay')!.markAsDirty();
          } else {
            this.headForm.controls.exceededAmt.setValue(0);
            this.headForm.controls.isAccordExpenseRule.setValue('Y');
            this.headForm.get('handleWay')!.clearValidators();
            this.headForm.get('handleWay')!.markAsPristine();
          }
          this.headForm.get('handleWay')!.updateValueAndValidity();
        }
      } else {
        this.headForm.controls.isAccordExpenseRule.reset();
        this.headForm.controls.exceededAmt.reset();
      }
    });
    this.headForm.controls.actualAmt.valueChanges.subscribe((value) => {
      if (value != null && value != '') {
        if (
          this.headForm.controls.totalBudget.value != null &&
          this.headForm.controls.totalBudget.value != 0
        ) {
          if (this.headForm.controls.totalBudget.value < value) {
            this.headForm.controls.exceededAmt.setValue(
              Number(
                (value - this.headForm.controls.totalBudget.value).toFixed(2)
              )
            );
            this.headForm.controls.isAccordExpenseRule.setValue('N');
            this.headForm.get('handleWay')!.setValidators(Validators.required);
            this.headForm.get('handleWay')!.markAsDirty();
          } else {
            this.headForm.controls.exceededAmt.setValue(0);
            this.headForm.controls.isAccordExpenseRule.setValue('Y');
            this.headForm.get('handleWay')!.clearValidators();
            this.headForm.get('handleWay')!.markAsPristine();
          }
          this.headForm.get('handleWay')!.updateValueAndValidity();
        }
        if (this.headForm.controls.handleWay.value != null) {
          // 重新计算实际金额
          if (this.headForm.controls.handleWay.value == '0') {
            value -= this.headForm.controls.exceededAmt.value;
          }
          let selfAffordAmt = 0;
          this.detailTableData
            .filter((o) => o.affordPartyValue == 'self')
            .map((o) => (selfAffordAmt += o.taxLoss));
          this.headForm.controls.actualExpenseAmt.setValue(
            Number((value - selfAffordAmt).toFixed(2))
          );
        }
      } else {
        this.headForm.controls.isAccordExpenseRule.reset();
        this.headForm.controls.exceededAmt.reset();
      }
    });
    this.headForm.controls.handleWay.valueChanges.subscribe((value) => {
      if (
        this.headForm.controls.actualAmt.value != null &&
        this.headForm.controls.actualAmt.value != ''
      ) {
        let amount = this.headForm.controls.actualAmt.value; // 重新计算实际金额
        if (value == '0') {
          amount -= this.headForm.controls.exceededAmt.value;
        }
        let selfAffordAmt = 0;
        this.detailTableData
          .filter((o) => o.affordPartyValue == 'self')
          .map((o) => (selfAffordAmt += o.taxLoss));
        this.headForm.controls.actualExpenseAmt.setValue(
          Number((amount - selfAffordAmt).toFixed(2))
        );
      }
    });
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
    return {};
  };

  reasonValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
    if (this.isReasonRequired && !control.value) {
      return { error: true, required: true };
    }
    return {};
  };

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
    if (this.userIdList?.indexOf(this.applicantInfo.emplid) == -1) {
      this.userIdList.push(this.applicantInfo.emplid);
    }
    // if (!this.headForm.controls.companyCode.value) {
    //   this.headForm.controls.companyCode.setValue(this.applicantInfo.company);
    // }
    if (
      this.headForm.controls.companyCode.value != this.applicantInfo.company
    ) {
      this.headForm.controls.attribDept.reset();
      this.deptList = [];
    }
    // this.headForm.controls.emplid.setValue(this.applicantInfo.emplid);
    // this.headForm.controls.dept.setValue(this.applicantInfo.deptid);
    this.headForm.controls.emplid.setValue(this.userInfo.emplid);
    this.headForm.controls.applicantName.setValue(this.userInfo.cname);
    this.headForm.controls.dept.setValue(this.userInfo.deptid);
    this.headForm.controls.ext.setValue(this.applicantInfo.phone);
    this.headForm.controls.curr.setValue(this.applicantInfo.curr);
    this.headForm.controls.attribDept.setValue(this.applicantInfo.costdeptid);
    this.curr = this.applicantInfo.curr;
    // if (
    //   this.detailListTableColumn
    //     .filter((o) => o.columnKey == 'taxLoss')[0]
    //     .title.indexOf('(') == -1
    // )
    //   this.detailListTableColumn.filter(
    //     (o) => o.columnKey == 'taxLoss'
    //   )[0].title += `(${this.curr})`;
    this.costdeptid.deptId = this.applicantInfo.costdeptid;
    //this.getDeptList(this.costdeptid);
    // this.deptList.push(this.costdeptid);
    this.detailForm.controls.curr.setValue(this.curr);
    if (!this.isAuditMode && !this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'));
      return;
    }
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

  getProjectCode(value) {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!');
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
      this.message.error('Please choose company first!');
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
        } else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
    } else this.deptList = [];
  }

  // getCustomerList(value) {
  //   if (!this.headForm.controls.companyCode.value) {
  //     this.message.error('Please choose company first!');
  //     return;
  //   }
  //   if (value.length > 0) {
  //     const params = {
  //       name: value,
  //       company: this.headForm.controls.companyCode.value
  //     };
  //     this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCustomerName, params).subscribe((res) => {
  //       if (res && res.status === 200) {
  //         this.customerList = res.body.map(item => item.nickname);
  //       }
  //     })
  //   }
  //   else
  //     this.customerList = [];
  // }

  getAllCustomerList() {
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!');
      return;
    }
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetAllCustomerName,
      { company: this.headForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.customerList = res.body;
        console.log('this.customerList',this.customerList)
      } else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  getComTopList(value) {
    this.commonSrv
      .getManagerList(value)
      .subscribe((res) => (this.comTopList = res));
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
      this.isSpinning = false;
    });
  }

  getBudget() {
    let param = {
      customernum: this.headForm.controls.cusAllPeopleNum.value,
      employeenum: this.headForm.controls.comAllPeopleNum.value,
      budget: this.headForm.controls.comTopBudget.value,
    };
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetBudget,
      param
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.headForm.controls.totalBudget.setValue(res.body.budget);
        this.headForm.controls.isAccordPeopleRule.setValue(
          res.body.stat ? 'Y' : 'N'
        );
      } else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  ////////带选择框表
  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: ExpenseInfo[] = [];
  setOfCheckedId = new Set<number>();
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: ExpenseInfo[]): void {
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

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {
      //多选操作
      this.detailTableData = this.detailTableData.filter(
        (d) => !this.setOfCheckedId.has(d.id)
      );
      this.setDetailTableShowData();
      this.setOfCheckedId.clear();
    } else {
      this.detailTableData = this.detailTableData.filter((d) => d.id != id);
      this.setDetailTableShowData();
      this.setOfCheckedId.delete(id);
    }
    this.deleteloading = false;
  }

  addExceptionItem(): void {
    let id = ++this.keyId;
    this.detailForm.reset({ disabled: false, id: id, curr: this.curr });
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
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSaveLoading = false;
      return;
    }
    if (this.detailForm.controls.amount.value == 0) {
      this.message.error(this.translate.instant('ex-amount-zero-error'));
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

    let taxLoss = this.detailForm.controls.taxLoss.value; // 税金转换统一币别
    this.detailForm.controls.toLocalTaxLoss.setValue(taxLoss);
    let item = this.detailForm.getRawValue();
    //【CN】若選擇由員工承擔，可報銷金額須扣除所得稅損
    if(this.selectedArea === ERSConstants.Area.CN && item.affordParty === this.radioParam1){
      item['oamount'] = item.amount -item.taxLoss;
    } else {
      item['oamount'] = item.amount;
    }
    this.detailTableData.push(item);
    this.setDetailTableShowData();
    // 重新计算实际金额
    let amount = this.headForm.controls.actualAmt.value;
    let handleWay = this.headForm.controls.handleWay.value;
    if (!!amount) {
      if (handleWay == '0') {
        amount -= this.headForm.controls.exceededAmt.value;
      }
      let selfAffordAmt = 0;
      this.detailTableData
        .filter((o) => o.affordPartyValue == 'self')
        .map((o) => (selfAffordAmt += o.taxLoss));
      this.headForm.controls.actualExpenseAmt.setValue(
        Number((amount - selfAffordAmt).toFixed(2))
      );
    }
    // this.detailTableData = [...this.detailTableData];   // 刷新表格
    this.isSaveLoading = false;
    this.exceptionModal = false;
  }

  handleExCancel(): void {
    this.exceptionModal = false;
  }

  uploadInvoice(): void {
    this.isSaveLoading = true;
    this.isSpinning = true;
    const formData = new FormData();
    this.fileList
      .filter((o) => !o.upload)
      .forEach((file: any) => {
        formData.append(file.id, file.originFileObj);
      });
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.PostFileToRead,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.exInfo = res.body.list; // 读出来的发票信息
        this.exWarning = [];
        let exInfoWarning = this.exInfo.filter(
          (o) => o['expcode'] != '' && o['expcode'] != null
        ); // 异常发票信息
        if (this.exInfo.length > 0) {
          this.exInfo.forEach((item) => {
            // 改文件名
            item['curr'] = this.curr; // 默认币别与所选币别一致
            var aList = document
              .getElementById('invoiceUpload')
              .getElementsByClassName('ant-upload-list-item-name');
            for (var i = 0; i < aList.length; i++) {
              let title = this.fileList.filter((o) => o.id == item.item)[0]
                .name;
              if (aList[i].getAttribute('title') == title) {
                aList[i].innerHTML = item.invdesc;
                aList[i].setAttribute('title', item.invdesc);
              }
            }
            this.fileList.filter((o) => o.id == item.item)[0].name =
              item.invdesc;
            this.fileList.filter((o) => o.id == item.item)[0].filename =
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
        this.message.error('Failed to upload, please try it again.');
      }
      this.fileList = [...this.fileList];
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    return new Observable((observer: Observer<boolean>) => {
      this.keyId++;
      file.id = this.keyId;
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
          )
        );
      observer.next(upload);
      observer.complete();
    });
  };

  beforeUploadAtt = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
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
          )
        );
      observer.next(upload);
      observer.complete();
    });
  };

  removeFile = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      this.detailTableData = this.detailTableData.filter(
        (o) => o.id != file.id
      );
      this.setDetailTableShowData();
      this.invoicePartyChange();
      observer.next(true);
      observer.complete();
    });
  };

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.id = !file.id ? file.originFileObj['id'] : file.id;
      file.upload = file.upload ? true : false;
      file.url = file.upload ? (!file.url ? '...' : file.url) : null;
      return file;
    });
    this.exWarning = [];
    this.fileList = fileList;
    this.isAllUpload = this.fileList.filter((o) => !o.upload).length == 0;
  }

  handleAttachChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.attachmentList = fileList;
  }

  handleTipOk(): void {
    this.isSaveLoading = true;
    if (this.tipAffordParty == '' && this.canConfirm != null) {
      this.message.error(this.translate.instant('afford-party-cannot-be-null'));
      this.isSaveLoading = false;
      return;
    }
    this.saveExInfo();
    this.isSaveLoading = false;
    this.tipModal = false;
  }

  addInvoice(value): void {
    this.detailTableData = this.detailTableData.filter((o) => !o.disabled);
    this.fileList = [];
    value.forEach((o) => {
      o.invdate = format(new Date(o.invdate), 'yyyy-MM-dd');
      let item = o;
      this.detailTableData.push(item);
      this.fileList.push({
        id: o.id,
        uid: o.invoiceid,
        url: o.fileurl,
        name: o.invtype + o.oamount,
        upload: true,
      });
    });
    this.setDetailTableShowData();
    // 重新计算实际金额
    let amount = this.headForm.controls.actualAmt.value;
    let handleWay = this.headForm.controls.handleWay.value;
    if (!!amount) {
      if (handleWay == '0') {
        amount -= this.headForm.controls.exceededAmt.value;
      }
      let selfAffordAmt = 0;
      this.detailTableData
        .filter((o) => o.affordPartyValue == 'self')
        .map((o) => (selfAffordAmt += o.taxLoss));
      this.headForm.controls.actualExpenseAmt.setValue(
        Number((amount - selfAffordAmt).toFixed(2))
      );
    }
    this.invoicePartyChange();
    this.isAllUpload = true;
  }

  saveExInfo() {
    //删除无法请款的文件及item
    let cannotExpenseUid = this.exInfo
      .filter((o) => !o.paymentStat)
      .map((o) => o.item);
    if (cannotExpenseUid.length > 0) {
      this.fileList = this.fileList.filter(
        (o) => cannotExpenseUid.indexOf(Number(o.id)) == -1
      );
    }
    this.exInfo = this.exInfo.filter((o) => o.paymentStat);
    if (this.exInfo.length > 0) {
      this.exInfo.forEach((item) => {
        this.detailTableData.push({
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
          id: item['item'],
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
          invtype: item['invtype'],
          invoiceid: null,
          invabnormalreason: null,
          fileurl: null,
        });
      });
      this.setDetailTableShowData();
      // 重新计算实际金额
      let amount = this.headForm.controls.actualAmt.value;
      let handleWay = this.headForm.controls.handleWay.value;
      if (!!amount) {
        if (handleWay == '0') {
          amount -= this.headForm.controls.exceededAmt.value;
        }
        let selfAffordAmt = 0;
        this.detailTableData
          .filter((o) => o.affordPartyValue == 'self')
          .map((o) => (selfAffordAmt += o.taxLoss));
        this.headForm.controls.actualExpenseAmt.setValue(
          Number((amount - selfAffordAmt).toFixed(2))
        );
      }
      // this.detailTableData = [...this.detailTableData];   // 刷新表格
    }
    this.fileList.map((o) => {
      o.upload = true;
      o.url = !o.url ? '...' : o.url;
    });
    this.invoicePartyChange();
    this.isAllUpload = true;
    this.fileList = [...this.fileList];
  }

  setDetailTableShowData() {
    this.detailTableShowData = this.detailTableData;
    this.detailTableShowData = [...this.detailTableShowData]; // 刷新表格
  }

  handleTipCancel(): void {
    this.exInfo = [];
    this.fileList = this.fileList.filter((o) => o.upload);
    this.isAllUpload = true;
    this.detailTableData = this.detailTableData.filter(
      (o) => !o.disabled || this.fileList.map((t) => t.id).indexOf(o.id) != -1
    );
    this.tipModal = false;
  }

  invoicePartyChange(): void {
    let invoice = this.detailTableData
      .filter((o) => o.disabled)
      .sort((a, b) => b.amount - a.amount)[0];
    if (invoice == null) {
      this.isReasonRequired = false;
      this.headForm.controls.timeDifferReason.reset();
      this.invoiceParty = invoice;
      return;
    }
    if (
      this.invoiceParty != null &&
      invoice != this.invoiceParty &&
      this.headForm.controls.banquetDate.value != null
    ) {
      this.isReasonRequired = false;
      if (
        invoice.invdate.toString().substring(0, 10) !=
        format(new Date(this.headForm.controls.banquetDate.value), 'yyyy-MM-dd')
      ) {
        this.timeDifferModal = true;
      }
    }
    this.invoiceParty = invoice;
    if (this.invoiceParty.collectionName != null)
      this.headForm.controls.restaurantName.setValue(
        this.invoiceParty.collectionName
      );
  }

  handleTDOk(): void {
    if (
      this.headForm.controls.timeDifferReason.value == null ||
      this.headForm.controls.timeDifferReason.value == ' '
    ) {
      this.isReasonRequired = false;
      this.message.error(this.translate.instant('fill-in-form'));
    } else {
      this.isReasonRequired = true;
      this.timeDifferModal = false;
    }
  }

  handleTDCancel(): void {
    this.isReasonRequired = false;
    this.headForm.controls.timeDifferReason.reset();
    this.headForm.controls.banquetDate.reset();
    this.timeDifferModal = false;
  }

  checkParam(): boolean {
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'));
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
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.fileList.filter((o) => !o.upload).length > 0) {
      this.message.error(this.translate.instant('upload-invoice-required'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (this.detailTableData.length == 0 && this.fileList.length == 0) {
      this.message.error(this.translate.instant('add-invoice-required'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }
    if (
      this.headForm.controls.banquetTimespan.value == '2' &&
      this.attachmentList.length == 0
    ) {
      this.message.error(this.translate.instant('add-attachment-required'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return false;
    }

    // let total = 0;
    // this.detailTableData.filter(o => o.disabled).map(o => total += Number(o.amount));
    // let actualAmt: number = this.headForm.controls.actualAmt.value;
    // if (total != 0 && actualAmt > total) {
    //   this.message.error(this.commonSrv.FormatString(this.translate.instant('amount-error'), actualAmt.toLocaleString(), total.toLocaleString()));
    //   this.isSpinning = false;
    //   this.isSaveLoading = false;
    //   return false;
    // }

    let totalExAmount = 0;
    let invoiceExAmount = 0;
    let actualAmt: number = this.headForm.controls.actualAmt.value;
    this.detailTableData.map((o) => {
      if (o.disabled) {
        invoiceExAmount += Number(o.oamount);
        totalExAmount += Number(o.oamount);
      } else totalExAmount += Number(o.amount);
    });
    if (invoiceExAmount > 0) {
      if (totalExAmount < actualAmt) {
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('amount-error'),
            actualAmt.toLocaleString(),
            totalExAmount.toLocaleString()
          )
        );
        this.isSpinning = false;
        this.isSaveLoading = false;
        return false;
      }
    } else {
      if (totalExAmount != 0 && totalExAmount != actualAmt) {
        this.message.error(
          this.commonSrv.FormatString(
            this.translate.instant('amount-error-not-equal'),
            actualAmt.toLocaleString(),
            totalExAmount.toLocaleString()
          )
        );
        this.isSpinning = false;
        this.isSaveLoading = false;
        return false;
      }
    }

    return true;
  }

  submit(): void {
    this.isSpinning = true;
    this.isSaveLoading = true;
    if (!this.checkParam()) return;

    let formData = this.SetParam();
    // 提交表單
    // this.Service.Post('http://localhost:5000' + URLConst.SubmitRq201a, formData).subscribe((res) => {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq201a,
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
          nzOnOk: () => this.router.navigate([`ers/rq201a`]), // reset form data
        });
      } else
        this.message.error(
          this.translate.instant('submit-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error'))
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
    // 暫存表單
    // this.Service.Post('http://localhost:5000' + URLConst.SaveRq201a, formData).subscribe((res) => {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq201a,
      formData
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.headForm.controls.rno.setValue(res.body.data.rno);
        this.message.success(
          this.translate.instant('save-successfully') +
            `Request NO: ${res.body.data.rno}`
        );
        if (!this.cuser) {
          this.cuser = this.userInfo?.emplid;
        }
      } else
        this.message.error(
          this.translate.instant('save-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error'))
        );
      this.isSpinning = false;
      this.isSaveLoading = false;
    });
  }

  SetParam(): FormData {
    //#region 组装数据
    const formData = new FormData();
    let rno = this.headForm.controls.rno.value;
    let headData = {
      rno: rno,
      cname: this.headForm.controls.applicantName.value,
      deptid: this.headForm.controls.dept.value,
      ext: this.headForm.controls.ext.value,
      company: this.headForm.controls.companyCode.value,
      projectcode: this.headForm.controls.projectCode.value,
      currency: this.curr,
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payeename.value,
      whetherapprove: this.headForm.controls.isPriorApproval.value,
      approvereason: this.headForm.controls.noApprovalReason.value,
      paymentway: this.headForm.controls.paymentWay.value,
    };
    formData.append('head', JSON.stringify(headData));

    let infoData = [
      {
        // formcode: 'CASH_2',
        rno: rno,
        seq: 1,
        rdate: this.headForm.controls.banquetDate.value,
        currency: this.headForm.controls.curr.value,
        amount1: this.headForm.controls.totalBudget.value,
        deptid: this.headForm.controls.attribDept.value,
        object: this.headForm.controls.customerName.value,
        keep: this.headForm.controls.comTop.value,
        basecurr: this.curr,
        baseamt: this.headForm.controls.actualAmt.value,
        rate: this.headForm.controls.cusAllPeopleNum.value,
        // expcode: 'EXP02',
        // expname: '交際費',
        // acctcode: '71412000',
        // acctname: '交際費-其他',
        treataddress: this.headForm.controls.restaurantName.value,
        otherobject: this.headForm.controls.cusOtherMember.value,
        objectsum: this.headForm.controls.cusAllPeopleNum.value,
        keepcategory: this.headForm.controls.comTopCategory.value,
        otherkeep: this.headForm.controls.comOtherMember.value,
        keepsum: this.headForm.controls.comAllPeopleNum.value,
        isaccordnumber: this.headForm.controls.isAccordPeopleRule.value,
        datediffreason: this.headForm.controls.timeDifferReason.value,
        actualexpense: this.headForm.controls.actualAmt.value,
        paymentexpense: this.headForm.controls.actualExpenseAmt.value,
        isaccordcost: this.headForm.controls.isAccordExpenseRule.value,
        overbudget: this.headForm.controls.exceededAmt.value,
        processmethod: this.headForm.controls.handleWay.value,
        custsuperme: this.headForm.controls.cusTop.value,
        treattime:
          this.headForm.controls.banquetTimespan.value == '0'
            ? this.translate.instant('lunch-on-weekdays')
            : this.headForm.controls.banquetTimespan.value == '1'
            ? this.translate.instant('dinner-or-weekends-lunch')
            : this.translate.instant('outside-lunch'),
        flag: Number(this.headForm.controls.banquetTimespan.value),
        amount: this.headForm.controls.actualExpenseAmt.value,
        senarioid: '',
      },
    ];
    formData.append('detail', JSON.stringify(infoData));

    let detailData = this.detailTableData.map((o) => {
      return {
        rno: rno,
        seq: 1,
        item: Number(o.id),
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
      };
    });
    formData.append('inv', JSON.stringify(detailData));

    let invoiceFileData = this.fileList.map((o) => {
      return {
        rno: rno,
        seq: 1,
        item: Number(o.id),
        filetype: o.type,
        filename: o.name,
        ishead: 'N',
        key: o.uid,
      };
    });
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
    // formData.append('file', JSON.stringify(invoiceFileData.concat(attachmentData)))
    formData.append('file', JSON.stringify(attachmentData));

    let amountData = {
      rno: rno,
      currency: this.curr,
      amount: this.headForm.controls.actualAmt.value,
      actamt: this.headForm.controls.actualExpenseAmt.value,
    };
    formData.append('amount', JSON.stringify(amountData));

    this.fileList.forEach((file: any) => {
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
    this.headForm.controls.payee.setValue(event?.emplid);
    this.headForm.controls.payeename.setValue(event?.cname);
    //this.columnValueChange();
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
        console.log('paymentWayList = ', this.paymentWayList);
      }
    });
  }
}
