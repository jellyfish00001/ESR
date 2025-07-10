import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-proxy',
  templateUrl: './proxy.component.html',
  styleUrls: ['./proxy.component.scss']
})
export class ProxyComponent implements OnInit {
  public bpmurl: any;
  isLoading = true;
  userInfo: any;
  constructor(
    // private authService: AuthService,
    private sanitizer: DomSanitizer,
    private EnvironmentconfigService: EnvironmentconfigService,
    private router: Router,
    private commonSrv: CommonService,
    private message: NzMessageService,
    public translate: TranslateService,
  ) { }

  ngOnInit(): void {
    this.userInfo = this.commonSrv.getUserInfo;
    this.isLoading = false;
  }

  click() {
    if (this.userInfo.isMobile) {
      this.message.info(this.translate.instant('only-pc'), { nzDuration: 6000 }); return;
    }
    else {
      this.isLoading = true;
      var url = this.EnvironmentconfigService.authConfig.BPMProxy;
      this.bpmurl = url; // this.sanitizer.bypassSecurityTrustResourceUrl(url);
      setTimeout(() => {
        this.isLoading = false;
      }, 1000);
      console.log("调用的bpm url：" + this.bpmurl);
      window.open(this.bpmurl);
    }
  }
}
