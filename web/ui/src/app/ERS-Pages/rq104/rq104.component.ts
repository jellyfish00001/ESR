import { Component, OnInit, ViewChild } from '@angular/core';
import {
  UntypedFormBuilder,
  FormControl,
  UntypedFormGroup,
  Validators,
  FormGroup,
  FormArray,
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
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { firstValueFrom, Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import {
  ExpenseTableColumn,
  OvertimeMealExpenseTableColumn,
  DriveFuelExpenseTableColumn,
} from './classes/table-column';
import { GeneralExpenseInfo } from './classes/data-item';
import { AccountingDetailTableColumn } from './classes/table-column';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import _ from 'lodash';

@Component({
  selector: 'app-rq104',
  templateUrl: './rq104.component.html',
  styleUrls: ['./rq104.component.scss'],
})
export class RQ104Component implements OnInit {
  nzFilterOption = () => true;
  rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = ExpenseTableColumn;
  detailTableData: GeneralExpenseInfo[] = [];
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
  type: string = null;
  preUrl: string;
  showTransformButton: boolean = false;
  currency: string = 'RMB';
  accountingDetailsList: any[] = [];
  orginalAccountingDetailsList: any[] = [];
  accountingDetailsColumns = AccountingDetailTableColumn;
  accountingDetailsForm!: FormGroup;
  editId: string | null = null;
  editRDId: string | null = null;
  attachList: NzUploadFile[] = [];
  editloading = false;
  showModal = false;
  editForm: UntypedFormGroup = null;
  showEditAccountingDetails: boolean = false;
  accountingDetailFG!: FormGroup;
  get accountingDetailFAN() {
    return this.accountingDetailFG.get('accountingDetailFAN') as FormArray;
  }
  modifiedAccountingDetaliList: any[] = [];
  showEditReimbursementDetail: boolean = false;
  modifiedReimbursementDetaliList: any[] = [];
  ADCurrency: string = '';
  ADAccountNumber: string = '';
  ADCostCenter: string = '';
  ADOrder: string = '';
  ADLineText: string = '';
  currList: any[] = [];

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
    private commonSrv: CommonService
  ) {}

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
      totalAmt: [{ value: 0, disabled: true }],
      actualAmt: [{ value: 0, disabled: true }],
      requestAmount:[{ value: 0, disabled: true }],
      deductionAmount:[{ value: 0, disabled: true }],
      actualReimbursementAmount:[{ value: 0, disabled: true }],
    });
    this.getEmployeeInfo();
    this.getRnoInfo();
    this.getCurrency();
    console.log('this.accountingDetailsList', this.accountingDetailsList);
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    },
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
  }

  getRnoRole() {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.CheckIsAccount,
      null
    ).subscribe((res) => {
      if (!!res && res.status === 200 && !!res.body) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 6000 });
        } else {
          this.isFinUser = res.body.data ;
          this.showTransformButton = res.body.data == 1;
          if (this.isFinUser) {
            this.infoForm.controls.actualAmt.enable();
          }
        }
      } else if (res?.status !== 200) {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  async getRnoInfo() {
    this.actRoute.queryParams.subscribe(async (res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        let rno: string = this.crypto.decrypt(data.rno);
        this.infoForm.controls.rno.setValue(rno);
        this.rno = rno;
        this.getRnoRole();
        this.preUrl = data.preUrl;
        // 获取单据信息
        // this.Service.Post('http://localhost:5000' + URLConst.GetFormData, { rno: rno }).subscribe((res) => {
        this.Service.Post(
          this.EnvironmentconfigService.authConfig.ersUrl +
            URLConst.GetFormData,
          { rno: rno }
        ).subscribe(async (res2) => {
          if (res2 != null && res2.status === 200 && !!res2.body) {
            if (res2.body.status == 1) {
              this.assembleFromData(res2.body.data);
              this.getSceneList();
            } else {
              this.message.error(res2.body.message, { nzDuration: 6000 });
            }
          } else {
            this.message.error(
              res2.message ?? this.translate.instant('server-error'),
              { nzDuration: 6000 }
            );
          }
        });

        //获取入账明细
        this.getCashCarryDetailData();
      }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    console.log('value', value);
    this.attachmentList = value.file;
    let summaryAtmData = value.amount;

    if (headData != null) {
      this.infoForm.controls.companyCode.setValue(headData.company);
      this.infoForm.controls.expenseProject.setValue(headData.dtype);
      this.infoForm.controls.projectCode.setValue(headData.projectcode);
      this.infoForm.controls.applicantEmplid.setValue(headData.payeeId);
      this.infoForm.controls.applicantName.setValue(headData.payeename);
      this.infoForm.controls.dept.setValue(headData.deptid);
      this.infoForm.controls.requestAmount.setValue(headData.requestAmount);
      this.infoForm.controls.deductionAmount.setValue(headData.deductionAmount);
      this.infoForm.controls.actualReimbursementAmount.setValue(headData.actualamount);
    }
    if (summaryAtmData != null) {
      this.infoForm.controls.totalAmt.setValue(summaryAtmData.amount);
      this.infoForm.controls.actualAmt.setValue(summaryAtmData.actamt);
      this.currency = summaryAtmData.currency;
    }

    this.attachmentList.map(
      (i) => (i.url = this.commonSrv.changeDomain(i.url))
    );

    if (listData != null) {
      let exInfoList: any = [];
      listData.map((o) => {
        let fileList = o.fileList
          .filter((f) => f.ishead == 'N' && f.status != 'F')
          .sort((a, b) => b.item - a.item)
          .map((f) => {
            return {
              id: f.seq,
              uid: f.item.toString(),
              name: f.filename,
              filename: f.tofn,
              status: 'done',
              url: this.commonSrv.changeDomain(f.url),
              type: f.filetype,
              originFileObj: null,
              category: f.category,
              invno: f.invoiceno,
              cdate: f.cdate
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
              cdate: f.cdate
            };
          });
        let selfTaxAmt = 0;
        o.invList.map((i) => {
          if (i.expcode != null && i.expcode != '') {
            this.exTotalWarning.push(i.expcode);
          }
          if (i.undertaker == 'self') {
            selfTaxAmt += i.taxloss;
          }
        });
        o.deptList?.map((i) => {
          i.amount = Number((o.amount1 * i.percent * 0.01).toFixed(2));
          i.baseamount = Number((o.baseamt * i.percent * 0.01).toFixed(2));
        });
        this.detailTableData.push({
          advanceRno: o.advancerno,
          scene: o.expcode,
          sceneName: o.expname,
          attribDept: o.deptList?.map((i) => i.deptId).join(','),
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
          invoiceDetailList: o.invList,
          fileCategory: attachList.length > 0 ? attachList[0].category : null,
          id: o.seq,
          disabled: false,
          selfTaxAmt: selfTaxAmt,
          actualAmt: o.amount,
          accountingRemarks: o.remarks,
          taxexpense: o.taxexpense,
          invoiceNo: o.invoice,
          companyCode: o.companycode,
          cashDetailId:o.id
        });
      });
      // this.detailTableData.map(o => {
      //   if (o.fileCategory == null) {
      //     this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFileRequest, { expcode: o.scene }).subscribe((res) => {
      //       if (res && res.status === 200 && res.body.length > 0) {
      //         let result = res.body[0];
      //         o.fileCategory = result.filecategory;
      //       }
      //     });
      //   }
      // });
      this.detailTableData = this.detailTableData?.sort((a, b) => a.id - b.id);
      this.detailTableData = [...this.detailTableData];
    }
    this.isSpinning = false;
  }

  async getSceneList() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +URLConst.GetSenarioByExpenseCode,//URLConst.GetSceneList,
      { Companycategory: this.infoForm.controls.companyCode.value, Keyword:'' }
    ).subscribe( async (res)  => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          var sceneList = res.body.data;
          console.log('sceneList',sceneList)
          console.log('this.detailTableData',this.detailTableData)
          if (this.detailTableData.length > 0) {
            let sceneData = sceneList.filter(
              (o) => o.expensecode == this.detailTableData[0].scene
            )[0];
            let res = await this.getSenarioById(sceneData?.id);
            let sceneExtra = res?.body.data;
            console.log('sceneExtra', sceneExtra);
            if (sceneExtra?.category == 0) {
              this.type = 'default';
              this.detailListTableColumn = ExpenseTableColumn;
            } else if (sceneExtra?.category == 1) {
              this.type = 'overtimeMeal';
              this.detailListTableColumn = OvertimeMealExpenseTableColumn;
            } else {
              this.type = 'drive';
              this.getCarType();
              this.detailListTableColumn = DriveFuelExpenseTableColumn;
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
    });
  }

  getCarType() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCarTypeList,
      { company: this.infoForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          var carTypeList = res.body.data;
          if (this.detailTableData.length > 0) {
            this.detailTableData
              .filter((o) => o.carTypeName == null)
              .map((o) => {
                o.carTypeName = carTypeList.filter(
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

  checkInvoiceDetail(id: number): void {
    console.log('this.detailTableData',this.detailTableData)
    let rowData = this.detailTableData.filter((o) => o.id == id);
    if (rowData.length > 0) this.fileList = rowData[0].invoiceDetailList;
    console.log('filelist',this.fileList)
  }

  checkAttachDetail(id: number): void {
    if (this.detailTableData.filter((o) => o.id == id).length > 0) {
      this.fileList = this.detailTableData.filter(
        (o) => o.id == id
      )[0].invoiceDetailList;
      this.attachList =  this.detailTableData.filter(
        (o) => o.id == id
      )[0].fileList;
      console.log('============filelist', this.detailTableData);
      console.log('============filelist2', this.attachList);
    }
  }

  approve(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.accountingRemarks,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Approval(data, this.preUrl);
    this.isSpinning = false;
  }
  reject(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.accountingRemarks,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Reject(data, this.preUrl);
    this.isSpinning = false;
  }
  transform(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.accountingRemarks,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }

  startEdit(id: string): void {
    this.editId = id;
    if(this.modifiedAccountingDetaliList.indexOf(id) === -1) {
      this.modifiedAccountingDetaliList.push(id);
    }
  }

  stopEdit(): void {
    this.editId = null;
  }

  async getSenarioById(id: string): Promise<any> {
    return firstValueFrom(
      this.Service.doGet(
        this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSenarioById + id,
        null
      )
    );
  }

  editRow(data: any): void {
    console.log('editRow', data);
    this.editForm = this.fb.group({
      sceneName: [data.sceneName], // expname
    feeDate: [data.feeDate], // date-of-expense
    curr: [data.curr], // col.currency
    attribDept: [data.attribDept], // col.expense-attribution-department
    percent: [data.attribDeptList?.[0].percent], // percent
    expenseAmt: [0 , [Validators.required]], // reimbursement-amount
    toLocalAmt: [data.toLocalAmt], // col.conversion-to-local-currency
    exchangeRate: [data.exchangeRate], // col.exchange-rate
    digest: [data.digest], // digest
    selfTaxAmt: [data.selfTaxAmt], // individual-responsibility-for-taxes
    actualAmt: [data.actualAmt], // actual-reimbursable-amount
    advanceRno: [data.advanceRno], // advance-fund-no
    invoiceNo: [data.invoiceDetailList.length === 1 ? (data.invoiceDetailList[0].invno || data.invoiceDetailList[0].invoiceNo) : '-' ], // invoice-no
    amount: [data.invoiceDetailList.length === 1 ? (data.invoiceDetailList[0].amount || data.invoiceDetailList[0].oamount - data.invoiceDetailList[0].taxLoss) : '-'], // price-excluding-tax
    taxamount: [data.invoiceDetailList.length === 1 ? (data.invoiceDetailList[0].taxamount || data.invoiceDetailList[0].taxLoss) : '-'], // tax-amount
    oamount: [data.invoiceDetailList.length === 1 ? data.invoiceDetailList[0].oamount : '-'], // total-amount-including-tax
    paymentNo: [data.invoiceDetailList.length === 1 ? data.invoiceDetailList[0].paymentNo : '-'], // seller-tax-number
    taxexpense: [data.taxexpense], // col-tax-deductible
    accountingRemarks: [data.accountingRemarks, [Validators.required]], // accountingRemarks
    id: [data.cashDetailId], // id
    });
    this.editForm.controls.expenseAmt.setValue(data.expenseAmt);
    this.editForm.disable();
    this.editForm.controls.accountingRemarks.enable();
    this.editForm.controls.expenseAmt.enable();
    this.editloading = true;
    this.showModal = true;
  }

  handleCancel(){
    this.showModal = false;
    this.editloading = false;
  }

  handleOk(){
    console.log('handleOk1=', this.editForm.controls.expenseAmt.value);
    console.log('handleOk2=', this.editForm.controls.accountingRemarks.value);
    if (this.editForm.controls.expenseAmt.value === null  || !this.editForm.controls.accountingRemarks.value) {
      this.message.error(this.translate.instant('fill-in-form'));
      return;
    }
    let param = {
      id: this.editForm.controls.id.value,
      remarks: this.editForm.controls.accountingRemarks.value,
      // amount1: this.editForm.controls.expenseAmt.value,
    };
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.UpdateCashDetailRemark,
      param
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.showModal = false;
          //更新成功后刷新资料

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

  DownloadFlow(flow:any[]) {
    let title: string[] = [
      'SequenceNo','DocumentDate','PostingDate','Company','Currency','ExchangeRate','Reference','DocumentHeaderText','DocumentType','PostingKey','AccountNumber','SpecialGLIndicator','AmountInDocumentCurrency','AmountInLocalCurrency','PaymentTerm','PaymentMethod','BaseLineDate','TaxCode','TaxBaseAmount','LCTaxBaseAmount','WithholdingTaxType','WithholdingTaxCode','WithholdingTaxBaseAmount','Withholdingtaxamount','CostCenter','Order','LineText','AssignmentNumber','ProfitCenter','PartnerProfitCenter','CustomerCode','Plant', 'BusinessType', 'EndCustomer', 'MaterialDivision', 'SalesDivision', 'Reference1', 'Reference2', 'Reference3', 'UnifyCode', 'Certificate'
    ];
    const workbook = new ExcelJS.Workbook();
    const sheet = workbook.addWorksheet('Sheet1');
    sheet.addRow(title);

    if (flow.length > 0) {
      flow.forEach(item => {
        sheet.addRow([item.seq, item.docdate, item.postdate, item.companysap, item.basecurr, item.rate, item.ref, item.dochead, item.doctyp, item.postkey, item.acctant, item.specgl, item.actamt1, item.actamt2, item.payterm, item.paytyp, item.baslindate, item.taxcode, item.taxamt1, item.taxamt2, item.wtaxtyp, item.wtaxcode, item.wtaxamt1, item.wtaxamt2, item.costcenter, item.order, item.linetext, item.asinmnt, item.proficenter1, item.proficenter2, item.custercode, item.plant, item.busityp, item.ecuster, item.mtrldiv, item.salsdiv, item.ref1, item.ref2, item.ref3, item.unifycode,item.certificate]);
      });
    }

    const filename = `${new Date().toISOString().replace(/[:.-]/g, '').split('T')[0]}_${Date.now() % 10000}`;
    workbook.xlsx.writeBuffer().then(buffer => {
      const blob = new Blob([buffer], {type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
      saveAs(blob, 'AccountingDetailInfo.xlsx');
    }).catch(err => console.error('Error writing excel export', err));

  }

  editAccountingDetails() {
    this.showEditAccountingDetails = true;
    this.modifiedAccountingDetaliList = [];
    this.accountingDetailsList = _.cloneDeep(this.orginalAccountingDetailsList);
  }

  handleADCancel(){
    this.showEditAccountingDetails = false;
  }
  handleADOk(){
    console.log('modifiedAccountingDetaliList', this.modifiedAccountingDetaliList);
    console.log('accountingDetailsList', this.accountingDetailsList);
    let params: any[] = [];
    this.accountingDetailsList.forEach((item) => {
      if(this.modifiedAccountingDetaliList.indexOf(item.id) !== -1) {
        let param = {
          Id: item.id,
          basecurr: item.basecurr,
          acctant: item.acctant,
          costcenter: item.costcenter,
          order: item.order,
          linetext: item.linetext,
        };
        params.push(param);
      }
    })
    console.log('params', JSON.stringify(params));
    const formData = new FormData();
    formData.append('data', JSON.stringify(params));
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.UpdateCashCarrydetailData,
        formData
    ).subscribe((res) => {
      console.log('res', res);
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          //更新成功后刷新资料
          this.showEditAccountingDetails = false;
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


  createChildForm(child?: any) {
    return this.fb.group({
      sequenceNo: [child?.sequenceNo || null],
      documentDate: [child?.documentDate || null],
      postingDate: [child?.postingDate || null],
      company: [child?.company || null],
      currency: [child?.currency || null],
      exchangeRate: [child?.exchangeRate || null],
      reference: [child?.reference || null],
      documentHeaderText: [child?.documentHeaderText || null],
      documentType: [child?.documentType || null],
      postingKey: [child?.postingKey || null],
      accountNumber: [child?.accountNumber || null],
      specialGLIndicator: [child?.specialGLIndicator || null],
      amountInDocumentCurrency: [child?.amountInDocumentCurrency || null],
      amountInLocalCurrency: [child?.amountInLocalCurrency || null],
      paymentTerm: [child?.paymentTerm || null],
      paymentMethod: [child?.paymentMethod || null],
      baseLineDate: [child?.baseLineDate || null],
      taxCode: [child?.taxCode || null],
      taxBaseAmount: [child?.taxBaseAmount || null],
      LCTaxBaseAmount: [child?.LCTaxBaseAmount || null],
      withholdingTaxType: [child?.withholdingTaxType || null],
      withholdingTaxCode: [child?.withholdingTaxCode || null],
      withholdingTaxBaseAmount: [child?.withholdingTaxBaseAmount || null],
      withholdingtaxamount: [child?.withholdingtaxamount || null],
      costCenter: [child?.costCenter || null],
      order: [child?.order || null],
      lineText: [child?.lineText || null], // Line Text
      assignmentNumber: [child?.assignmentNumber || null], // Assignment number
      profitCenter: [child?.profitCenter || null], // Profit Center
      partnerProfitCenter: [child?.partnerProfitCenter || null], // Partner Profit Center
      customerCode: [child?.customerCode || null], // Customer Code (ie. Bill-to Party)
      plant: [child?.plant || null], // Plant
      businessType: [child?.businessType || null], // Business Type
      endCustomer: [child?.endCustomer || null], // End customer
      materialDivision: [child?.materialDivision || null], // Material Division
      salesDivision: [child?.salesDivision || null], // Sales Division
      reference1: [child?.reference1 || null], // Reference 1
      reference2: [child?.reference2 || null], // Reference 2
      reference3: [child?.reference3 || null], // Reference 3
      unifyCode: [child?.unifyCode || null], // UNIFYCODE
      certificate: [child?.certificate || null], // CERTIFICATE
    })
  }

  updateColumn(columnKey: string, value: string): void {
    // 遍历 accountingDetailsList，更新对应列的值
    this.accountingDetailsList.forEach((row) => {
      row[columnKey] = value;
    });
    //更新整列之后，需要把所有资料的ID记录起来，更新时全部都要往后传进行更新
    this.modifiedAccountingDetaliList = this.accountingDetailsList.map(item => item.id);
  }

  handleRDCancel(){
    this.showEditReimbursementDetail = false;
  }
  handleRDOk(){
    console.log('modifiedReimbursementDetaliList', this.modifiedReimbursementDetaliList);
    console.log('detailTableData', this.detailTableData);
    let pass = true;
    this.detailTableData.forEach((item) => {
      if(item.expenseAmt === null || item.expenseAmt === undefined) {
        pass = false;
        return;
      }
    });
    if(!pass) {
      this.message.error(this.translate.instant('fill-in-form'));
      return;
    }

    let params: any[] = [];
    this.detailTableData.forEach((item) => {
      let param = {
        Id: item.cashDetailId,
        //remarks: this.editForm.controls.accountingRemarks.value,
        amount1: item.expenseAmt,
      };
      params.push(param);
    })
    console.log('params', JSON.stringify(params));
    const formData = new FormData();
    formData.append('data', JSON.stringify(params));
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.UpdateCashDetailData,
        formData
    ).subscribe((res) => {
      console.log('res', res);
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          //更新成功后刷新资料
          this.showEditReimbursementDetail = false;
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

  startEditRD(id: string): void {
    this.editRDId = id;
    if(this.modifiedReimbursementDetaliList.indexOf(id) === -1) {
      this.modifiedReimbursementDetaliList.push(id);
    }
  }

  stopEditRD(): void {
    this.editRDId = null;
  }

  editReimbursementDetails() {
    this.showEditReimbursementDetail = true;
  }

  modifiedInvList(value: any){
    // 将每次更新的invoice内容更新回detailTableData里的invoiceDetailList
    this.fileList = value;
    console.log('fileList', this.fileList);
    console.log('detailTableData', this.detailTableData);
    //api 更新invoiceDetailList
    let params: any[] = [];
    this.fileList.forEach((item) => {
      let param = {
        id: item.id,
        amount: item.amount,
        oamount: item.oamount,
        sellerTaxId: item.paymentNo,
        taxamount: item.taxamount,
        invno: item.invno,
        invtype: item.invtype,
      };
      params.push(param);
    })
    console.log('params', JSON.stringify(params));
    const formData = new FormData();
    formData.append('data', JSON.stringify(params));
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.UpdateInvoiceData,
        formData
    ).subscribe((res) => {
      console.log('res', res);
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          //更新成功后刷新资料
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

  batchUpdate(){
    // 遍历 accountingDetailsList，更新对应列的值
    this.accountingDetailsList.forEach((row) => {
      if((this.ADCurrency || '').trim()) row.currency = this.ADCurrency;
      if((this.ADAccountNumber || '').trim()) row.accountNumber = this.ADAccountNumber;
      if((this.ADCostCenter || '').trim()) row.costCenter = this.ADCostCenter;
      if((this.ADOrder || '').trim()) row.order = this.ADOrder;
      if((this.ADLineText || '').trim()) row.lineText = this.ADLineText;
    });
    //更新整列之后，需要把所有资料的ID记录起来，更新时全部都要往后传进行更新
    this.modifiedAccountingDetaliList = this.accountingDetailsList.map(item => item.id);
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

  getCashCarryDetailData() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetCashCarryDetailData,
      {rno: this.rno}
    ).subscribe((res) => {
      if (res && res.status === 200) {
        console.log('res', res.body.data);
        this.accountingDetailsList = res.body.data;
        this.orginalAccountingDetailsList = _.cloneDeep(res.body.data);
        this.accountingDetailFG = this.fb.group({
          accountingDetailFAN: this.fb.array([]),
        });
        _.each(this.accountingDetailsList, (item) => {
          const formGroup = this.fb.group({
            //accountingDetailFAN: this.fb.array([]),
            item
          });
          // set form group to form array
          this.accountingDetailFAN.push(formGroup);
          //this.createForm(item, formGroup);

        });

      } else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

}
