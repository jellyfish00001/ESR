import { Component, OnInit } from '@angular/core';
import { FormArray, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { URLConst } from 'src/app/shared/const/url.const';
import { CommonService } from 'src/app/shared/service/common.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { format } from 'date-fns';
import { AuthService } from 'src/app/shared/service/auth.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from 'ng-zorro-antd/upload';
import { BehaviorSubject, Observable, Observer, catchError, debounceTime, filter, map, of, switchMap, tap } from 'rxjs';
import _ from 'lodash';
import { Approver, AssignStep } from 'src/app/shared/models/assign-step';
import { formAtLeastRequiredCheckValidator } from 'src/app/shared/validators/validators';
import { DetailInfo } from "./classes/data-item";
import { InfoTableColumn } from "./classes/table-column";
import { ERSConstants } from "src/app/common/constant";

@Component({
  selector: 'app-bd-expense-senario',
  templateUrl: './bd-expense-senario.component.html',
  styleUrls: ['./bd-expense-senario.component.scss']
})
export class BdExpenseSenarioComponent implements OnInit {
  queryForm: UntypedFormGroup;
  listForm: UntypedFormGroup;
  companyList: any[] = [];
  invTypeList: any[] = [];
  subjectList: any[] = [];
  acctcodeList: any[] = [];
  expensecodeList: any[] = [];
  auditLevelCodeList: any[] = [];
  senarioCategoryList: any[] = [];
  calmethodList: any[] = [];
  extraformcodeList: any[] = [];
  isSpinning = false;
  total: any;
  screenWidth: any;
  pageIndex: number = 1;
  pageSize: number = 10;
  isQueryLoading = false;
  isAdd = false;
  isEdit = false;
  isView = false;
  addloading = false;
  downloadloading = false;
  listTableData: DetailInfo[] = [];
  showModal: boolean = false;
  showTable = false;
  index: number = 0;
  queryParam: any;
  userInfo: any;

  listTableColumn = InfoTableColumn;
  batchUploadModal: boolean = false;
  isSaveLoading: boolean = false;
  batchUploadList: NzUploadFile[] = [];
  searchChange$ = new BehaviorSubject('');
  optionList: string[] = [];
  employeeInfoList: Approver[] = [];
  isLoading = false;
  listOfSelectedValue: any[] = [];

  private readonly requiredValidator = [Validators.required];

  get assignStepList() {
    return this.listForm.get('assignSteps') as FormArray;
  }

  onSearch(value: string): void {
    this.isLoading = true;
    this.searchChange$.next(value);
  }

  constructor(
    private fb: UntypedFormBuilder,
    public commonSrv: CommonService,
    private Service: WebApiService,
    public translate: TranslateService,
    private message: NzMessageService,
    private authService: AuthService,
    private modal: NzModalService,
  ) {
    const getEmployeeInfoList = (keyword: string): Observable<any> =>
      this.Service
        .doGet(URLConst.QueryEmployeeInfos + `?keyword=${keyword}`, null)
        .pipe(
          catchError((err, caught) => {
            console.log(err);
            throw err;
          }),
          map((res: any) => res.body.data),
          map((list: Approver[]) => list.map((item: any) => {
            return {
              emplid: item.emplid,
              name: item.name,
              nameA: item.nameA,
              display: _.isEmpty(_.trim(item.nameA)) ? item.name : item.nameA,
            } as Approver;
          })
          ),
        );

    const employeeInfoList$: Observable<Approver[]> = this.searchChange$
      .asObservable()
      .pipe(
        filter((keyword: string) => !_.isEmpty(_.trim(keyword)) && keyword.length >= 3),
        debounceTime(300),
        switchMap(getEmployeeInfoList)
      );

    employeeInfoList$.subscribe(data => {
      this.employeeInfoList = data;
      this.isLoading = false;
    });
  }

  ngOnInit(): void {
    this.authService.CheckPermissionByRoleAndRedirect(['Admin', 'FinanceAdmin']);
    this.isAdd = false;
    this.isEdit = false;
    this.isSpinning = true;
    this.screenWidth = window.innerWidth < 700 ? window.innerWidth * 0.9 + 'px' : '700px';
    this.queryForm = this.fb.group({
      companyCode: [''],
      senarioname: [null],
      //accountingSubject: [null], //to-remove
      expcode: [null], //to-remove
    });

    this.listForm = this.fb.group({
      id: [null],
      companycategory: [null, [Validators.required]],
      category: [null, [Validators.required]], //分类
      senarioname: [null, [Validators.required, Validators.maxLength(30)]],
      expcode: [null, [Validators.required]],
      acctcode: [null, [Validators.required]],
      auditlevelcode: [null, [Validators.required]],
      descriptionnotice: [null, [Validators.maxLength(100)]],
      attachmentnotice: [null, [Validators.maxLength(50)]],
      keyword: [null],
      requirespaperattachment: [null],
      extraformcode: [null], //报销模块
      requiresinvoice: [null],
      requiresattachment: [null],
      isvatdeductable: [null],
      canbypassfinanceapproval: [null],
      attachmentname: [null, [Validators.maxLength(50)]],
      departday: [null],
      sectionday: [null],
      calmethod: [null],
      costcenter: [null],
      assignment: [null],
      pjcode: [null],
      datelevel: [null],
      authorizer: [null],
      authorized: [null],
      sdate: [null],
      edate: [null],
      assignSteps: this.fb.array([]),
    });
    // push default assign step
    this.assignStepList.push(this.createStepForm(true));

    this.userInfo = this.commonSrv.getUserInfo;
    this.listForm.controls.departday.setValue(0);
    this.listForm.controls.sectionday.setValue(0);
    this.getCompanyData();
    this.GetExpenseList();
    this.getSenarioCategoryList();

    this.queryForm.valueChanges.subscribe(value => {
      this.showTable = false;
    });

    this.listForm.controls.companycategory.valueChanges.subscribe(value => {
      this.listForm.controls.acctcode.setValue('');
      this.listForm.controls.auditlevelcode.setValue('');
      if (!!value) {
        this.getAuditLevelCode(value);
        this.getAccountantSubject(value);
      } else if (value == '' && !!this.listForm.controls.acctcode.value) {
        this.acctcodeList = [];
        this.auditLevelCodeList = [];
      }
    });

    this.listForm.controls.category.valueChanges.subscribe(value => {
      if (!!value && value == "advance") {
        this.listForm.controls.datelevel.setValidators([Validators.required]);
      } 
      else {
        this.listForm.controls.datelevel.clearValidators();
      }
    });
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    }
  };

  getCompanyData() {
    this.commonSrv.getOthersCompanys().subscribe(res => {
      this.companyList = res;
      this.isSpinning = false;
    });
  }

  getAuditLevelCode(company: string) {
    this.Service.doGet(URLConst.GetAuditLevelCode + `?company=${company}`, {})
      .pipe(
        map((res) => res.body.data),
        catchError((err, caught) => {
          console.log(err);
          this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 });
          throw err;
        }),
      )
      .subscribe((res) => {
        this.auditLevelCodeList = [...res];
        if (_.isEmpty(res)) {
          this.listForm.controls.auditlevelcode.setValue(null);
        }
        this.isSpinning = false;
      });
  }

  getAccountantSubject(company: string) {
    this.Service.doGet(URLConst.AccountantSubject + `?company=${company}`, {}).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          const data = res.body.data;

          this.acctcodeList = [...data];
          if (!this.acctcodeList.includes(this.listForm.controls.acctcode.value))
            this.listForm.controls.acctcode.setValue('');
        }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 }); }
      this.isSpinning = false;
    });
  }

  GetExpenseList() {
    this.expensecodeList = [];
    this.Service.doGet(URLConst.GetExpenseList, null).subscribe((res) => {
      if (res && res.status === 200) {
        this.expensecodeList = res.body.data;
      }
      else { this.message.error(res.message); }
      this.isSpinning = false;
    });
  }

  getSenarioCategoryList() {
    if (this.senarioCategoryList.length === 0) {
      this.Service.doGet(URLConst.GetDataDictionary + `?category=${ERSConstants.DataDictionary.SenarioCategory}`, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.senarioCategoryList = res.body.data;
        }
        else { this.message.error(res.message); }
      });
    }
  }

  getCalmethodList() {
    if (this.calmethodList.length === 0) {
      this.Service.doGet(URLConst.GetDataDictionary + `?category=${ERSConstants.DataDictionary.CalMethod}`, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.calmethodList = res.body.data;
        }
        else { this.message.error(res.message); }
      });
    }
  }

  getExtraFormCodeList() {
    if (this.extraformcodeList.length === 0) {
      this.Service.doGet(URLConst.GetDataDictionary + `?category=${ERSConstants.DataDictionary.ExtraFormCode}`, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.extraformcodeList = res.body.data;
        }
        else { this.message.error(res.message); }
      });
    }
  }

  pageIndexChange(value) {
    this.pageIndex = value;
    this.queryResultWithParam();
  }

  pageSizeChange(value) {
    this.pageSize = value;
    this.queryResultWithParam();
  }

  addRow() {
    this.addloading = true;
    this.showModal = true;
    this.isAdd = true;
    this.listForm.reset({
      // category: 1, flag: '0', invoiceflag: "0", type: "0", special: "N", iszerotaxinvter: "N", isupload: "N", isinvoice: "N", isdeduction: "N", calmethod: 0, expcategory: 0, datelevel: "Y"
      category: 1, flag: '0', invoiceflag: "0", type: "0", special: "N", iszerotaxinvter: "N", requiresinvoice: "N", requiresattachment: "N", isvatdeductable: "N", canbypassfinanceapproval: "N", calmethod: 0, extraformcode: 0, datelevel: "Y", requirespaperattachment: "N"
    });
    this.acctcodeList = [];
    this.expensecodeList = [];
    this.auditLevelCodeList = [];

    this.getCalmethodList();
    this.getExtraFormCodeList();
    this.GetExpenseList();

    // reset assign step
    this.assignStepList.clear();
    this.assignStepList.push(this.createStepForm(true));

  }

  showRow(item: DetailInfo) {
    this.isSpinning = true;
    this.isView = true;
    this.listForm.reset(item);

    this.getCalmethodList();
    this.getExtraFormCodeList();
    this.GetExpenseList();
    this.getAssignSteps(item);

    this.listForm.disable({ emitEvent: false });
    this.showModal = true;
    this.isSpinning = false;
  }

  editRow(item: DetailInfo) {
    this.isSpinning = true;
    this.isEdit = true;
    console.log("item: ", item);

    this.getCalmethodList();
    this.getExtraFormCodeList();
    this.GetExpenseList();
    this.listForm.reset(item);
    this.getAssignSteps(item);

    this.isSpinning = false;
    this.showModal = true;
  }

  // reset assign step and add assign step from item
  private getAssignSteps(item: DetailInfo) {
    if (!_.isEmpty(item.assignSteps)) {
      this.assignStepList.clear();

      item.assignSteps.forEach((step, index) => {
        step.approverList = _.map(step.approverList, (o) => {
          return {
            emplid: o.emplid,
            name: o.name,
            nameA: o.nameA,
            display: _.isEmpty(_.trim(o.nameA)) ? o.name : o.nameA,
          } as Approver;
        });

        this.assignStepList.push(this.createStepForm(index === 0, step, index));
      });
    } else {
      // reset assign step
      this.assignStepList.clear();
      this.assignStepList.push(this.createStepForm(true));
    }
  }

  deleteRow(item: DetailInfo) {
    this.isQueryLoading = true;
    this.Service.Delete(URLConst.DeleteExpSenario + `?id=${item.id}`, null).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        console.log("res.body.status: ", res.body.status);
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('operate-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.isQueryLoading = false;
    });
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
        companyList: paramValue.companyCode == '' ? this.companyList.filter(o => o != '') : [paramValue.companyCode],
        expcode: paramValue.expcode,
        senarioname: paramValue.senarioname,
      }
    }

    this.Service.Post(URLConst.QueryExpSenarioList, this.queryParam).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        this.total = res.body.total;
        let result: DetailInfo[] = [];

        console.log("res.body.data: ", res.body.data);

        res.body.data?.map(o => {
          result.push({
            id: o.id,
            companycategory: o.companycategory,
            category: o.category,
            expcode: o.expcode,
            expname: o.expname,
            senarioname: o.senarioname,
            keyword: o.keyword,
            auditlevelcode: o.auditlevelcode,
            acctcode: o.acctcode,
            description: o.description,
            //iszerotaxinvter: o.iszerotaxinvter,
            requirespaperattachment: o.requirespaperattachment ? ERSConstants.BooleanDisplayValue.Y : ERSConstants.BooleanDisplayValue.N,
            assignment: o.assignment,
            costcenter: o.costcenter,
            pjcode: o.pjcode,
            // filecategory: o.filecategory,
            // isupload: o.isupload,
            requiresinvoice: o.requiresinvoice ? ERSConstants.BooleanDisplayValue.Y : ERSConstants.BooleanDisplayValue.N,
            requiresattachment: o.requiresattachment ? ERSConstants.BooleanDisplayValue.Y : ERSConstants.BooleanDisplayValue.N,
            attachmentname: o.attachmentname,
            isvatdeductable: o.isvatdeductable ? ERSConstants.BooleanDisplayValue.Y : ERSConstants.BooleanDisplayValue.N,
            canbypassfinanceapproval: o.canbypassfinanceapproval ? ERSConstants.BooleanDisplayValue.Y : ERSConstants.BooleanDisplayValue.N,
            departday: o.departday,
            sectionday: o.sectionday,
            calmethod: o.calmethod,
            extraformcode: o.extraformcode,
            cuser: o.cuser,
            cdate: o.cdate == null ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
            muser: o.muser,
            mdate: o.mdate == null ? null : format(new Date(o.mdate), "yyyy/MM/dd"),
            datelevel: o.datelevel,
            authorized: o.authorized,
            authorizer: o.authorizer,
            sdate: o.sdate == null ? null : format(new Date(o.sdate), "yyyy/MM/dd"),
            edate: o.edate == null ? null : format(new Date(o.edate), "yyyy/MM/dd"),
            assignSteps: o.assignSteps,
            descriptionnotice: o.descriptionnotice,
            attachmentnotice: o.attachmentnotice,
          })
        });
        this.listTableData = result;
        this.showTable = true;
        this.isQueryLoading = false;
      }
    });
  }

  resetQueryForm(): void {
    this.queryForm.reset({
      companyCode: '',
      senarioname: null,
      expcode: null
    });
    this.showTable = false;
  }

  handleCancel(): void {
    this.showModal = false;
    this.addloading = false;
    this.isAdd = false;
    this.isEdit = false;
    if (this.isView) {
      this.isView = false;
      this.listForm.enable({ emitEvent: false });
    }
  }

  handleOk(): void {
    // 需要檢查assign step彼此之間是否有重複。若加簽名稱和加簽位置一樣，則視為重複。
    // check this.listForm.controls.assignSteps.value has duplicate with the same name and position in the list
    const assignSteps = this.listForm.controls.assignSteps.value;
    const isDuplicate = _.uniqWith(assignSteps, (a, b) => !_.isEmpty(a.name) && !_.isEmpty(b.name) && !_.isEmpty(a.position) && !_.isEmpty(b.position) && a.name === b.name && a.position === b.position).length !== assignSteps.length;
    if (isDuplicate) {
      this.message.error(this.translate.instant('assign-step-duplicate'));
      // this.message.error(this.translate.instant('duplicate-assign-step'));
      return;
    }

    if (this.isView) {
      this.isView = false;
      this.showModal = false;
      this.listForm.enable({ emitEvent: false });
      return;
    }
    this.isSpinning = true;
    this.isQueryLoading = true;
    if (!this.listForm.valid) {
      Object.values(this.listForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error(this.translate.instant('fill-in-form'));
      this.isSpinning = false;
      this.isQueryLoading = false;
      return;
    }
    let listFormData = this.listForm.getRawValue();
    var param = {
      Id: listFormData.id,
      companycategory: listFormData.companycategory,
      category: listFormData.category,
      expcode: listFormData.expcode,
      //expname: listFormData.expname,
      keyword: listFormData.keyword,
      auditlevelcode: listFormData.auditlevelcode,
      acctcode: listFormData.acctcode,
      requiresinvoice: listFormData.requiresinvoice,
      requiresattachment: listFormData.requiresattachment,
      attachmentname: listFormData.attachmentname,
      flag: listFormData.flag,
      type: listFormData.type,
      invter: listFormData.invter,
      senarioname: listFormData.senarioname,
      wording: listFormData.wording,
      special: listFormData.special,
      requirespaperattachment: listFormData.requirespaperattachment,
      assignment: listFormData.assignment,
      costcenter: listFormData.costcenter,
      pjcode: listFormData.pjcode,
      isvatdeductable: listFormData.isvatdeductable,
      canbypassfinanceapproval: listFormData.canbypassfinanceapproval,
      departday: Number(listFormData.departday),
      sectionday: Number(listFormData.sectionday),
      calmethod: Number(listFormData.calmethod),
      extraformcode: Number(listFormData.extraformcode),
      datelevel: listFormData.category == 'advance' ? listFormData.datelevel : '',
      sdate: listFormData.sdate == null ? null : format(new Date(listFormData.sdate), "yyyy-MM-dd"),
      edate: listFormData.edate == null ? null : format(new Date(listFormData.edate), "yyyy-MM-dd"),
      authorized: listFormData.authorized,
      authorizer: listFormData.authorizer,
      assignSteps: this.checkAssignSteps(listFormData.assignSteps),
      descriptionnotice: listFormData.descriptionnotice,
      attachmentnotice: listFormData.attachmentnotice,
    }
    if (this.isAdd) this.addItem(param);
    if (this.isEdit) this.editItem(param);
  }

  checkAssignSteps(assignSteps: AssignStep[]) {
    // 代表沒有填寫，則傳null到後端，代表不需要填寫；但後端仍需要做檢查並更新。
    if (assignSteps.length == 1 && _.isEmpty(assignSteps[0].name)) {
      return null;
    }
    return assignSteps;
  }

  addItem(params: any) {
    this.Service.Post(URLConst.AddEditExpSenario, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam(true);
          this.showModal = false;
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
    });
  }

  editItem(params: any) {
    this.Service.Put(URLConst.AddEditExpSenario, params).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          this.message.success(this.translate.instant('save-successfully'));
          this.queryResultWithParam();
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
      this.showModal = false;
    });
  }

  DownloadFile() {
    this.downloadloading = true;
    let paramValue = this.queryForm.getRawValue();
    const data = {
      companyList: paramValue.companyCode == '' ? this.companyList.filter(o => o != '') : [paramValue.companyCode],
      expcode: paramValue.expcode,
      senarioname: paramValue.senarioname,
    };
    this.Service.Download(URLConst.DownloadExpSenario, data, `Expense.xlsx`);
    this.downloadloading = false;
  }

  filters: UploadFilter[] = [
    {
      name: 'type',
      fn: (fileList: NzUploadFile[]) => {
        const filterFiles = fileList.filter(w =>
          ~['application/vnd.ms-excel'].indexOf(w.type) ||
          ~['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'].indexOf(w.type));
        if (filterFiles.length !== fileList.length) {
          this.message.error(this.translate.instant('file-format-erro-excel'));
          return filterFiles;
        }
        return fileList;
      }
    }
  ];

  handlePreview = async (file: NzUploadFile): Promise<void> => {
    const objectUrl = URL.createObjectURL(file.originFileObj);
    const a = document.createElement('a');
    document.body.appendChild(a);
    a.setAttribute('style', 'display:none');
    a.setAttribute('href', objectUrl);
    a.setAttribute('download', file.name);
    a.click();
    URL.revokeObjectURL(objectUrl);
  };

  clickBatchUpload(): void {
    this.batchUploadList = [];
    this.batchUploadModal = true;
  }

  beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {   // 上传批量文件
    return new Observable((observer: Observer<boolean>) => {
      let upload = !(this.batchUploadList.length > 0);
      if (!upload) this.message.error(this.translate.instant('can-upload-only-one-item'));
      observer.next(upload);
      observer.complete();
    });
  };

  uploadIcons: NzShowUploadList = {
    showPreviewIcon: true,
    showRemoveIcon: true,
    showDownloadIcon: false,
  };

  handleFileChange(info: NzUploadChangeParam): void {   // 批量上传
    let fileList = [...info.fileList];
    fileList = fileList.map(file => {
      file.status = "done";
      file.url = !file.url ? '...' : file.url;
      return file;
    });
    this.batchUploadList = fileList;
  }

  handleBatchUpload() {
    this.addloading = true;
    const formData = new FormData();
    this.batchUploadList.forEach((file: any) => { formData.append('excelFile', file.originFileObj); });
    this.Service.Post(URLConst.BatchUploadExpSenario, formData).subscribe((res) => {
      if (res && res.status === 200 && res.body != null) {
        if (res.body.status == 1) {
          let successList = res.body.data.filter(o => o.status == 1).map(o => { return o.data });
          let failedMsgs = res.body.data.filter(o => o.status != 1).map(o => { return { seq: o.seq, msg: o.uploadmsg } });
          let tips = '';
          if (failedMsgs.length > 0) {
            tips = '<p>' + this.commonSrv.FormatString(this.translate.instant('tips-success-upload-info'), successList.length.toString()) + '</p>';
            tips += '<p>' + this.commonSrv.FormatString(this.translate.instant('tips-failed-upload-info'), failedMsgs.length.toString()) + '</p>';
            let idx = 1;
            failedMsgs.map(o => {
              tips += `<p>${idx.toString()}. Row${o.seq}: ${o.msg}</p>`;
              idx++;
            });
            this.modal.info({
              nzTitle: this.translate.instant('tips'),
              nzContent: tips
            });
          }
          else {
            tips = this.commonSrv.FormatString(this.translate.instant('tips-success-upload-info'), successList.length.toString());
            this.message.success(this.translate.instant('save-successfully') + tips);
          }
          if (successList.length > 0) { this.queryResultWithParam(true); }
          this.batchUploadModal = false;
        }
        else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
      }
      else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
      this.addloading = false;
      this.isQueryLoading = false;
      this.isSpinning = false;
    });
  }

  onAdd() {
    this.assignStepList.push(this.createStepForm());
  }

  onDeleteItem(item) {
    this.assignStepList.removeAt(item);
  }

  createStepForm(isFirst: boolean = false, step?: AssignStep, index?: number) {
    if (!_.isEmpty(step?.approverList)) {
      this.listOfSelectedValue[index] = step.approverList;
    }

    return this.fb.group({
      name: [step?.name ?? null, isFirst ? null : this.requiredValidator],
      position: [step?.position ?? null, isFirst ? null : this.requiredValidator],
      approverList: [_.isEmpty(step?.approverList) ? null : [...step.approverList], isFirst ? null : this.requiredValidator],
    }, { validators: [formAtLeastRequiredCheckValidator()] });
  }

  compareFn = (o1: Approver, o2: Approver): boolean => o1 && o2 && o1.emplid === o2.emplid && o1.name === o2.name && o1.nameA === o2.nameA && o1.display === o2.display;

  getCategoryLocalizedName(value: string): string {
    // 查找匹配的类别项
    const item = this.senarioCategoryList.find(item => item.value === value);
    
    if (item) {
      // 使用 commonSrv 的多语言方法
      return this.commonSrv.getLocalizedName(
        item.name, 
        item.nameZhcn, 
        item.nameZhtw,
        item.nameEs,
        item.nameVn,
        item.nameCz
      );
    }
    
    // 如果找不到匹配项，返回原始值
    return value;
  }

  getExpenseCategoryLocalizedName(expcode: string): string {
    const item = this.expensecodeList.find(item => item.expcode === expcode);
    
    if (item) {
      // 使用 commonSrv 的多语言方法
      return this.commonSrv.getLocalizedName(
        item.expname, 
        item.expnamezhcn, 
        item.expnamezhtw,
        item.expnamees,
        item.expnamevn,
        item.expnamecz
      );
    }
    
    // 如果找不到匹配项，返回原始值
    return expcode;
  }

}
