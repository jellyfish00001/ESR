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
import { ExpensesInfo } from './classes/data-item';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-rq204',
  templateUrl: './rq204.component.html',
  styleUrls: ['./rq204.component.scss']
})

export class RQ204Component implements OnInit {
  ExpensesTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('form.fee-voucher-date'),
      columnKey: 'feeDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) =>
        a.feeDate.localeCompare(b.feeDate),
    },
    {
      title: this.translate.instant('col.host-object'),
      columnKey: 'entertainObject',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.entertainObject.localeCompare(b.entertainObject),
    },
    {
      title: this.translate.instant('col.company-escort'),
      columnKey: 'accompany',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.accompany.localeCompare(b.accompany),
    },
    {
      title: this.translate.instant('col.remark'),
      columnKey: 'remark',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.remark.localeCompare(b.remark),
    },
    {
      title: this.translate.instant('col.expense-attribution-department'),
      columnKey: 'expenseAttribDept',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.expenseAttribDept.localeCompare(b.expenseAttribDept),
    },
    {
      title: this.translate.instant('col.currency'),
      columnKey: 'curr',
      columnWidth: '90px',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.curr.localeCompare(b.curr),
    },
    {
      title: this.translate.instant('col.applied-amount'),
      columnKey: 'appliedAmount',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => (a.appliedAmount - b.appliedAmount),
    },
    {
      title: this.translate.instant('col.conversion-to-local-currency'),
      columnKey: 'toLocalCurrAmount',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => (a.toLocalCurrAmount - b.toLocalCurrAmount),
    },
    {
      title: this.translate.instant('col.exchange-rate'),
      columnKey: 'exchangeRate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => (a.exchangeRate - b.exchangeRate),
    },
    {
      title: this.translate.instant('individual-responsibility-for-taxes'),
      columnKey: 'selfTaxAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.selfTaxAmt - b.selfTaxAmt,
    },
    {
      title: this.translate.instant('actual-reimbursable-amount'),
      columnKey: 'actualAmt',
      columnWidth: '',
      align: 'center',
      sortFn: (a: ExpensesInfo, b: ExpensesInfo) => a.actualAmt - b.actualAmt,
    },
  ];

  nzFilterOption = () => true;
  // rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = this.ExpensesTableColumn;
  detailTableData: ExpensesInfo[] = [];
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
  paymentWayList: any[] = [];

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
    public commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    this.infoForm = this.fb.group({
      emplid: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
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
      paymentWay: [{ value: null, disabled: true }],
    });
    this.getEmployeeInfo();
    this.getRnoInfo();
    this.getPaymentList();
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
    console.log('value',value);
    let headData = value.head;
    let listData = value.detail;
    this.attachmentList = value.file;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.infoForm.controls.companyCode.setValue(headData.company);
      this.infoForm.controls.expenseProject.setValue(headData.dtype);
      this.infoForm.controls.projectCode.setValue(headData.projectcode);
      this.infoForm.controls.applicantEmplid.setValue(headData.payeeId);
      this.infoForm.controls.applicantName.setValue(headData.cname);
      this.infoForm.controls.dept.setValue(headData.deptid);
      this.infoForm.controls.emplid.setValue(headData.cuser);
      this.infoForm.controls.paymentWay.setValue(headData.paymentway);
    }
    if (summaryAtmData != null) {
      this.infoForm.controls.totalAmt.setValue(summaryAtmData.amount);
      this.infoForm.controls.actualExpenseAmt.setValue(summaryAtmData.actamt);
      this.infoForm.controls.actualPayment.setValue(summaryAtmData.actamt);
    }
    this.attachmentList.map(i => i.url = this.commonSrv.changeDomain(i.url))
    if (listData != null) {
      listData.map(o => {
        let individualAffordAmt = 0;
        let companyAffordAmt = 0;
        let selfTaxAmt = 0;
        //o.fileList.map(i => i.url = this.commonSrv.changeDomain(i.url))
        o.fileList.map(i => {
          i.url = this.commonSrv.changeDomain(i.url); // 将 i.url 修改为新的域名
          i.invno = i.invoiceno; // 添加 invno 属性
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
          entertainObject: o.object,
          accompany: o.keep,
          remark: o.remarks,
          expenseAttribDept: o.deptid,
          curr: o.currency,
          appliedAmount: o.amount1,
          toLocalCurrAmount: o.baseamt,
          exchangeRate: o.rate,
          selfAffordAmt: individualAffordAmt,
          companyAffordAmt: companyAffordAmt,
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
