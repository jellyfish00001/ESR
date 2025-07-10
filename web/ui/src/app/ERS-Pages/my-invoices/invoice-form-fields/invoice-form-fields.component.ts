import { Component, Input } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl, UntypedFormBuilder, Validators } from '@angular/forms';
import { InvoiceFieldDefinition } from '../classes/data-item';
import { TranslateService } from "@ngx-translate/core";
import { ERSConstants } from "src/app/common/constant";
import { CommonService } from "src/app/shared/service/common.service";
import { WebApiService } from "src/app/shared/service/webapi.service";
import { URLConst } from "src/app/shared/const/url.const";
import { NzMessageService } from "ng-zorro-antd/message";

@Component({
  selector: 'app-invoice-form-fields',
  templateUrl: './invoice-form-fields.component.html'
})
export class InvoiceFormFieldsComponent {

  public infoForm: UntypedFormGroup;
  @Input() readOnly: boolean = false; // 是否只读模式

  currList: any[] = [];
  invTypeList: any[] = [];
  salesTaxNoList: any[] = [];
  buyerTaxNoList: any[] = [];
  responsiblePartyList: any[] = [];
  taxTypeList: any[] = [];

  constructor(
    public commonSrv: CommonService,
    private fb: UntypedFormBuilder,
    public translate: TranslateService,
    private Service: WebApiService,
    private message: NzMessageService,
  ) { }

  // 提供枚举给模板使用
  invoiceField = InvoiceFieldDefinition;

  ngOnInit(): void {
    this.initForm();

    this.getCurrencyList();
    this.getInvTypeList();
    this.getSalesTaxNoList();
    this.getBuyerTaxNoList();
    this.getTaxTypeList();
    this.getResponsiblePartyList();

    if (this.readOnly) {
      this.setDisable(true);
      this.clearValidators();
    }

    //添加发票时，支持手填，默认看到的栏位和A类发票一样
    this.displayInvoiceField(ERSConstants.InvoiceCategory.A);
  }

  setDisable(disable: boolean): void {
    //隐藏必填标识并设置为不可编辑
    const fieldKeys = Object.values(InvoiceFieldDefinition);

    for (const field of fieldKeys) {
      if (field == InvoiceFieldDefinition.isAbnormal || field == InvoiceFieldDefinition.remarks) continue;
      const control = this.infoForm.get(field);
      // 检查控件是否存在
      if (control) {
        if (disable) {
          // 禁用控件（设为只读）
          control.disable();
        }
        else {
          // 否则启用控件
          control.enable();
        }
      }
    }
  }

  clearValidators(): void {

    const fieldKeys = Object.values(InvoiceFieldDefinition);
    for (const field of fieldKeys) {
      const control = this.infoForm.get(field);
      // 检查控件是否存在
      if (control) {
        // 清除所有验证器，包括必填验证
        control.clearValidators();
        control.updateValueAndValidity();
      }
    }
  }

  private initForm(): void {
    this.infoForm = this.fb.group({
      id: [null],
      invcode: [null, null],
      invno: [null, null],
      invdate: [null, [Validators.required, this.dateValidator]],
      invtype: [null, [Validators.required]],
      curr: [null, [Validators.required]],
      startstation: [null, null],
      endstation: [null, null],
      buyertaxno: [null, null],
      salestaxno: [null, null],
      amount: [null, null],
      oamount: [null, [Validators.required]],
      taxamount: [null, null],
      invoicetitle: [null, null],
      taxbase: [null, null],
      importtaxamount: [null, null],
      servicefee: [null, null],
      shippingfee: [null, null],
      transactionfee: [null, null],
      quantity: [null, null],
      productinfo: [null, null],
      remarks: [null, null],
      isabnormal: ['false', [Validators.required]], //默认为不是异常发票
      abnormalreason: [null, null],
      responsibleparty: [null, null],
      taxtype: [null, null],
    });
  }

  dateValidator = (control: UntypedFormControl): { [s: string]: boolean } => {
    if (!!control.value && control.value > new Date()) {
      return { date: true, error: true };
    }
  };

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
      date: this.translate.instant('can-not-be-future-date'),
      startdate: this.translate.instant('can-not-later-than-end-date'),
      enddate: this.translate.instant('can-not-earlier-than-start-date'),
    }
  };

  fieldVisibility: { [key: string]: boolean } = {
    //这些栏位默认不显示
    [InvoiceFieldDefinition.invNo]: false,
    [InvoiceFieldDefinition.invCode]: false,
    [InvoiceFieldDefinition.amount]: false,
    [InvoiceFieldDefinition.taxAmount]: false,
    [InvoiceFieldDefinition.startStation]: false,
    [InvoiceFieldDefinition.endStation]: false,
    [InvoiceFieldDefinition.buyerTaxNo]: false,
    [InvoiceFieldDefinition.salesTaxNo]: false,
    [InvoiceFieldDefinition.invoiceTitle]: false,
    [InvoiceFieldDefinition.taxBase]: false,
    [InvoiceFieldDefinition.importTaxAmount]: false,
    [InvoiceFieldDefinition.serviceFee]: false,
    [InvoiceFieldDefinition.shippingFee]: false,
    [InvoiceFieldDefinition.transactionFee]: false,
    [InvoiceFieldDefinition.quantity]: false,
    [InvoiceFieldDefinition.productInfo]: false,
  };

  fieldRequired: { [key: string]: boolean } = {
    //这些栏位默认不非必填
    [InvoiceFieldDefinition.invNo]: false,
    [InvoiceFieldDefinition.invCode]: false,
    [InvoiceFieldDefinition.amount]: false,
    [InvoiceFieldDefinition.taxAmount]: false,
    [InvoiceFieldDefinition.startStation]: false,
    [InvoiceFieldDefinition.endStation]: false,
    [InvoiceFieldDefinition.buyerTaxNo]: false,
    [InvoiceFieldDefinition.salesTaxNo]: false,
    [InvoiceFieldDefinition.invoiceTitle]: false,
    [InvoiceFieldDefinition.taxBase]: false,
    [InvoiceFieldDefinition.importTaxAmount]: false,
    [InvoiceFieldDefinition.serviceFee]: false,
    [InvoiceFieldDefinition.shippingFee]: false,
    [InvoiceFieldDefinition.transactionFee]: false,
    [InvoiceFieldDefinition.quantity]: false,
    [InvoiceFieldDefinition.productInfo]: false,
  };

  getCurrencyList() {
    if (this.currList.length == 0) {
      this.Service.doGet(URLConst.GetCurrencyList, null).subscribe((res) => {
        if (res && res.status === 200) { this.currList = res.body.map(item => item.currency); }
        else { this.message.error(res.message); }
      });
    }
  }

  getInvTypeList() {
    if (this.invTypeList.length == 0) {
      this.Service.doGet(URLConst.GetAllInvTypes, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.invTypeList = res.body.data?.map(o => { return { invCode: o.invcode, invType: o.invtype, category: o.category } });
        }
        else { this.message.error(res.message); }
      });
    }
  }


  getSalesTaxNoList() {
    if (this.salesTaxNoList.length == 0) {
      this.Service.doGet(URLConst.GetAllVendors, null).subscribe((res) => {
        if (res && res.status === 200) {

          this.salesTaxNoList = res.body.data?.map(o => { return { unifyCode: o.unifyCode, venderName: o.venderName } });
        }
        else { this.message.error(res.message); }
      });
    }
  }

  getBuyerTaxNoList() {
    if (this.buyerTaxNoList.length == 0) {
      this.Service.doGet(URLConst.GetAllCompanyCategory, null).subscribe((res) => {
        if (res && res.status === 200) {

          this.buyerTaxNoList = res.body.data?.map(o => { return { identificationNo: o.identificationNo, companyDesc: o.companyDesc } });
        }
        else { this.message.error(res.message); }
      });
    }
  }

  getResponsiblePartyList() {
    if (this.responsiblePartyList.length === 0) {
      this.Service.doGet(URLConst.GetDataDictionary + `?category=${ERSConstants.DataDictionary.ResponsibleParty}`, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.responsiblePartyList = res.body.data;
        }
        else { this.message.error(res.message); }
      });
    }
  }

  getTaxTypeList() {
    if (this.taxTypeList.length === 0) {
      this.Service.doGet(URLConst.GetDataDictionary + `?category=${ERSConstants.DataDictionary.TaxType}`, null).subscribe((res) => {
        if (res && res.status === 200) {
          this.taxTypeList = res.body.data;
        }
        else { this.message.error(res.message); }
      });
    }
  }

  onInvTypeChange(value): void {
    if (this.invTypeList.length && !!value) {
      const selectedItem = this.invTypeList.find(item => item.invType === value);
      //发票类型变化时，更新表单字段的可见性
      //首先重置所有表单字段可见性
      this.resetFieldVisibility();
      this.displayInvoiceField(selectedItem.category);
    }

  }

  resetFieldVisibility(): void {
    // 首先将所有字段重置为不可见
    Object.keys(this.fieldVisibility).forEach(key => {
      this.fieldVisibility[key] = false;
      this.fieldRequired[key] = false;

      // 同时清除验证器
      const control = this.infoForm.get(key);
      if (control) {
        control.clearValidators();
        control.updateValueAndValidity();
      }
    });
  }

  //根据发票类型设置特定字段的可见性和验证器
  displayInvoiceField(invType: string): void {

    console.log("displayInvoiceField", invType);
    this.fieldVisibility[InvoiceFieldDefinition.remarks] = true;
    this.fieldVisibility[InvoiceFieldDefinition.isAbnormal] = true;

    switch (invType) {
      case ERSConstants.InvoiceCategory.A:
        this.fieldVisibility[InvoiceFieldDefinition.invNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.invCode] = true;
        this.fieldVisibility[InvoiceFieldDefinition.buyerTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.salesTaxNo] = true;
        break;
      case ERSConstants.InvoiceCategory.B:
        //稅局發票
        this.fieldVisibility[InvoiceFieldDefinition.invNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.invCode] = true;
        this.fieldVisibility[InvoiceFieldDefinition.buyerTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.salesTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.amount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.taxAmount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.taxType] = true;

        this.fieldRequired[InvoiceFieldDefinition.invNo] = true;
        this.fieldRequired[InvoiceFieldDefinition.amount] = true;
        this.fieldRequired[InvoiceFieldDefinition.taxAmount] = true;
        this.fieldRequired[InvoiceFieldDefinition.taxType] = true;

        this.infoForm.get(InvoiceFieldDefinition.invNo).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.amount).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.taxAmount).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.taxType).setValidators([Validators.required]);

        //更新验证状态
        this.infoForm.get(InvoiceFieldDefinition.invNo)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.amount)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.taxAmount)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.taxType)?.updateValueAndValidity();

        //课税别默认为应税
        this.infoForm.get(InvoiceFieldDefinition.taxType).setValue(ERSConstants.InvoiceTaxType.Taxable);
        break;
      case ERSConstants.InvoiceCategory.C:
        this.fieldVisibility[InvoiceFieldDefinition.salesTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.amount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.taxAmount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.startStation] = true;
        this.fieldVisibility[InvoiceFieldDefinition.endStation] = true;

        this.fieldRequired[InvoiceFieldDefinition.salesTaxNo] = true;
        this.fieldRequired[InvoiceFieldDefinition.amount] = true;
        this.fieldRequired[InvoiceFieldDefinition.taxAmount] = true;

        this.infoForm.get(InvoiceFieldDefinition.salesTaxNo).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.amount).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.taxAmount).setValidators([Validators.required]);

        //更新验证状态
        this.infoForm.get(InvoiceFieldDefinition.salesTaxNo)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.amount)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.taxAmount)?.updateValueAndValidity();

        break;
      case ERSConstants.InvoiceCategory.D:
        this.fieldVisibility[InvoiceFieldDefinition.startStation] = true;
        this.fieldVisibility[InvoiceFieldDefinition.endStation] = true;
        break;
      case ERSConstants.InvoiceCategory.E:
        this.fieldVisibility[InvoiceFieldDefinition.buyerTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.salesTaxNo] = true;
        this.fieldVisibility[InvoiceFieldDefinition.taxAmount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.importTaxAmount] = true;
        this.fieldVisibility[InvoiceFieldDefinition.serviceFee] = true;
        this.fieldVisibility[InvoiceFieldDefinition.taxBase] = true;


        this.fieldRequired[InvoiceFieldDefinition.buyerTaxNo] = true;
        this.fieldRequired[InvoiceFieldDefinition.salesTaxNo] = true;
        this.fieldRequired[InvoiceFieldDefinition.taxAmount] = true;
        this.fieldRequired[InvoiceFieldDefinition.importTaxAmount] = true;
        this.fieldRequired[InvoiceFieldDefinition.serviceFee] = true;
        this.fieldRequired[InvoiceFieldDefinition.taxBase] = true;

        this.infoForm.get(InvoiceFieldDefinition.buyerTaxNo).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.salesTaxNo).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.taxAmount).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.importTaxAmount).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.serviceFee).setValidators([Validators.required]);
        this.infoForm.get(InvoiceFieldDefinition.taxBase).setValidators([Validators.required]);

        //更新验证状态
        this.infoForm.get(InvoiceFieldDefinition.buyerTaxNo)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.salesTaxNo)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.taxAmount)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.importTaxAmount)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.serviceFee)?.updateValueAndValidity();
        this.infoForm.get(InvoiceFieldDefinition.taxBase)?.updateValueAndValidity();
        break;
    }
  }

  // 处理 isAbnormal 变化的方法
  onIsAbnormalChange(value: string): void {
    if (value === 'true') {
      // 如果是异常发票，设置异常原因和承担方为必填
      this.infoForm.get(InvoiceFieldDefinition.abnormalReason)?.setValidators([Validators.required]);
      this.infoForm.get(InvoiceFieldDefinition.responsibleParty)?.setValidators([Validators.required]);

    } else {
      // 如果不是异常发票，清除验证器
      this.infoForm.get(InvoiceFieldDefinition.abnormalReason)?.clearValidators();
      this.infoForm.get(InvoiceFieldDefinition.responsibleParty)?.clearValidators();

      // 清空值
      this.infoForm.get(InvoiceFieldDefinition.abnormalReason)?.setValue(null);
      this.infoForm.get(InvoiceFieldDefinition.responsibleParty)?.setValue(null);
    }

    // 更新验证状态
    this.infoForm.get(InvoiceFieldDefinition.abnormalReason)?.updateValueAndValidity();
    this.infoForm.get(InvoiceFieldDefinition.responsibleParty)?.updateValueAndValidity();
  }
}