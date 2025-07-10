import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/shared/service/common.service';
import { BDInfoTableColumn } from './classes/table-column';
import { VenderInfo } from './classes/data-item';
import { NzMessageService } from 'ng-zorro-antd/message';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { URLConst } from 'src/app/shared/const/url.const';
import { format } from 'date-fns';
import { NzModalService } from 'ng-zorro-antd/modal';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-supplier-info',
  templateUrl: './supplier-info.component.html',
  styleUrls: ['./supplier-info.component.scss']
})
export class SupplierInfoComponent implements OnInit {

  isSpinning = false;
  isQueryLoading = false;
  queryForm: UntypedFormGroup;
  showTable = true;
  listTableData: VenderInfo[] = [];
  total: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  checked = false;
  indeterminate = false;
  listTableColumn = BDInfoTableColumn;
  userInfo: any;
  listOfCurrentPageData: VenderInfo[] = [];
  setOfCheckedId = new Set<string>();
  showModal = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  screenWidth: any;
  isSaveLoading: boolean = false;
  listForm: UntypedFormGroup;
  queryParam: any;

  constructor(public translate: TranslateService,
    private fb: UntypedFormBuilder,
    private commonSrv: CommonService,
    private message: NzMessageService,
    private Service: WebApiService,
    private modal: NzModalService,
  ) { }

  ngOnInit(): void {
    this.queryForm = this.fb.group({
      unifyCode: [null],
      venderCode: [null],
      venderName: [null],
    });
    this.listForm = this.fb.group({
      Id: [null],
      UnifyCode: [null, [Validators.required]],
      VenderCode: [null, [Validators.required]],
      VenderName: [null, [Validators.required]],
      // iscarry: [null, [Validators.required]],
  });
  }

  search(){
    this.isQueryLoading = true;
    this.queryParam = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      data: {
        UnifyCode: (this.queryForm.get('unifyCode').value || '').trim(),
        VenderCode: (this.queryForm.get('venderCode').value || '').trim(),
        VenderName: (this.queryForm.get('venderName').value || '').trim(),
      }
  }
    this.Service.Post(URLConst.QuerySupplier, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          this.total = res.body.total;
          let result: VenderInfo[] = [];
          res.body.data?.map(o => {
              result.push({
                  Id: o.id,
                  UnifyCode: o.unifyCode,
                  VenderName: o.venderName,
                  VenderCode: o.venderCode,
                  Iscarry: o.iscarry,
                  cuser: o.cuser,
                  cdate: o.cdate == null ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
                  muser: o.muser,
                  mdate: o.mdate == null ? null : format(new Date(o.mdate), "yyyy/MM/dd"),
                  disabled: false,
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
    this.listForm.reset({ iscarry: 'N' });
    if (!this.listForm.controls.UnifyCode.enabled) this.listForm.controls.UnifyCode.enable();
    this.showModal = true;
  }

  update(item){
    this.isSpinning = true;
    this.editloading = true;
    this.listForm.reset(item);
    this.isSpinning = false;
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

onCurrentPageDataChange(listOfCurrentPageData: VenderInfo[]): void {
  this.listOfCurrentPageData = listOfCurrentPageData;
  this.refreshCheckedStatus();
}

refreshCheckedStatus(): void {
  const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
  this.checked = listOfEnabledData.every(({ Id }) => this.setOfCheckedId.has(Id));
  this.indeterminate = listOfEnabledData.some(({ Id }) => this.setOfCheckedId.has(Id)) && !this.checked;
}

onAllChecked(checked: boolean): void {
  this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ Id }) => this.updateCheckedSet(Id, checked));
  this.refreshCheckedStatus();
}

updateCheckedSet(id: string, checked: boolean): void {
  if (checked) {
      this.setOfCheckedId.add(id);
  } else {
      this.setOfCheckedId.delete(id);
  }
}

onItemChecked(id: string, checked: boolean): void {
  this.updateCheckedSet(id, checked);
  this.refreshCheckedStatus();
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
  if(!((this.listForm.get('UnifyCode').value || '').trim()) || !((this.listForm.get('VenderCode').value || '').trim()) || !((this.listForm.get('VenderName').value || '').trim())){
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isSaveLoading = false;
      return;
  }
  let listFormData = this.listForm.getRawValue();
  let params = {
      Id: listFormData.Id,
      UnifyCode: listFormData.UnifyCode,
      VenderCode: listFormData.VenderCode,
      VenderName: listFormData.VenderName,
  }
  if (!this.editloading) this.addItem(params);
  else this.editItem(params);
}

addItem(params: any) {
  params.Id = null;
  this.Service.Post(URLConst.AddSupplier, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
          if (res.body.status == 1) {
              this.message.success(this.translate.instant('save-successfully'));
              this.setOfCheckedId.clear();
              this.search();
              this.showModal = false;
          }
          else this.message.error((res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isSaveLoading = false;
      this.isSpinning = false;
  });
}

editItem(params: any) {
  this.Service.Put(URLConst.EditSupplier, params).subscribe((res) => {
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
  this.Service.Delete(URLConst.DeleteSupplier, ids).subscribe((res) => {
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

DownloadFile() {
  this.queryParam = {
    data: {
      UnifyCode: (this.queryForm.get('unifyCode').value || '').trim(),
      VenderCode: (this.queryForm.get('venderCode').value || '').trim(),
      VenderName: (this.queryForm.get('venderName').value || '').trim(),
    }
  }
  this.Service.Post(URLConst.DownloadSupplier, this.queryParam).subscribe((res) => {
    if (res && res.status === 200 && res.body != null) {
      this.DownloadFlow(res.body.data, `SupplierInfo.xlsx`);
    }
});
}

DownloadFlow(flow:any[], name) {
  let title: string[] = [
    this.translate.instant('unifyCode'),
    this.translate.instant('venderCode'),
    this.translate.instant('venderName'),
  ];
  const workbook = new ExcelJS.Workbook();
  const sheet = workbook.addWorksheet('Sheet1');
  sheet.addRow(title);

  if (flow.length > 0) {
    flow.forEach(item => {
      sheet.addRow([item.unifyCode, item.venderCode, item.venderName]);
    });
  }

  // 保存文件，檔名: RMIS+yyyymmdd_hhmmss.xlsx
  const filename = `${new Date().toISOString().replace(/[:.-]/g, '').split('T')[0]}_${Date.now() % 10000}`;
  workbook.xlsx.writeBuffer().then(buffer => {
    const blob = new Blob([buffer], {type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
    saveAs(blob, 'SupplierInfo.xlsx');
  }).catch(err => console.error('Error writing excel export', err));

}

autoTips: Record<string, Record<string, string>> = {
  default: {
      required: this.translate.instant('can-not-be-null'),
  }
};
}
