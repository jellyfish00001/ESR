import { Component, OnInit, ViewChild } from "@angular/core";
import { FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from "@angular/forms";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzModalService } from "ng-zorro-antd/modal";
import { NzShowUploadList, NzUploadChangeParam, NzUploadFile, UploadFilter } from "ng-zorro-antd/upload";
import { Observable, Observer } from "rxjs";
import { URLConst } from "src/app/shared/const/url.const";
import { AuthService } from "src/app/shared/service/auth.service";
import { CommonService } from "src/app/shared/service/common.service";
import { WebApiService } from "src/app/shared/service/webapi.service";
import { InvoiceFieldDefinition, InvoiceInfo } from "./classes/data-item";
import { InvoiceFormFieldsComponent } from "./invoice-form-fields/invoice-form-fields.component";
import { DetailTableColumn } from "./classes/table-column";
import { format } from 'date-fns';
import { ERSConstants } from "src/app/common/constant";

@Component({
    selector: 'my-invoices',
    templateUrl: './my-invoices.component.html',
    styleUrls: ['./my-invoices.component.scss']
})

export class MyInvoicesComponent implements OnInit {
    @ViewChild('editFormFields') editFormFields: InvoiceFormFieldsComponent;// 用于获取子组件的表单实例
    @ViewChild('viewFormFields') viewFormFields: InvoiceFormFieldsComponent;
    @ViewChild('addFormFields') addFormFields: InvoiceFormFieldsComponent;

    nzFilterOption = () => true;
    screenHeight: any;
    screenWidth: any;
    minWidth: any;
    queryForm: UntypedFormGroup;
    isSpinning = false;
    isSaveLoading = false;
    userInfo: any;
    invoiceItemList: InvoiceInfo[] = [];
    verifyStateList: any[] = [];
    payStateList: any[] = [];
    invoiceAreaList: any[] = [];
    isMobile: boolean;
    showItems: boolean = false;
    queryParam: any;
    total: any;
    pageIndex: number = 1;
    pageSize: number = 10;
    isQueryLoading: boolean = false;
    // invDetailModal: boolean = false;
    editModal: boolean = false;
    shareModal: boolean = false;
    receiver: string = null;
    itemDetail: InvoiceInfo = null;
    detailListTableColumn = DetailTableColumn;
    paymentDic: { [key: string]: string } = { 'requested': this.translate.instant('have-been-payout'), 'unrequested': this.translate.instant('to-be-payout'), 'recorded': this.translate.instant('funds-recorded') }
    drawerVisible: boolean = false;
    frameSrc: SafeResourceUrl;
    addOption: string;
    previewVisible = false;
    previewImage: string | undefined = '';
    uploadModal: boolean = false;
    uploadMode: string = "";
    invoiceNo: string = "";
    invoiceArea: string = "";
    invoiceCode: string = "";
    isModalSpinning = false;
    spinningText = 'Loading...';
    invoiceFile: any[] = [];
    fileTypeDictionary: { [key: string]: string } = { ['jpg']: 'image/jpeg', ['jpeg']: 'image/jpeg', ['png']: 'image/png', ['bmp']: 'image/bmp', ['pdf']: 'application/pdf' }
    originalOcrResult: InvoiceInfo = null; // 存储原始OCR结果
    ocrResultVisible: boolean = false; // 控制OCR结果模态窗口显示
    // 将常量暴露给模板
    public InvoiceSource = ERSConstants.InvoiceSource;

    constructor(
        private fb: UntypedFormBuilder,
        private Service: WebApiService,
        private authService: AuthService,
        private modal: NzModalService,
        public translate: TranslateService,
        private message: NzMessageService,
        private router: Router,
        private actRoute: ActivatedRoute,
        public commonSrv: CommonService,
        public domSanitizer: DomSanitizer,
    ) { }

    ngOnInit(): void {
        this.initForm();

        this.isSpinning = true;
        this.minWidth = window.innerWidth < 300 ? window.innerWidth * 0.9 + 'px' : (window.innerWidth > 580 ? '580px' : '300px');
        let today = new Date();
        let year = new Date(`${today.getFullYear()}-01-01`);
        this.queryForm.controls.startDate.setValue(year);
        this.queryForm.controls.endDate.setValue(today);
        this.isMobile = this.commonSrv.CheckIsMobile();
        this.getEmployeeInfo();
        this.getPayTypeList();
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

    private initForm(): void {
        this.queryForm = this.fb.group({
            startDate: [null, [this.startDateValidator]],
            endDate: [null, [this.endDateValidator]],
            invno: [null],
            invtype: [null],
            verifytype: [null],
            paytype: [null],
        });
    }

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

    getInvoiceAreaList() {
        this.Service.doGet(URLConst.GetCompanyAreaList, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body) {
                if (res.body.status == 1) {
                    this.invoiceAreaList = res.body.data;
                }
                else { this.message.error(res.body.message, { nzDuration: 6000 }); }
            }
            else { this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 }); }
            this.isSpinning = false;
        });
    }

    setDefaultInvoiceArea() {
        this.Service.doGet(URLConst.GetCompanyUserArea, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body) {
                if (res.body.status == 1) {
                    this.invoiceArea = res.body.data;
                }
                else { this.message.error(res.body.message, { nzDuration: 6000 }); }
            }
            else { this.message.error(this.translate.instant('server-error'), { nzDuration: 6000 }); }
            this.isSpinning = false;
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
                        salestaxno: o.salestaxno,
                        invcode: o.invcode,
                        invno: o.invno,
                        invtype: o.invtype,
                        invdate: !o.invdate ? null : format(new Date(o.invdate), "yyyy/MM/dd"),
                        oamount: o.oamount,
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
                        buyertaxno: o.buyertaxno,
                        remarks: o.remarks,
                        startstation: o.startstation,
                        endstation: o.endstation,
                        source: o.source,
                        ocrid: o.ocrid,
                        identificationno: o.identificationno,
                        invoicetitle: o.invoicetitle,
                        taxbase: o.taxbase,
                        importtaxamount: o.importtaxamount,
                        servicefee: o.servicefee,
                        shippingfee: o.shippingfee,
                        transactionfee: o.transactionfee,
                        quantity: o.quantity,
                        productinfo: o.productinfo,
                        invoicecategory: o.invoicecategory,
                        responsibleparty: o.responsibleparty,
                        taxtype: o.taxtype,
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
            this.frameSrc = await this.commonSrv.getFileData(url, 'invoice.pdf', 'application/pdf');
            this.drawerVisible = true;
        }
    }

    editRow(item: any = null) {
        this.editModal = true;

        if (item) {
            this.itemDetail = item;
        }
        //if (this.itemDetail.paytype != "unrequested" || !this.itemDetail.isfill) {
        if (this.itemDetail.paytype != "unrequested") {
            this.message.error(this.translate.instant('tips-item-cannot-edit'));
            return;
        }

        //使用setTimeout 确保在组件加载后设置表单数据
        setTimeout(() => {
            if (this.editFormFields) {
                // 初始化子组件的表单
                this.editFormFields.infoForm.reset(this.prepareFormData(this.itemDetail));

                this.editFormFields.infoForm.get(InvoiceFieldDefinition.isAbnormal).setValue((this.itemDetail.abnormalreason || '').trim() ? 'true' : 'false');

                // 更新字段可见性
                this.editFormFields.displayInvoiceField(this.itemDetail.invoicecategory);

                //如果发票来自于发票池，除备注栏外，其他栏位唯读
                if (this.itemDetail.source == ERSConstants.InvoiceSource.InvoicePool) {
                    this.editFormFields.setDisable(true);
                }
            } else {
                console.error('InvoiceFormFieldsComponent editFormFields not found');
            }
        });

    }

    showOcrResult() {
        // 根据当前发票ID或OCR ID获取OCR结果
        const ocrId = this.itemDetail.ocrid;
        if (ocrId) {
            // 调用API获取OCR识别结果
            this.Service.doGet(URLConst.GetOcrResult + `?ocrid=${ocrId}`, null).subscribe((res) => {
                if (res && res.status === 200 && !!res.body) {
                    if (res.body.status == 1 && res.body.data) {

                        this.originalOcrResult = res.body.data;

                        //使用setTimeout 确保在组件加载后设置表单数据
                        setTimeout(() => {
                            if (this.viewFormFields) {
                                // 初始化子组件的表单
                                this.viewFormFields.infoForm.reset(this.prepareFormData(this.originalOcrResult));

                                //根据发票类型设置字段可见性
                                this.viewFormFields.displayInvoiceField(this.originalOcrResult.invoicecategory);

                                //非OCR辨析结果原生栏位，隐藏不显示
                                this.viewFormFields.fieldVisibility[InvoiceFieldDefinition.remarks] = false;
                                this.viewFormFields.fieldVisibility[InvoiceFieldDefinition.taxType] = false;
                                this.viewFormFields.fieldVisibility[InvoiceFieldDefinition.isAbnormal] = false;
                                this.viewFormFields.fieldVisibility[InvoiceFieldDefinition.abnormalReason] = false;
                                this.viewFormFields.fieldVisibility[InvoiceFieldDefinition.responsibleParty] = false;

                            } else {
                                console.error('InvoiceFormFieldsComponent viewFormFields not found');
                            }
                        });

                        // 显示OCR结果模态窗口
                        this.ocrResultVisible = true;
                    }
                    else { this.message.error(res.body.message); }
                } else { this.message.error(res.message); }

                this.isSpinning = false;
            },
            );
        }
    }

    onRadioBtnChange(item) {
        this.addOption = item;

        setTimeout(() => {
            if (this.addFormFields) {
                //若發票來自於發票池，除備註欄外，其他欄位唯讀
                if (this.addOption == 'number') {
                    console.log("setDisable: ", "true");
                    //若發票來自於發票池，除備註欄外，其他欄位唯讀
                    this.addFormFields.setDisable(true);
                }
                else {
                    this.addFormFields.setDisable(false);
                }
            }
        });

    }

    recognizeInvoiceFile() {
        if (this.invoiceFile.length > 0) {

            this.isModalSpinning = true;
            this.spinningText = this.translate.instant('ocr-progress-loading');

            const formData = new FormData();
            formData.append('invoiceArea', this.invoiceArea);
            let partfileList = [];
            //一次只能上传一份发票
            partfileList = this.invoiceFile.slice(0, 1);
            partfileList.forEach((file: any) => {
                formData.append(file.uid, file.originFileObj);
            });

            this.Service.Post(URLConst.RecognizeInvoice, formData).subscribe((res) => {
                if (res && res.status === 200 && !!res.body?.data && res.body.data.length > 0) {

                    let resultList = [];
                    resultList = resultList.concat(res.body.data);
                    //把resultList添加到 invoiceItemList中
                    this.addInvoiceItemList(resultList, res.body.data[0].source);

                    setTimeout(() => {
                        if (this.addFormFields && this.invoiceItemList.length > 0) {

                            console.log("InvoiceItem: ", this.invoiceItemList[0]);

                            // 初始化子组件的表单
                            this.addFormFields.infoForm.reset(this.prepareFormData(this.invoiceItemList[0]));

                            // 更新字段可见性
                            this.addFormFields.displayInvoiceField(this.invoiceItemList[0].invoicecategory);

                            //如果发票来自于发票池，除备注栏外，其他栏位唯读
                            if (this.invoiceItemList[0].source == ERSConstants.InvoiceSource.InvoicePool) {
                                this.addFormFields.setDisable(true);
                            }

                            this.addFormFields.infoForm.get(InvoiceFieldDefinition.isAbnormal).setValue("false");
                        } else {
                            console.error('InvoiceFormFieldsComponent addFormFields not found');
                        }
                    });
                }
                else {
                    this.message.error(res.message ?? res.body.message);

                    this.isSpinning = false;
                    this.isModalSpinning = false;
                    this.isSaveLoading = false;
                }
            });
        }
    }

    queryInvoiceInfo() {
        if (!this.invoiceNo) {
            this.message.error(this.translate.instant('fill-in-form'));
            this.isSaveLoading = false;
            this.isSpinning = false;
            this.isModalSpinning = false;
            return;
        }
        this.invoiceNo = this.invoiceNo.trim();
        this.invoiceCode = this.invoiceCode.trim();
        let resultList = [];

        this.Service.Post(URLConst.ManaulInvoiceQuery + `?invno=${[this.invoiceNo]}&invcode=${[this.invoiceCode]}&invoiceArea=${[this.invoiceArea]}`, null).subscribe((res) => {
            if (res && res.status === 200 && !!res.body?.data) {
                if (res.body.data.length > 0) {

                    resultList = resultList.concat(res.body.data);
                    //把resultList添加到 invoiceItemList中
                    this.addInvoiceItemList(resultList, res.body.data[0].source);

                    setTimeout(() => {
                        if (this.addFormFields && this.invoiceItemList.length > 0) {
                            console.log("InvoiceItem: ", this.invoiceItemList[0]);
                            // 初始化子组件的表单
                            this.addFormFields.infoForm.reset(this.prepareFormData(this.invoiceItemList[0]));

                            // 更新字段可见性
                            this.addFormFields.displayInvoiceField(this.invoiceItemList[0].invoicecategory);

                            //若發票來自於發票池，除備註欄外，其他欄位唯讀
                            this.addFormFields.setDisable(true);
                        } else {
                            console.error('InvoiceFormFieldsComponent addFormFields not found');
                        }
                    });

                }
                else { this.message.error(res.body.message); }
            }
            else { this.message.error(res.message); }

            this.isSaveLoading = false;
            this.isSpinning = false;
            this.isModalSpinning = false;
        });
    }

    //添加发票
    addInvoice() {
        this.isSaveLoading = true;
        this.isModalSpinning = true;

        if (this.addFormFields) {
            if (!this.addFormFields.infoForm.valid) {
                Object.values(this.addFormFields.infoForm.controls).forEach(control => {
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
            let formData = this.addFormFields.infoForm.getRawValue();
            if (!formData.oamount || formData.oamount == 0) {
                this.message.error(this.translate.instant('amount-zero-error'));
                this.isSpinning = false;
                this.isModalSpinning = false;
                this.isSaveLoading = false;
                return;
            }

            if (formData.amount != null && formData.taxamount != null) {
                const totalAmount = formData.amount + formData.taxamount;
                if (totalAmount > formData.oamount) {
                    this.message.error(this.translate.instant('amount-taxamount-exceed-oamount'));
                    this.isSpinning = false;
                    this.isModalSpinning = false;
                    this.isSaveLoading = false;
                    return;
                }
            }

            if (this.invoiceItemList.length > 0) {
                const formData = new FormData();
                let firstItem = this.invoiceItemList[0];

                let invoiceSource = firstItem.source;
                //发票池的发票信息不能修改；OCR辨识的发票信息可以修改，一旦有修改，则source为manual
                if (invoiceSource === ERSConstants.InvoiceSource.OCR) {
                    // 获取所有 InvoiceFieldDefinition 枚举值
                    const invoiceFieldDefinition = Object.values(InvoiceFieldDefinition);

                    //遍历属性列表，逐一比较
                    for (const property of invoiceFieldDefinition) {
                        const ocrValue = firstItem[property];
                        const formFillValue = this.addFormFields.infoForm.get(property)?.value;

                        console.log("property: " + property + "; ocrValue: " + ocrValue + "; formFillValue: " + formFillValue);

                        if (ocrValue !== formFillValue) {
                            invoiceSource = ERSConstants.InvoiceSource.Manual;
                            break;
                        }
                    }
                }

                firstItem.invcode = this.addFormFields.infoForm.get(InvoiceFieldDefinition.invCode)?.value?.trim() || null;
                firstItem.invno = this.addFormFields.infoForm.get(InvoiceFieldDefinition.invNo)?.value?.trim() || null;
                firstItem.invdate = this.addFormFields.infoForm.get(InvoiceFieldDefinition.invDate)?.value || null;
                firstItem.invtype = this.addFormFields.infoForm.get(InvoiceFieldDefinition.invType)?.value || null;
                firstItem.curr = this.addFormFields.infoForm.get(InvoiceFieldDefinition.curr)?.value || null;
                firstItem.oamount = this.addFormFields.infoForm.get(InvoiceFieldDefinition.oAmount)?.value || 0.0;
                firstItem.amount = this.addFormFields.infoForm.get(InvoiceFieldDefinition.amount)?.value || 0.0;
                firstItem.taxamount = this.addFormFields.infoForm.get(InvoiceFieldDefinition.taxAmount)?.value || 0.0;
                firstItem.startstation = this.addFormFields.infoForm.get(InvoiceFieldDefinition.startStation)?.value?.trim() || null;
                firstItem.endstation = this.addFormFields.infoForm.get(InvoiceFieldDefinition.endStation)?.value?.trim() || null;
                firstItem.buyertaxno = this.addFormFields.infoForm.get(InvoiceFieldDefinition.buyerTaxNo)?.value?.trim() || null;
                firstItem.salestaxno = this.addFormFields.infoForm.get(InvoiceFieldDefinition.salesTaxNo)?.value?.trim() || null;
                firstItem.invoicetitle = this.addFormFields.infoForm.get(InvoiceFieldDefinition.invoiceTitle)?.value?.trim() || null;
                firstItem.taxbase = this.addFormFields.infoForm.get(InvoiceFieldDefinition.taxBase)?.value || 0.0;
                firstItem.importtaxamount = this.addFormFields.infoForm.get(InvoiceFieldDefinition.importTaxAmount)?.value || 0.0;
                firstItem.servicefee = this.addFormFields.infoForm.get(InvoiceFieldDefinition.serviceFee)?.value || 0.0;
                firstItem.shippingfee = this.addFormFields.infoForm.get(InvoiceFieldDefinition.shippingFee)?.value || 0.0;
                firstItem.transactionfee = this.addFormFields.infoForm.get(InvoiceFieldDefinition.transactionFee)?.value || 0.0;
                firstItem.quantity = this.addFormFields.infoForm.get(InvoiceFieldDefinition.quantity)?.value || 0.0;
                firstItem.productinfo = this.addFormFields.infoForm.get(InvoiceFieldDefinition.productInfo)?.value?.trim() || null;
                firstItem.remarks = this.addFormFields.infoForm.get("remarks")?.value?.trim() || null;
                firstItem.source = invoiceSource;
                firstItem.abnormalreason = this.addFormFields.infoForm.get("abnormalreason")?.value?.trim() || null;
                firstItem.responsibleparty = this.addFormFields.infoForm.get("responsibleparty")?.value || null;
                firstItem.taxtype = this.addFormFields.infoForm.get("taxtype")?.value || null;

                let partInvList = [];
                //一次只能上传一份发票
                partInvList = this.invoiceItemList.slice(0, 1);
                formData.append('invoices', JSON.stringify(partInvList));
                let fileList = partInvList.filter(o => !!o.file).map(o => { return o.file })
                fileList.forEach((file: any) => {
                    formData.append(file.uid, file.originFileObj);
                });

                this.Service.Post(URLConst.AddInvoice, formData).subscribe((res) => {
                    if (res && res.status === 200 && !!res.body) {
                        if (res.body.status == 1 && res.body.data.length > 0) {
                            this.modal.info({
                                nzTitle: this.translate.instant('tips'),
                                nzContent: this.translate.instant('tips.upload-invoice-success')
                            });

                            this.isSaveLoading = false;
                            this.isModalSpinning = false;
                            this.isSpinning = false;
                            this.uploadModal = false;

                            this.queryResultWithParam();

                        }
                        else if (res.body.status == 2) {
                            this.message.error(res.body.message);
                            this.isSpinning = false;
                            this.isSaveLoading = false;
                            this.isModalSpinning = false;
                        }
                    }
                });
            }
            else {
                const formData = new FormData();
                let itemList = [];
                itemList.push(this.addFormFields.infoForm.getRawValue());
                formData.append('invoices', JSON.stringify(itemList));

                this.Service.Post(URLConst.AddInvoice, formData).subscribe((res) => {
                    if (res && res.status === 200 && !!res.body) {
                        if (res.body.status === 1) {
                            this.modal.info({
                                nzTitle: this.translate.instant('tips'),
                                nzContent: this.translate.instant('tips.upload-invoice-success'),
                            });

                            this.isSaveLoading = false;
                            this.isModalSpinning = false;
                            this.isSpinning = false;
                            this.uploadModal = false;

                            this.queryResultWithParam();
                        } else {
                            this.message.error(res.body.message);
                            this.isSpinning = false;
                            this.isSaveLoading = false;
                            this.isModalSpinning = false;
                        }
                    }
                });
            }
        }
    }

    //发票内容编辑
    handleEditOk() {
        this.isSpinning = true;
        this.isSaveLoading = true;

        //获取要访问的子组件实例
        if (this.editFormFields) {
            //将子组件表单值同步到父组件
            const childFormValues = this.editFormFields.infoForm.getRawValue();
            Object.keys(childFormValues).forEach(key => {
                if (this.editFormFields.infoForm.get(key)) {
                    this.editFormFields.infoForm.get(key).setValue(childFormValues[key]);
                }
            });

            console.log("this.editFormFields.infoForm: ", this.editFormFields.infoForm);

            if (!this.editFormFields.infoForm.valid) {
                Object.values(this.editFormFields.infoForm.controls).forEach(control => {
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
            let formData = this.editFormFields.infoForm.getRawValue();
            if (!formData.oamount || formData.oamount == 0) {
                this.message.error(this.translate.instant('amount-zero-error'));
                this.isSpinning = false;
                this.isSaveLoading = false;
                return;
            }
            if (formData.amount != null && formData.taxamount != null) {
                const totalAmount = formData.amount + formData.taxamount;
                if (totalAmount > formData.oamount) {
                    this.message.error(this.translate.instant('amount-taxamount-exceed-oamount'));
                    this.isSpinning = false;
                    this.isSaveLoading = false;
                    return;
                }
            }
            if (formData.isabnormal === 'false') {
                //如果之前是异常发票，现在修改为正常发票，则需要清空异常原因
                formData.abnormalreason = null;
                formData.responsibleparty = null;
            }

            console.log("formData: ", formData);

            this.Service.Put(URLConst.MaintainInvoice, formData).subscribe(res => {
                if (res && res.status === 200 && res.body != null) {
                    if (res.body.status == 1) {
                        this.message.success(res.body.message);

                        this.queryResultWithParam();
                        this.editModal = false;
                    } else {
                        this.message.error(res.body.message);
                    }
                    this.isSpinning = false;
                    this.isSaveLoading = false;
                }
            });
        }
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

    showAddInvliceModal() {
        if (!this.userInfo) {
            this.getEmployeeInfo();
        }
        this.uploadModal = true;
        this.uploadMode = "";

        //重置发票上传页面，清空发票上传历史列表
        this.invoiceItemList = [];
        this.invoiceFile = [];
        this.showItems = false;
        this.invoiceNo = "";
        this.invoiceCode = "";
        this.addOption = "file";

        this.getInvoiceAreaList();
        this.setDefaultInvoiceArea();
    }

    handleFileChange(info: NzUploadChangeParam) {
        //console.log("info.file.status: ", info.file.status);
        //文件预处理和实际上传都会触发handleFileChange(状态都是uploading)，所以要加上这个判断，比较异步触发两次OCR辨识API
        if (info.file.status !== 'uploading') {
            let fileList = [...info.fileList];
            fileList = fileList.map(file => {
                file.status = "done";
                file.url = "...";
                if (file.type == 'application/pdf') { file.safeUrl = 'assets/image/pdf.png' }
                return file;
            });
            this.invoiceFile = fileList;
            this.invoiceFile.map(o => { o.safeUrl = !!o.safeUrl ? o.safeUrl : this.commonSrv.getFileUrl(o.originFileObj) });

            this.recognizeInvoiceFile();
        }
    }

    removeFile = (file: NzUploadFile) => {
        return new Observable((observer: Observer<boolean>) => {
            this.addFormFields.infoForm.controls.fileList.setValue(this.invoiceFile);
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

    beforeUpload = (file: NzUploadFile, _fileList: NzUploadFile[]) => {
        return new Observable((observer: Observer<boolean>) => {
            try {
                let isUpload = true;
                if (this.uploadModal && this.invoiceFile.length > 0) {
                    isUpload = false
                    this.message.error(this.translate.instant('can-upload-only-one-item'));
                }

                observer.next(isUpload); //文件上传执行到这里会发生异常，这是原来程式代码问题，待修复
                observer.complete();
            } catch (error) {
                console.error('Error in beforeUpload:', error);
                observer.error(error);
            }
        });
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

    addInvoiceItemList(resultList: any, source: string) {
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
                        existRepeatItems = existRepeatItems.concat(repeatItem);
                    }
                    else {
                        let repeatItem = [];
                        existRepeatItems = existRepeatItems.concat(repeatItem);
                    }
                });
            }
            if (existRepeatItems.length == 0 && existDuplicationItems.length == 0) { this.loadData(resultList, source) }
            if (existRepeatItems.length > 0) {
                let repeatMsg = this.translate.instant('invoice-list-repeat-items') + '<br>';
                let idx = 1;
                existRepeatItems.map(o => {
                    if (!o.invno && !o.invcode) {
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
                            if (existDuplicationItems.length == 0) {
                                this.loadData(list, source)
                            }
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
                        nzOnOk: () => this.loadData(list, source)
                    });
                }
            }
        }
        else {
            this.message.error('system error');
        }

        this.isSaveLoading = false;
        this.isSpinning = false;
        this.isModalSpinning = false;
        this.spinningText = 'Loading...'
    }

    //把发票数据加载到列表invoiceItemList
    loadData(data: any, source: string): any {
        let itemList = [];
        let expMsg = [];
        let idx = 0;
        data.map(o => {
            if (o.paymentStat) { //可请款
                let item = o;

                item['file'] = this.addOption == 'file' ? this.invoiceFile[0] : null;
                item['remarks'] = null;
                item['source'] = source;

                itemList.push(item);
            }
            else {
                idx++;
                let msg = `<p>${idx}. Invoice No.${o.invno ?? ''} ${this.translate.instant('add-failed')},`;
                if (!o.paymentStat) { msg += o.expinfo + ';' }
                if (!o.existautopa) { msg += '<br>' + o.msg + ';' }
                msg += '</p>'
                expMsg.push(msg);
            }
        });
        if (itemList.length > 0) {
            this.invoiceItemList = this.invoiceItemList.concat(itemList);
        }
        if (expMsg.length > 0) {
            let tips = "";
            expMsg.map(o => tips += o);
            this.modal.info({
                nzTitle: this.translate.instant('tips'),
                nzContent: tips
            });
        }
    }

    prepareFormData(data: any): any {
        const formData = { ...data };

        //转换所有日期字段，nz-date-picker要绑定Date类型，如果绑定string类型会报错
        const dateFields = ['invdate', 'cdate', 'mdate']; // 所有日期字段

        for (const field of dateFields) {
            if (formData[field] && typeof formData[field] === 'string') {
                try {
                    formData[field] = new Date(formData[field]);
                } catch (e) {
                    formData[field] = null;
                }
            }
        }

        return formData;
    }
}