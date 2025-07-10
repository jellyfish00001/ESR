import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'src/app/shared/service/common.service';
import { TranslateService } from '@ngx-translate/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { UntypedFormBuilder } from '@angular/forms';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { URLConst } from 'src/app/shared/const/url.const';

@Component({
    selector: 'file-list-modal',
    templateUrl: './file-list-modal.component.html',
})
export class FileListModalComponent implements OnInit {
    @Input() fileList: any[] = [];
    @Input() fileCategory: string = '';
    @Input() linkName: any = this.translate.instant('invoice-detail');
    @Input() pageKey: string = '';
    @Output() filterFileList = new EventEmitter();
    @Output() modifiedInvList = new EventEmitter();
    showPicList: boolean = false;
    showEditModel: boolean = false;
    editForm: any = [];
    isSaveLoading: boolean = false;
    constructor(
        private commonSrv: CommonService,
        public translate: TranslateService,
        private message: NzMessageService,
        private fb: UntypedFormBuilder,
        private Service: WebApiService,
        private EnvironmentconfigService: EnvironmentconfigService,
    ) { }


    ngOnInit(): void {
        this.getDetail();
    }

    openPicFile(preview: any, name: any): void {
        const img = new window.Image();
        img.src = preview;
        const newWin = window.open('');
        newWin.document.write(img.outerHTML);
        newWin.document.title = name;
        newWin.document.close();
    }
    handlePicCancel(): void {
        this.showPicList = false;
    }

    getDetail(): void {
        this.filterFileList.emit();
        console.log('fileList', this.fileList);
        this.fileList.map(async o => {
            o.display = ['image/jpeg', 'image/png', 'image/bmp', 'application/pdf'].indexOf(o.type) !== -1;
            o.name = o.name ? o.name : o.filename;
            if (o.url == '...') {
                if (o.type.indexOf('image') !== -1) {
                    if (!o.preview) { o.preview = await this.commonSrv.getPicBase64(o.originFileObj!); }
                }
                else if (!o.safeUrl) {
                    o.safeUrl = this.commonSrv.getFileUrl(o.originFileObj);
                }
            }
            else {
                o.safeUrl = o.url;
                o.category = o.category;
                o.invno = o.invno || o.invoiceno;
            }
        });
    }

    openModal() {
        this.getDetail();
        this.showPicList = true;
    }

    showTips(): void {
        this.message.info(this.translate.instant('tips-no-electronic-file'), { nzDuration: 5000 });
    }

    editRow(data: any){
      console.log('editRow', data);
      this.editForm = this.fb.group({
        id: [data.id],
        amount: [data.amount],
        invno: [data.invno],
        oamount: [data.baseamt],
        taxamount: [data.taxamount],
        paymentNo: [data.sellerTaxId]
      });
      this.editForm.controls.amount.disable();
      this.showEditModel = true;
    }

    handleCancel(): void {
        this.showEditModel = false;
    }

    handleOk() {
      this.fileList.map(o => {
        if (o.id === this.editForm.controls.id.value) {
          o.invno = this.editForm.controls.invno.value;
          o.oamount = this.editForm.controls.oamount.value;
          o.taxamount = this.editForm.controls.taxamount.value;
          o.paymentNo = this.editForm.controls.paymentNo.value;
        }
      })
      this.modifiedInvList.emit(this.fileList);
      this.showEditModel = false;
      return;

      if(!(this.editForm.controls.invno.value || '') ){
        this.message.error(this.translate.instant('fill-in-form'));
        return;
      }
      let param = {
        id: this.editForm.controls.id.value,
        invno: this.editForm.controls.invno.value,
        oamount: this.editForm.controls.oamount.value,
        taxamount: this.editForm.controls.taxamount.value,
        sellerTaxId: this.editForm.controls.paymentNo.value,
      };
      this.Service.Post(
        this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.UpdateInvoice,
        param
      ).subscribe((res) => {
        if (res && res.status === 200 && !!res.body) {
          if (res.body.status == 1) {
            this.message.success(this.translate.instant('save-successfully'));
            //更新成功后刷新资料

          } else {
            this.message.error(res.body.message, { nzDuration: 6000 });
          }
        } else {
          this.message.error(
            res.message ?? this.translate.instant('server-error'),
            { nzDuration: 6000 }
          );
        }
      });
    }

    autoTips: Record<string, Record<string, string>> = {
      default: {
        required: this.translate.instant('can-not-be-null'),
      },
    };
}
