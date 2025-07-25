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
import { ExceptionDetail, ExpenseInfo } from './classes/data-item';
import { ExpenseTableColumn, DetailTableColumn } from './classes/table-column';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { BehaviorSubject, catchError, debounceTime, filter, firstValueFrom, map, Observable, Observer, switchMap } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CompanyInfo } from '../bd008/classes/data-item';
import { HttpParams } from '@angular/common/http';
import { Approver } from 'src/app/shared/models/assign-step';
import _ from 'lodash';
import { ERSConstants } from 'src/app/common/constant';

@Component({
  selector: 'app-rq201',
  templateUrl: './rq201.component.html',
  styleUrls: ['./rq201.component.scss'],
})
export class RQ201Component implements OnInit, OnDestroy {
  navigationSubscription;
  //#region 参数
  nzFilterOption = (input, option) => {
    let name = this.customerList.filter((o) => o.nickname.toLowerCase().indexOf(input.toLowerCase()) >= 0)[0]?.name;
    console.log('input',input)
    console.log('name',name)
    if (!!name)
      return (
        name.toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
        option.nzValue.toLowerCase().indexOf(input.toLowerCase()) >= 0
      );
    else return false;
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
  customerList: any[] = [];
  originCustomerList: any[] = [];
  curr: string;
  costdeptid: any = { deptId: null, deptName: null, deptLabel: null };
  isaccount: boolean = false;
  showModal: boolean = false;
  exceptionModal: boolean = false;
  tipModal: boolean = false;
  isSaveLoading: boolean = false;
  detailListTableColumn = DetailTableColumn;
  listTableColumn = ExpenseTableColumn;
  totalDetailTableData: ExceptionDetail[] = [];
  detailTableData: ExceptionDetail[] = [];
  detailTableShowData: ExceptionDetail[] = [];
  listTableData: ExpenseInfo[] = [];
  listOfAction = ['Delete'];
  keyId: number = 0;
  uid: number = 0;
  exInfo: any[] = [];
  exWarning: string[] = [];
  exTip: string = '';
  exTotalWarning: any[] = [];
  tipAffordParty = '';
  isSpinning = false;
  isAllUpload = true;
  canConfirm = this.translate.instant('confirm');
  exchangeRate = 1;
  totalFileList: NzUploadFile[] = [];
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
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  applicantInfo: any;
  userInfo: any;
  userIdList: string[] = [];
  cuser: string;
  taxRate: number = 0.25;
  //#endregion
  paymentWayList: any[] = [];
  searchChange$ = new BehaviorSubject('');
  employeeInfoList: Approver[] = [];
  isLoading = false;
  selectedArea: string = '';
  selectedVatrate: number = 0;
  onSearch(value: string): void {
    this.isLoading = true;
    this.searchChange$.next(value);
  }
  showTypeHostObj = false;

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
    this.navigationSubscription = this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.dataInitial();
      }
    });
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
                console.log('item', item);
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
      emplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: false }],
      payee: [{ value: null, disabled: false }],
      payeeName: [{ value: null, disabled: true }],
      ext: [{ value: null, disabled: false }],
      projectCode: [null],
      companyCode: [null, [Validators.required]],
      appliedTotal: [{ value: 0, disabled: true }],
      individualTax: [{ value: 0, disabled: true }],
      actualTotal: [{ value: 0, disabled: true }],
      totalAmount: [{ value: 0, disabled: true }],
      paymentWay: [null, [Validators.required]],
      selectedUser: [null],
    });
    this.listForm = this.fb.group({
      feeDate: [null, [this.dateValidator]],
      expenseAttribDept: [null, [Validators.required]],
      curr: [null, [Validators.required]],
      appliedAmount: [null, [Validators.required]],
      toLocalCurrAmount: [null],
      exchangeRate: [null],
      entertainObject: [null, [Validators.required]],
      inputEntertainObject: [null],
      entertainDate: [null, [this.dateValidator]],
      accompany: [null, [Validators.required]],
      remark: [null, [Validators.required]],
      invoiceTotalAmount: [null],
      disabled: [false],
      id: [this.keyId],
      invoiceDetailList: [[]],
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
      index: [0],
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
    this.columnSubscribe();
    this.getRnoInfo();
    //this.getCompanyData();
    this.getCurrency();
    this.getPaymentList();

    //预设收款人为申请人工号
    this.headForm.controls.payee.setValue(this.userInfo?.emplid);
    this.headForm.controls.payeeName.setValue(this.userInfo?.ename);
    let defaultUser: Approver ={ emplid: this.userInfo?.emplid, name: this.userInfo?.cname, nameA: this.userInfo?.ename, display: this.userInfo?.ename };
    this.employeeInfoList.push(defaultUser);
    this.headForm.controls.selectedUser.setValue(this.employeeInfoList[0])
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
        console.log('res.body', res.body);
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
  async columnSubscribe() {
    this.headForm.controls.payee.valueChanges.subscribe((value) => {
      if (
        !!value &&
        this.headForm.controls.payee.enabled &&
        !this.isSpinning
      ) {
        // let rno = this.headForm.controls.rno.value;
        // let data = {
        //   rno: !!rno ? this.crypto.encrypt(rno) : null,
        //   user: value
        // };
        // this.router.navigate([`pages/ers-pages/rq201`], { queryParams: { data: JSON.stringify(data) } });
        this.isSpinning = true;
        this.commonSrv.getEmployeeInfoById(value).subscribe(async (res) => {
          this.applicantInfo = res;
          let costDeptInfo = await this.getGetCostDeptid(this.applicantInfo?.deptid, this.applicantInfo?.company);
          if(costDeptInfo) this.applicantInfo.costdeptid = costDeptInfo?.body?.data;
          this.costdeptid.deptId = this.applicantInfo.costdeptid;
          this.getEmployeeInfo();

          this.isSpinning = false;
        });
        this.getCategoryList(value);
      }
    });
    this.headForm.controls.companyCode.valueChanges.subscribe((value) => {
      if (!!value && this.headForm.controls.companyCode.enabled) {
        this.headForm.controls.projectCode.reset();
        this.selectedArea = '';
        //update area
        if (this.headForm.controls.companyCode.value) {
          this.companyList.forEach((o) => {
            if (o.companyCategory == this.headForm.controls.companyCode.value) {
              this.selectedArea = o.area;
              console.log('selectedArea = ', this.selectedArea);
              return;
            }
          });
        }

        this.getAllCustomerList();
        this.getTaxRate(value);
        if(this.costdeptid.deptId) this.getDeptList(this.costdeptid.deptId);

      }
    });
    this.detailForm.get('amount').valueChanges.subscribe((value) => {
      this.detailForm.controls.taxLoss.setValue(
        (value * this.exchangeRate * this.taxRate).toFixed(2)
      );
    });
    this.listForm.get('curr').valueChanges.subscribe((value) => {
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
          this.afterChangeCurr(value);
        });
      } else if (value == this.curr) {
        this.exchangeRate = 1;
        this.afterChangeCurr(value);
      }
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
      this.deptList = [];
      // this.customerList = [];
      this.curr = null;
      this.costdeptid = { deptId: null, deptName: null, deptLabel: null };
      this.isaccount = false;
      this.showModal = false;
      this.exceptionModal = false;
      this.tipModal = false;
      this.isSaveLoading = false;
      this.totalDetailTableData = [];
      this.detailTableData = [];
      this.detailTableShowData = [];
      this.listTableData = [];
      this.keyId = 0;
      this.uid = 0;
      this.exInfo = [];
      this.exWarning = [];
      this.exTip = '';
      this.exTotalWarning = [];
      this.tipAffordParty = '';
      this.isSpinning = false;
      this.isAllUpload = true;
      this.canConfirm = this.translate.instant('confirm');
      this.exchangeRate = 1;
      this.totalFileList = [];
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
    if (
      this.detailTableData.length > 0 &&
      this.detailTableData[0].curr != value
    ) {
      this.detailTableData.map((o) => {
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
      console.log('getRnoInfo============',res.data);
      if (this.isSpinning) {
        if (res && res.data && JSON.parse(res.data)) {
          let data = JSON.parse(res.data);
          console.log('getRnoInfo', data);
          // if (!!data.user) {
          //   this.commonSrv.getEmployeeInfoById(data.user).subscribe(res => {
          //     this.isSpinning = true;
          //     this.userInfo = res;
          //     this.getEmployeeInfo();
          //     this.isSpinning = false;
          //   });
          // }
          if (!!data.rno) {
            let rno: string = this.crypto.decrypt(data.rno);
            this.headForm.controls.rno.setValue(rno);
            // 获取单据信息
            if (!data.user) {
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
          }
        } else {
          this.getEmployeeInfo();
        }
      }
    });
  }

  assembleFromData(value): void {
    console.log('assembleFromData', value);
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
      this.headForm.controls.individualTax.setValue(
        Number((summaryAtmData.amount - summaryAtmData.actamt).toFixed(2))
      );
    }

    if (listData != null) {
      let exInfoList: any = [];
      let invoiceFileList: any = [];
      listData.map((o) => {
        let selfTaxAmt = 0;
        o.invList
          .filter((i) => i.undertaker == 'self')
          .map((i) => {
            selfTaxAmt += i.taxloss;
          });
        this.listTableData.push({
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          curr: o.currency,
          appliedAmount: o.amount1,
          toLocalCurrAmount: o.baseamt,
          exchangeRate: o.rate,
          expenseAttribDept: o.deptid,
          entertainObject: o.object,
          accompany: o.keep,
          remark: o.remarks,
          invoiceTotalAmount: o.amount2,
          entertainDate: format(new Date(o.hospdate), 'yyyy/MM/dd'),
          id: o.seq,
          disabled: false,
          selfTaxAmt: selfTaxAmt,
          actualAmt: o.amount,
          unifyCode: o.unifycode,
          inputEntertainObject: '',
        });
        exInfoList = exInfoList.concat(o.invList);
        invoiceFileList = invoiceFileList.concat(o.fileList);
      });

      exInfoList.map((o) => {
        if (o.expcode != null && o.expcode != '') {
          this.exTotalWarning.push(o.expcode);
        }
        this.totalDetailTableData.push({
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
          id: o.seq,
          index: o.item,
          uid: o.item,
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
          invoiceSource: o.source,
        });
      });
      // 组装文件
      invoiceFileList.map((o) => {
        this.totalFileList.push({
          id: o.seq,
          uid: o.item.toString(),
          name: o.filename,
          filename: o.tofn,
          status: 'done',
          url: this.commonSrv.changeDomain(o.url),
          upload: true,
          type: o.filetype,
          invoiceno: o.invoiceno,
          category: o.category,
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
      this.keyId = this.listTableData.sort((a, b) => b.id - a.id)[0].id;
      if (this.totalDetailTableData.length > 0)
        this.uid = this.totalDetailTableData.sort(
          (a, b) => b.index - a.index
        )[0].index; // uid与index共用this.uid

      if (this.listTableData.length > 0) {
        this.headForm.controls.emplid.disable({ emitEvent: false });
        this.headForm.controls.companyCode.disable({ emitEvent: false });
      }
      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
      this.attachmentList = [...this.attachmentList];
      this.setDetailTableShowData();
    }
    // this.totalFileList.filter(o => o.url != '...').map(o => {    // 皆可用
    //   this.getFile(o.url, function (file) {
    //     o.originFileObj = file
    //   });
    // });
    this.totalFileList.map(async (o) => {
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

  getEmployeeInfo() {
    if (!this.applicantInfo) {
      this.applicantInfo = this.userInfo;
    }
    this.userIdList = this.commonSrv.getUserInfo?.proxylist ?? [];
    if (this.userIdList.indexOf(this.applicantInfo.emplid) == -1) {
      this.userIdList.push(this.applicantInfo.emplid);
    }
    //this.headForm.controls.companyCode.setValue(this.applicantInfo.company);
    this.headForm.controls.emplid.setValue(this.userInfo.emplid);
    this.headForm.controls.applicantName.setValue(this.userInfo.cname);
    this.headForm.controls.dept.setValue(this.userInfo.deptid);
    //this.headForm.controls.payee.setValue(this.applicantInfo.cname);
    this.headForm.controls.ext.setValue(this.applicantInfo.phone);
    this.curr = this.applicantInfo.curr;
    // if (
    //   this.detailListTableColumn
    //     .filter((o) => o.columnKey == 'taxLoss')[0]
    //     .title.indexOf('(') == -1
    // )
    //   this.detailListTableColumn.filter(
    //     (o) => o.columnKey == 'taxLoss'
    //   )[0].title += `(${this.curr})`;

    console.log('this.applicantInfo', this.applicantInfo);
    this.costdeptid.deptId = this.applicantInfo.costdeptid;
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
    console.log('getDeptList', value);
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
          console.log('res.body', res.body);
          this.deptList = res.body.map((item) => {
            return {
              deptId: item.deptid,
              deptName: item.descr,
              deptLabel: `${item.deptid} : ${item.descr}`,
            };
          });
          if (this.deptList.length > 0) {
            if (
              value == this.applicantInfo.costdeptid &&
              !this.costdeptid?.deptLabel
            ) {
              this.costdeptid = this.deptList[0];
            }
            if (!this.showModal) {
              console.log('this.showModal', this.deptList);
              this.listForm.controls.expenseAttribDept.setValue(
                this.deptList[0].deptId
              );
            }
          }
        } else {
          this.message.error(res.message, { nzDuration: 6000 });
        }
      });
    } //else this.deptList = [];
  }

  resetFormData(first: boolean = false) {
    let deptId = null;
    if (
      this.headForm.controls.companyCode.value == this.applicantInfo.company
    ) {
      this.deptList = [];
      this.deptList.push(this.costdeptid);
      // this.deptList.push({ deptId: this.costdeptid, deptName: '', deptLabel: this.costdeptid });
      deptId = this.costdeptid.deptId;
    }
    if (first) {
      this.listForm.controls.expenseAttribDept.setValue(deptId);
      this.listForm.controls.curr.setValue(this.curr);
      this.detailForm.controls.curr.setValue(this.curr);
    } else {
      this.keyId++;
      this.listForm.reset({
        disabled: false,
        id: this.keyId,
        curr: this.curr,
        expenseAttribDept: deptId,
      });
      this.detailForm.reset({
        disabled: false,
        id: this.keyId,
        curr: this.curr,
      });
    }
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
    this.customerList = [];
    this.originCustomerList = [];
    if (!this.headForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetAllCustomerName,
      { company: this.headForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200) {
        this.customerList = res.body;
        this.originCustomerList = res.body;

        if(this.selectedArea === ERSConstants.Area.TW){
          this.customerList.unshift({nickname: 'Other',name: 'Other'});
          this.customerList = [...this.customerList];
        }
      } else {
        this.message.error(res.message, { nzDuration: 6000 });
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
      this.isSpinning = false;
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
    // js 单元格显示千分位
    // var aList = document.getElementsByTagName('td');
    // for (var i = 0; i < aList.length; i++) {
    //   if (aList[i].innerHTML.length == 0 || isNaN(Number(aList[i].innerHTML))) continue;
    //   aList[i].innerHTML = Number(aList[i].innerHTML).toLocaleString();
    // }
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
    this.addloading = true;
    if (!this.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'), {
        nzDuration: 6000,
      });
      this.addloading = false;
      return;
    }
    this.detailTableData = [];
    this.setDetailTableShowData();
    this.resetFormData();
    this.isAllUpload = true;
    this.fileList = [];
    this.showModal = true;
  }
  checkInvoiceDetail(id: number): void {
    // let invoiceDetailString = ``;
    // let uidList = this.totalDetailTableData.filter(o => o.id == id && o.uid !== undefined).map(o => o.uid.toString());
    // if (uidList.length > 0) {

    this.totalFileList
      .filter((o) => o.id == id && o.url == '...' && !o.preview)
      .map(
        async (o) =>
          (o.preview = await this.commonSrv.getPicBase64(o.originFileObj!))
      );
    this.fileList = this.totalFileList.filter((o) => o.id == id);
    // // this.totalFileList.filter(o => uidList.indexOf(o.uid) !== -1).map(o => invoiceDetailString += `<a (click)="test(${o.preview})>` + o.name + '</a>' + '<br>')
    // // this.totalFileList.filter(o => uidList.indexOf(o.uid) !== -1).map(o => invoiceDetailString += `<a href="${o.preview}" target="_blank">` + o.name + '</a>' + '<br>')
    // // this.totalFileList.filter(o => uidList.indexOf(o.uid) !== -1).map(async o => invoiceDetailString += `<a href="${await this.commonSrv.getPicBase64(o.originFileObj!)}">` + o.name + '</a>' + '<br>')
    // }

    // // this.modal.info({
    // //   nzTitle: this.translate.instant('invoice-detail'),
    // //   nzContent: invoiceDetailString,
    // //   nzMaskClosable: true
    // // });
  }

  editRow(id: number): void {
    this.editloading = true;
    this.detailTableData = [];
    let rowFormData = this.listTableData.filter((d) => d.id == id)[0];
    let rowdetailData = this.totalDetailTableData.filter((d) => d.id == id);
    this.listForm.reset(rowFormData);
    this.deptList.push({
      deptId: rowFormData.expenseAttribDept,
      deptName: '',
      deptLabel: rowFormData.expenseAttribDept,
    });
    // this.customerList.push(rowFormData.entertainObject);
    this.detailTableData = rowdetailData;
    this.setDetailTableShowData();
    this.fileList = this.totalFileList.filter((o) => o.id == id);
    this.showModal = true;
    this.editloading = false;
  }

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {
      //多选操作
      this.totalFileList = this.totalFileList.filter((o) =>
        this.setOfCheckedId.has(o.id)
      );
      this.totalDetailTableData = this.totalDetailTableData.filter(
        (o) => !this.setOfCheckedId.has(o.id)
      );
      this.listTableData = this.listTableData.filter(
        (d) => !this.setOfCheckedId.has(d.id)
      );
      this.setOfCheckedId.clear();
    } else {
      this.totalFileList = this.totalFileList.filter((o) => o.id == id);
      this.totalDetailTableData = this.totalDetailTableData.filter(
        (o) => o.id != id
      );
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

    if (this.selectedArea === ERSConstants.Area.TW) {
      if (this.detailTableData.length > 1) {
          this.message.error(this.translate.instant('only-select-one-invoice'), {
            nzDuration: 6000,
          });
          this.isSpinning = false;
          this.isSaveLoading = false;
          return;
        }
    }

    // if (this.fileList.filter((o) => !o.upload).length > 0) {
    //   this.message.error(this.translate.instant('upload-invoice-required'), {
    //     nzDuration: 6000,
    //   });
    //   this.isSpinning = false;
    //   this.isSaveLoading = false;
    //   return;
    // }
    if (this.detailTableData.length == 0 && this.fileList.length == 0) {
      this.message.error(this.translate.instant('add-invoice-required'), {
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
    let totalExAmount = 0;
    let invoiceExAmount = 0;
    this.detailTableData.map((o) => {
      if (o.disabled) {
        invoiceExAmount += Number(o.oamount);
        totalExAmount += Number(o.oamount);
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
    } else {
      if (
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
    }
    this.setListTableData();
  }

  setListTableData(): void {
    let amount = this.listForm.controls.appliedAmount.value;
    this.listForm.controls.exchangeRate.setValue(this.exchangeRate);
    this.listForm.controls.toLocalCurrAmount.setValue(
      Number((amount * this.exchangeRate).toFixed(2))
    );
    // 计算发票总金额
    let invoiceTotal = 0;
    this.detailTableData
      .filter((o) => o.disabled)
      .map((item) => (invoiceTotal += Number(item.oamount)));
    this.listForm.controls.invoiceTotalAmount.setValue(invoiceTotal);

    let feeType = typeof this.listForm.controls.feeDate.value;
    let enType = typeof this.listForm.controls.entertainDate.value;
    if (this.listForm.controls.feeDate.value != null && feeType != 'string') {
      let feeDate =
        this.listForm.controls.feeDate.value == null
          ? null
          : format(this.listForm.controls.feeDate.value, 'yyyy/MM/dd');
      this.listForm.controls.feeDate.setValue(feeDate);
    }
    if (
      this.listForm.controls.entertainDate.value != null &&
      enType != 'string'
    ) {
      let entertainDate =
        this.listForm.controls.entertainDate.value == null
          ? null
          : format(this.listForm.controls.entertainDate.value, 'yyyy/MM/dd');
      this.listForm.controls.entertainDate.setValue(entertainDate);
    }
    let rowId = this.listForm.controls.id.value;
    this.listTableData = this.listTableData.filter((o) => o.id != rowId);
    let data = this.listForm.getRawValue();
    // 计算amount2
    let selfAmt = 0;
    this.detailTableData
      .filter((f) => f.affordPartyValue == 'self')
      .forEach((f) => (selfAmt += Number(f.toLocalTaxLoss)));
    data['selfTaxAmt'] = Number(selfAmt.toFixed(2));
    let actualAmt = Number(
      (this.listForm.controls.toLocalCurrAmount.value - selfAmt).toFixed(2)
    );
    data['actualAmt'] = Number(actualAmt.toFixed(2));

    this.listTableData.push(data);
    this.listTableData = [...this.listTableData]; // 刷新表格

    // 编辑模式下，覆盖前数据
    if (this.totalFileList.length > 0)
      this.totalFileList = this.totalFileList.filter((o) => o.id != rowId);
    if (this.fileList.length > 0)
      this.totalFileList = this.fileList.concat(this.totalFileList);

    if (this.totalDetailTableData.length > 0)
      this.totalDetailTableData = this.totalDetailTableData.filter(
        (o) => o.id != rowId
      );
    if (this.detailTableData.length > 0)
      this.totalDetailTableData = this.detailTableData.concat(
        this.totalDetailTableData
      );
    this.setStatistic();
    this.showModal = false;
    this.isSaveLoading = false;
    this.addloading = false;
    this.editloading = false;
    this.isSpinning = false;
  }

  setStatistic(): void {
    let appliedTotal = 0;
    let selfTax = 0;
    if (this.listTableData.length > 0) {
      this.headForm.controls.emplid.disable({ emitEvent: false });
      this.headForm.controls.companyCode.disable({ emitEvent: false });
    } else {
      this.headForm.controls.emplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
    }
    this.listTableData.map((o) => {
      appliedTotal += Number(o.toLocalCurrAmount);
    });
    this.totalDetailTableData
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
      Number((appliedTotal - selfTax).toFixed(2)).toLocaleString('zh-CN')
    );
    // 异常提示：
    this.exTotalWarning = [];
    let idx = 0;
    this.totalDetailTableData.map((o) => {
      if (!!o.exTips || !o.disabled) {
        this.exTotalWarning.push(++idx + '. ' + o.exTips);
      }
    });
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  addExceptionItem(): void {
    // index 与 uid 合用自增长唯一排序值：this.uid  => detailTableData中 index == uid
    this.detailForm.reset({
      disabled: false,
      id: this.listForm.controls.id.value,
      index: ++this.uid,
      curr: this.listForm.controls.curr.value,
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
    if (this.detailForm.controls.amount.value == 0) {
      this.message.error(this.translate.instant('ex-amount-zero-error'), {
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
    let item = this.detailForm.getRawValue();
    item['oamount'] = item.amount;
    this.detailTableData.push(item);
    this.setDetailTableShowData();
    this.isSaveLoading = false;
    this.exceptionModal = false;
  }

  handleExCancel(): void {
    this.exceptionModal = false;
  }

  deleteExRow(item: any): void {
    if (item.data.disabled) {
      this.message.warning(this.translate.instant('delete-warning'));
      return;
    }
    let index = item.data.index;
    this.detailTableData = this.detailTableData.filter((d) => d.index != index);
    this.setDetailTableShowData();
  }

  uploadInvoice(): void {
    this.isSaveLoading = true;
    this.isSpinning = true;
    const formData = new FormData();
    this.fileList
      .filter((o) => !o.upload)
      .forEach((file: any) => {
        formData.append(file.uid, file.originFileObj);
      });
    // this.Service.Post('http://localhost:5000' + URLConst.PostFileToRead, formData).subscribe((res) => {
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
      file.uid = (++this.uid).toString();
      let uploadedFile;
      if (this.showModal)
        uploadedFile = this.fileList.filter(
          (o) => o.originFileObj.name == file.name
        );
      else
        uploadedFile = this.attachmentList.filter(
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

  // beforeUploadAtt = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
  //   return new Observable((observer: Observer<boolean>) => {
  //     let uploadedFile = this.attachmentList.filter(o => o.originFileObj.name == file.name);
  //     let upload = uploadedFile.length == 0;
  //     if (!upload) this.message.error(this.commonSrv.FormatString(this.translate.instant('has-been-uploaded-that'), uploadedFile[0].originFileObj.name, uploadedFile[0].name));
  //     observer.next(upload);
  //     observer.complete();
  //   });
  // };

  removeFile = (file: NzUploadFile) => {
    return new Observable((observer: Observer<boolean>) => {
      this.detailTableData = this.detailTableData.filter(
        (o) => o.uid != file.uid
      );
      this.setDetailTableShowData();
      observer.next(true);
      observer.complete();
    });
  };

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
    console.log('add inv',value)
    this.detailTableData = this.detailTableData.filter((o) => !o.disabled);
    this.fileList = [];
    value.forEach((o) => {
      let item = o;
      item['uid'] = (++this.uid).toString();
      item['index'] = this.uid.toString();
      this.detailTableData.push(item);
      // this.fileList.push({
      //   id: o.id,
      //   uid: this.uid.toString(),
      //   url: o.fileurl,
      //   name: o.invtype + o.oamount,
      //   upload: true,
      // });
      this.fileList.push(o);
    });
    this.listForm.controls.invoiceDetailList.setValue(this.detailTableData);
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
        (o) => cannotExpenseUid.indexOf(Number(o.uid)) == -1
      );
    }
    this.exInfo = this.exInfo.filter((o) => o.paymentStat);
    if (this.exInfo.length > 0) {
      let rowId = this.listForm.controls.id.value;
      // this.detailTableData = this.detailTableData.filter(o => !o.disabled || this.fileList.filter(p => p.upload).map(t => t.uid).indexOf(o.uid) != -1);
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
          invtype: item['invtype'],
          invoiceid: null,
          invabnormalreason: null,
          fileurl: null,
          invoiceSource: item['source'],
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
    // this.detailTableShowData = this.detailTableData.filter(
    //   (o) => o.exTips != null && o.exTips != ''
    // );
    this.detailTableShowData = [...this.detailTableData]; // 刷新表格
  }

  handleTipCancel(): void {
    this.exInfo = [];
    this.fileList = this.fileList.filter((o) => o.upload);
    this.detailTableData = this.detailTableData.filter(
      (o) =>
        !o.disabled ||
        this.fileList.map((t) => t.uid).indexOf(o.uid.toString()) != -1
    );
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq201,
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
          nzOnOk: () => this.router.navigate([`ers/rq201`]), // reset form data
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq201,
      formData
    ).subscribe((res) => {
      // this.Service.Post('http://localhost:5000' + URLConst.SaveRq201, formData).subscribe((res) => {
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
      cname: this.headForm.controls.applicantName.value,
      deptid: this.headForm.controls.dept.value,
      ext: this.headForm.controls.ext.value,
      company: this.headForm.controls.companyCode.value,
      projectcode: this.headForm.controls.projectCode.value,
      currency: this.curr,
      // amount: this.headForm.controls.appliedTotal.value,
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payeeName.value,
      paymentway: this.headForm.controls.paymentWay.value,
    };
    formData.append('head', JSON.stringify(headData));

    let listData = this.listTableData.map((o) => {
      return {
        rno: rno,
        seq: o.id,
        rdate: o.feeDate,
        deptid: o.expenseAttribDept,
        currency: o.curr,
        amount1: o.appliedAmount,
        baseamt: o.toLocalCurrAmount,
        rate: o.exchangeRate,
        //選other時代表是user手動輸入的招待對象
        object: o.entertainObject !== 'Other' ? o.entertainObject : o.inputEntertainObject,
        hospdate: o.entertainDate,
        keep: o.accompany,
        remarks: o.remark,
        basecurr: this.curr,
        // payeedeptid: o.expenseAttribDept,
        amount2: o.invoiceTotalAmount,
        amount: o.actualAmt,
        senarioid: '',
      };
    });
    formData.append('detail', JSON.stringify(listData));

    let detailData = this.totalDetailTableData.map((o) => {
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
      };
    });
    formData.append('inv', JSON.stringify(detailData));

    let invoiceFileData = this.totalFileList.map((o) => {
      return {
        rno: rno,
        seq: o.id,
        item: Number(o.uid),
        filetype: o.type,
        filename: o.name,
        ishead: 'N',
        key: Guid.create().toString(),
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
      amount: this.headForm.controls.appliedTotal.value,
      actamt: this.headForm.controls.actualTotal.value,
    };
    formData.append('amount', JSON.stringify(amountData));

    // this.totalFileList.forEach((file: any) => {
    //   let key = invoiceFileData.filter(o => o.seq == file.id && o.item == Number(file.uid)).map(o => o.key)[0];
    //   formData.append(key, file.originFileObj);
    // });
    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
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
        console.log('QueryCategoryListByUserId = ', res.body.data);
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

  compareFn = (o1: Approver, o2: Approver): boolean => o1 && o2 && o1.emplid === o2.emplid && o1.name === o2.name && o1.nameA === o2.nameA && o1.display === o2.display;

  selectedUser(event: any) {
    console.log('selectedUser event: ', event);
    this.isSpinning = false;
    this.headForm.controls.payee.setValue(event?.emplid);
    this.headForm.controls.payeeName.setValue(event?.nameA);
    //this.columnValueChange();
  }

  hostObjChange(){
    console.log('hostObjChange: ', this.listForm.controls.entertainObject.value);
    if(this.listForm.controls.entertainObject.value === 'Other'){
      this.showTypeHostObj = true;
    } else {
      this.showTypeHostObj = false;
      this.listForm.controls.entertainObject2.setValue('');
    }
  }

}
