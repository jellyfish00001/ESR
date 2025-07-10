import { Component, OnInit } from "@angular/core";
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from "@angular/forms";
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

@Component({
    selector: 'app-invoice-upload',
    templateUrl: './invoiceUpload.component.html',
    styleUrls: ['./invoiceUpload.component.scss']
})

export class InvoiceUploadComponent implements OnInit {
    nzFilterOption = () => true;
    minWidth: any;
    infoForm: UntypedFormGroup;
    isSpinning = false;
    isSaveLoading = false;
    addloading = false;
    remarkloading = false;
    uploadloading = false;
    loading = false;
    userInfo: any;
    invoiceItemList: InvoiceInfo[] = [];
    currList: any[] = [];
    invTypeList: any[] = [];
    company: string;
    // setOfCheckedId = new Set<number>();
    invoiceListState: string;
    uploadModal: boolean = false;
    uploadMode: string = ""; // "number"/"url"/"file"/"input"
    elecInvoiceNo: string = null;
    elecFileUrl: string = null;
    remark: string = "";
    invoiceFile: any[] = [];
    detailModal: boolean = false;
    remarkModal: boolean = false;

    drawerVisible: boolean = false;
    frameSrc: SafeResourceUrl;
    previewVisible = false;
    previewImage: string | undefined = '';
    uploadedCount: number = 0;
    uploadModeDic: { [key: string]: number } = { 'number': 1, 'url': 2, 'file': 3, 'input': 4 };
    isModalSpinning = false;
    spinningText = 'Loading...';

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
        this.infoForm = this.fb.group({
            index: [null],
            invCode: [null],
            invNumber: [null],
            salesname: [null],
            salestaxno: [null],
            invDate: [null, [this.dateValidator]],
            invType: [null, [Validators.required]],
            curr: [null, [Validators.required]],
            invAmt: [null, [Validators.required]],
            fileList: [null]
        });
        this.invoiceListState = this.commonSrv.FormatString(this.translate.instant('tips.upload-invoice-state'), this.invoiceItemList.length.toString(), this.uploadedCount.toString());
        this.isSpinning = false;
        this.company = this.commonSrv.getUserCompany;
    }

    autoTips: Record<string, Record<string, string>> = {
        default: {
            required: this.translate.instant('can-not-be-null'),
            date: this.translate.instant('can-not-be-future-date')
        }
    };
    dateValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
        if (!!control.value && control.value > new Date()) {
            return { date: true, error: true };
        }
    };

    getEmployeeInfo() {
        this.userInfo = this.commonSrv.getUserInfo;
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

    showUploadModal() {
        if (!this.userInfo) { this.getEmployeeInfo(); }
        this.uploadModal = true;
        this.uploadMode = "";
    }

    chooseUploadMode(mode: string) {
        this.uploadMode = mode;
        this.resetModalData();
        if (mode == 'input') {
            this.getCurrency();
            this.getInvTypeList();
            this.infoForm.reset({ curr: this.userInfo.curr });
            this.infoForm.get('fileList')!.setValidators(Validators.required);
            this.infoForm.get('fileList')!.markAsDirty();
            this.infoForm.get('fileList')!.updateValueAndValidity();
        }
    }

    resetModalData() {
        this.elecInvoiceNo = null;
        this.elecFileUrl = null;
        this.invoiceFile = [];
    }

    handleOk() {
        // this.isSpinning = true;
        this.isSaveLoading = true;
        this.isModalSpinning = true;
        if (this.uploadMode == 'number') { this.handleNumberOk(); }
        if (this.uploadMode == 'url') { this.handleUrlOk(); }
        if (this.uploadMode == 'file') { this.handleFileOk(); }
        if (this.uploadMode == 'input') { this.handleInputOk(); }
    }

    loadData(data: any): any {
        let itemList = [];
        let expMsg = [];
        let idx = 0;
        data.map(o => {
            if (o.paymentStat) {
                let item = o;
                item['index'] = Guid.create().toString();
                item['invName'] = this.uploadMode == 'number' ? this.elecInvoiceNo : this.invoiceFile.filter(f => f.uid == o.flag)[0]?.name;
                item['invCode'] = o.invcode;
                item['invNumber'] = o.invno;
                item['invDate'] = !o.invdate ? null : new Date(o.invdate);
                item['invType'] = o.invtype;
                item['invDesc'] = o.invdesc;
                item['salesname'] = o.salesname;
                item['salestaxno'] = o.salestaxno;
                item['curr'] = o.curr;
                item['invAmt'] = o.oamount == 0 ? null : o.oamount;
                item['file'] = this.uploadMode == 'number' ? null : this.invoiceFile.filter(f => f.uid == o.flag)[0];
                item['remark'] = null;
                item['expMsg'] = o.expdesc;
                item['expdesc'] = o.expdesc;
                item['state'] = o.isfill ? '0' : '1';
                item['uploadmethod'] = this.uploadModeDic[this.uploadMode];
                item['uploadMode'] = this.uploadMode;
                itemList.push(item);
            }
            else {
                idx++;
                // let msg = `<p>${idx}. <b>[${this.uploadMode == 'number' ? this.elecInvoiceNo : this.invoiceFile.filter(f => f.uid == o.flag)[0]?.name}]:</b><br>&nbsp;&nbsp;&nbsp;&nbsp;Invoice No.${o.invno} ${this.translate.instant('add-failed')},`;
                let msg = `<p>${idx}. Invoice No.${o.invno ?? ''} ${this.translate.instant('add-failed')},`;
                if (!o.paymentStat) { msg += o.expinfo + ';' }
                if (!o.existautopa) { msg += '<br>' + o.msg + ';' }
                msg += '</p>'
                expMsg.push(msg);
            }
        });
        if (itemList.length > 0) {
            this.invoiceItemList = this.invoiceItemList.concat(itemList);
            this.invoiceListState = this.commonSrv.FormatString(this.translate.instant('tips.upload-invoice-state'), this.invoiceItemList.length.toString(), this.uploadedCount.toString());
            this.uploadModal = false;
        }
        if (expMsg.length > 0) {
            let tips = "";
            expMsg.map(o => tips += o);
            this.modal.info({
                nzTitle: this.translate.instant('tips'),
                nzContent: tips
            });
        }
        let result = itemList.map(o => { return o.index });
        return result;
    }

    handleNumberOk() {
        if (!this.elecInvoiceNo || (this.elecInvoiceNo.trim()).length != 20) {
            this.message.error(this.translate.instant('invoice-number-length-not-satisfied'));
            this.isSaveLoading = false;
            this.isSpinning = false;
            this.isModalSpinning = false;
            return;
        }
        this.elecInvoiceNo = this.elecInvoiceNo.trim();
        let existRepeat = this.invoiceItemList.filter(o => o.invName.indexOf(this.elecInvoiceNo) !== -1).length > 0;
        if (existRepeat) {
            this.message.error(this.translate.instant('invoice-list-repeat-item') + `Invoice: ${this.elecInvoiceNo}`, { nzDuration: 6000 });
            this.isSaveLoading = false;
            this.isSpinning = false;
            this.isModalSpinning = false;
            return;
        }
        this.Service.Post(URLConst.ReadInvoiceByNum + `?invno=${[this.elecInvoiceNo]}`, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body?.data) {
                if (res.body.data.length > 0) { this.loadData(res.body.data); }
                else { this.message.error(res.body.message); }
            }
            else { this.message.error(res.message); }
            this.isSaveLoading = false;
            this.isSpinning = false;
            this.isModalSpinning = false;
        });
    }

    fileTypeDictionary: { [key: string]: string } = { ['jpg']: 'image/jpeg', ['jpeg']: 'image/jpeg', ['png']: 'image/png', ['bmp']: 'image/bmp', ['pdf']: 'application/pdf' }
    handleUrlOk() {
        if (this.elecFileUrl.length == 0) {
            this.message.error(this.translate.instant('fill-in-form'));
        } else {
            // let fileName = this.elecFileUrl.match(/(?<=\/).*(?=\?)/g)
            let fileName = this.elecFileUrl.split('?')[0];
            let startIdx = fileName.lastIndexOf('/');
            if (fileName.length < startIdx + 2) {
                this.message.error(this.translate.instant('tips-invalid-url'));
            }
            else {
                fileName = fileName.substring(startIdx + 1);
                let fileData = fileName.split('.');
                if (fileData.length == 2 && this.fileTypeDictionary.hasOwnProperty(fileData[1])) {
                    let fileType = fileData[1]?.toLowerCase();
                    this.Service.Post(URLConst.CheckUrlValid, { url: this.elecFileUrl }).subscribe((res) => {
                        if (res && res.status === 200 && res.body?.data) {
                            this.Service.doPost(URLConst.GetFileByUrl, { url: this.elecFileUrl }, true).subscribe(res => {
                                if (!!res) {
                                    let safeUrl = this.elecFileUrl;
                                    if (fileType == 'pdf') { safeUrl = 'assets/image/pdf.png' }
                                    let file = { uid: Guid.create().toString(), name: fileName, originFileObj: res, safeUrl: safeUrl };
                                    this.invoiceFile = [];
                                    this.invoiceFile.push(file);
                                    this.handleFileOk();
                                }
                            })
                        }
                        else {
                            this.message.error(res.body?.message ?? res.message);
                            this.isSaveLoading = false;
                            this.isSpinning = false;
                            this.isModalSpinning = false;
                        }
                    })
                    // let safeUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(this.elecFileUrl);
                    // this.drawerVisible = true;
                    // this.frameSrc = safeUrl;
                    // let file = await this.commonSrv.getFileData(safeUrl, fileName, this.fileTypeDictionary[fileType]);
                    // // FileUtils
                    // if (!!file) {
                    //     file['uid'] = Guid.create().toString();
                    //     this.invoiceFile.push(file);
                    //     this.handleFileOk();
                    // }
                    // else { this.message.error('Convert file failed!') }
                }
                else {
                    this.message.error(this.translate.instant('tips-invalid-url'));
                    this.isSaveLoading = false;
                    this.isSpinning = false;
                    this.isModalSpinning = false;
                }
            }
        }
    }

    handleFileOk() {
        this.isModalSpinning = true;
        if (this.invoiceFile.length == 0) {
            this.message.error(this.translate.instant('fill-in-form'));
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }

        if (this.uploadMode == 'file') {
            this.spinningText = this.commonSrv.FormatString(this.translate.instant('add-progress-loading'), '0', this.invoiceFile.length.toString());
        }
        let batchCount = 3;
        let startCursor = 0;
        let endCursor = 0;
        let groupCount = Math.ceil(Number(this.invoiceFile.length / batchCount));
        let groupNum = 0;
        let resultList = [];
        while (endCursor < this.invoiceFile.length) {
            startCursor = endCursor;
            endCursor = this.invoiceFile.length > (endCursor + batchCount) ? (endCursor + batchCount) : this.invoiceFile.length;
            const formData = new FormData();
            let partfileList = [];
            partfileList = this.invoiceFile.slice(startCursor, endCursor);
            partfileList.forEach((file: any) => {
                formData.append(file.uid, file.originFileObj);
            });
            this.Service.Post(URLConst.ReadInvoiceByFile, formData).subscribe((res) => {
                groupNum++;
                if (res && res.status === 200 && !!res.body?.data && res.body.data.length > 0) {
                    resultList = resultList.concat(res.body.data);
                }
                else { this.message.error(res.message ?? res.body.message); }
                let uploaded = groupNum * batchCount;
                uploaded = uploaded > this.invoiceFile.length ? this.invoiceFile.length : uploaded;
                let remains = this.invoiceFile.length - uploaded;
                if (this.uploadMode == 'file') { this.spinningText = this.commonSrv.FormatString(this.translate.instant('add-progress-loading'), (uploaded).toFixed(0), (remains).toFixed(0)); }
                if (groupNum == groupCount) {
                    this.showReadTips(resultList);
                }
            });
        }
    }

    showReadTips(resultList: any) {
        if (resultList.length > 0) {
            let existRepeatItems = [];
            let existDuplicationItems = [];
            let list = resultList;
            if (list.length > 0) {
                resultList.map(o => {
                    let duplicationItem = [];
                    duplicationItem = list.filter(f => !!f.invcode && !!f.invno && f.invcode == o.invcode && f.invno == o.invno);
                    if (duplicationItem.length > 1) {
                        existDuplicationItems.push(duplicationItem.shift());
                        duplicationItem.map(i => {
                            let idx = list.indexOf(i);
                            list.splice(idx, 1);
                        });
                    }
                });
            }
            if (this.invoiceItemList.length > 0) {
                resultList.map(o => {
                    if (!o.invcode && !o.invno) {
                        let repeatItem = [];
                        let objectName = this.invoiceFile.filter(i => i.uid == o.flag)[0]?.name;
                        repeatItem = this.invoiceItemList.filter(f => f.invName == objectName).map(f => { return { objectName: objectName, invName: f.invName, invcode: f.invcode, invno: f.invno } });
                        existRepeatItems = existRepeatItems.concat(repeatItem);
                    }
                    else {
                        let repeatItem = [];
                        let objectName = this.invoiceFile.filter(i => i.uid == o.flag)[0]?.name;
                        repeatItem = this.invoiceItemList.filter(f => f.invcode == o.invcode && f.invno == o.invno).map(f => { return { objectName: objectName, invName: f.invName, invcode: f.invcode, invno: f.invno } });
                        existRepeatItems = existRepeatItems.concat(repeatItem);
                    }
                });
            }
            if (existRepeatItems.length == 0 && existDuplicationItems.length == 0) { this.loadData(resultList) }
            if (existRepeatItems.length > 0) {
                let repeatMsg = this.translate.instant('invoice-list-repeat-items') + '<br>';
                let idx = 1;
                existRepeatItems.map(o => {
                    if (!o.invno && !o.invcode) {
                        // repeatMsg += `<p>${idx}. <b>[${o.objectName}]:</b> [Invoice Name]${o.invName},[Invoice No]${o.invno}, [Invoice Code]${o.invcode}</p>`;
                        repeatMsg += `<p>${idx}. ${this.commonSrv.FormatString(this.translate.instant('tips-invoice-repeat-name'), o.objectName)}</p>`;
                    }
                    else {
                        repeatMsg += `<p>${idx}. ${this.commonSrv.FormatString(this.translate.instant('tips-invoice-repeat-no-code'), o.objectName, o.invName, o.invno, o.invcode)}</p>`;
                    }
                    idx++;
                });
                existRepeatItems = existRepeatItems.map(o => { return o.invName + '^' + o.invcode + '^' + o.invno });
                list = list.filter(o => existRepeatItems.indexOf(this.invoiceFile.filter(i => i.uid == o.flag)[0]?.name + '^' + o.invcode + '^' + o.invno) == -1);
                if (list.length == 0) {
                    this.modal.error({
                        nzTitle: this.translate.instant('tips'),
                        nzContent: repeatMsg
                    });
                }
                else {
                    this.modal.confirm({
                        nzTitle: this.translate.instant('tips-continue-add'),
                        nzContent: repeatMsg,
                        nzOnOk: () => {
                            if (existDuplicationItems.length == 0) { this.loadData(list) }
                        }
                    });
                }
            }
            if (existDuplicationItems.length > 0) {
                let repeatMsg = this.translate.instant('invoice-list-duplication-items') + '<br>';
                let idx = 1;
                existDuplicationItems.map(o => {
                    repeatMsg += `<p>${idx}. Invoice No.${o.invno}, Invoice Code.${o.invcode}</p>`;
                    idx++;
                })
                if (list.length > 0) {
                    this.modal.confirm({
                        nzTitle: this.translate.instant('tips-choose-one-to-add'),
                        nzContent: repeatMsg,
                        nzOnOk: () => this.loadData(list)
                    });
                }
            }
        }
        else { this.message.error('system error'); }
        this.isSaveLoading = false;
        this.isSpinning = false;
        this.isModalSpinning = false;
        this.spinningText = 'Loading...'
    }

    handleInputOk() {
        if (!this.infoForm.valid) {
            Object.values(this.infoForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
            this.message.error(this.translate.instant('fill-in-form'));
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }
        let formData = this.infoForm.getRawValue();
        if (this.company == "WVN") {
            if (!formData['salesname'] || !formData['salestaxno'] || formData['salesname'].trim() == '' || formData['salestaxno'].trim() == '') {
                this.message.error(this.translate.instant('fill-in-form'));
                this.isSpinning = false;
                this.isModalSpinning = false;
                this.isSaveLoading = false;
                return;
            }
        }
        if (!formData.invAmt || formData.invAmt == 0) {
            this.message.error(this.translate.instant('amount-zero-error'));
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }
        let existRepeat = true;
        if (!formData['invCode'] && !formData['invNumber']) {
            existRepeat = this.invoiceItemList.filter(o => o.invName == this.invoiceFile[0]?.name).length > 0;
        } else {
            existRepeat = this.invoiceItemList.filter(o => o.invcode == formData['invCode'] && o.invno == formData['invNumber']).length > 0;
        }
        if (existRepeat) {
            this.message.error(this.translate.instant('invoice-list-repeat-item') + `Invoice Name: ${this.invoiceFile[0]?.name},Invoice No:${formData['invNumber']},Invoice Code:${formData['invCode']}`, { nzDuration: 6000 })
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }

        this.Service.Post(URLConst.ManaulInvoiceQuery + `?invno=${formData['invNumber'] ?? ''}&invcode=${formData['invCode'] ?? ''}`, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body?.data) {
                if (res.body.data.length > 0) {
                    res.body.data[0].flag = this.invoiceFile[0]?.uid;
                    let count = this.loadData(res.body.data)?.length;
                    if (count > 0) { this.message.warning(this.commonSrv.FormatString(this.translate.instant('tips-replace-inv-info'), res.body.data[0].invno, res.body.data[0].invcode), { nzDuration: 6000 }) }
                }
                else {
                    let item = new InvoiceInfo();
                    item.index = Guid.create().toString();
                    item.invName = this.invoiceFile[0]?.name;
                    item.invCode = formData['invCode'];
                    item.invcode = formData['invCode'];
                    item.invNumber = formData['invNumber'];
                    item.salesname = formData['salesname'];
                    item.salestaxno = formData['salestaxno'];
                    item.invno = formData['invNumber'];
                    item.invDate = formData['invDate'];
                    item.invdate = formData['invDate'];
                    item.invType = formData['invType'];
                    item.invtype = formData['invType'];
                    item.invdesc = formData['invType'];
                    item.curr = formData['curr'];
                    item.invAmt = formData['invAmt'];
                    item.tlprice = formData['invAmt'];
                    item.file = this.invoiceFile[0];
                    item.state = '2';
                    item.uploadMode = this.uploadMode;
                    item.uploadmethod = 4;
                    item.isfill = true;
                    item.paymentStatDesc = '待請款';
                    item.flag = this.invoiceFile[0]?.uid;
                    this.invoiceItemList.push(item);
                }
                this.uploadModal = false;
            }
            else { this.message.error(res.message); }
            this.invoiceListState = this.commonSrv.FormatString(this.translate.instant('tips.upload-invoice-state'), this.invoiceItemList.length.toString(), this.uploadedCount.toString());
            this.isSaveLoading = false;
            this.isModalSpinning = false;
            this.isSpinning = false;
        });
    }

    editItem(index: string, state: string) {
        if (state == '1' || state == '3') return;
        this.getCurrency();
        this.getInvTypeList();
        let rowData = this.invoiceItemList.filter(o => o.index == index)[0];
        rowData.curr = rowData.curr ?? this.userInfo.curr;
        this.infoForm.reset(rowData);
        this.infoForm.get('fileList')!.clearValidators();
        this.infoForm.get('fileList')!.markAsPristine();
        this.infoForm.get('fileList')!.updateValueAndValidity();
        this.detailModal = true;
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
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }
        let formData = this.infoForm.getRawValue();
        if (this.company == "WVN") {
            if (!formData['salesname'] || !formData['salestaxno'] || formData['salesname'].trim() == '' || formData['salestaxno'].trim() == '') {
                this.message.error(this.translate.instant('fill-in-form'));
                this.isSpinning = false;
                this.isModalSpinning = false;
                this.isSaveLoading = false;
                return;
            }
        }
        if (!formData.invAmt || formData.invAmt == 0) {
            this.message.error(this.translate.instant('amount-zero-error'));
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }
        let existRepeat = false;
        if (!!formData['invCode'] || !!formData['invNumber']) {
            existRepeat = this.invoiceItemList.filter(o => o.index != formData.index && o.invcode == formData['invCode'] && o.invno == formData['invNumber']).length > 0;
        }
        if (existRepeat) {
            this.message.error(this.translate.instant('invoice-list-repeat-item') + `Invoice Name: ${this.invoiceFile[0]?.name},Invoice No:${formData['invNumber'] ?? ''},Invoice Code:${formData['invCode'] ?? ''}`, { nzDuration: 6000 })
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
            return;
        }

        this.Service.Post(URLConst.ManaulInvoiceQuery + `?invno=${formData['invNumber'] ?? ''}&invcode=${formData['invCode'] ?? ''}`, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body?.data) {
                let rowData = this.invoiceItemList.filter(o => o.index == formData.index)[0];
                if (res.body.data.length > 0) {
                    this.invoiceFile = [];
                    this.invoiceFile.push(rowData.file);
                    res.body.data[0].flag = rowData.file?.uid;
                    let replaceItems = this.loadData(res.body.data);
                    if (!!replaceItems && replaceItems.length > 0) {
                        this.invoiceItemList = this.invoiceItemList.filter(o => o.index != formData.index);
                        this.invoiceItemList.filter(o => replaceItems.indexOf(o.index) !== -1).map(o => o.state = '2');
                    }
                    else {
                        rowData.state = '3';
                        rowData.expMsg = `Invoice No.${(res.body.data[0].invno ?? '')}` + (res.body.data[0].expinfo ?? '') + (res.body.data[0].msg ?? '');
                    }
                }
                else {
                    rowData.invCode = formData['invCode'];
                    rowData.invcode = formData['invCode'];
                    rowData.invNumber = formData['invNumber'];
                    rowData.invno = formData['invNumber'];
                    rowData.invDate = formData['invDate'];
                    rowData.invdate = formData['invDate'];
                    rowData.salesname = formData['salesname'];
                    rowData.salestaxno = formData['salestaxno'];
                    rowData.invType = formData['invType'];
                    rowData.invtype = formData['invType'];
                    rowData.invdesc = formData['invType'];
                    rowData.curr = formData['curr'];
                    rowData.invAmt = formData['invAmt'];
                    rowData.tlprice = formData['invAmt'];
                    rowData.state = '2';
                }
                this.detailModal = false;
            }
            else { this.message.error(res.message); }
            this.isSpinning = false;
            this.isModalSpinning = false;
            this.isSaveLoading = false;
        });
    }

    deleteItem(index: string) {
        this.invoiceItemList = this.invoiceItemList.filter(o => o.index != index);
        this.message.success('delete successfully');
        this.invoiceListState = this.commonSrv.FormatString(this.translate.instant('tips.upload-invoice-state'), this.invoiceItemList.length.toString(), this.uploadedCount.toString());
    }
    clickBatchRemark() {
        this.remark = "";
        this.remarkModal = true;
    }

    handleBatchRemark() {
        this.remarkloading = true;
        this.invoiceItemList.map(o => { o.remark = this.remark; });
        this.remarkloading = false;
        this.remarkModal = false;
    }

    handleUpload() {
        let editItems = this.invoiceItemList.filter(o => o.state == '0');
        if (editItems.length > 0) {
            let msg = this.translate.instant('tips-invoice-list-lacked-items') + '<br>';
            let idx = 1;
            editItems.map(o => { msg += `<p>${idx}. Invoice Name: ${o.invName ?? ''},Invoice No: ${o.invno ?? this.translate.instant('null')}, Invoice Code: ${o.invcode ?? this.translate.instant('null')}</p>`; idx++; });
            this.modal.confirm({
                nzTitle: this.translate.instant('tips-continue-upload'),
                nzContent: msg,
                nzOnOk: () => this.upload()
            });
        }
        else { this.upload() }
    }

    upload() {
        this.isSpinning = true;
        this.uploadloading = true;
        let invList = this.invoiceItemList.filter(o => o.state == '1' || o.state == '2');
        if (invList.length == 0) {
            this.message.error(this.translate.instant('tips-no-items-upload'), { nzDuration: 6000 });
            this.isSpinning = false;
            this.uploadloading = false;
            return;
        }
        this.spinningText = this.commonSrv.FormatString(this.translate.instant('upload-progress-loading'), '0', invList.length.toString());
        let batchCount = 3;
        let startCursor = 0;
        let endCursor = 0;
        let groupCount = Math.ceil(Number(invList.length / batchCount));
        let groupNum = 0;
        let resultList = [];
        while (endCursor < invList.length) {
            startCursor = endCursor;
            endCursor = invList.length > (endCursor + batchCount) ? (endCursor + batchCount) : invList.length;
            const formData = new FormData();
            let partInvList = [];
            partInvList = invList.slice(startCursor, endCursor);
            formData.append('invoices', JSON.stringify(partInvList));
            let fileList = partInvList.filter(o => !!o.file).map(o => { return o.file })
            fileList.forEach((file: any) => {
                formData.append(file.uid, file.originFileObj);
            });
            this.Service.Post(URLConst.UploadInvoice, formData).subscribe((res) => {
                groupNum++;
                if (res && res.status === 200 && !!res.body) {
                    if (res.body.status == 1 && res.body.data.length > 0) {
                        resultList = resultList.concat(res.body.data);
                    }
                    else {
                        partInvList = partInvList.map(o => {
                            o.status = 2;
                            o.message = res.message ?? res.body.message;
                        });
                        resultList = resultList.concat(partInvList);
                    }
                }
                let uploaded = groupNum * batchCount;
                uploaded = uploaded > invList.length ? invList.length : uploaded;
                let remains = invList.length - uploaded;
                this.spinningText = this.commonSrv.FormatString(this.translate.instant('upload-progress-loading'), (uploaded).toFixed(0), (remains).toFixed(0));
                if (groupNum == groupCount) {
                    this.showUploadTips(resultList);
                }
            });
        }
    }

    showUploadTips(resultList: any) {
        if (resultList.length > 0) {
            let failedList = resultList.filter(o => o.status == 2);
            let successList = resultList.filter(o => o.status == 1);
            let tips = "";
            if (successList.length > 0) {
                let flagList = successList.map(o => { return o.data });
                this.invoiceItemList = this.invoiceItemList.filter(o => !flagList.includes(o.flag));
                if (failedList.length == 0) tips = this.translate.instant('tips.upload-invoice-success');
                this.uploadedCount += successList.length;
            }
            if (failedList.length > 0) {
                let idx = 0;
                failedList.map(o => {
                    idx++;
                    let item = this.invoiceItemList.filter(f => f.flag == o.data)[0];
                    item.state = '3';
                    item.expMsg = o.message;
                    tips += '<p>' + `${idx}. ${item.invName}` + o.message + '</p>';
                })
            }
            this.invoiceListState = this.commonSrv.FormatString(this.translate.instant('tips.upload-invoice-state'), this.invoiceItemList.length.toString(), this.uploadedCount.toString());
            this.modal.info({
                nzTitle: this.translate.instant('tips'),
                nzContent: tips
            });
        }
        else { this.message.error('system error'); }
        this.uploadloading = false;
        this.isSaveLoading = false;
        this.isSpinning = false;
        this.spinningText = 'Loading...'
    }

    beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
        return new Observable((observer: Observer<boolean>) => {
            let upload = true;
            if (this.uploadModal && this.uploadMode == 'input' && this.invoiceFile.length > 0) {
                upload = false;
                this.message.error(this.translate.instant('can-upload-only-one-item'));
            } else if (this.uploadModal && this.uploadMode == 'file' && this.invoiceFile.length > 0) {
                let uploadedFile = this.invoiceFile.filter(o => o.originFileObj.name == file.name);
                upload = uploadedFile.length == 0;
                if (!upload) this.message.error(this.commonSrv.FormatString(this.translate.instant('has-been-uploaded-that'), uploadedFile[0].originFileObj.name, uploadedFile[0].name));
            }
            observer.next(upload);
            observer.complete();
        });
    };

    handleChange(info: NzUploadChangeParam): void {
        let fileList = [...info.fileList];
        fileList = fileList.map(file => {
            file.status = "done";
            file.url = "...";
            if (file.type == 'application/pdf') { file.safeUrl = 'assets/image/pdf.png' }
            return file;
        });
        this.invoiceFile = fileList;
        this.invoiceFile.map(o => { o.safeUrl = !!o.safeUrl ? o.safeUrl : this.commonSrv.getFileUrl(o.originFileObj) });
        if (this.uploadMode == 'input') this.infoForm.controls.fileList.setValue(this.invoiceFile);
    }

    removeFile = (file: NzUploadFile) => {
        return new Observable((observer: Observer<boolean>) => {
            this.infoForm.controls.fileList.setValue(this.invoiceFile);
            observer.next(true);
            observer.complete();
        });
    }

    handlePreview = async (file: NzUploadFile): Promise<void> => {
        if (file.type.indexOf('image') !== -1) {
            this.previewImage = file.safeUrl;
            this.previewVisible = true;
        }
        else {
            this.frameSrc = file.safeUrl;
            this.drawerVisible = true;
        }
    };

    filters: UploadFilter[] = [
        {
            name: 'type',
            fn: (fileList: NzUploadFile[]) => {
                const filterFiles = fileList.filter(w =>
                    ~['image/jpeg'].indexOf(w.type) ||
                    ~['image/png'].indexOf(w.type) ||
                    ~['image/bmp'].indexOf(w.type) ||
                    ~['application/pdf'].indexOf(w.type));
                if (filterFiles.length !== fileList.length) {
                    this.message.error(this.translate.instant('file-format-erro-inv'));
                    return filterFiles;
                }
                return fileList;
            }
        }
    ];
    uploadIcons: NzShowUploadList = {
        showPreviewIcon: true,
        showRemoveIcon: true,
        showDownloadIcon: false,
    };

}
