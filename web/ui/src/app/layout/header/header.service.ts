import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import _ from 'lodash';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserInfo } from 'src/app/common/user-info';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
// import { BaseApiService } from 'src/app/services/base-service.service';
import { UserService } from 'src/app/shared/service/user.service';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {
  private currentLocateName$: BehaviorSubject<string> = new BehaviorSubject(localStorage.getItem('i18n') ?? 'zh_TW');  //en_US
  public userEnName$ = new BehaviorSubject<string>(
    UserInfo.getInstance() ? UserInfo.getInstance().enName ?? localStorage.getItem('sessionChangeUserName') : '');
  private userId$ = new BehaviorSubject<string>(
    UserInfo.getInstance() ? UserInfo.getInstance().employeeNo ?? localStorage.getItem('sessionChangeUserNo') : '');

  private firstChar$ = new BehaviorSubject<string>(this.userEnName$.getValue()?.charAt(0));

  constructor(private httpClient: HttpClient,
    private userService:UserService,private environmentConfig: EnvironmentconfigService,) {
    this.userService.getUserInfo().subscribe((userInfo) => {
      console.log('header-userInfo',userInfo);
      if(!_.isEmpty(userInfo)){
        this.userEnName$.next(userInfo.enName ?? localStorage.getItem('sessionChangeUserName'));
        this.userId$.next(userInfo.employeeNo ?? localStorage.getItem('sessionChangeUserNo'));
        this.firstChar$.next(this.userEnName$.getValue()?.charAt(0));

      }
    });
  }



  set language(language: string) {
    this.currentLocateName$.next(language);
  }

  get language(): string {
    return this.currentLocateName$.getValue();
  }

  get currentLanguage$(): BehaviorSubject<string> {
    return this.currentLocateName$;
  }


  public get userEnName(): string {
    return this.userEnName$?.getValue().toUpperCase() ?? localStorage.getItem('sessionChangeUserName');
  }

  public set userEnName(value: string) {
    this.userEnName$.next(value);
    this.firstChar$.next(value.charAt(0));
  }

  public get userId(): string {
    return this.userId$.getValue() ?? localStorage.getItem('sessionChangeUserNo');
  }

  public set userId(value: string) {
    this.userId$.next(value);
  }

  public get firstChar(): string {
    return this.firstChar$.getValue() ?? localStorage.getItem('sessionChangeUserName').charAt(0);
  }

   public get envLabel(): string {
    const env = this.environmentConfig.authConfig.env;
    const configStr = localStorage.getItem('config');
    const env1 = configStr ? (JSON.parse(configStr).env || 'Dev') : 'Dev';
    return env?env: env1;
   }
}
