import { Component, OnInit } from '@angular/core';
import { LayoutService } from '../layout.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter, take, takeUntil } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import _ from 'lodash';
import { TranslateService } from '@ngx-translate/core';
import { MenusService } from 'src/app/shared/service/menus.service';
@UntilDestroy()
@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  currentPageName: string;
  // currentPageName: string = this.router.url.slice(1);

  menu: any
  constructor(
    private router: Router,
    public layoutService: LayoutService,
    public menusService: MenusService,
    public translate: TranslateService,
  ) {

    // 訂閱路由事件，以便在每次導航結束時更新路由信息
    this.router.events
      .pipe(
        filter((event: any) => event instanceof NavigationEnd),
        untilDestroyed(this)
      )
      .subscribe(() => {
        this.isSubMenuOpen(this.router.url);
        this.changeCurrentPageName();
      });
  }
  ngOnInit(): void {
    // 确保菜单数据加载完成后调用
    this.menusService.menusBS$
      .pipe(
        // take(1),// 只订阅一次
        untilDestroyed(this)
      )
      .subscribe((menus) => {
        if (menus && menus.length > 0) {
          this.changeCurrentPageName();
          this.isSubMenuOpen(this.router.url);
        }
      });
  }

  changeCurrentPageName() {
    let _this = this;
    // 如果当前路由是根路径，设置 currentPageName 为空
    if (_this.router.url === '/') {
      this.currentPageName = '';
      return;
    }
    _.find(this.menusService.menus, function iter(menuItem) {
      if (menuItem.url === _this.router.url.slice(1)) {
        _this.currentPageName = menuItem.title
        return;
      }
      return _.find(menuItem.children, iter);
    });
  }

  // 判斷是否有開啟子選單 並 設定openSubMenu屬性
  isSubMenuOpen(subMenuRouterLink: string) {
    let menus = this.menusService.menus;
    const currentUrl = subMenuRouterLink.slice(1); // 去掉第一个斜杠
    _.each(menus, function iter(menuItem) {
      // 检查子菜单是否匹配
      if (menuItem.children && menuItem.children.length > 0) {
        const childMatch = _.find(menuItem.children, (child) => _.includes(currentUrl, child.url));
        if (childMatch) {
          // console.log('匹配的子菜单:', childMatch);
          // 设置父菜单的状态
          menuItem.open = true;
          menuItem.selected = true;
          // 同步更新子菜单的状态
          _.each(menuItem.children, (child) => {
            if (currentUrl === child.url) {
              // 只更新匹配的子菜单
              child.open = true;
              child.selected = true;
            } else {
              child.open = false;
              child.selected = false;
            }
          });
          // console.log('父菜单已更新:', menuItem);
        } else {
          menuItem.open = false;
          menuItem.selected = false;
          // 同步更新子菜单的状态
          _.each(menuItem.children, (child) => {
            child.open = menuItem.open;
            child.selected = menuItem.selected;
          });
        }
        // 递归处理子菜单
        _.each(menuItem.children, iter);
      }
    });

  }

  checkClick(e: KeyboardEvent, menu: any) {
    // only left click should prevent default
    if (false === e.ctrlKey) {
      e.preventDefault();
    }
  }
  // _.each(menus, function iter(menuItem) {
  //   console.log('menuItem.url', menuItem.url);
  //   console.log('currentUrl', currentUrl);  
  //   console.log('menuItem.children',_.includes(currentUrl, menuItem.url));
  //   if (_.includes(currentUrl, menuItem.url)) {
  //     console.log('menuItem.children', menuItem);

  //     menuItem.open = true;
  //     menuItem.selected = true;

  //   } else {
  //     menuItem.open = false;
  //   }
  //   return _.each(menuItem.children, iter);
  // });

}
