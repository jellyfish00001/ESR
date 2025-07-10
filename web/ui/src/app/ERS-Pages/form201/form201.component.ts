import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TableColumnModel } from 'src/app/shared/models';
import { FormDetail } from './classes/data-item';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-form201',
  templateUrl: './form201.component.html',
  styleUrls: ['./form201.component.scss']
})

export class FORM201Component implements OnInit {
  DetailTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('application-number'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.rno.localeCompare(b.rno),
    },
    {
      title: this.translate.instant('form-type'),
      columnKey: 'formType',
      columnWidth: '',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.formType.localeCompare(b.formType),
    },
    {
      title: this.translate.instant('form-type-name'),
      columnKey: 'formTypeName',
      columnWidth: '',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.formTypeName.localeCompare(b.formTypeName),
    },
    {
      title: this.translate.instant('applicant'),
      columnKey: 'applicant',
      columnWidth: '',
      align: 'left',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicant.localeCompare(b.applicant),
    },
    {
      title: this.translate.instant('applicant-name'),
      columnKey: 'applicantName',
      columnWidth: '',
      align: 'right',
      sortFn: (a: FormDetail, b: FormDetail) => a.applicantName.localeCompare(b.applicantName),
    },
    {
      title: this.translate.instant('applied-date'),
      columnKey: 'appliedDate',
      columnWidth: '',
      align: 'right',
      sortFn: (a: FormDetail, b: FormDetail) => a.appliedDate.localeCompare(b.appliedDate),
    },
    {
      title: this.translate.instant('step'),
      columnKey: 'step',
      columnWidth: '',
      align: 'right',
      sortFn: (a: FormDetail, b: FormDetail) => a.step.localeCompare(b.step),
    },
  ];

  // rno: string;
  screenHeight: any;
  screenWidth: any;
  detailListTableColumn = this.DetailTableColumn;
  detailTableData: FormDetail[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;



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
    this.getEmployeeInfo();
    this.queryResultWithParam();
  }

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithParam();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithParam();
  }

  queryResultWithParam(initial: boolean = false) {
    if (initial) {
      this.pageIndex = 1;
      this.pageSize = 10;
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        emplid: this.userInfo.emplid,
      }
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetPaperFormToBeSigned, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null && res.body.status == 1) {
        this.total = res.body.total;
        let result: FormDetail[] = [];
        res.body.data?.map(o => {
          result.push({
            rno: o.rno,
            formType: o.formcode,
            formTypeName: o.formname,
            applicant: o.cemplid,
            applicantName: o.cname,
            appliedDate: o.cdate != null ? format(new Date(o.cdate), "yyyy/MM/dd") : '',
            step: o.stepname,
            apid: o.apid,
            status: o.status,
            disabled: false,
          })
        });
        this.detailTableData = result;
      }
      else this.message.error(this.translate.instant('server-error'));
      this.isSpinning = false;
    });
  }

  ////////带选择框表
  checked = false;
  approveloading = false;
  indeterminate = false;
  listOfCurrentPageData: FormDetail[] = [];
  setOfCheckedId = new Set<string>();
  updateCheckedSet(rno: string, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(rno);
    } else {
      this.setOfCheckedId.delete(rno);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: FormDetail[]): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
    this.checked = listOfEnabledData.every(({ rno }) => this.setOfCheckedId.has(rno));
    this.indeterminate = listOfEnabledData.some(({ rno }) => this.setOfCheckedId.has(rno)) && !this.checked;
  }

  onItemChecked(rno: string, checked: boolean): void {
    this.updateCheckedSet(rno, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ rno }) => this.updateCheckedSet(rno, checked));
    this.refreshCheckedStatus();
  }

  approve(rno: string = ''): void {
    this.approveloading = true;
    this.isSpinning = true;
    let rnoList: string[] = [];
    if (rno == '') {   //多选操作
      // 核准单据
      rnoList = Array.from(this.setOfCheckedId);
      this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SignPaperForm, rnoList).subscribe((res) => {
        if (res && res.status === 200 && res.body != null && res.body.status == 1) {
          this.message.success(res.body.message);
          this.detailTableData = this.detailTableData.filter(d => !this.setOfCheckedId.has(d.rno));
          this.setOfCheckedId.clear();
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.status === 200 ? res.body.message ?? res.message : this.translate.instant('server-error')));
        this.isSpinning = false;
      });
    }
    else {
      rnoList.push(rno);
      this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SignPaperForm, rnoList).subscribe((res) => {
        if (res && res.status === 200 && res.body != null && res.body.status == 1) {
          this.message.success(res.body.message);
          this.detailTableData = this.detailTableData.filter(d => d.rno != rno);
          this.setOfCheckedId.delete(rno);
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.status === 200 ? res.body.message ?? res.message : this.translate.instant('server-error')));
        this.isSpinning = false;
      });
    }
    this.approveloading = false;
  }

  checkForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    this.router.navigate([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form201' } })
  }

}
