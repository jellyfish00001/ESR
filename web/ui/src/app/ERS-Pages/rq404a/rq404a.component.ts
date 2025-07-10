import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { DelayInfo } from '../rq401a/classes/data-item';
import { TableColumnModel } from 'src/app/shared/models';
import { format } from 'date-fns';
import { ApprovalParams } from '../rq204/classes/data-item';
import { CommonService } from 'src/app/shared/service/common.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-rq404a',
  templateUrl: './rq404a.component.html',
  styleUrls: ['./rq404a.component.scss']
})
export class RQ404AComponent implements OnInit {
  headForm: UntypedFormGroup;
  rno: string;
  preUrl: string;
  listTableData: DelayInfo[] = [];
  isSpinning = false;
  DelayInfoTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('advance-fund-no'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) =>
        a.rno.localeCompare(b.rno),
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
      sortFn: (a: DelayInfo, b: DelayInfo) => a.notChargeAgainstAmt - b.notChargeAgainstAmt,
    },
    {
      title: this.translate.instant('scheduled-debit-date'),
      columnKey: 'scheduledDebitDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.scheduledDebitDate.localeCompare(b.scheduledDebitDate),
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
      sortFn: (a: DelayInfo, b: DelayInfo) => a.delayChargeAgainstDays - b.delayChargeAgainstDays,
    },
    {
      title: this.translate.instant('after-date'),
      columnKey: 'afterDate',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.afterDate.localeCompare(b.afterDate),
    },
    {
      title: this.translate.instant('delay-reason'),
      columnKey: 'delayReason',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DelayInfo, b: DelayInfo) => a.delayReason.localeCompare(b.delayReason),
    },
  ];
  listTableColumn = this.DelayInfoTableColumn;


  constructor(
    private fb: UntypedFormBuilder,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    private Service: WebApiService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private commonSrv: CommonService,
    private message: NzMessageService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    this.headForm = this.fb.group({
      emplid: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      payee: [{ value: null, disabled: true }],
      ext: [{ value: null, disabled: true }],
      companyCode: [{ value: null, disabled: true }],
    });
    this.getRnoInfo();
  }
  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    }
  };

  getRnoInfo() {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        let rno: string = this.crypto.decrypt(data.rno);
        this.headForm.controls.rno.setValue(rno);
        this.rno = rno;
        this.preUrl = data.preUrl;
        // 获取单据信息
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
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.headForm.controls.companyCode.setValue(headData.company);
      this.headForm.controls.payee.setValue(headData.cname);
      this.headForm.controls.ext.setValue(headData.ext);
      this.headForm.controls.dept.setValue(headData.deptid);
      this.headForm.controls.emplid.setValue(headData.cuser);
    }

    if (listData != null) {
      listData.map(o => {
        let afterDate = new Date(o.revsdate);
        afterDate.setDate(afterDate.getDate() + o.delaydays);
        this.listTableData.push({
          id: o.seq,
          rno: o.advancerno,
          digest: o.summary,
          appliedAmt: o.amount1,
          notChargeAgainstAmt: o.amount2,
          scheduledDebitDate: format(new Date(o.revsdate), 'yyyy/MM/dd'),
          openDays: Math.ceil((Number(new Date()) - Number(new Date(o.cdate))) / (1000 * 3600 * 24)),
          delayChargeAgainstDays: o.delaydays,
          afterDate: format(afterDate, 'yyyy/MM/dd'),
          delayReason: o.delayreason,
          disabled: false,
          cdate: o.cdate
        });
      });
      this.listTableData = this.listTableData.sort((a, b) => a.id - b.id);
      this.listTableData = [...this.listTableData];
    }
    this.isSpinning = false;
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
