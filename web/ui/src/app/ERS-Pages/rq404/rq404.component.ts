import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, FormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { AdvanceFundInfo } from './classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';

@Component({
  selector: 'app-rq404',
  templateUrl: './rq404.component.html',
  styleUrls: ['./rq404.component.scss']
})

export class RQ404Component implements OnInit {
  AdvanceTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('advance-situation'),
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

  nzFilterOption = () => true;
  rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = this.AdvanceTableColumn;
  detailTableData: AdvanceFundInfo[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  invoiceFileList: any[] = [];
  attachmentList: any[] = [];
  fileList: any[] = [];
  isSaveLoading = false;
  employeeList: any[] = [];
  exTotalWarning: any[] = [];
  isFinUser: boolean = false;
  preUrl: string
  showTransformButton: boolean = false;

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    // private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    private commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    this.infoForm = this.fb.group({
      companyCode: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      expenseProject: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      projectCode: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      payeeEmplid: [{ value: null, disabled: true }],
      payeeName: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      actualAmt: [{ value: 0, disabled: true }]
    });
    this.getEmployeeInfo();
    this.getRnoInfo();
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    }
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) { this.message.error('Can not get user information. Please refresh the page...', { nzDuration: 6000 }); }
  }

  getRnoRole() {
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.CheckIsAccount, null).subscribe(res => {
      if (!!res && res.status === 200 && !!res.body) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 6000 });
        }
        else {
          this.isFinUser = res.body.data;
          this.showTransformButton = res.body.data == 1;
          if (this.isFinUser) { this.infoForm.controls.actualAmt.enable(); }
        }
      }
      else if (res?.status !== 200) {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  getRnoInfo() {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        let rno: string = this.crypto.decrypt(data.rno);
        this.infoForm.controls.rno.setValue(rno);
        this.rno = rno;
        this.getRnoRole();
        this.preUrl = data.preUrl;
        // 获取单据信息
        // this.Service.Post('http://localhost:5000' + URLConst.GetFormData, { rno: rno }).subscribe((res) => {
        this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFormData, { rno: rno }).subscribe(res => {
          if (res != null && res.status === 200 && !!res.body) {
            if (res.body.status == 1) { this.assembleFromData(res.body.data); }
            else { this.message.error(res.body.message, { nzDuration: 6000 }); }
          }
          else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
        });
      }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    this.attachmentList = value.file;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.infoForm.controls.companyCode.setValue(headData.company);
      this.infoForm.controls.expenseProject.setValue(headData.dtype);
      this.infoForm.controls.projectCode.setValue(headData.projectcode);
      this.infoForm.controls.applicantEmplid.setValue(headData.cuser);
      this.infoForm.controls.applicantName.setValue(headData.cname);
      this.infoForm.controls.payeeEmplid.setValue(headData.payeeId);
      this.infoForm.controls.payeeName.setValue(headData.payeename);
      this.infoForm.controls.dept.setValue(headData.deptid);
    }
    if (summaryAtmData != null) {
      this.infoForm.controls.totalAmt.setValue(summaryAtmData.amount);
      this.infoForm.controls.actualAmt.setValue(summaryAtmData.actamt);
    }

    if (listData != null) {
      listData.map(o => {
        // let fileCategory = null;
        // if (o.fileList.length > 0)
        //   fileCategory = o.fileList[0].category;
        this.detailTableData.push({
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
          fileList: o.fileList,
          disabled: false,
        });
      });
      this.detailTableData = this.detailTableData.sort((a, b) => a.id - b.id);
      this.detailTableData = [...this.detailTableData];
    }
    this.isSpinning = false;
  }

  checkInvoiceDetail(id: number): void {
    let rowData = this.detailTableData.filter(o => o.id == id);
    if (rowData.length > 0)
      this.fileList = rowData[0].fileList;
  }

  approve(data: ApprovalParams): void {
    this.isSpinning = true;
    this.commonSrv.Approval(data, this.preUrl);

    this.isSpinning = false;
  }
  reject(data: ApprovalParams): void {
    this.isSpinning = true;
    this.commonSrv.Reject(data, this.preUrl);
    this.isSpinning = false;
  }
  transform(data: ApprovalParams): void {
    this.isSpinning = true;
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }
}
