import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, filter } from 'rxjs';
// import { MenuItem } from '../common/models/menu.interface';
import { HttpClient } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';
import { URLConst } from "../const/url.const";
import { WebApiService } from './webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { fi } from 'date-fns/locale';
import { navigationImage } from '../constants/menu-config.const'; // 导入 navigationImage


export interface ApiResponse {
  status: number;
  message: string;
  messages: string[];
  body: any;
}


@Injectable({
  providedIn: 'root'
})
export class MenusService {
  public menus$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // getter/setter
  get menus() {
    return this.menus$.getValue();
  }

  get menusBS$(): BehaviorSubject<any[]> {
    return this.menus$;
  }

  set menus(menus) {
    this.menus$.next(menus);
  }

  constructor(
    private httpClient: HttpClient,
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
  ) { }

  getAllMenu(): Observable<any> {
    return this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetMenu,
      null
    ).pipe(
      filter((res) => res && res.code !== 200), // 过滤掉错误的响应
      map((res) => {
        // 使用 navigationImage 筛选符合条件的菜单
        const filteredMenus = this.filterMenusByAuthID(
          navigationImage, // 使用导入的 navigationImage
          res.body || []
        );
        this.menus$.next(filteredMenus);
        // this.menus = res.body; // 更新菜单数据
      }),
      catchError((error) => {
        console.error("Error in getAllMenu:", error);
        return of(null); // 返回一个默认值
      })
    );
  }

  // 根据 authID 筛选菜单
  private filterMenusByAuthID(menus: any[], authIDs: string[]): any[] {
    return menus.map((menu) => {
      // 如果有子菜单，递归筛选
      if (menu.children) {
        const filteredChildren = this.filterMenusByAuthID(
          menu.children,
          authIDs
        );
        // 如果子菜单有符合条件的项，则保留当前菜单
        if (filteredChildren.length > 0) {
          return { ...menu, children: filteredChildren };
        }
      }

      // 如果没有子菜单，或者子菜单不符合条件，则检查当前菜单
      if (authIDs.includes(menu.authID)) {
        return menu;
      }

      // 如果当前菜单和子菜单都不符合条件，则过滤掉
      return null;
    })
      .filter((menu) => menu !== null); // 过滤掉空值
  }

  getMenus$(): Observable<any[]> {
    if (this.menus.length === 0) {
      return this.getAllMenu()
    }
    return this.menus$.asObservable();
  }
}
