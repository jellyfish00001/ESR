import { Inject, Injectable } from "@angular/core";
import { MsalService, MsalBroadcastService, MSAL_GUARD_CONFIG, MsalGuardConfiguration } from '@azure/msal-angular';
import { AuthenticationResult, EventMessage, EventType, InteractionStatus, RedirectRequest } from '@azure/msal-browser';
import { filter, switchMap, tap } from 'rxjs/operators';
import { BehaviorSubject, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import _ from 'lodash';
import { UserService } from "./user.service";
import { UserInfo } from "../../common/user-info";
import { MenusService } from "./menus.service";
import { PermissionService } from "./permission.service";
import { AuthService } from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class AADInitService {
  isIframe = false;
  public loginDisplay$: BehaviorSubject<Boolean> = new BehaviorSubject<Boolean>(false);
  public token = "";
  private readonly _destroying$ = new Subject<void>();
  public isinit = false;

  constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private broadcastService: MsalBroadcastService,
    private msalService: MsalService,
    private userService: UserService,
    private menusService: MenusService,
    private permissionService: PermissionService,
    private authService:AuthService
  ) {
  }

  setLoginDisplay() {
    this.loginDisplay$.next(this.msalService.instance.getAllAccounts().length > 0);
  }

  login() {
    if (this.msalGuardConfig.authRequest) {
      this.msalService.loginRedirect({ ...this.msalGuardConfig.authRequest } as RedirectRequest);
    } else {
      this.msalService.loginRedirect();
    }
    // this.setLoginDisplay()
  }

  logout() {
    // this.msalService.logoutRedirect({
    //   postLogoutRedirectUri: 'http://localhost:4200'
    // });
    localStorage.removeItem('accessToken');
    this.loginDisplay$.next(false);
  }

  // unsubscribe to events when component is destroyed
  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
  }

  initData() {
    this.userService.isInit$.next(false);
    return Promise.all([]).then(() => {
      this.broadcastService.msalSubject$
        .pipe(
          filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS || msg.eventType === EventType.ACQUIRE_TOKEN_SUCCESS),
          tap(() => this.setLoginDisplay()), // 更新登录状态
          takeUntil(this._destroying$),
        )
        .subscribe((result: any) => {
          const payload = result.payload as AuthenticationResult;
          this.msalService.instance.setActiveAccount(payload.account);
          // console.log("payload:-> ", payload);
          localStorage.setItem('accessToken', "Bearer " + payload.accessToken);
          this.setLoginDisplay();

        });
      this.broadcastService.inProgress$
        .pipe(
          filter((status: InteractionStatus) => status === InteractionStatus.None),
          tap(() => this.setLoginDisplay()), //// 更新登录状态
          filter(() => this.loginDisplay$.getValue() === true), // 确保用户已登录
          switchMap(() => this.userService.getUser()), // 获取用户信息
          tap((userDetail) => {
            UserInfo.setInstance(userDetail, this.msalService);
            this.userService.setUserInfo(UserInfo.getInstance());
          }),
          switchMap(() => this.menusService.getMenus$()),
          // switchMap(() => this.authService.getPermission(['Admin'])),
          // tap((menus) => {
          //   console.log('Menus loaded:', menus); // 确保菜单数据已加载
          // }),
          takeUntil(this._destroying$)
        )
        .subscribe();
    });
  }
}
