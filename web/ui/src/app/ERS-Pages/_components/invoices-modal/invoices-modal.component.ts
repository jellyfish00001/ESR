import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonService } from 'src/app/shared/service/common.service';
import { TranslateService } from '@ngx-translate/core';
import { URLConst } from 'src/app/shared/const/url.const';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { format } from 'date-fns';
import { DetailTableColumn } from "./classes/table-column";
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { InvoiceDetail } from './classes/data-item';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ERSConstants } from 'src/app/common/constant';

@Component({
    selector: 'invoices-modal',
    templateUrl: './invoices-modal.component.html',
    styleUrls: ['./invoices-modal.component.scss']
})
export class InvoicesModalComponent implements OnInit, OnChanges {
    @Input() userId: string = null;
    @Input() allSelectedInvItemList: any[] = [];
    @Input() keyId: number;
    @Input() curr: string;
    @Input() company: string;
    @Output() addInvoice = new EventEmitter();
    @Input() area: string;
    isLoading: boolean = false;

    detailListTableColumn = DetailTableColumn;
    invoiceItemList: InvoiceDetail[] = [];
    selectedInvItemList: any[] = [];
    isMobile: boolean;
    showItems: boolean = false;
    total: any;
    pageIndex: number = 1;
    pageSize: number = 10;
    exInvForm: UntypedFormGroup;
    invExceptionModal: boolean = false;
    isSaveLoading: boolean = false;
    isSpinning: boolean = false;

    affordDic: { [key: string]: string } = { 'self': this.translate.instant('individual-afford'), 'company': this.translate.instant('company-afford') }
    showInvoicePageModal = false;
    constructor(
        private commonSrv: CommonService,
        public translate: TranslateService,
        private Service: WebApiService,
        private fb: UntypedFormBuilder,
        private message: NzMessageService,
    ) { }


    ngOnInit(): void {
        this.invoiceItemList = [];
        this.isMobile = this.commonSrv.CheckIsMobile();
        this.exInvForm = this.fb.group({
            invoiceid: [null],
            invcode: [{ value: null, disabled: true }],
            invno: [{ value: null, disabled: true }],
            oamount: [{ value: 0, disabled: true }],
            curr: [{ value: null, disabled: true }],
            expdesc: [{ value: null, disabled: true }],
            toLocalTaxLoss: [{ value: 0, disabled: true }],
            invabnormalreason: [null, [Validators.required]],
            affordParty: [null, [Validators.required]]
        });
        if (!!this.allSelectedInvItemList) {
            console.log('this.allSelectedInvItemList',this.allSelectedInvItemList)
            this.selectedInvItemList = this.allSelectedInvItemList.filter(o => o.id == this.keyId && o.disabled);
            this.selectedInvItemList.map(o => this.setOfCheckedId.add(o.invoiceid));
        }
    }
    autoTips: Record<string, Record<string, string>> = {
        default: {
            required: this.translate.instant('can-not-be-null'),
        }
    };

    ngOnChanges(changes: SimpleChanges): void {
        if (!!changes.company) {
            this.invoiceItemList = [];
        }
    }

    openFolder() {
      console.log('this.invoiceItemList', this.invoiceItemList);
        if (this.invoiceItemList.length == 0) {
            this.getInvoices(true);
        }
        else { this.showItems = true; }
    }

    getInvoices(initial: boolean = false) {
        this.isLoading = true;
        // if (initial) {
        //     this.pageIndex = 1;
        //     this.pageSize = 10;
        // }
        // let queryParam = {
        //     pageIndex: this.pageIndex,
        //     pageSize: this.pageSize,
        //     isphone: this.isMobile,
        //     data: {}
        // }
        // this.Service.Post(URLConst.QueryInvoice, queryParam).subscribe((res) => {
        this.Service.doGet(URLConst.GetUnpaidInvoice + `/${this.userId}` + `?company=${this.company}`, null).subscribe((res) => {
            if (res && res.status === 200 && res.body != null) {
                if (res.body.status == 1) {
                    this.total = res.body.total;
                    let result: InvoiceDetail[] = [];
                    res.body.data?.map(o => {
                        let item = o;
                        item['invoiceid'] = o.id;
                        item['disabled'] = false;
                        item['invdate'] = !o.invdate ? null : format(new Date(o.invdate), "yyyy/MM/dd");
                        result.push(item);
                    });
                    this.invoiceItemList = result;
                    let disabledInvIds = this.allSelectedInvItemList.filter(o => o.id != this.keyId && o.disabled).map(o => { return o.invoiceid })
                    this.invoiceItemList.filter(o => disabledInvIds.includes(o.invoiceid)).forEach(o => o.disabled = true);
                    disabledInvIds.map(o => this.setOfCheckedId.add(o));
                    let selectedInvIds = this.selectedInvItemList.map(o => { return o.invoiceid });
                    this.selectedInvItemList = [];
                    selectedInvIds.map(o => { this.packUpItem(o) });
                    this.showItems = true;
                }
                else { this.message.error(res.body.message, { nzDuration: 6000 }); }
            }
            else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
            this.isLoading = false;
        });
    }

    pageIndexChange(value) {
        this.pageIndex = value;
        this.getInvoices();
    }

    pageSizeChange(value) {
        this.pageSize = value;
        this.getInvoices();
    }

    ////////带选择框表
    checked = false;
    indeterminate = false;
    listOfCurrentPageData: InvoiceDetail[] = [];
    setOfCheckedId = new Set<string>();
    updateCheckedSet(id: string, checked: boolean): void {
        if (checked) {
            this.setOfCheckedId.add(id);
        } else {
            this.setOfCheckedId.delete(id);
        }
    }

    onCurrentPageDataChange(listOfCurrentPageData: InvoiceDetail[]): void {
        this.listOfCurrentPageData = listOfCurrentPageData;
        this.refreshCheckedStatus();
    }

    refreshCheckedStatus(): void {
        const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
        this.checked = listOfEnabledData.every(({ invoiceid: id }) => this.setOfCheckedId.has(id));
        this.indeterminate = listOfEnabledData.some(({ invoiceid: id }) => this.setOfCheckedId.has(id)) && !this.checked;
    }

    onItemChecked(item: InvoiceDetail, checked: boolean): void {
        this.updateCheckedSet(item.invoiceid, checked);
        this.refreshCheckedStatus();
        if (checked) {
            if (!!item.expdesc) {
                this.showInvDetailModal(item);
            }
            else {
                this.packUpItem(item.invoiceid);
            }
        } else {
            this.deleteInvItem(item.invoiceid);
        }
    }

    showInvDetailModal(item: InvoiceDetail) {
        item['invabnormalreason'] = null;
        this.exInvForm.reset(item);
        let toLocalTaxLoss = Number((item['baseamt'] * 0.25).toFixed(2));
        this.exInvForm.controls.toLocalTaxLoss.setValue(toLocalTaxLoss);
        this.invExceptionModal = true;
    }

    packUpItem(inoviceid: string) {
        let row = this.invoiceItemList.filter(o => o.invoiceid == inoviceid)[0];
        if (!!row) {
            let item = JSON.parse(JSON.stringify(row));
            item['id'] = this.keyId;
            item['affordPartyValue'] = this.invExceptionModal ? this.exInvForm.controls.affordParty.value : null;
            item['affordParty'] = this.invExceptionModal ? this.affordDic[this.exInvForm.controls.affordParty.value] : null;
            item['reason'] = row.expdesc;
            item['invabnormalreason'] = this.invExceptionModal ? this.exInvForm.controls.invabnormalreason.value : null;
            item['invoiceCode'] = row.invcode;
            item['invoiceNo'] = row.invno;
            item['taxLoss'] = this.invExceptionModal ? this.exInvForm.controls.toLocalTaxLoss.value : row.taxloss;   // 目前申请页面taxloss与localtaxloss无异
            item['toLocalTaxLoss'] = this.invExceptionModal ? this.exInvForm.controls.toLocalTaxLoss.value : row.taxloss;
            // item['exTips'] = row.expcode;
            item['exTips'] = !!row.expcode ? this.commonSrv.FormatString(this.translate.instant('manual-inv-exception-warning-sample'), row.invno, row.expdesc, item['invabnormalreason'], item['toLocalTaxLoss'], item['affordParty']) : null;
            item['abnormalamount'] = 0;
            item['invoiceid'] = row.invoiceid;
            item['uid'] = row.invoiceid;
            item.disabled = true;
            this.selectedInvItemList.push(item);
        }
        else {
            this.message.warning(this.translate.instant('tips-invalid-invoice'));
        }
    }

    handleConfirm() {
        if(this.area === ERSConstants.Area.TW && this.selectedInvItemList.length > 1) {
          this.message.error(this.translate.instant('only-select-one-invoice'), {
            nzDuration: 6000,
          });
          return;
        }
        this.showItems = false;
        this.addInvoice.emit(this.selectedInvItemList);
    }

    handleExCancel() {
        this.invExceptionModal = false
        this.setOfCheckedId.delete(this.exInvForm.controls.invoiceid.value);
    }

    handleExOk() {
        this.isSpinning = true;
        this.isSaveLoading = true;
        if (!this.exInvForm.valid) {
            Object.values(this.exInvForm.controls).forEach(control => {
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
        this.packUpItem(this.exInvForm.controls.invoiceid.value);
        this.isSpinning = false;
        this.isSaveLoading = false;
        this.invExceptionModal = false;
    }

    deleteInvItem(invoiceid: string) {
        this.selectedInvItemList = this.selectedInvItemList.filter(o => o.invoiceid != invoiceid);
        let invItem = this.invoiceItemList.filter(o => o.invoiceid == invoiceid)[0];
        if (!!invItem) { invItem.disabled = false; }
        this.setOfCheckedId.delete(invoiceid);
        if (!this.showItems)
            this.addInvoice.emit(this.selectedInvItemList);
    }

    showInvoicePage() {
      this.showInvoicePageModal = true;
    }
}
