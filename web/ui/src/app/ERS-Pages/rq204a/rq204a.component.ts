import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { TableColumnModel } from 'src/app/shared/models';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { CommonService } from 'src/app/shared/service/common.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';

@Component({
  selector: 'app-rq204a',
  templateUrl: './rq204a.component.html',
  styleUrls: ['./rq204a.component.scss']
})

export class RQ204AComponent implements OnInit, OnDestroy {

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
  ) { }

  rno: string;
  totalExTipModal: boolean = false;
  isSaveLoading: boolean = false;
  exTotalWarning: any[] = [];
  showInvoiceList: boolean = false;
  exSummaryData: ExceptionSummary[] = [];
  isFinUser: boolean = false;
  fileList: NzUploadFile[] = [];
  isSpinning = false;
  preUrl: string;
  showTransformButton:boolean = false;

  ngOnInit(): void {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        let rno: string = this.crypto.decrypt(data.rno);
        this.rno = rno;
        this.preUrl = data.preUrl;
      }
    });
    this.getRnoRole();
  }

  ngOnDestroy() { }

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

  getTotalExTips(value): void {
    this.exTotalWarning = value;
  }

  getExSummaryData(value): void {
    this.exSummaryData = value;
  }

  getFileData(value): void {
    this.fileList = value;
  }

  checkInvoiceDetail(): void {
    this.showInvoiceList = !this.showInvoiceList;
  }

  approve(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      this.exSummaryData.map(o => {
        data.detail.push({ seq: 1, assignment: o.whiteRemark })
      });
    }
    this.commonSrv.Approval(data, this.preUrl);
    this.isSpinning = false;
  }
  reject(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      this.exSummaryData.map(o => {
        data.detail.push({ seq: 1, assignment: o.whiteRemark })
      });
    }
    this.commonSrv.Reject(data, this.preUrl);
    this.isSpinning = false;
  }
  transform(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      this.exSummaryData.map(o => {
        data.detail.push({ seq: 1, assignment: o.whiteRemark })
      });
    }
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }
}

export interface ExceptionSummary {
  selfAffordAmt: number;
  companyAffordAmt: number;
  whiteRemark: string;
  company: string;
}
