import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonService } from 'src/app/shared/service/common.service';
import { TranslateService } from '@ngx-translate/core';
import { UntypedFormBuilder, FormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Accountant } from 'src/app/ERS-Pages/rq204/classes/data-item';
import { TableColumnModel } from 'src/app/shared/models';
import { format } from 'date-fns';
import { URLConst } from 'src/app/shared/const/url.const';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { ApprovalParams, SignList, SignLogList } from './classes/data-item';

@Component({
  selector: 'sign-off',
  templateUrl: './sign-off.component.html',
  styleUrls: ['./sign-off.component.scss']
})
export class SignOffTemplateComponent implements OnInit {
  @Input() rno: string = '';
  @Input() showExModal: boolean = true;
  @Input() showSignPaperForm: boolean = true;
  @Input() exTotalWarning = [];
  @Input() isFinUser: boolean = false;
  @Input() actionButtonName: string = this.translate.instant('transfor-form');
  @Input() showTransformButton: boolean = false;
  @Input() showPrintButton: boolean = true;
  @Output() approve = new EventEmitter<ApprovalParams>();
  @Output() reject = new EventEmitter<ApprovalParams>();
  @Output() transform = new EventEmitter<ApprovalParams>();
  signForm: UntypedFormGroup;
  employeeList: any[] = [];
  checked = false;
  company: string;
  isSaveLoading: boolean = false;
  isDownloadLoading: boolean = false;
  totalExTipModal: boolean = false;
  showModal: boolean = false;
  screenHeight: any;
  screenWidth: any;
  signTableData: SignList[] = [];
  FinDetailTableData: Accountant[] = [];
  signLogTableData: SignLogList[] = [];
  SignLogTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('signform.step'),
      columnKey: 'step',
      columnWidth: '94px',
      align: 'left',
      sortFn: (a: SignLogList, b: SignLogList) =>
        a.step.localeCompare(b.step),
    },
    {
      title: this.translate.instant('signform.signUser'),
      columnKey: 'signUser',
      columnWidth: '153px',
      align: 'left',
      sortFn: (a: SignLogList, b: SignLogList) =>
        a.signUser.localeCompare(b.signUser),
    },
    {
      title: this.translate.instant('signform.signDate'),
      columnKey: 'signDate',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignLogList, b: SignLogList) =>
        a.signDate.localeCompare(b.signDate),
    },
    {
      title: this.translate.instant('signform.status'),
      columnKey: 'status',
      columnWidth: '30px',
      align: 'center',
      sortFn: (a: SignLogList, b: SignLogList) =>
        a.status.localeCompare(b.status),
    },
    {
      title: this.translate.instant('signform.remark'),
      columnKey: 'remark',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignLogList, b: SignLogList) =>
        a.remark.localeCompare(b.remark),
    },
  ];
  SignTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('signform.step'),
      columnKey: 'step',
      columnWidth: '94px',
      align: 'left',
      sortFn: (a: SignList, b: SignList) =>
        a.step.localeCompare(b.step),
    },
    {
      title: this.translate.instant('signform.signUser'),
      columnKey: 'signUser',
      columnWidth: '200px',
      align: 'left',
      sortFn: (a: SignList, b: SignList) =>
        a.signUser.localeCompare(b.signUser),
    },
    {
      title: this.translate.instant('signform.status'),
      columnKey: 'status',
      columnWidth: '',
      align: 'center',
      sortFn: (a: SignList, b: SignList) =>
        a.status.localeCompare(b.status),
    }
  ];
  FinDetailListTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('companyCode'),
      columnKey: 'companyCode',
      columnWidth: '',
      align: 'left',
      sortFn: (a: Accountant, b: Accountant) =>
        a.companyCode.localeCompare(b.companyCode),
    },
    {
      title: this.translate.instant('plant'),
      columnKey: 'plant',
      columnWidth: '',
      align: 'left',
      sortFn: (a: Accountant, b: Accountant) =>
        a.plant.localeCompare(b.plant),
    },
    {
      title: this.translate.instant('rv1'),
      columnKey: 'rv1',
      columnWidth: '',
      align: 'left',
      sortFn: (a: Accountant, b: Accountant) =>
        a.rv1.localeCompare(b.rv1),
    },
    {
      title: this.translate.instant('rv2'),
      columnKey: 'rv2',
      columnWidth: '',
      align: 'left',
      sortFn: (a: Accountant, b: Accountant) =>
        a.rv2.localeCompare(b.rv2),
    },
    {
      title: this.translate.instant('rv3'),
      columnKey: 'rv3',
      columnWidth: '',
      align: 'left',
      sortFn: (a: Accountant, b: Accountant) =>
        a.rv3.localeCompare(b.rv3),
    }
  ];
  signPara: ApprovalParams = {
    rno: "",
    inviteMethod: 0,
    inviteEmplid: "",
    comment: "",
    approverEmplid: ''
  };
  selectAccountant = new Set<string>();
  nzFilterOption = () => true;
  constructor(
    private fb: UntypedFormBuilder,
    private commonSrv: CommonService,
    private translate: TranslateService,
    private message: NzMessageService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private Service: WebApiService,
  ) { }

  ngOnInit(): void {
    this.signForm = this.fb.group({
      inviteColleague: [null],
      inviteType: [null],
      comment: [null],
      signPaperForm: [false]
    });

    this.GetSignLog(this.rno);
    // this.GetSignList(this.rno);
  }
  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    }
  };

  getEmployeeList(value) {
    this.commonSrv.getManagerList(value).subscribe(res => {
      this.employeeList = res;
      this.employeeList = [...this.employeeList];
    });
  }
  checkParam(): boolean {
    if (this.signForm.controls.inviteColleague.value != null && this.signForm.controls.inviteType.value == null) {
      this.message.error(this.translate.instant('choose-invitation-type'));
      this.isSaveLoading = false;
      return false;
    }
    return true;
  }

  GetSignLog(rno): void {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetHistoryFlow + `?rno=${rno}`, {}).subscribe(res => {
      if (res.body?.status == 1 && res.body?.data) {
        res.body.data.map(i => {
          this.signLogTableData.push({
            step: i.stepname,
            signUser: i.cemplid + '/' + i.approvername,
            signDate: format(new Date(i.cdate), 'yyyy/MM/dd HH:mm:ss'),
            status: i.status,
            remark: ''
          })
        });
        this.signLogTableData = [...this.signLogTableData];
      }
    });
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCurrentFlowList + `?rno=${rno}`, {}).subscribe(res => {
      if (res.body?.status == 1 && res.body?.data) {
        res.body.data.map(i => {
          this.signTableData.push({
            step: i.stepname,
            signUser: i.assignedEmplids ? i.assignedEmplids.join(',') : '',
            status: i.status == 'F' ? this.translate.instant('signform.status.finish') : this.translate.instant('signform.status.unfinish'),
            company: i.company
          });
        });
        this.signTableData = [...this.signTableData];
        if (this.signTableData.length > 0) {
          this.company = this.signTableData[0].company;
        }
      }
    });
  }


  handleReject(): void {
    if (this.signForm.controls.comment.value == null || this.signForm.controls.comment.value.trim() == "") {
      this.message.error(this.translate.instant('input-reject-reason'));
      this.isSaveLoading = false;
      return;
    }
    this.isSaveLoading = true;
    this.SetParams();
    this.reject.emit(this.signPara);
    this.isSaveLoading = false;
  }

  handleApprove(): void {
    if (!this.checkParam()) return;
    if (this.showExModal && this.exTotalWarning.length > 0) this.totalExTipModal = true;
    else {
      this.isSaveLoading = true;
      this.SetParams();
      this.approve.emit(this.signPara);
      this.isSaveLoading = false;
    }
  }

  handleTransform(): void {
    this.showModal = true;
    this.GetAccountant();
  }

  handlePrint(): void {
    this.Service.Post(URLConst.Printing, [this.rno]).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        var wind = window.open('');
        wind.document.body.innerHTML = res.body.data;
        wind.print();
      }
      else {
        this.message.error(res.message, { nzDuration: 6000 })
      }
    });
  }

  approveConfirm(): void {
    this.isSaveLoading = true;
    this.SetParams();
    this.approve.emit(this.signPara);
    this.isSaveLoading = false;
    this.totalExTipModal = false;
  }

  SetParams(): void {
    if (this.signForm.controls.comment.value != null) {
      this.signPara.comment = this.signForm.controls.comment.value;
    }
    if (this.signForm.controls.inviteType.value != null) {
      this.signPara.inviteMethod = this.signForm.controls.inviteType.value;
    }
    if (this.signForm.controls.inviteColleague.value != null) {
      this.signPara.inviteEmplid = this.signForm.controls.inviteColleague.value;
    }
    // if (this.signForm.controls.signPaperForm.value != null) {
    //   this.signPara.paperSign = this.signForm.controls.signPaperForm.value;
    // }
    this.signPara.rno = this.rno;
  }
  GetAccountant(): void {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetFinReviewApi + `?company=${this.company}`, null).subscribe((res) => {
      if (res && res.status === 200) {
        var data = res.body;
        if (data.status == 1) {
          data.data.map(i => {
            this.FinDetailTableData.push({
              Id: i.id,
              companyCode: i.company_code,
              plant: i.plant,
              rv1: i.rv1,
              rv2: i.rv2,
              rv3: i.rv3
            })
          });
          this.FinDetailTableData = [...this.FinDetailTableData];
        } else {
          this.message.error(data.message, { nzDuration: 6000 });
        }
      }
      else {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  handleCancel(): void {
    this.showModal = false;
  }
  handleOk(): void {
    if (this.selectAccountant.size != 1) {
      this.message.warning(this.translate.instant('only-select-one'));
      return;
    }
    this.isSaveLoading = true;
    this.SetParams();
    this.transform.emit(this.signPara);
    this.isSaveLoading = false;
  }

  onItemChecked(id: string, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }
  updateCheckedSet(id: string, checked: boolean): void {
    if (checked) {
      this.selectAccountant.clear();
      this.selectAccountant.add(id);
    } else {
      this.selectAccountant.delete(id);
    }
  }
  refreshCheckedStatus(): void {
    this.checked = this.FinDetailTableData.every(({ Id }) => this.selectAccountant.has(Id));
  }

  DecorateLine(item: string): string {
    return (this.exTotalWarning.indexOf(item) + 1) + '. ' + item;
  }

  download() {
    if (this.rno != null) {
      this.isDownloadLoading = true;
      this.Service.doPost(URLConst.DownloadInvoiceList + `?rno=${this.rno}`, null, true).subscribe((res) => {
        this.downloadFlow(res, `${this.translate.instant('Report')}-${format(new Date(), 'yyyyMMdd')}.xlsx`);
        this.isDownloadLoading = false;
      });
    }
  }

  downloadFlow(flow, name) {
    if (flow.size > 0) {
      const blob = new Blob([flow], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const objectUrl = URL.createObjectURL(blob);
      const a = document.createElement('a');
      document.body.appendChild(a);
      a.setAttribute('style', 'display:none');
      a.setAttribute('href', objectUrl);
      a.setAttribute('download', name);
      a.click();
      URL.revokeObjectURL(objectUrl);
    }
  }
}
