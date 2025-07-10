import { environment } from './../../../../environments/environment';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { navigationImage } from 'src/app/shared/constants';
// import { IdmService } from 'src/app/shared/service/idm.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { CommonService } from 'src/app/shared/service/common.service';
@Component({
  selector: 'app-side-navbar',
  templateUrl: './side-navbar.component.html',
  styleUrls: ['./side-navbar.component.scss'],
})
export class SideNavbarComponent implements OnInit {
  @Input() isCollapsed: boolean;
  eventdata: any;
  isShowName: boolean = true;
  subscription: Subscription;
  menus = [];
  version = environment.VERSION;
  show: boolean[] = [];
  // clientId: string = this.idmenvironment.authConfig.client_id;
  @Output() handleCollapsed = new EventEmitter<boolean>();
  constructor(
    // private idmService: IdmService,
    private idmenvironment: EnvironmentconfigService,
    public translate: TranslateService,
    // private authService: AuthService,
    private commonSrv: CommonService,
    ) {}
  ngOnInit(): void {
    let idm_token = localStorage.getItem('user_idm_token');
    let userId = JSON.parse(idm_token).profile.sub;
    if (localStorage.getItem('menus') == null || localStorage.getItem('userInfo') == null || JSON.parse(localStorage.getItem('userInfo')).emplid != userId){
      let menus = [...navigationImage];
      menus.map(o => o.title = this.translate.instant(o.title));
      this.menus = menus;
      // this.idmService.getAuthority().then((data) => {
      //   this.judgeChildren(data, menus);
      //   localStorage.setItem('menus', JSON.stringify(menus));
      //   localStorage.setItem('permission', JSON.stringify(data));
      //   console.log('load permission!');
      // }).catch(error=>{
      //   console.log(error);
      // });
    }else{
      this.menus = JSON.parse(localStorage.getItem('menus'));
    }
    // this.idmService.getAuthority().then((data) => {
    //   this.judgeChildren(data, menus);
    //   this.menus = menus;
    // }).catch(error=>{
    //   console.log(error);
    //   this.judgeChildren([], menus);
    //   this.menus = menus;
    // });
    // this.subscription = this.idmService.receiveMsgSender.subscribe(
    //   (data: any) => {
    //     this.isShowName = data.showTopBarUserName;
    //   }
    // );
  }

  judgeChildren(auth, data) {
    // data.forEach(ele => {
    //   if (ele.children && ele.children.length) {
    //     ele.show = true;
    //     this.judgeChildren(auth, ele.children);
    //   } else {
    //     let role = `ers.${ele.authID}.View`;
    //     let i = auth.indexOf(role);
    //     if (this.authService.userInfo.debug){
    //       ele.show = true;
    //     }
    //     else if (ele.openPermission){
    //       if (i >= 0) {
    //         ele.show = true;
    //       } else {
    //         ele.show = false;
    //       }
    //     }else{
    //       ele.show = true;
    //     }
    //   }
    //   if (ele.children && ele.children.length) {
    //     let shows = ele.children.some(item=>item.show==true);
    //     if(shows){
    //       ele.show = true;
    //     }else{
    //       ele.show = false;
    //     }
    //   }
    // });
  }

  ClickMenu() {
    if (window.screen.width <= 767) {
      this.isCollapsed = !this.isCollapsed;
      this.handleCollapsed.emit(this.isCollapsed);
    }
  }
}
