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
import { ReturnTaiwanInfo } from '../rq601/classes/data-item';
import { ReturnTaiwanTableColumn } from '../rq601/classes/table-column';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-rq604',
  templateUrl: './rq604.component.html',
  styleUrls: ['./rq604.component.scss']
})

export class RQ604Component implements OnInit {
  nzFilterOption = () => true;
  // rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = ReturnTaiwanTableColumn;
  detailTableData: ReturnTaiwanInfo[] = [];
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
  rno: string;
  preUrl: string;
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
      expenseProject: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      projectCode: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      actualExpenseAmt: [{ value: 0, disabled: true }],
      actualPayment: [0],
      inviteColleague: [null],
      inviteType: [null],
      comment: [null],
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
      this.infoForm.controls.applicantEmplid.setValue(headData.payeeId);
      this.infoForm.controls.applicantName.setValue(headData.payeename);
    }
    if (summaryAtmData != null) {
      this.infoForm.controls.totalAmt.setValue(summaryAtmData.amount);
      this.infoForm.controls.actualExpenseAmt.setValue(summaryAtmData.actamt);
      this.infoForm.controls.actualPayment.setValue(summaryAtmData.actamt);
    }
    this.attachmentList.map(i => i.url = this.commonSrv.changeDomain(i.url));
    if (listData != null) {
      listData.map(o => {
        let individualAffordAmt = 0;
        let companyAffordAmt = 0;
        let selfTaxAmt = 0;
        o.fileList.map(i => {
          i.url = this.commonSrv.changeDomain(i.url);
          i.invno = i.invoiceno;
          return i;
        })
        o.invList.map(i => {
          if (i.undertaker == 'self') {
            individualAffordAmt += Number(i.amount);
            selfTaxAmt += Number(i.taxloss);
          }
          else if (i.undertaker == 'company') {
            companyAffordAmt += Number(i.amount);
          }
          if (i.expcode != null && i.expcode != '') {
            this.exTotalWarning.push(i.expcode);
          }
        });
        this.detailTableData.push({
          id: o.seq,
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          curr: o.currency,
          appliedAmount: o.amount1,
          toLocalCurrAmount: o.baseamt,
          exchangeRate: o.rate,
          expenseAttribDept: o.deptid,
          digest: o.summary,
          feeType: o.expcode,
          feeTypeName: o.expname,
          remark: o.remarks,
          disabled: false,
          whiteRemark: o.assignment,
          selfTaxAmt: selfTaxAmt,
          actualAmt: o.amount
        });
        this.invoiceFileList = this.invoiceFileList.concat(o.fileList).sort((a, b) => a.item - b.item);
      });
      this.detailTableData = this.detailTableData.sort((a, b) => a.id - b.id);
      this.detailTableData = [...this.detailTableData];
    }
    this.isSpinning = false;
  }

  checkInvoiceDetail(id: number): void {
    this.fileList = this.invoiceFileList.filter(o => o.seq == id);
  }

  approve(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map(o => {
        return { seq: o.id, assignment: o.whiteRemark }
      });
    }
    this.commonSrv.Approval(data, this.preUrl);
    this.isSpinning = false;
  }
  reject(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map(o => {
        return { seq: o.id, assignment: o.whiteRemark }
      });
    }
    this.commonSrv.Reject(data, this.preUrl);
    this.isSpinning = false;
  }
  transform(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map(o => {
        return { seq: o.id, assignment: o.whiteRemark }
      });
    }
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }
}
