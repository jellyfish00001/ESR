import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CommonService } from 'src/app/shared/service/common.service';
import { URLConst } from 'src/app/shared/const/url.const';

@Component({
  selector: 'help',
  template: `
  <nz-spin nzTip="Loading..." [nzSpinning]="isSpinning" [nzDelay]="500">
    <!-- <div class="title">Help</div> -->
    <div style="padding:1% 1%">
      <h3>{{'help-document'|translate}} ：</h3>
      <ul nz-list [nzDataSource]="dataList" nzBordered nzSize="large" style="width: fit-content;min-width: 300px;">
        <li nz-list-item *ngFor="let item of dataList" nzNoFlex>
          <ul nz-list-item-actions>
            <nz-list-item-action>
              <a  (click)="download(item.path,item.url)">{{'download'|translate}}</a>
            </nz-list-item-action>
          </ul>
          {{ item.name }}
        </li>
      </ul>
    </div>
  </nz-spin>
  `,
  styles: [``]
})
export class HelpComponent implements OnInit {
  constructor(
    private translate: TranslateService,
    private Service: WebApiService,
    private message: NzMessageService,
    private commonSrv: CommonService,
  ) { }

  isSpinning: boolean;
  userInfo: any;
  dataList = []

  ngOnInit(): void {
    this.isSpinning = true;
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) { this.message.error('Can not get user information. Please refresh the page...', { nzDuration: 6000 }); }
    else { this.getData(); }
  }

  getData() {
    let company = this.userInfo.company;
    this.Service.doGet(URLConst.GetHelpDocument + `?company=${company}`, null).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) { this.dataList = res.body.data.map(o => { return { name: o.name, url: o.url, path: o.path } }); }
        else { this.message.error(res.body.message, { nzDuration: 6000 }); }
      }
      else { this.message.error(res.message ?? this.translate.instant('server-error'), { nzDuration: 6000 }); }
      this.isSpinning = false;
    });
  }

  download(path: string, url: string) {
    let fileName = path;
    if (!path || !url) {
      this.message.error(this.translate.instant('tips-invalid-url'), { nzDuration: 6000 });
      return;
    }
    let startIdx = path.lastIndexOf('/');
    if (path.length < startIdx + 2) {
      this.message.error(this.translate.instant('tips-invalid-url'));
    }
    else {
      fileName = path.substring(startIdx + 1);
    }
    const a = document.createElement('a');
    document.body.appendChild(a);
    a.setAttribute('style', 'display:none');
    a.setAttribute('target', 'blank');
    a.setAttribute('href', url);
    a.setAttribute('download', fileName);
    a.click();
  }

}
