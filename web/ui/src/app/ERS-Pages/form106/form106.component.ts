import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { format } from 'date-fns';
import { CommonService } from 'src/app/shared/service/common.service';
import { Guid } from 'guid-typescript';
import { Router } from '@angular/router';

@Component({
    selector: 'app-form106',
    templateUrl: './form106.component.html',
    styleUrls: ['./form106.component.scss']
})

export class Form106Component implements OnInit {
    navigationSubscription;

    //#region 参数
    queryForm: UntypedFormGroup;
    isSpinning = false;
    isQueryLoading = false;
    isFirstLoading: boolean = true;
    companyList: any[];
    //#endregion

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
    ) { }

    ngOnInit(): void {
        // if (!this.authService.CheckPermissionByRole('fin')) this.router.navigateByUrl(`pages/ers/permissiondenied`);
        this.isSpinning = true;
        this.isFirstLoading = false;
        this.queryForm = this.fb.group({
            startDate: [null, [this.startDateValidator]],
            endDate: [null, [this.endDateValidator]]
        });
        let today = new Date();
        let year = new Date(`${today.getFullYear()}-01-01`);
        this.queryForm.controls.startDate.setValue(year);
        this.queryForm.controls.endDate.setValue(today);
        this.getCompanyData();
        // this.isSpinning = false;
    }

    autoTips: Record<string, Record<string, string>> = {
        default: {
            required: this.translate.instant('can-not-be-null'),
            date: this.translate.instant('can-not-be-future-date'),
            startdate: this.translate.instant('can-not-later-than-end-date'),
            enddate: this.translate.instant('can-not-earlier-than-start-date'),
        }
    };


    startDateValidator = (control: FormControl): { [s: string]: boolean } => {
        if (!!control.value) {
            if (control.value > new Date())
                return { date: true, error: true };
            if (!!this.queryForm.controls.endDate.value && new Date(control.value).setHours(0, 0, 0, 0) > (new Date(this.queryForm.controls.endDate.value)).setHours(0, 0, 0, 0))
                return { startdate: true, error: true };
            if (!this.queryForm.controls.endDate.pristine) {
                this.queryForm.controls.endDate!.markAsPristine();
                this.queryForm.controls.endDate!.updateValueAndValidity();
            }
        }
    };
    endDateValidator = (control: FormControl): { [s: string]: boolean } => {
        if (!!control.value) {
            if (control.value > new Date())
                return { date: true, error: true };
            if (!!this.queryForm.controls.startDate.value && new Date(control.value).setHours(0, 0, 0, 0) < (new Date(this.queryForm.controls.startDate.value)).setHours(0, 0, 0, 0))
                return { enddate: true, error: true };
            if (!this.queryForm.controls.startDate.pristine) {
                this.queryForm.controls.startDate!.markAsPristine();
                this.queryForm.controls.startDate!.updateValueAndValidity();;
            }
        }
    };

    getCompanyData() {
        this.companyList = this.commonSrv.getCompanyAddOptionsByPermission;
        this.isSpinning = false;
    }

    queryResultWithParam() {
        this.isQueryLoading = true;
        if (!this.queryForm.valid) {
            Object.values(this.queryForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
            this.message.error(this.translate.instant('exist-invalid-field'));
            this.isQueryLoading = false;
            return;
        }
        let paramValue = this.queryForm.getRawValue();
        let param = {
            startDate: paramValue.startDate,
            endDate: paramValue.endDate,
            companyList: this.companyList
        }
        this.Service.Post(URLConst.GetOverspendReport, param).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    let header: any[] = [];
                    let body: any[] = [];
                    header = res.body.data.header;
                    body = res.body.data.body;
                    this.commonSrv.ExportDataToExcel(header, body, 'Report.xlsx');
                }
                else this.message.error(this.translate.instant('operate-failed') + res.body.message);
            } else this.message.error(this.translate.instant('operate-failed') + this.translate.instant('server-error'));
            this.isQueryLoading = false;
        });
    }
}