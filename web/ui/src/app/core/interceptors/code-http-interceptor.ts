
import { Injectable } from '@angular/core';
import { EMPTY, Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';

@Injectable()
export class CodeHttpInterceptor implements HttpInterceptor {
  constructor(private router: Router) { }
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('HTTP Error:', error);
        if (error.status === 401) {
          console.error('API 返回 401 Unauthorized，重定向到登录页面');
          this.redirectToLogin(); // 重定向到登录页面
          return  EMPTY; // 返回空 Observable
          // return this.handle401Error(req, next); // 调用静默刷新逻辑
        }
        return throwError(() => error);
      })
    );
  }
  // private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  //   const activeAccount = this.msalService.instance.getActiveAccount();
  //   console.log('当前活动账户:', activeAccount);
    
  //   // 检查是否有活动账户
  //   if (!activeAccount) {
  //     console.error('没有活动账户，重定向到登录页面');
  //     this.redirectToLogin();
  //     return EMPTY;
  //   }
  //   // 
  //   return this.msalService.acquireTokenSilent({ account: activeAccount, scopes: ['user.read'] }).pipe(
  //     switchMap((res) => {
  //       console.log('Token 刷新成功:', res.accessToken);
  //       // 更新本地存储中的 token
  //       localStorage.setItem('accessToken', res.accessToken);

  //       // 克隆原始请求并附加新的 Authorization 头
  //       // const clonedRequest = req.clone({
  //       //   setHeaders: {
  //       //     Authorization: res.accessToken,
  //       //   },
  //       // });

  //       // 重新发送原始请求
  //       // return next.handle(clonedRequest);
  //       return next.handle(req);
  //     }),
  //     catchError((refreshError) => {
  //       console.error('Token 刷新失败:', refreshError);
  //       this.redirectToLogin(); // 刷新失败，重定向到登录页面
  //       return EMPTY;
  //     })
  //   );
  // }

  private redirectToLogin(): void {
    // sessionStorage.clear();
    localStorage.clear();
    // 可选：保存当前页面路径以便登录后重定向
    // const currentUrl = this.router.url;
    // sessionStorage.setItem('redirect', currentUrl.slice(1));
    this.router.navigate(['/login']);
  }
  // private handle401Error(): void {
  //   sessionStorage.clear();
  //   localStorage.clear();
  //   // 可选：保存当前页面路径以便登录后重定向
  //   const currentUrl = this.router.url;
  //   sessionStorage.setItem('redirect', currentUrl);
  //   this.router.navigate(['/login']);
  // }
}