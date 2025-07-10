import { Component, OnInit } from "@angular/core";
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from "@angular/forms";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { Guid } from "guid-typescript";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzModalService } from "ng-zorro-antd/modal";
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from "ng-zorro-antd/upload";
import { Observable, Observer } from "rxjs";
import { URLConst } from "src/app/shared/const/url.const";
// import { AuthService } from "src/app/shared/service/auth.service";
import { CommonService } from "src/app/shared/service/common.service";
import { WebApiService } from "src/app/shared/service/webapi.service";
import { InvoiceInfo } from "./classes/data-item";
import { DetailTableColumn } from "./classes/table-column";
import { format } from 'date-fns';

@Component({
    selector: 'app-invoice-query',
    templateUrl: './invoiceQuery.component.html',
    styleUrls: ['./invoiceQuery.component.scss']
})

export class InvoiceQueryComponent implements OnInit {
    nzFilterOption = () => true;
    screenHeight: any;
    screenWidth: any;
    minWidth: any;
    queryForm: UntypedFormGroup;
    infoForm: UntypedFormGroup;
    isSpinning = false;
    isSaveLoading = false;
    userInfo: any;
    invoiceItemList: InvoiceInfo[] = [];
    currList: any[] = [];
    invTypeList: any[] = [];
    verifyStateList: any[] = [];
    payStateList: any[] = [];
    isMobile: boolean;
    showItems: boolean = false;
    queryParam: any;
    total: any;
    pageIndex: number = 1;
    pageSize: number = 10;
    isQueryLoading: boolean = false;
    invDetailModal: boolean = false;
    editModal: boolean = false;
    shareModal: boolean = false;
    receiver: string = null;
    itemDetail: InvoiceInfo = null;
    detailListTableColumn = DetailTableColumn;
    paymentDic: { [key: string]: string } = { 'requested': this.translate.instant('have-been-payout'), 'unrequested': this.translate.instant('to-be-payout'), 'recorded': this.translate.instant('funds-recorded') }
    // detailFooter:[ { label: 'Close', onClick: () => {this.invDetailModal = false;} }, {label: 'Confirm',type: 'primary',onClick: () => this.modal.confirm({ nzTitle: 'Confirm Modal Title', nzContent: 'Confirm Modal Content' })}]
    drawerVisible: boolean = false;
    frameSrc: SafeResourceUrl;
    previewVisible = false;
    previewImage: string | undefined = '';
    fileTypeDictionary: { [key: string]: string } = { ['jpg']: 'image/jpeg', ['jpeg']: 'image/jpeg', ['png']: 'image/png', ['bmp']: 'image/bmp', ['pdf']: 'application/pdf' }

    constructor(
        private fb: UntypedFormBuilder,
        private Service: WebApiService,
        // private authService: AuthService,
        private modal: NzModalService,
        public translate: TranslateService,
        private message: NzMessageService,
        private router: Router,
        private actRoute: ActivatedRoute,
        private commonSrv: CommonService,
        public domSanitizer: DomSanitizer,
    ) { }

    ngOnInit(): void {
        this.isSpinning = true;
        this.minWidth = window.innerWidth < 300 ? window.innerWidth * 0.9 + 'px' : (window.innerWidth > 580 ? '580px' : '300px');
        this.queryForm = this.fb.group({
            startDate: [null, [this.startDateValidator]],
            endDate: [null, [this.endDateValidator]],
            invno: [null],
            invtype: [null],
            verifytype: [null],
            paytype: [null],
        });
        this.infoForm = this.fb.group({
            id: [null],
            invcode: [{ value: null, disabled: true }],
            invno: [{ value: null, disabled: true }],
            invdate: [null, [this.dateValidator]],
            invtype: [null, [Validators.required]],
            curr: [null, [Validators.required]],
            amount: [null, [Validators.required]]
        });
        let today = new Date();
        let year = new Date(`${today.getFullYear()}-01-01`);
        this.queryForm.controls.startDate.setValue(year);
        this.queryForm.controls.endDate.setValue(today);
        this.isMobile = this.commonSrv.CheckIsMobile();
        this.getEmployeeInfo();
        this.getPayTypeList();
        // this.getVerifyTypeList();
        this.queryForm.valueChanges.subscribe(value => {
            this.showItems = false;
        });
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

    dateValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
        if (!!control.value && control.value > new Date()) {
            return { date: true, error: true };
        }
    };

    getEmployeeInfo() {
        this.userInfo = this.commonSrv.getUserInfo;
        this.isSpinning = false;
    }

    getCurrency() {
        if (this.currList.length == 0) {
            this.isSpinning = true;
            this.Service.doGet(URLConst.GetCurrencyList, null).subscribe((res) => {
                if (res && res.status === 200) { this.currList = res.body.map(item => item.currency); }
                else { this.message.error(res.message); }
                this.isSpinning = false;
            });
        }
    }

    getInvTypeList() {
        if (this.invTypeList.length == 0) {
            this.isSpinning = true;
            this.Service.doGet(URLConst.GetInvTypeByCompany + `?company=${this.userInfo.company}`, null).subscribe((res) => {
                if (res && res.status === 200) {
                    this.invTypeList = res.body.data?.map(o => { return { invCode: o.invcode, invType: o.invtype, company: o.company } });
                }
                else { this.message.error(res.message); }
                this.isSpinning = false;
            });
        }
    }

    getPayTypeList() {
        this.Service.doGet(URLConst.GetPayTypeList, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body) {
                if (res.body.status == 1) {
                    this.payStateList = res.body.data.map(o => { return { code: o, name: this.paymentDic[o] } });
                }
                else { this.message.error(res.body.message, { nzDuration: 6000 }); }
            }
            else { this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 }); }
            this.isSpinning = false;
        });
    }

    // getVerifyTypeList() {
    //     this.Service.doGet(URLConst.GetPayTypeList, null).subscribe((res) => {
    //         if (res && res.status === 200) {
    //             this.verifyStateList = res.body.data;
    //         }
    //         else { this.message.error(res.message); }
    //         this.isSpinning = false;
    //     });
    // }

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
                startdate: paramValue.startDate,
                enddate: paramValue.endDate,
                emplid: this.userInfo.emplid,
                invno: paramValue.invno == null ? null : paramValue.invno.trim(),
                invtype: paramValue.invtype,
                verifytype: paramValue.verifytype,
                paytype: paramValue.paytype,
                isphone: this.isMobile,
            }
        }
        this.Service.Post(URLConst.QueryInvoice, this.queryParam).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                this.total = res.body.total;
                let result: InvoiceInfo[] = [];
                res.body.data?.map(o => {
                    result.push({
                        id: o.id,
                        buyername: o.buyername,
                        sellername: o.sellername,
                        sellertaxid: o.sellertaxid,
                        invcode: o.invcode,
                        invno: o.invno,
                        invtype: o.invtype,
                        invdate: !o.invdate ? null : format(new Date(o.invdate), "yyyy/MM/dd"),
                        untaxamount: o.untaxamount,
                        taxamount: o.taxamount,
                        amount: o.amount,
                        taxrate: o.taxrate,
                        verifytype: paramValue.verifytype,
                        paytype: o.paytype,
                        abnormalreason: o.abnormalreason,
                        rno: o.rno,
                        filepath: o.filepath,
                        emplid: o.emplid,
                        cuser: o.cuser,
                        isfill: o.isfill,
                        url: o.url,
                        curr: o.curr,
                        cdate: !o.cdate ? null : format(new Date(o.cdate), "yyyy/MM/dd"),
                        buyertaxid: o.buyertaxid,
                        remark: o.remark
                    })
                });
                this.invoiceItemList = result;
                this.showItems = true;
                this.isQueryLoading = false;
                this.isSaveLoading = false;
                this.isSpinning = false;
            }
        });
    }

    checkDetail(item: any) {
        this.invDetailModal = true;
        this.itemDetail = item;
    }

    // previewInv() {   // 新窗口预览
    //     let url = this.itemDetail.url;
    //     let name = this.itemDetail.invno;
    //     const img = new window.Image();
    //     img.src = url;
    //     const newWin = window.open('');
    //     newWin.document.write(img.outerHTML);
    //     newWin.document.title = name;
    //     newWin.document.close();
    // }

    async previewInv() {
        if (!this.itemDetail.filepath) {
            this.message.info(this.translate.instant('tips-no-electronic-file'), { nzDuration: 6000 });
            return;
        }
        let fileName = this.itemDetail.filepath.split('/')?.pop();
        let fileType = this.fileTypeDictionary[fileName.split('.')?.pop()];
        let url = this.itemDetail.url;

        if (fileType.indexOf('image') !== -1) {
            this.previewImage = url;
            this.previewVisible = true;
        }
        else {
            // this.frameSrc = this.domSanitizer.bypassSecurityTrustResourceUrl(url);
            this.frameSrc = await this.commonSrv.getFileData(url, 'invoice.pdf', 'application/pdf');
            this.drawerVisible = true;
        }
    }

    editRow(item: any = null) {
        if (!!item) {
            this.itemDetail = item;
        }
        if (this.itemDetail.paytype != "unrequested" || !this.itemDetail.isfill) {
            this.message.error(this.translate.instant('tips-item-cannot-edit'));
            return;
        }
        this.getInvTypeList();
        this.getCurrency();
        this.infoForm.reset(this.itemDetail);
        this.editModal = true;
    }

    handleEditOk() {
        this.isSpinning = true;
        this.isSaveLoading = true;
        if (!this.infoForm.valid) {
            Object.values(this.infoForm.controls).forEach(control => {
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
        let formData = this.infoForm.getRawValue();
        if (!formData.amount || formData.amount == 0) {
            this.message.error(this.translate.instant('amount-zero-error'));
            this.isSpinning = false;
            this.isSaveLoading = false;
            return;
        }
        this.Service.Put(URLConst.MaintainInvoice, formData).subscribe(res => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.message.success(res.body.message);
                    if (this.isMobile) {
                        this.itemDetail.invcode = formData.invcode;
                        this.itemDetail.invno = formData.invno;
                        this.itemDetail.invdate = !formData.invdate ? null : format(new Date(formData.invdate), "yyyy/MM/dd");
                        this.itemDetail.invtype = formData.invtype;
                        this.itemDetail.curr = formData.curr;
                        this.itemDetail.amount = formData.amount;
                    }
                    else { this.queryResultWithParam(); }
                    this.editModal = false;
                } else {
                    this.message.error(res.body.message);
                }
                this.isSpinning = false;
                this.isSaveLoading = false;
            }
        });

    }

    shareInv(item: any = null) {
        if (!!item) {
            this.itemDetail = item;
        }
        if (this.itemDetail.paytype != "unrequested") {
            this.message.error(this.translate.instant('tips-item-cannot-operate'));
            return;
        }
        this.shareModal = true;
        this.receiver = null;
    }

    handleShare() {
        if (!this.receiver) {
            this.message.error(this.translate.instant('fill-in-form'));
            return;
        }
        this.isSpinning = true;
        this.isSaveLoading = true;
        this.receiver = this.receiver.trim();
        let params = {
            id: this.itemDetail.id,
            emplid: this.receiver
        }
        this.Service.Post(URLConst.ShareInvoice, params).subscribe(res => {
            if (res && res.status === 200 && !!res.body) {
                if (res.body.status == 1) {
                    this.message.success(res.body.message);
                    this.queryResultWithParam();
                    this.invDetailModal = false;
                    this.shareModal = false;
                } else {
                    this.message.error(res.body.message);
                    this.isSaveLoading = false;
                    this.isSpinning = false;
                }
            }
        });
    }

    deleteRow(item: any = null) {
        if (!!item) {
            this.itemDetail = item;
        }
        if (this.itemDetail.paytype == "requested") {
            this.message.error(this.translate.instant('tips-item-cannot-operate'));
            return;
        }
        this.isSpinning = true;
        this.Service.Post(URLConst.DeleteInvoice + `?id=${this.itemDetail.id}`, null).subscribe(res => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.message.success(this.translate.instant('tips-delete-success'));
                    this.invDetailModal = false;
                    this.queryResultWithParam();
                } else {
                    this.message.error(res.body.message);
                }
            } else {
                this.message.error(this.translate.instant('server-error'));
            }
            this.isSpinning = false;
        });
    }

}