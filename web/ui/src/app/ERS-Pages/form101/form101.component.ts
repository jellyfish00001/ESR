import { Component, OnInit } from '@angular/core';
import { TableColumnModel } from 'src/app/shared/models';
import { TranslateService } from '@ngx-translate/core';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { format } from 'date-fns';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-form101',
  templateUrl: './form101.component.html',
  styleUrls: ['./form101.component.scss']
})
export class Form101Component implements OnInit {
  SignTableColumn: TableColumnModel[] = [
    {
      title: this.translate.instant('selfsignform.company'),
      columnKey: 'company',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.company.localeCompare(b.company),
    },
    {
      title: this.translate.instant('selfsignform.rno'),
      columnKey: 'rno',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.rno.localeCompare(b.rno),
    },
    // {
    //   title: this.translate.instant('selfsignform.category'),
    //   columnKey: 'category',
    //   columnWidth: '',
    //   align: 'left',
    //   sortFn: (a: SignList, b: SignList) => a.category.localeCompare(b.category),
    // },
    {
      title: this.translate.instant('selfsignform.formname'),
      columnKey: 'formname',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.formname.localeCompare(b.formname),
    },
    {
      title: this.translate.instant('selfsignform.expname'),
      columnKey: 'expname',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.expname.localeCompare(b.expname),
    },
    {
      title: this.translate.instant('selfsignform.currency'),
      columnKey: 'currency',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.currency.localeCompare(b.currency),
    },
    {
      title: this.translate.instant('selfsignform.amount'),
      columnKey: 'amount',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.amount.localeCompare(b.amount),
    },
    {
      title: this.translate.instant('selfsignform.cuser'),
      columnKey: 'cuser',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.cuser.localeCompare(b.cuser),
    },
    {
      title: this.translate.instant('selfsignform.cname'),
      columnKey: 'cname',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.cname.localeCompare(b.cname),
    },
    {
      title: this.translate.instant('selfsignform.cdate'),
      columnKey: 'cdate',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.cdate.localeCompare(b.cdate),
    },
    {
      title: this.translate.instant('selfsignform.stepname'),
      columnKey: 'stepname',
      columnWidth: '',
      align: 'left',
      sortFn: (a: SignList, b: SignList) => a.stepname.localeCompare(b.stepname),
    },
  ];
  signTableData: SignList[] = [];
  pageIndex: number = 1;
  pageSize: number = 10;
  total: any;
  isSpinning = false;

  constructor(
    private translate: TranslateService,
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private crypto: CryptoService,
    private router: Router,
    private message: NzMessageService,
  ) { }

  ngOnInit(): void {
    this.isSpinning = true;
    this.Query();
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.Query();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.Query();
  }
  Query() {
    var queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    }
    this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.SelfSignApi, queryParam).subscribe(res => {
      this.total = res.body.total;
      this.signTableData = [];
      if (res.body.status == 1 && res.body.data?.length > 0) {
        res.body.data.map(i => {
          this.signTableData.push({
            company: i.company,
            rno: i.rno,
            category: i.formcategory,
            formname: i.formname,
            expname: i.expname,
            currency: i.currency,
            amount: i.actamt,
            cuser: i.cuser,
            cname: i.cname,
            cdate: format(new Date(i.cdate), 'yyyy/MM/dd HH:MM:SS'),
            stepname: i.stepname,
            apid: i.apid
          })
        });
        this.signTableData = [...this.signTableData];
      }
      else if (res.body.status == 2) { this.message.error(res.body.message); }
      this.isSpinning = false;
    });
  }

  skipForm(item) {
    let data = {
      rno: this.crypto.encrypt(item.rno)
    };
    let target = item.apid.toLowerCase();
    this.router.navigate([`ers/${target}`], { queryParams: { data: JSON.stringify(data), preUrl: 'form101' } })
  }

}
export interface SignList {
  company: string;
  rno: string;
  category: string;
  formname: string;
  expname: string;
  currency: string;
  amount: string;
  cuser: string;
  cname: string;
  cdate: string;
  stepname: string;
  apid: string;
}