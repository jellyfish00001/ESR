import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { catchError, concatMap, Observable, of } from 'rxjs';
import { AADInitService } from 'src/app/shared/service/aad-init.service';
import _ from 'lodash';
import { log } from 'console';
@Injectable({
  providedIn: 'root',
})
export class LoginGuard implements CanActivate {

  constructor(
    private router: Router,
    private msalService: MsalService,
    private aadInit: AADInitService,
  ) { }
  //next--- 当前激活的路由快照 和 state---目标路由的状态快照
  canActivate(
    next: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    return this.msalService.handleRedirectObservable()
      .pipe(  //处理 MSAL 的重定向登录流程
        concatMap(() => {
          const accounts = this.msalService.instance.getAllAccounts();
               console.log('state.url', state.url);
             // 已登录且token有效
        if (accounts.length > 0 && !this.isTokenExpired()) {
          // 登录成功后，如果有重定向路径，跳转并清除
          const redirectUrl = sessionStorage.getItem("redirect");
          if (redirectUrl) {
            sessionStorage.removeItem("redirect");
            this.router.navigateByUrl(redirectUrl);
          }
          return of(true);
        } else {
          // 未登录或token失效，记录当前页面并跳转登录
          const currentUrl = state.url;
          sessionStorage.setItem("redirect", currentUrl);
          this.router.navigate(['/login']);
          return of(false);
        }
        }),
        catchError(() => {
          // this.redirectToLogin(state.url);
          this.router.navigate(['/login']); // 跳转到登录页面
          return of(false)
        })
      );

  }
  // 检查 token 是否过期
  private isTokenExpired(): boolean {
    const accounts = this.msalService.instance.getAllAccounts(); // 获取所有账户判断是否有用户
    if (accounts.length === 0) {
      return true; // 没有账户，视为 token 无效
    }
    const activeAccount = this.msalService.instance.getActiveAccount();    // 获取活动账户(当前用户)的 ID Token
    if (!activeAccount || !activeAccount.idTokenClaims) {
      return true; // 没有活动账户或 ID Token，视为无效
    }
    const idToken = activeAccount.idTokenClaims as any;
    if (!idToken.exp) {
      return true; // 没有过期时间，视为无效
    }
    const currentTime = Math.floor(Date.now() / 1000); // 当前时间（秒）
    return currentTime >= idToken.exp; // 检查是否过期
    }

  // // 重定向到登录页面
  // private redirectToLogin(redirectUrl: string): void {
  //   this.router.navigate(['/login']); // 跳转到登录页面
  // }

}
