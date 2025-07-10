import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import _ from 'lodash';
import { switchMap } from 'rxjs/operators';
import { AADInitService } from 'src/app/shared/service/aad-init.service';
import { ERSConstants } from 'src/app/common/constant';
import { User } from 'oidc-client';
import { MsalService } from '@azure/msal-angular';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  CONSTANT_WORDING = ERSConstants.COPYRIGHT;
  localeID = window.navigator.language;

  constructor(
    private aadInit: AADInitService,
    private router: Router,
    private route: ActivatedRoute,
    private msalService: MsalService
  ) { }

  ngOnInit(): void {
    this.aadInit.initData();
    console.log("is Login -> ", this.aadInit.loginDisplay$.getValue());
    // 只在有 redirect 参数时更新 sessionStorage
    // this.route.queryParams.subscribe(params => {
    //   if (!_.isEmpty(params['redirect'])) {
    //     sessionStorage.setItem('redirect', params['redirect']);
    //   }
    //   if (!_.isEmpty(params['tabindex'])) {
    //     sessionStorage.setItem('tabindex', params['tabindex']);
    //   }
    // });
    this.msalService.instance.initialize().then(() => {
      return this.msalService.instance.handleRedirectPromise();
    }).then((result) => {
      console.log('MSAL handleRedirectPromise result:', result);
      if (result && result.account) {
        this.msalService.instance.setActiveAccount(result.account);
        console.log('MSAL login successful:', result.account);
        this.aadInit.setLoginDisplay();
        const redirectUrl = sessionStorage.getItem('redirect') || '/';
        sessionStorage.removeItem('redirect'); // 跳转后清理
        this.router.navigateByUrl(redirectUrl);
      }
      // else {
      //   // 没有账号时自动触发登录
      //   this.aadInit.login();
      // }
    }).catch((error) => {
      console.error('Error during MSAL login:', error);
    });
  }

  login() {
    this.aadInit.login();
  }
}
