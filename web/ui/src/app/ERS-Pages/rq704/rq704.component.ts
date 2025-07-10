import { Component, OnInit, ViewChild } from '@angular/core';
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
import { SalaryInfo } from '../rq704/classes/data-item';
import { SalaryTableColumn } from '../rq704/classes/table-column';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-rq704',
  templateUrl: './rq704.component.html',
  styleUrls: ['./rq704.component.scss'],
})
export class RQ704Component implements OnInit {
  nzFilterOption = () => true;
  // rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = SalaryTableColumn;
  detailTableData: SalaryInfo[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  invoiceFileList: any[] = [];
  attachmentList: any[] = [];
  fileList: any[] = [];
  isSaveLoading = false;
  employeeList: any[] = [];
  isFinUser: boolean = false;
  isApplicant: boolean = true;
  rno: string;
  preUrl: string;
  showTransformButton: boolean = false;
  previewImage: string | undefined = '';
  previewVisible = false;
  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };
  frameSrc: SafeResourceUrl;
  drawerVisible: boolean = false;
  inputDown: boolean = true;
  currency: string = 'RMB';

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
      expenseProject: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      projectCode: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      // actualExpenseAmt: [{ value: 0, disabled: true }],
      actualPayment: [{ value: 0, disabled: true }],
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
    },
  };

  formatter = (value: number) => {
    if (value != undefined && value != null) {
      return value.toLocaleString();
    }
  };

  focusOnInputNumber(id) {
    if (!!id) {
      let rowData = this.detailTableData.filter((o) => o.id == id)[0];
      if (!rowData) {
        return;
      }
      if (rowData.actualAmount == 0) {
        rowData.actualAmount = '';
        this.detailTableData = [...this.detailTableData];
      }
      this.inputDown = false;
    }
  }

  checkActualAmount(id) {
    if (!!id) {
      let rowData = this.detailTableData.filter((o) => o.id == id)[0];
      if (!rowData) {
        return;
      }
      if (!rowData?.actualAmount) {
        rowData.actualAmount = 0;
        this.detailTableData = [...this.detailTableData];
      }
      if (rowData.actualAmount > rowData.appliedAmount) {
        this.message.error(
          this.translate.instant('can-not-exceed-applied-amount')
        );
        rowData.actualAmount = rowData.appliedAmount;
        this.detailTableData = [...this.detailTableData];
        return;
      }
      let actualTotal = 0;
      this.detailTableData.map((o) => {
        actualTotal += o.actualAmount;
      });
      actualTotal = Number(actualTotal.toFixed(2));
      this.infoForm.controls.actualPayment.reset(actualTotal);
      setTimeout(() => {
        this.inputDown = true;
      }, 500); // 防止修改完数额后未进行blur，便直接点击同意导致来不及显示检查提示便送单了
    }
  }

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
  }

  //call isApplicant api 赋值
  checkIsAppliacant() {
    this.Service.doGet(
      URLConst.CheckIsApplicant + `?rno=${this.rno}`,
      null
    ).subscribe((res) => {
      if (!!res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          this.isApplicant = res.body.data == 1;
          // if (!this.isApplicant) { this.infoForm.controls.actualPayment.disable(); }
        } else {
          this.message.error(res.body.message, { nzDuration: 6000 });
        }
      } else if (res?.status !== 200) {
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
        this.checkIsAppliacant();
        this.preUrl = data.preUrl;
        // 获取单据信息
        // this.Service.Post('http://localhost:5000' + URLConst.GetFormData, { rno: rno }).subscribe((res) => {
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
    let attachmentList = value.file;
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
      // this.infoForm.controls.actualExpenseAmt.setValue(summaryAtmData.actamt);
      this.infoForm.controls.actualPayment.setValue(summaryAtmData.actamt);
      this.currency = summaryAtmData.currency;
    }
    if (listData != null) {
      listData.map((o) => {
        this.detailTableData.push({
          id: o.seq,
          sceneCode: o.expcode,
          sceneName: o.expname,
          company: o.companycode,
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          salaryMonth: o.summary,
          curr: o.currency,
          appliedAmount: o.amount1,
          actualAmount: !o.amount ? o.amount1 : o.amount,
          bank: o.bank,
          digest: o.remarks,
          requestPayment: o.paytyp,
          requestPaymentName: o.payname,
          disabled: false,
        });
      });
      // 组装文件
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
      this.detailTableData = this.detailTableData.sort((a, b) => a.id - b.id);
      this.detailTableData = [...this.detailTableData];
      this.attachmentList = [...this.attachmentList];
    }
    this.isSpinning = false;
    if (this.isApplicant) {
      this.attachmentList.map(async (o) => {
        o.originFileObj = await this.commonSrv.getFileData(
          o.url,
          o.filename,
          o.type
        );
      });
    }
  }

  //上传
  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
    // 上传bpm & 批量文件
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

  handleChange(info: NzUploadChangeParam): void {
    let fileList = [...info.fileList];
    fileList = fileList.map((file) => {
      file.status = 'done';
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.attachmentList = fileList;
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

  checkParam(): boolean {
    if (!this.inputDown) {
      return false;
    }
    if (this.attachmentList.length == 0) {
      this.message.error(this.translate.instant('required-for-attachment'), {
        nzDuration: 6000,
      });
      return false;
    }
    return true;
  }

  approve(data: any): void {
    this.isSpinning = true;
    if (!this.checkParam()) {
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
    }
    const formData = new FormData();
    if (this.isApplicant) {
      let fileInfo = this.attachmentList.map((o) => {
        let indx = 0;
        return {
          rno: this.rno,
          seq: 0,
          item: indx++,
          filetype: o.type,
          filename: o.name,
          ishead: 'Y',
          key: o.uid,
        };
      });
      data['amount'] = {
        actamt: this.infoForm.controls.actualPayment.value,
      };
      data['detail'] = this.detailTableData.map((o) => {
        return { seq: o.id, amount: o.actualAmount };
      });
      formData.append('file', JSON.stringify(fileInfo));
      this.attachmentList.forEach((file: any) => {
        formData.append(file.uid, file.originFileObj);
      });
    }
    formData.append('sign', JSON.stringify(data));
    this.commonSrv.Approval(data, this.preUrl, formData);
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
