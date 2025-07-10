import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { PermissionTableColumn } from './classes/table-column';
import { AuthorityInfo } from './classes/data-item';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';

@Component({
    selector: 'app-permission',
    templateUrl: './permission.component.html',
    styleUrls: ['./permission.component.scss']
})

export class PermissionComponent implements OnInit {
    navigationSubscription;

    //#region 参数
    nzFilterOption = () => true;
    queryForm: FormGroup;
    companyList: any[] = [];
    roleList: any[] = [];
    emplidList: any[] = [];
    listTableColumn = PermissionTableColumn;
    listTableData: AuthorityInfo[] = [];
    isSpinning = false;
    userInfo: any;
    total: any;
    showTable = false;
    isQueryLoading = false;
    isFirstLoading: boolean = true;
    addloading = false;
    deleteloading = false;
    //#endregion

    constructor(
        private fb: FormBuilder,
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
        this.authService.CheckPermissionByRoleAndRedirect(['Finance']);
        this.isSpinning = true;
        this.isFirstLoading = false;
        this.queryForm = this.fb.group({
            company: [null, [Validators.required]],
            role: [null, [Validators.required]],
            emplidList: [[]],
        });
        this.getEmployeeInfo();
        this.getCompanyData();
        this.getRoleData();

        this.queryForm.valueChanges.subscribe(value => {
            this.showTable = false;
        });

        // this.queryForm.controls.emplidList.valueChanges.subscribe(value => {
        //     if (!!value) {
        //         this.emplidList = value.map(o => o.trim()).filter(o => o != '');
        //         return this.emplidList;
        //     }
        // })
    }

    autoTips: Record<string, Record<string, string>> = {
        default: {
            required: this.translate.instant('can-not-be-null'),
        }
    };

    getEmployeeInfo() {
        this.userInfo = this.commonSrv.getUserInfo;
    }

    getRoleData() {
        this.Service.doGet(URLConst.GetPermissionRoles, null).subscribe((res) => {
            if (res && res.status === 200) { this.roleList = res.body.data; }
            this.isSpinning = false;
        });
    }

    getCompanyData() {
        this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
    }

    // pageIndexChange(value) {
    //     this.pageIndex = value;
    //     this.queryResultWithParam();
    // }

    // pageSizeChange(value) {
    //     this.pageSize = value;
    //     this.queryResultWithParam();
    // }

    ResetQueryParams(): void {
        this.isSpinning = true;
        this.queryForm.reset({ emplidList: [] });
        this.emplidList = [];
        this.showTable = false;
        this.isSpinning = false;
    }

    queryResultWithParam() {
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
        this.emplidList = paramValue.emplidList.map(o => o.trim()).filter(o => o != '');
        this.queryForm.controls.emplidList.setValue(this.emplidList);
        let queryParam = {
            company: paramValue.company,
            role: paramValue.role,
            emplid: this.emplidList
        }
        this.Service.Post(URLConst.QueryPermissionList, queryParam).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.total = res.body.data == null ? 0 : res.body.data.length;
                    let result: AuthorityInfo[] = [];
                    res.body.data?.map(o => {
                        result.push({
                            roleId: o.roleId,
                            roleName: o.roleName,
                            emplid: o.subjectId,
                            name: o.userName,
                            userId: o.userId
                        });
                    });
                    this.listTableData = result;
                    this.showTable = true;
                }
                else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
            }
            else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
            this.isQueryLoading = false;
        });
    }

    grantPermission(): void {
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
        if (!this.queryForm.controls.emplidList.value || this.queryForm.controls.emplidList.value.length == 0) {
            this.message.error(this.translate.instant('input-emplid-required'));
            return;
        }
        this.isSpinning = true;
        this.addloading = true;
        let paramValue = this.queryForm.getRawValue();
        this.emplidList = paramValue.emplidList.map(o => o.trim()).filter(o => o != '');
        this.queryForm.controls.emplidList.setValue(this.emplidList);
        let params = {
            company: paramValue.company,
            role: paramValue.role,
            emplid: this.emplidList
        }
        this.Service.Post(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.MaintainPermission, params).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.message.success(this.translate.instant('operate-successfully'));
                    this.queryResultWithParam();
                }
                else this.message.error(this.translate.instant('operate-failed') + (res.body.message == null ? '' : res.body.message));
            }
            else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
            this.addloading = false;
            this.isSpinning = false;
        });

    }

    deleteRow(item: AuthorityInfo): void {
        this.deleteloading = true;
        let params = {
            company: this.queryForm.controls.company.value,
            roleId: item.roleId,
            role: this.queryForm.controls.role.value,
            userName: item.name,
            userId: item.userId,
            subjectId: item.emplid
        }
        this.Service.Delete(this.EnvironmentconfigService.authConfig.ersUrl + URLConst.MaintainPermission, params).subscribe((res) => {
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
    // checked = false;
    // indeterminate = false;
    // listOfCurrentPageData: AccountantInfo[] = [];
    // setOfCheckedId = new Set<string>();
    // updateCheckedSet(id: string, checked: boolean): void {
    //     if (checked) {
    //         this.setOfCheckedId.add(id);
    //     } else {
    //         this.setOfCheckedId.delete(id);
    //     }
    // }

    // onCurrentPageDataChange(listOfCurrentPageData: AccountantInfo[]): void {
    //     this.listOfCurrentPageData = listOfCurrentPageData;
    //     this.refreshCheckedStatus();
    // }

    // refreshCheckedStatus(): void {
    //     const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
    //     this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
    //     this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
    // }

    // onItemChecked(id: string, checked: boolean): void {
    //     this.updateCheckedSet(id, checked);
    //     this.refreshCheckedStatus();
    // }

    // onAllChecked(checked: boolean): void {
    //     this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
    //     this.refreshCheckedStatus();
    // }
}