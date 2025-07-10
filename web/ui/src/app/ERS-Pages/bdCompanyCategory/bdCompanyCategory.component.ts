import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { CommonService } from 'src/app/shared/service/common.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { CompanyInfo, CompanySite } from './classes/data-item';
import { BDInfoTableColumn } from './classes/table-column';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';

@Component({
  selector: 'app-bd_company_category',
  templateUrl: './bdCompanyCategory.component.html',
  styleUrls: ['./bdCompanyCategory.component.scss']
})
export class BdCompanyCategoryComponent implements OnInit {

  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  queryParam: any;
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  listTableData: CompanyInfo[] = [];
  showTable = true;
  isQueryLoading = false;
  addloading = false;
  showModal = false;
  isSpinning = false;
  editloading = false;
  userInfo: any;
  isSaveLoading = false;
  setOfCheckedId = new Set<string>();
  listOfCurrentPageData: CompanyInfo[] = [];
  listTableColumn = BDInfoTableColumn;
  screenWidth: any;
  companySiteArray: Array<CompanySite> = [];
  newTemp: any = {};
  currList: any[] = [];
  utcTimeZones = [
    { key: 'UTC-12', value: -12 },
    { key: 'UTC-11', value: -11 },
    { key: 'UTC-10', value: -10 },
    { key: 'UTC-9', value: -9 },
    { key: 'UTC-8', value: -8 },
    { key: 'UTC-7', value: -7 },
    { key: 'UTC-6', value: -6 },
    { key: 'UTC-5', value: -5 },
    { key: 'UTC-4', value: -4 },
    { key: 'UTC-3', value: -3 },
    { key: 'UTC-2', value: -2 },
    { key: 'UTC-1', value: -1 },
    { key: 'UTC+0', value: 0 },
    { key: 'UTC+1', value: 1 },
    { key: 'UTC+2', value: 2 },
    { key: 'UTC+3', value: 3 },
    { key: 'UTC+4', value: 4 },
    { key: 'UTC+5', value: 5 },
    { key: 'UTC+6', value: 6 },
    { key: 'UTC+7', value: 7 },
    { key: 'UTC+8', value: 8 },
    { key: 'UTC+9', value: 9 },
    { key: 'UTC+10', value: 10 },
    { key: 'UTC+11', value: 11 },
    { key: 'UTC+12', value: 12 },
    { key: 'UTC+13', value: 13 },
    { key: 'UTC+14', value: 14 },
  ];
  areaList:any[] = [];

  constructor(public translate: TranslateService,
    private fb: UntypedFormBuilder,
    private commonSrv: CommonService,
    private message: NzMessageService,
    private Service: WebApiService,
    private modal: NzModalService,
    private EnvironmentconfigService: EnvironmentconfigService,
  ) { }

  ngOnInit(): void {
    this.queryForm = this.fb.group({
      companyCategory: [null],
      companySap: [null],
      companyDesc: [null],
    });
    this.listForm = this.fb.group({
      Id: [null],
      CompanyCategory: [null, [Validators.required]],
      CompanySap: [null, [Validators.required]],
      CompanyDesc: [null, [Validators.required]],
      Stwit: [null, [Validators.required]],
      BaseCurrency: [null, [Validators.required]],
      IdentificationNo: [null],
      IncomeTaxRate: [null],
      Vatrate: [null],
      Area: [null, [Validators.required]],
      TimeZone: [null, [Validators.required]],
      // Site: [null, [Validators.required]],
      // Company: [null, [Validators.required]],
      CompanySite: [null]
      // iscarry: [null, [Validators.required]],
  });
  this.getCurrency();
  this.getAreaList();
  }

  search(){
    this.isQueryLoading = true;
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        CompanyCategory: (this.queryForm.get('companyCategory').value || '').trim(),
      }
  }
    this.Service.Post(URLConst.QueryCompany, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null && res.body.status == 1) {
          this.total = res.body.total;
          let result: CompanyInfo[] = [];
          res.body.data?.map(o => {
              result.push({
                  Id: o.id,
                  CompanyCategory: o.companyCategory,
                  CompanyDesc: o.companyDesc,
                  CompanySap: o.companySap,
                  Stwit: o.stwit,
                  BaseCurrency: o.baseCurrency,
                  IncomeTaxRate: o.incomeTaxRate,
                  IdentificationNo: o.identificationNo,
                  Area: o.area ,
                  Vatrate: o.vatrate,
                  TimeZone: o.timeZone,
                  // Company: o.company,
                  // Site: o.site,
                  Status: o.status,
                  CompanySite: o.companySite,
              })
          });
          this.listTableData = result;
          this.showTable = true;
          this.isQueryLoading = false;
      }
      else {
        this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
        this.isQueryLoading = false;
      }
  });
  }

  create(){
    this.addloading = true;
    this.listForm.enable();
    this.listForm.reset({ iscarry: 'N' });
    if (!this.listForm.controls.CompanyCategory.enabled) this.listForm.controls.CompanyCategory.enable();

    this.companySiteArray = [];
    this.newTemp = {
      Id: '',
      CompanyCategory: '',
      Company: '',
      Site: '',
    };
    this.companySiteArray.push(this.newTemp);
    this.companySiteArray.forEach((x, index) => { x.seq = index + 1; });

    this.showModal = true;
  }

  update(item){
    this.listForm.enable();
    this.listForm.controls.CompanyCategory.disable();
    this.companySiteArray = item.CompanySite;
    this.isSpinning = true;
    this.editloading = true;
    this.listForm.reset(item);
    this.companySiteArray = [];
    let i = 1;
    item.CompanySite?.forEach(e => {
      this.newTemp = {
        Id: e.id,
        seq: i++,
        CompanyCategory: e.companyCategory,
        Company: e.company,
        Site: e.site,
      };
      this.companySiteArray.push(this.newTemp);
    });;
    this.isSpinning = false;
    this.showModal = true;
  }

  view(item){
    console.log(item);
    this.listForm.reset(item);
    this.listForm.disable();
    this.companySiteArray = [];
    let i = 1;
    item.CompanySite?.forEach(e => {
      this.newTemp = {
        Id: e.id,
        seq: i++,
        CompanyCategory: e.companyCategory,
        Company: e.company,
        Site: e.site,
      };
      this.companySiteArray.push(this.newTemp);
    });;
    this.showModal = true;
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.search();
}

pageSizeChange(value) {
    this.pageSize = value;
    this.search();
}

getEmployeeInfo() {
  this.userInfo = this.commonSrv.getUserInfo;
}

handleCancel(): void {
  this.showModal = false;
  this.addloading = false;
  this.editloading = false;
}

handleOk(): void {
  this.isSpinning = true;
  this.isSaveLoading = true;
  if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach(control => {
          if (control.invalid) {
              control.markAsDirty();
              control.updateValueAndValidity({ onlySelf: true });
          }
      });
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
  }
  //检查适用区域的资料是否有重复
  let duplicateKey: string[] = [];
  let hasDeplicate: boolean = false;
  this.companySiteArray.forEach((x, index) => {
    let key = x.Company.trim() + x.Site.trim();
    if (duplicateKey.includes(key)) {
      hasDeplicate = true;
      return;
    } else {
      duplicateKey.push(key);
    }
  });
  if (hasDeplicate) {
    this.message.error(this.translate.instant('duplicate-company-site'));
    this.isSpinning = false;
    this.isSaveLoading = false;
    return;
  }
  //检查适用区域的资料是否有空白
  let hasEmptyCompanySite: boolean = false;
  this.companySiteArray.forEach((x, index) => {
    if (!x.Company.trim() || !x.Site.trim()) {
      hasEmptyCompanySite = true;
      return;
    }
  });

  if(!((this.listForm.get('CompanyCategory').value || '').trim()) || !((this.listForm.get('CompanyDesc').value || '').trim()) || !((this.listForm.get('CompanySap').value || '').trim())
     || !((this.listForm.get('Stwit').value || '').trim()) || !((this.listForm.get('BaseCurrency').value || '').trim())
     || !((this.listForm.get('Area').value || '').trim())
     || !(this.listForm.get('TimeZone').value) || hasEmptyCompanySite) {
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
  }

  let listFormData = this.listForm.getRawValue();
  this.companySiteArray.forEach((x, index) => {x.CompanyCategory = listFormData.CompanyCategory;});
  let params: CompanyInfo = {
      Id: listFormData.Id,
      CompanyCategory: listFormData.CompanyCategory,
      CompanyDesc: listFormData.CompanyDesc,
      CompanySap: listFormData.CompanySap,
      Stwit: listFormData.Stwit,
      BaseCurrency: listFormData.BaseCurrency,
      IncomeTaxRate: listFormData.IncomeTaxRate,
      IdentificationNo: listFormData.IdentificationNo,
      Area: listFormData.Area ,
      Vatrate: listFormData.Vatrate,
      TimeZone: listFormData.TimeZone,
      // Site: listFormData.Site,
      // Company: listFormData.Company,
      Status: '',
      CompanySite: this.companySiteArray,
  }
  if (!this.editloading) this.addItem(params);
  else this.editItem(params);
}

addItem(params: CompanyInfo) {
  params.Id = '';
  this.Service.Post(URLConst.AddCompany, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          if (res.body.status == 1) {
              this.message.success(this.translate.instant('save-successfully'));
              this.setOfCheckedId.clear();
              this.search();
              this.showModal = false;
              this.addloading = false;
          }
          else this.message.error((res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));

      this.isSaveLoading = false;
      this.isSpinning = false;
  });
}

editItem(params: CompanyInfo) {
  this.Service.Put(URLConst.EditCompany, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          if (res.body.status == 1) {
              this.message.success(this.translate.instant('save-successfully'));
              this.setOfCheckedId.clear();
              this.search();
          }
          else this.message.error((res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.editloading = false;
      this.isSaveLoading = false;
      this.isSpinning = false;
      this.showModal = false;
  });
}

delete(id: string){
  let ids:string[] =[]
  ids.push(id);
  this.Service.Delete(URLConst.DeleteCompany, ids).subscribe((res) => {
    if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
            this.message.success(this.translate.instant('delete-successfully'));
            this.setOfCheckedId.clear();
            this.search();
        }
        else this.message.error(this.translate.instant('delete-failed') + (res.body.message == null ? '' : res.body.message));
    }
    else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
    this.editloading = false;
    this.isSaveLoading = false;
    this.isSpinning = false;
    this.showModal = false;
});
}

deleteConfirm(id: string): void {
  this.modal.confirm({
    nzTitle: this.translate.instant('delete-confirm'),
    nzContent: '',
    nzOnOk: () => {
        this.delete(id);
    }
});
}

onCurrentPageDataChange(listOfCurrentPageData: CompanyInfo[]): void {
  this.listOfCurrentPageData = listOfCurrentPageData;
}

trackByFn(i: number) {
  return i;
}

delEmpRow(index) {
  if (this.companySiteArray.length == 1) {
    alert(this.translate.instant("At least one row is needed"));
    return false;
  } else {
    this.companySiteArray.splice(index, 1);
    this.companySiteArray.forEach((x, index) => { x.seq = index + 1; });
    return true;
  }
}

addEmpRow() {
  this.newTemp = {
    Id: '',
    CompanyCategory: '',
    Company: '',
    Site: '',
  };
  this.companySiteArray.push(this.newTemp);
  this.companySiteArray.forEach((x, index) => { x.seq = index + 1; });
  return true;
}

autoTips: Record<string, Record<string, string>> = {
  default: {
      required: this.translate.instant('can-not-be-null'),
  }
};

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

getAreaList() {
  const queryParam = {
    pageIndex: 1,
    pageSize: 10,
    data: {category: 'area'},
  };
  this.Service.Post(
    this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryDataDictionary,
    queryParam
  ).subscribe((res) => {
    if (res && res.status === 200 && res.body != null) {
      let result: any[] = [];
      res.body.data?.map((o) => {
        result.push({
          value: o.value,
          label: o.name,
        });
      });
      this.areaList = result;
    }
  });
}

}
class CompanySiteGrid {
  Id: string;
  seq: number;
  CompanyCategory: string;
  CompanyCode: string;
  CompanySite: string;
}