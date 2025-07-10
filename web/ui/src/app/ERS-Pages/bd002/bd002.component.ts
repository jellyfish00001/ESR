import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, FormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BDInfoTableColumn } from './classes/table-column';
import { AccountantInfo } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';

@Component({
    selector: 'app-bd002',
    templateUrl: './bd002.component.html',
    styleUrls: ['./bd002.component.scss']
})

export class BD002Component implements OnInit {
    navigationSubscription;

    //#region 参数
    nzFilterOption = () => true;
    screenWidth: any;
    queryForm: UntypedFormGroup;
    listForm: UntypedFormGroup;
    queryCompanyList: any[] = [];
    companyList: any[] = [];
    employeeList: any[] = [];
    showModal: boolean = false;
    isSaveLoading: boolean = false;
    listTableColumn = BDInfoTableColumn;
    listTableData: AccountantInfo[] = [];
    isSpinning = false;
    queryParam: any;
    userInfo: any;
    total: any;
    pageIndex: number = 1;
    pageSize: number = 10;
    showTable = false;
    isQueryLoading = false;
    isFirstLoading: boolean = true;
    addloading = false;
    editloading = false;
    deleteloading = false;
    categoryDic: { [key: number]: string } = { 0: this.translate.instant('options-reimbursement-advance'), 1: this.translate.instant('options-payroll') }
    signStepDic: { [key: number]: string } = { 0: this.translate.instant('rv1'), 1: this.translate.instant('rv2') }
    //#endregion

    constructor(
        private fb: UntypedFormBuilder,
        private Service: WebApiService,
        private authService: AuthService,
        private modal: NzModalService,
        public translate: TranslateService,
        private EnvironmentconfigService: EnvironmentconfigService,
        private message: NzMessageService,
        private commonSrv: CommonService,
        private router: Router,
    ) { }

    ngOnInit(): void {
        this.authService.CheckPermissionByRoleAndRedirect(['Admin','FinanceAdmin']);
        this.isSpinning = true;
        this.isFirstLoading = false;
        this.screenWidth = window.innerWidth < 580 ? window.innerWidth * 0.9 + 'px' : '580px';
        this.queryForm = this.fb.group({
            category: [null],
            signStep: [null],
            company: [""]
        });

        this.listForm = this.fb.group({
            id: [null],
            company: [null, [Validators.required]],
            category: [null, [Validators.required]],
            signStep: [null, [Validators.required]],
            plant: [null],
            approver: [null, [Validators.required]]
        });
        this.getEmployeeInfo();
        this.getCompanyData();

        this.queryForm.valueChanges.subscribe(value => {
            this.showTable = false;
        });
    }

    autoTips: Record<string, Record<string, string>> = {
        default: {
            required: this.translate.instant('can-not-be-null'),
        }
    };

    getEmployeeInfo() {
        this.userInfo = this.commonSrv.getUserInfo;
    }

    getCompanyData() {
      this.commonSrv.getOthersCompanys().subscribe(res => {
        this.companyList = res;
        this.queryCompanyList = res;
        this.isSpinning = false;
      });
    }

    getEmployeeList(value) {
        this.commonSrv.getEmployeeList(value).subscribe(res => this.employeeList = res);
    }

    getEmployeeLabel(value) {
      var label = value;
      this.commonSrv.getEmployeeList(value.split("/")[0].trim()).subscribe(res => this.employeeList = res);

      if(this.employeeList.length>0){
        label = this.employeeList[0].label
      }
      return label;
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
        this.isQueryLoading = true;
        let paramValue = this.queryForm.getRawValue();
        if (initial) {
            this.pageIndex = 1;
            this.pageSize = 10;
            this.setOfCheckedId.clear();
        }
        console.log(this.companyList);
        this.queryParam = {
            pageIndex: this.pageIndex,
            pageSize: this.pageSize,
            data: {
                id: null,
                companyList: paramValue.company == '' ? this.companyList.filter((o) => o != '') : [paramValue.company],
                company_code: paramValue.companyCode,
                category: paramValue.category==null?-1:paramValue.category,
                signStep: paramValue.signStep==null?-1:paramValue.signStep,
            }
        }

        this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.QueryBd002, this.queryParam).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                this.total = res.body.total;
                let result: AccountantInfo[] = [];
                res.body.data?.map(o => {
                    result.push({
                        id: o.id,
                        company: o.company,
                        category: o.category,
                        signStep: o.rv1 ? "0" : "1",//this.translate.instant("rv1") : this.translate.instant("rv2"),
                        approver: o.rv1 ? o.rv1 : o.rv2,
                        companyCode: o.company_code,
                        plant: o.plant,
                        accountant1: o.rv1,
                        accountant2: o.rv2,
                        accountant3: o.rv3,
                        creator: o.cuser,
                        createDate: o.cdate == null ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
                        updateUser: o.muser,
                        updateDate: o.mdate == null ? null : format(new Date(o.mdate), "yyyy/MM/dd HH:mm:ss"),
                        disabled: false,
                    })
                });
                this.listTableData = result;
                this.showTable = true;
                this.isQueryLoading = false;
            }
        });
    }

    addRow(): void {
        this.addloading = true;
        this.listForm.reset();
        this.showModal = true;
    }

    editRow(item): void {
        this.isSpinning = true;
        this.editloading = true;
        //if (item.accountant2 == '*') item.accountant2 = null;
        //if (item.accountant3 == '*') item.accountant3 = null;
        //item.signStep = item.signStep===this.translate.instant("rv1") ? 0 : 1;
        this.listForm.reset(item);

        this.employeeList = [];
        this.employeeList.push({ emplid: item.accountant1, name: '', label: item.accountant1 });
        //if (!!item.accountant2) this.employeeList.push({ emplid: item.accountant2, name: '', label: item.accountant2 });
        //if (!!item.accountant3) this.employeeList.push({ emplid: item.accountant3, name: '', label: item.accountant3 });
        this.isSpinning = false;
        this.showModal = true;
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
        let listFormData = this.listForm.getRawValue();
        if (listFormData.category == 0 && (listFormData.plant == null || listFormData.plant == '')){
            this.message.error('Please input plant');
            this.isSpinning = false;
            this.isSaveLoading = false;
            return;
        }

        let params = {
            Id: listFormData.id,
            company: listFormData.company,
            category: listFormData.category,
            plant: listFormData.plant,
            rv1: listFormData.signStep == 0 ? listFormData.approver.split("/")[0].trim():null,
            rv2: listFormData.signStep == 1 ? listFormData.approver.split("/")[0].trim():null,
            //rv3: listFormData.accountant3,
            // cuser: this.userInfo.emplid,
            // muser: this.userInfo.emplid,
            // cdate: new Date(),
            // mdate: new Date(),
        };

        if (!this.editloading) this.addItem(params);
        else this.editItem(params);
    }

    addItem(params: any) {
        params.Id = null;
        this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.MaintainBd002, params).subscribe((res) => {
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
            this.isSaveLoading = false;
            this.isSpinning = false;
        });
    }

    editItem(params: any) {
        this.Service.Put(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.MaintainBd002, params).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.message.success(this.translate.instant('save-successfully'));
                    this.queryResultWithParam();
                }
                else this.message.error(this.translate.instant('save-failed') + (res.body.message == null ? '' : res.body.message));
            }
            else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
            this.editloading = false;
            this.isSaveLoading = false;
            this.isSpinning = false;
            this.showModal = false;
        });
    }

    handleCancel(): void {
        this.showModal = false;
        this.addloading = false;
        this.editloading = false;
    }

    deleteRow(id: string = null): void {
        this.deleteloading = true;
        let params: any = [];
        if (id == null) {   //多选操作
            params = Array.from(this.setOfCheckedId);
            this.setOfCheckedId.clear();
        }
        else {
            params.push(id);
            this.setOfCheckedId.delete(id);
        }
        this.Service.Delete(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.MaintainBd002, params).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.message.success(this.translate.instant('operate-successfully'));
                    this.queryResultWithParam();
                }
                else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
            }
            else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
            this.deleteloading = false;
        });
    }

    ////////带选择框表
    checked = false;
    indeterminate = false;
    listOfCurrentPageData: AccountantInfo[] = [];
    setOfCheckedId = new Set<string>();
    updateCheckedSet(id: string, checked: boolean): void {
        if (checked) {
            this.setOfCheckedId.add(id);
        } else {
            this.setOfCheckedId.delete(id);
        }
    }

    onCurrentPageDataChange(listOfCurrentPageData: AccountantInfo[]): void {
        this.listOfCurrentPageData = listOfCurrentPageData;
        this.refreshCheckedStatus();
    }

    refreshCheckedStatus(): void {
        const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
        this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
        this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
    }

    onItemChecked(id: string, checked: boolean): void {
        this.updateCheckedSet(id, checked);
        this.refreshCheckedStatus();
    }

    onAllChecked(checked: boolean): void {
        this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
        this.refreshCheckedStatus();
    }
}
