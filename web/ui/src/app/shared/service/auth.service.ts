
import { EventEmitter, Injectable } from '@angular/core';

// import { Observable } from 'rxjs/index';
import { User, UserManager } from 'oidc-client';
import { from, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { EnvironmentconfigService } from './environmentconfig.service';
import { URLConst } from '../const/url.const';
import { HttpClient, HttpResponse, HttpRequest, HttpParams, HttpHeaders } from '@angular/common/http';
import { filter, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { WebApiService } from './webapi.service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // isLoggedIn = environment.production ? false : true;
  userPermission = [
    { apid: '*', c: false, r: false, u: false, d: false }
  ];
  // zh_TW//en
  // userInfo = {
  //     lang: 'zh-TW', token: '', emplid: 'Z11071892', cname: '', //emplid: 'Z11071892'
  //     ename: '', sitecode: 'WZS',
  //     timezone: +8, /* user time zone in hours */
  //     systz: 0 /* system time zone in hours */,
  //     usertz: 0 /* user time zone in hours */,
  //     cmpcode: '',
  //     siteno: '', /* site no from PMCS_FNDA_USER, it's 002... */
  //     menu: [],
  //     userAcl: [
  //         // { permission: 'mro.imbd006.delete', apid: 'imbd006' },
  //         // { permission: 'mro.imbd006.edit', apid: 'imbd006' },
  //         // { permission: 'mro.imbd006.view', apid: 'imbd006' },
  //     ],
  //     data_pool: { pool: ['common'] },
  //     Role: '',
  //     debug: false
  // };
  // AllSiteList;
  // AllVendorList;
  // AllBuyerList;
  // AllDeptList;
  // ConditonsList;
  // defautDept;
  // language;
  // defautSite;
  public localStorage: any;
  // StatementMaintainData;
  // StatementMaintainDataTotal;
  // StatementAbnormalData;
  // StatementAbnormalDataTotal;
  // private manager: UserManager = new UserManager(this.idmenvironment.authConfig);

  public loginStatusChanged: EventEmitter<User> = new EventEmitter();

  public userKey = '';

  constructor(
    private httpClient: HttpClient,
    private idmenvironment: EnvironmentconfigService,
    private router: Router,
    private Service: WebApiService
  ) {
    // this.manager.events.addAccessTokenExpired(() => {
    //     this.login();
    //     // if (this.manager.getUser()) {
    //     //     this.manager.signoutRedirect();
    //     // }
    // });
    if (!localStorage) {
      throw new Error('Current browser does not support Local Storage');
    }
    this.localStorage = localStorage;
  }

  // login() {
  //     this.manager.removeUser();
  //     this.manager.signinRedirect({ state: window.location.href.replace(window.location.origin+'/', '').replace('#/', '') });
  // }

  // loginCallBack() {
  //     return Observable.create(observer => {
  //         from(this.manager.signinRedirectCallback())
  //             .subscribe((user: User) => {
  //                 this.loginStatusChanged.emit(user);
  //                 observer.next(user);
  //                 observer.complete();
  //             });
  //     });
  // }

  // tryGetUser() {
  //     return from(this.manager.getUser());
  // }

  // logout() {
  //     // sessionStorage.clear();
  //     // this.clearAllCookie();
  //     localStorage.removeItem('user_idm_token');
  //     localStorage.removeItem('userInfo');
  //     localStorage.removeItem('menus');
  //     localStorage.removeItem('companyByPermission');
  //     this.manager.signoutRedirect();
  //     this.router.navigateByUrl('');
  // }

  get type(): string {
    return 'Bearer';
  }

  // clearAllCookie() {
  //     const date = new Date();
  //     date.setTime(date.getTime() - 10000);
  //     const keys = document.cookie.match(/[^ =;]+(?=\=)/g);
  //     // console.log('需要删除的cookie名字：' + keys);
  //     if (keys) {
  //       for (let i = keys.length; i--;) {
  //         document.cookie =
  //           keys[i] + '=0; expire=' + date.toUTCString() + '; path=/';
  //       }
  //     }
  //   }

  get token(): string | null {
    const temp = localStorage.getItem(this.userKey);
    if (temp) {
      const user: User = JSON.parse(temp);
      return user.access_token;
    }
    return null;
  }

  get authorizationHeader(): string | null {
    if (this.token) {
      return `${this.type} ${this.token}`;
    }
    return null;
  }
  // checkPermission(apid: string): boolean {
  //     if (apid.trim().length === 0) {
  //         return true;
  //     }
  //     let isPass = false;
  //     for (const entry of this.userInfo.userAcl) {
  //         if (entry.apid) {
  //             if (apid.toUpperCase() === entry.apid.toUpperCase()) {
  //                 isPass = true;
  //                 break;
  //             }
  //         }
  //     }
  //     return isPass;
  // }

  // getUserPermissionByAPID(apid: string): any {
  //     const _userApidPermission = { apid: apid, c: false, r: false, u: false, d: false };
  //     for (const entry of this.userPermission) {
  //         if (entry.apid) {
  //             if (apid === entry.apid) {
  //                 _userApidPermission.c = entry.c;
  //                 _userApidPermission.r = entry.r;
  //                 _userApidPermission.u = entry.u;
  //                 _userApidPermission.d = entry.d;
  //                 break;
  //             }
  //         }
  //     }
  //     return _userApidPermission;
  // }

  // getApiByComponent(componentName: string): any {
  // const _URLConst: any = {};
  // // { getSiteAll: { url: '/api/Site/GetAllByInfo', permission: 'mro.imbd006.view', components: ['Imbd006Component'] } }
  // for (const key in URLConst) {
  //   // check each key of json
  //   if (URLConst.hasOwnProperty(key)) {
  //     const element = URLConst[key];
  //     if (element.hasOwnProperty('components')) {
  //       // check components key
  //       const components = element['components'];
  //       for (let index = 0; index < components.length; index++) {
  //         const configComponent: string = components[index];
  //         if (configComponent.toUpperCase() === componentName.toUpperCase()) {
  //           _URLConst[key] = element;
  //           break;
  //         }
  //       }
  //     }
  //   }
  // }
  // return _URLConst;
  // }

  CheckPermissionByRole(roleKeys: string[]): Observable<boolean> {
    return this.Service.doPost(this.idmenvironment.authConfig.ersUrl + URLConst.GetAuth, roleKeys);
  }
  CheckPermissionByRoleAndRedirect(roleKeys: string[]): void {
    this.Service.doPost(this.idmenvironment.authConfig.ersUrl + URLConst.GetAuth, roleKeys)
    .subscribe((hasPermission: boolean) => {
      console.log('hasPermission', hasPermission);
      if(!hasPermission) {
        this.router.navigateByUrl(`ers/permissiondenied`);
      }
    })
  }

  public set(key: string, value: string): void {
    this.localStorage[key] = value;
  }

  public get(key: string): string {
    return this.localStorage[key] || false;
  }

  public setArr(key: string, value: Array<any>): void {
    this.localStorage[key] = value;
  }

  public setObject(key: string, value: any): void {
    this.localStorage[key] = JSON.stringify(value);
  }

  public getObject(key: string): any {
    return JSON.parse(this.localStorage[key] || '{}');
  }

  public remove(key: string): any {
    this.localStorage.removeItem(key);
  }
  public removeAll(): any {

    this.localStorage.clear();
  }
}

