import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { UserInfo } from "src/app/common/user-info";
import { BehaviorSubject, catchError, map, Observable, of, tap } from "rxjs";
import { URLConst } from "../const/url.const";
import { WebApiService } from "./webapi.service";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // new behavior subject to UserInfo
  private userInfo$ = new BehaviorSubject<UserInfo>(null);
  public isReload = 0;
  public isInit$ = new BehaviorSubject<Boolean>(null);

  // add setter and getter to userInfo
  public setUserInfo(value: UserInfo) {
    this.userInfo$.next(value);
    this.isInit$.next(true);
  }

  public getUserInfo() {
    return this.userInfo$.asObservable();
  }
  public getInit() {
    return this.isInit$.asObservable();
  }

  // constructor( private http: HttpClient) {
  //     super();
  // }
  constructor(
    private http: HttpClient,
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
  ) { }


  getUser(): Observable<any> {
    // company、emplid、deptid、cname、phone、curr、costdeptid、isaccount、timezone、isproxy, proxylist
    const ersUrl = this.EnvironmentconfigService.authConfig.ersUrl;
    if (!ersUrl) {
      console.error('ERS URL is not initialized.');
      return of(null); // 返回一个默认值，避免错误
    }
    return this.Service.doGet(
      ersUrl + URLConst.GetEmployeeInfo,
      null
    ).pipe(
      tap((res) => {
        if (res && res.status === 200) {
          localStorage.setItem('userInfo', JSON.stringify(res.body));
        }
      }),
      map((res) => res.body), // 提取响应体
      catchError((error) => {
        console.error("Error in getUser:", error);
        return of(null); // 返回一个默认值
      })
    );
  }

  // getUser2(): any {
  //   // company、emplid、deptid、cname、phone、curr、costdeptid、isaccount、timezone、isproxy、proxylist
  //   this.Service.doGet(
  //     this.EnvironmentconfigService.authConfig.ersUrl +
  //     URLConst.GetEmployeeInfo,
  //     null
  //   ).subscribe((res) => {
  //     let result;
  //     if (res && res.status === 200) {
  //       console.log("getUser", res);

  //       localStorage.setItem('userInfo', JSON.stringify(res.body));
  //       result = res.body;
  //     }
  //     return result;
  //   });
  // }



  getUser1() {
    let getHrUserUrl = "api/getHrUserByToken";
    return this.http.get<any>(getHrUserUrl)
  }
}
