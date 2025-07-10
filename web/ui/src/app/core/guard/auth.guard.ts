
// import { Injectable } from '@angular/core';
// import {
//   CanActivate,
//   ActivatedRouteSnapshot,
//   CanActivateChild,
//   RouterStateSnapshot,
//   UrlTree,
//   Router,
// } from '@angular/router';
// import { User } from 'oidc-client';
// import { Observable } from 'rxjs';
// import { map } from 'rxjs/operators';
// // // import { AuthService } from 'src/app/shared/service/auth.service';
// import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
// // import { IdmService } from 'src/app/shared/service/idm.service';


// @Injectable({
//   providedIn: 'root',
// })
// export class AuthGuard implements CanActivate, CanActivateChild {

//   constructor(
//     private router: Router,
//     // // private authService: AuthService,
//     // private idmService: IdmService,
//     private idmConfig: EnvironmentconfigService
//   ) { }
//   canActivate(
//     next: ActivatedRouteSnapshot,
//     state: RouterStateSnapshot
//   ):
//     | Observable<boolean | UrlTree>
//     | Promise<boolean | UrlTree>
//     | boolean
//     | UrlTree {
//     const url: string = state.url;
//     return true;
//     // return this.authService.tryGetUser().pipe(map((user: User) => {
//     //   if (user) {
//     //     let emplid!: string; let name!: string; let email: string; let avatar: string;
//     //     const row = user.profile;
//     //     if (row.sub) { emplid = row.sub; }// employee id
//     //     if (row.name) { name = row.name; }
//     //     if (row.email) { email = row.email; }
//     //     if (row.avatar) { avatar = row.avatar; }

//     //     this.authService.userInfo.token = user.access_token;
//     //     this.authService.userInfo.emplid = emplid;
//     //     this.authService.userInfo.ename = name;
//     //     this.authService.userInfo.cname = name;

//     //     const timezoneoffset = new Date().getTimezoneOffset();
//     //     this.authService.userInfo.timezone = -timezoneoffset / 60;
//     //     console.log('用户已登录!');
//     //     return true;
//     //   } else {
//     //     this.authService.login();
//     //     return false;
//     //   }
//     // })
//     // );
//   }

//   canActivateChild(
//     next: ActivatedRouteSnapshot,
//     state: RouterStateSnapshot
//   ):
//     | Observable<boolean | UrlTree>
//     | Promise<boolean | UrlTree>
//     | boolean
//     | UrlTree {
//     return this.checkLogin(state.url);
//   }

//   async checkLogin(url: string): Promise<boolean | UrlTree> {
//     // let auth: string[] = await this.idmService.getAuthority();
//     // let RoleUrl = [{ role: 'APID001', url: '/pages/tokenInfo'},{ role: 'APID002', url: '/pages/bookingSpace'}
//     // ,{ role: 'user', url: '/pages/bookingSpace'}];
//     // let roleUrl = RoleUrl.find((item) => item.url == url);
//     // let role = `${this.idmConfig.authConfig.client_id}.${roleUrl.role}.view`;
//     // let index = auth.findIndex((item) => item === role);
//     // if (index >= 0) {
//     //   return true;
//     // } else {
//     //   let usrInfo = RoleUrl.find((item) => item.role === 'user');
//     //   return this.router.parseUrl(usrInfo.url);
//     // }

//     return true;
//   }
// }
