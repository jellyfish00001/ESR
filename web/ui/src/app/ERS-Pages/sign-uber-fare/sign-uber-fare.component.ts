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
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { debounceTime, Observable, Observer, Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import { CashUberDetail } from './classes/data-item';
import { DetailTableColumn } from './classes/table-column';

@Component({
  selector: 'app-sign-uber-fare',
  templateUrl: './sign-uber-fare.component.html',
  styleUrls: ['./sign-uber-fare.component.scss'],
})
export class SignUberFareComponent implements OnInit {
  nzFilterOption = () => true;
  rno: string = "";
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = DetailTableColumn;
  detailTableData: CashUberDetail[] = [];
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
  projectCodeList: any[] = [];
  getPJCodeTerms = new Subject<string>();
  isApplicant = false;
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
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    this.infoForm = this.fb.group({
      companyCode: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      businessTripNo: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      projectCode: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      actualAmt: [{ value: 0, disabled: true }],
    });
    this.getPJCodeTerms.pipe(debounceTime(1000)).subscribe((term) => {
      this.getProjectCode(term);
    });
    this.getEmployeeInfo();
    this.getRnoInfo();
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    },
  };
  getProjectCode(value) {
    if (!this.infoForm.controls.companyCode.value) {
      this.message.error('Please choose company first!', { nzDuration: 6000 });
      return;
    }
    if (value.length > 0) {
      const params = {
        code: value,
        company: this.infoForm.controls.companyCode.value,
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

  inputProjectCode(value) {
    this.getPJCodeTerms.next(value);
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

  //判断当前签核人是否是申请人
  getRnoRole() {
    this.Service.doGet(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCurrentApprovalFlow + `?rno=${this.rno}`, {}).subscribe(res => {
      if (res.body?.status == 1 && res.body?.data) {
        res.body.data.map(i => {
          if (i.status == 'P') {
            if (i.cemplid == this.userInfo.emplid) {
              this.infoForm.controls.businessTripNo.enable();
              this.infoForm.controls.projectCode.enable();
              this.isApplicant = true;
            }
          }
        });
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
      this.Service.Post(
        this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.GetUberFormData,
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

    if (headData != null) {
      this.infoForm.controls.rno.setValue(headData.rno);
      this.infoForm.controls.companyCode.setValue(headData.company);
      this.infoForm.controls.businessTripNo.setValue(headData.businessTripNo);
      this.infoForm.controls.projectCode.setValue(headData.projectCode);
      this.infoForm.controls.applicantEmplid.setValue(headData.emplid);
      this.infoForm.controls.applicantName.setValue(headData.name);
      this.infoForm.controls.dept.setValue(headData.deptid);
    }
    if (listData != null) {
      listData.map((o) => {
        this.detailTableData.push({
          formCode: o.formCode,
          rno: o.rno,
          item: o.item,
          startDate: o.startDate ? format(new Date(o.startDate), 'yyyy/MM/dd') : null,
          destination: o.destination,
          origin: o.origin,
          status: o.status,
          amount: o.amount ?? null,
          reason: o.reason,
          expCode: o.expCode,
          emplid: o.emplid,
          name: o.name,
          deptId: o.deptId,
        });
      });
      this.detailTableData = [...this.detailTableData];
    }
    this.isSpinning = false;
  }
  submit(): void {
    const projectCode = this.infoForm.controls.projectCode.value;
    const businessTripNo = this.infoForm.controls.businessTripNo.value;
    // 如果两个都为空，则不提交
    if ((!projectCode || projectCode === '') && (!businessTripNo || businessTripNo === '')) {
      this.message.warning(this.translate.instant('projectCode-or-businessTripNo-required'));
      return;
    }
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.UberSubmit,
      { rno: this.rno, projectCode: projectCode, businessTripNo: businessTripNo }
    ).subscribe((res) => {
      if (res && res.status === 200 && res.body.status === 1) {
        this.message.success(this.translate.instant('submit-successfully'));
      }
    });
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
    // if (this.userRole == 'Fin') {
    //   data['detail'] = this.detailTableData.map((o) => {
    //     return {
    //       seq: o.id,
    //       assignment: o.whiteRemark,
    //       taxexpense: o.taxexpense,
    //     };
    //   });
    // }
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }
}
