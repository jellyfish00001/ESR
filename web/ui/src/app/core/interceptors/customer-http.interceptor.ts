import { Injectable } from '@angular/core';
import { catchError, EMPTY, Observable, switchMap } from 'rxjs';

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { MsalService } from '@azure/msal-angular';
import { AADInitService } from 'src/app/shared/service/aad-init.service';
import { Router } from '@angular/router';

@Injectable()
export class CustomerHttpInterceptor implements HttpInterceptor {
  constructor(
    private jwtHelperService: JwtHelperService,
    private msalService: MsalService,
    private router: Router,
    private aadInit: AADInitService
  ) { }
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = localStorage.getItem('accessToken');
    if (token && !this.isTokenExpired(token)) {
      // 如果 token 存在且未过期，直接继续请求
      return next.handle(request);
    } else if (token) {
      // 如果 token 存在但已过期，尝试刷新 token
      const activeAccount = this.msalService.instance.getActiveAccount();
      return this.msalService.acquireTokenSilent({ account: activeAccount, scopes: ['user.read'] })
        .pipe(
          switchMap((res) => {
            // 刷新成功，更新 token 并继续请求
            localStorage.setItem('accessToken', "Bearer " + res.accessToken);
            return next.handle(request);
          }),
          catchError((error) => {
            // 刷新失败，重定向到登录页面
            console.error('Token refresh failed, redirecting to login');
            this.aadInit.login();
            return EMPTY;
          })
        );
    } else {
      // 如果 token 不存在，直接继续请求
      return next.handle(request);
    }
  }

  private isTokenExpired(token: string): boolean {
    const jwtToken = this.jwtHelperService.decodeToken(token);
    const exp = jwtToken?.exp ? new Date(jwtToken.exp * 1000) : null;
    return !exp || exp <= new Date();
  }
}