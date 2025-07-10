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
import { DelayInfo, OverdueChargeAgainstDetail } from './classes/data-item';
// import { OverdueChargeAgainstDetail } from '../rq401/classes/data-item'
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Item } from 'angular2-multiselect-dropdown';

@Component({
  selector: 'app-rq401a',
  templateUrl: './rq401a.component.html',
  styleUrls: ['./rq401a.component.scss'],
})
export class RQ401AComponent implements OnInit, OnDestroy {
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
      title: this.translate.instant('advance-fund-no'),
      columnKey: 'advanceFundRno',
      columnWidth: '115px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.advanceFundRno.localeCompare(b.advanceFundRno),
    },
    {
      title: this.translate.instant('digest'),
      columnKey: 'digest',
      columnWidth: '100px',
      align: 'left',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.digest.localeCompare(b.digest),
    },
    {
      title: this.translate.instant('col.applied-amount'),
      columnKey: 'appliedAmt',
      columnWidth: '85px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.appliedAmt - b.appliedAmt,
    },
    {
      title: this.translate.instant('not-charge-against-amount'),
      columnKey: 'notChargeAgainstAmt',
      columnWidth: '100px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.notChargeAgainstAmt - b.notChargeAgainstAmt,
    },
    {
      title: this.translate.instant('open-days'),
      columnKey: 'openDays',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.openDays - b.openDays,
    },
    {
      title: this.translate.instant('delay-times'),
      columnKey: 'delayTimes',
      columnWidth: '85px',
      align: 'center',
      sortFn: (a: OverdueChargeAgainstDetail, b: OverdueChargeAgainstDetail) =>
        a.delayTimes - b.delayTimes,
    },
  ];

  DelayInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('advance-fund-no'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('digest'),
      columnKey: 'digest',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.digest.localeCompare(b.digest),
    },
    {
      title: this.translate.instant('col.applied-amount'),
      columnKey: 'appliedAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.appliedAmt - b.appliedAmt,
    },
    {
      title: this.translate.instant('not-charge-against-amount'),
      columnKey: 'notChargeAgainstAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.notChargeAgainstAmt - b.notChargeAgainstAmt,
    },
    {
      title: this.translate.instant('scheduled-debit-date'),
      columnKey: 'scheduledDebitDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.scheduledDebitDate.localeCompare(b.scheduledDebitDate),
    },
    {
      title: this.translate.instant('open-days'),
      columnKey: 'openDays',
      columnWidth: '',
      align: 'right',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.openDays - b.openDays,
    },
    {
      title: this.translate.instant('delay-charge-against-days'),
      columnKey: 'delayChargeAgainstDays',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.delayChargeAgainstDays - b.delayChargeAgainstDays,
    },
    {
      title: this.translate.instant('after-date'),
      columnKey: 'afterDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.afterDate.localeCompare(b.afterDate),
    },
    {
      title: this.translate.instant('delay-reason'),
      columnKey: 'delayReason',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.delayReason.localeCompare(b.delayReason),
    },
  ];



  //#region 参数
  nzFilterOption = () => true;
  screenWidth: any;
  headForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  curr: string;
  showDelayInfoModal: boolean = false;
  isSaveLoading: boolean = false;
  showChargeAgainst: boolean = false;
  overdueTableColumn = this.chargeAgainstTableColumn;
  listTableColumn = this.DelayInfoTableColumn;
  chargeAgainstTableData: OverdueChargeAgainstDetail[] = [];
  chargeAgainstShowTableData: OverdueChargeAgainstDetail[] = [];
  listTableData: DelayInfo[] = [];
  keyId: number = 0;
  isSpinning = false;
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
  selectedRno: string = null;
  applicantInfo: any;
  userInfo: any;
  userIdList: string[] = [];
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
    public domSanitizer: DomSanitizer
  ) {
    this.navigationSubscription = this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        // this.dataInitial();
      }
    });
  }

  ngOnInit(): void {
    this.isSpinning = true;
    this.isFirstLoading = false;
    this.screenWidth =
      window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
    this.headForm = this.fb.group({
      emplid: [null],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      payee: [{ value: null, disabled: true }],
      ext: [null],
      companyCode: [null, [Validators.required]],
    });
    this.listForm = this.fb.group({
      id: [this.keyId],
      delayDays: [null, [Validators.required]],
      delayReason: [null, [Validators.required]],
    });
    this.headForm.controls.emplid.valueChanges.subscribe((value) => {
      if (!!value && !this.isSpinning) {
        this.isSpinning = true;
        this.commonSrv.getEmployeeInfoById(value).subscribe((res) => {
          this.applicantInfo = res;
          this.getEmployeeInfo();
        });
      }
    });

    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
    // this.getRnoInfo();
    // this.getCompanyData();
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  dataInitial(): void {
    if (!this.isFirstLoading) {
      this.companyList = [];
      this.curr = null;
      this.listTableData = [];
      this.keyId = 0;
      this.isSpinning = false;
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
    },
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
    if (headData != null) {
      this.applicantInfo = {
        company: headData.company,
        emplid: headData.cuser,
        deptid: headData.deptid,
        cname: headData.cname,
        phone: headData.ext,
        curr: headData.currency,
        costdeptid: headData.costdeptid,
        isaccount: true,
      };
      this.cuser = headData.cuser;
      this.getEmployeeInfo();
    }

    if (listData != null) {
      listData.map((o) => {
        let afterDate = new Date(o.revsdate);
        afterDate.setDate(afterDate.getDate() + o.delaydays);
        o.url = this.commonSrv.changeDomain(o.url);
        this.listTableData.push({
          id: o.seq,
          rno: o.advancerno,
          digest: o.summary,
          appliedAmt: o.amount1,
          notChargeAgainstAmt: o.amount2,
          scheduledDebitDate: format(new Date(o.revsdate), 'yyyy/MM/dd'),
          openDays: Math.ceil(
            (Number(new Date()) - Number(new Date(o.cdate))) /
              (1000 * 3600 * 24)
          ),
          delayChargeAgainstDays: o.delaydays,
          afterDate: format(afterDate, 'yyyy/MM/dd'),
          delayReason: o.delayreason,
          disabled: false,
          cdate: o.cdate,
        });
      });
      if (this.listTableData.length > 0) {
        this.headForm.controls.emplid.disable({ emitEvent: false });
        this.headForm.controls.companyCode.disable({ emitEvent: false });
      }
      this.keyId = this.listTableData.sort((a, b) => b.id - a.id)[0].id;
      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
    }
  }

  getChargeAgainstTableData() {
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'));
      this.isSpinning = false;
      return;
    }
    this.Service.Post(
      URLConst.GetDefferredData + `/${this.applicantInfo.emplid}`,
      null
    ).subscribe((res) => {
      this.chargeAgainstTableData = [];
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          let rnoList = this.listTableData.map((o) => {
            return o.rno;
          });
          res.body.data?.map((o) => {
            this.chargeAgainstTableData.push({
              companyCode: o.company,
              advanceFundRno: o.rno,
              digest: o.remark,
              appliedAmt: o.amount,
              notChargeAgainstAmt: o.actamt,
              openDays: o.opendays, //Math.ceil((Number(new Date()) - Number(new Date(o.cdate))) / (1000 * 3600 * 24)),
              delayTimes: o.delay,
              advanceDate: o.revsdate != null ? new Date(o.revsdate) : null,
              disabled: rnoList.indexOf(o.rno) !== -1,
              cdate: o.cdate,
            });
          });
          if (this.chargeAgainstTableData.length > 0) {
            this.chargeAgainstTableData = [...this.chargeAgainstTableData];
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
    this.isSpinning = true;
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

  showChargeAgainstModal() {
    if (!this.applicantInfo.isaccount) {
      this.message.error(this.translate.instant('have-not-bank-card'));
      this.addloading = false;
      return;
    }
    this.chargeAgainstShowTableData = JSON.parse(
      JSON.stringify(this.chargeAgainstTableData.filter((o) => !o.disabled))
    );
    this.chargeAgainstShowTableData
      .filter(
        (o) =>
          o.companyCode != this.headForm.controls.companyCode.value ||
          o.delayTimes > 0
      )
      .map((o) => (o.disabled = true));
    this.chargeAgainstShowTableData = [...this.chargeAgainstShowTableData];
    this.selectedRno = null;
    this.showChargeAgainst = true;
  }

  selectRnoOnChange(rno: string, value) {
    if (value) {
      if (rno != this.selectedRno) this.selectedRno = rno;
    } else {
      if (rno == this.selectedRno) this.selectedRno = null;
    }
  }

  addItem(): void {
    this.addloading = true;
    if (this.selectedRno == null) {
      this.message.error(this.translate.instant('select-one-item'));
      this.addloading = false;
      return;
    }
    this.keyId++;
    this.listForm.reset({ id: this.keyId });
    this.showDelayInfoModal = true;
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
    let rowInfo = this.chargeAgainstTableData.filter(
      (o) => o.advanceFundRno == this.selectedRno
    )[0];
    rowInfo.disabled = true;
    this.listTableData = this.listTableData.filter(
      (o) => o.id != listFormData.id
    );
    if (rowInfo != null) {
      let afterDate = new Date(rowInfo.advanceDate);
      afterDate.setDate(rowInfo.advanceDate.getDate() + listFormData.delayDays);
      this.listTableData.push({
        id: listFormData.id,
        rno: rowInfo.advanceFundRno,
        digest: rowInfo.digest,
        appliedAmt: rowInfo.appliedAmt,
        notChargeAgainstAmt: rowInfo.notChargeAgainstAmt,
        scheduledDebitDate:
          rowInfo.advanceDate == null
            ? ''
            : format(rowInfo.advanceDate, 'yyyy/MM/dd'),
        openDays: rowInfo.openDays,
        delayChargeAgainstDays: listFormData.delayDays,
        afterDate: format(afterDate, 'yyyy/MM/dd'),
        delayReason: listFormData.delayReason,
        disabled: false,
        cdate: rowInfo.cdate,
      });
    }
    if (this.listTableData.length > 0) {
      this.headForm.controls.emplid.disable({ emitEvent: false });
      this.headForm.controls.companyCode.disable({ emitEvent: false });
    }
    this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
    this.listTableData = [...this.listTableData]; // 刷新表格

    this.showDelayInfoModal = false;
    this.showChargeAgainst = false;
    this.isSaveLoading = false;
    this.addloading = false;
    this.editloading = false;
    this.isSpinning = false;
  }

  handleCancel(): void {
    this.showDelayInfoModal = false;
    this.addloading = false;
    this.editloading = false;
  }

  deleteRow(id: number = -1): void {
    this.deleteloading = true;
    if (id == -1) {
      //多选操作
      let rnoList = this.listTableData
        .filter((d) => this.setOfCheckedId.has(d.id))
        .map((o) => o.rno);
      this.chargeAgainstTableData
        .filter((o) => rnoList.indexOf(o.advanceFundRno) !== -1)
        .map((o) => (o.disabled = false));
      this.listTableData = this.listTableData.filter(
        (d) => !this.setOfCheckedId.has(d.id)
      );
      this.setOfCheckedId.clear();
    } else {
      let rno = this.listTableData.filter((d) => d.id == id)[0].rno;
      this.chargeAgainstTableData.filter(
        (o) => o.advanceFundRno == rno
      )[0].disabled = false;
      this.listTableData = this.listTableData.filter((d) => d.id != id);
      this.setOfCheckedId.delete(id);
    }
    if (this.listTableData.length == 0) {
      this.headForm.controls.emplid.enable({ emitEvent: false });
      this.headForm.controls.companyCode.enable({ emitEvent: false });
    }
    this.deleteloading = false;
  }

  ////////带选择框表
  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: DelayInfo[] = [];
  setOfCheckedId = new Set<number>();
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: DelayInfo[]): void {
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

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
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

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.attachmentList = fileList;
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
    if (this.listTableData.length == 0) {
      this.message.error(this.translate.instant('fill-in-detail'));
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
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SubmitRq401a,
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
          nzOnOk: () => this.router.navigateByUrl(`ers/rq401a`), // reset form data
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
    //暫存表單
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SaveRq401a,
      formData
    ).subscribe((res) => {
      // this.Service.Post('http://localhost:5000' + URLConst.SaveRq401a, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(
          this.translate.instant('save-successfully') +
            `Request NO: ${res.body.data.rno}`
        );
        this.headForm.controls.rno.setValue(res.body.data.rno);
        if (!this.cuser) {
          this.cuser = this.userInfo?.emplid;
        }
      } else {
        this.message.error(
          this.translate.instant('save-failed') +
            (res.status === 200
              ? res.body.message
              : this.translate.instant('server-error'))
        );
      }
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
      payeeId: this.headForm.controls.emplid.value,
      payeename: this.headForm.controls.payee.value,
      currency: this.curr,
    };
    formData.append('head', JSON.stringify(headData));

    let listData = this.listTableData.map((o) => {
      return {
        rno: rno,
        seq: o.id,
        advancerno: o.rno,
        summary: o.digest,
        amount1: o.appliedAmt,
        amount2: o.notChargeAgainstAmt,
        revsdate: o.scheduledDebitDate,
        cdate: new Date(o.cdate),
        delaydays: o.delayChargeAgainstDays,
        // xxxx2: o.afterDate,
        delayreason: o.delayReason,
        basecurr: this.curr,
        deptid: this.headForm.controls.dept.value,
      };
    });
    formData.append('detail', JSON.stringify(listData));

    let indx = 0;
    let attachmentData = this.attachmentList.map((o) => {
      return {
        rno: rno,
        seq: 0,
        item: indx++,
        category: null,
        filetype: o.type,
        filename: o.name,
        ishead: 'Y',
        key: o.uid,
      };
    });
    formData.append('file', JSON.stringify(attachmentData));

    // let amountData = {
    //     rno: rno,
    //     currency: this.curr,
    //     amount: this.headForm.controls.totalCost.value,
    //     actamt: this.headForm.controls.totalCost.value,
    // }
    // formData.append('amount', JSON.stringify(amountData));

    this.attachmentList.forEach((file: any) => {
      formData.append(file.uid, file.originFileObj);
    });
    //#endregion
    return formData;
  }
}
