import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService } from '@ngx-translate/core';
import { TableColumnModel } from 'src/app/shared/models';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';


@Component({
  selector: 'app-bd009',
  templateUrl: './bd009.component.html',
  styleUrls: ['./bd009.component.scss']
})
export class BD009Component implements OnInit {
  queryForm: UntypedFormGroup;
  isSpinning = false;
  total: any;
  isQueryLoading = false;
  pageIndex: number = 1;
  pageSize: number = 10;
  queryParam: any;
  // index: number = 0;
  addloading = false;
  listTableData: DetailInfo[] = [];
  showTable = false;
  userInfo: any;
  //表格
  bdaccountInfoTableColumn: TableColumnModel[] = [
    {
      title: 'category',
      columnKey: 'category',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.category.localeCompare(b.category),
    },
    {
      title: 'value',
      columnKey: 'value',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.value.localeCompare(b.value)
    },
    {
      title: 'dictionaryName',
      columnKey: 'name',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.name.localeCompare(b.name),
    },
    {
      title: 'nameZhtw',
      columnKey: 'nameZhtw',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.nameZhtw.localeCompare(b.nameZhtw),
    },
    {
      title: 'nameZhcn',
      columnKey: 'nameZhcn',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.nameZhcn.localeCompare(b.nameZhcn),
    },
    {
      title: 'nameVn',
      columnKey: 'nameVn',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.nameVn.localeCompare(b.nameVn),
    },
    {
      title: 'nameEs',
      columnKey: 'nameEs',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.nameEs.localeCompare(b.nameEs),
    },
    {
      title: 'nameCz',
      columnKey: 'nameCz',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.nameCz.localeCompare(b.nameCz),
    },
    {
      title: 'sortOrder',
      columnKey: 'sortOrder',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.sortOrder.localeCompare(b.sortOrder),
    },
    {
      title: 'criteria',
      columnKey: 'criteria',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.criteria.localeCompare(b.criteria),
    },
    {
      title: 'description',
      columnKey: 'description',
      columnWidth: '',
      align: 'center',
      sortFn: (a: DetailInfo, b: DetailInfo) =>
        a.description.localeCompare(b.description),
    }
  ];
  listTableColumn = this.bdaccountInfoTableColumn;

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    public translate: TranslateService,
    private message: NzMessageService,
    private authService: AuthService,
    private router: Router,
    private commonSrv: CommonService,
  ) { }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']); //to-do
    this.userInfo = this.commonSrv.getUserInfo;
    this.queryForm = this.fb.group({
      category: [null],
      criteria: [null],
    });
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
    if (!this.queryForm.valid) {
      Object.values(this.queryForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('exist-invalid-field'));
      return;
    }
    this.isQueryLoading = true;
    let paramValue = this.queryForm.getRawValue();
    if (initial) {
      this.pageIndex = 1;
      this.pageSize = 10;
    }
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        category: paramValue.category,
        criteria: paramValue.criteria,
      }
    }

    this.Service.Post(URLConst.QueryDataDictionary, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: DetailInfo[] = [];
        if (res.body.data) {
          res.body.data.map(o => {
            result.push({
              category: o.category,
              criteria: o.criteria,
              value: o.value,
              name: o.name,
              nameZhcn: o.nameZhcn,
              nameZhtw: o.nameZhtw,
              nameVn: o.nameVn,
              nameEs: o.nameEs,
              nameCz: o.nameCz,
              sortOrder: o.sortOrder,
              description: o.description,
            })
          });
        }
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }
}
export interface DetailInfo {
  category: string;
  value: string;
  name: string;
  nameZhtw: string;
  nameZhcn: string;
  nameVn: string;
  nameEs: string;
  nameCz: string;
  sortOrder: string;
  criteria: string;
  description: string;
}
