import { GetTitleService } from './../../../shared/service/get-title.service';
import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  OnDestroy,
  Input,
} from '@angular/core';
import { Router } from '@angular/router';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { TranslateService } from '@ngx-translate/core';
import { ConfigService } from '@ngx-config/core';
import { Subscription } from 'rxjs';
// import { IdmService } from 'src/app/shared/service/idm.service';
// import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-top-navbar',
  templateUrl: './top-navbar.component.html',
  styleUrls: ['./top-navbar.component.scss'],
})
export class TopNavbarComponent implements OnInit, OnDestroy {
  @Input() isCollapsed: boolean = false;
  count;
  dot = true;
  @Output() handleCollapsed = new EventEmitter<boolean>();
  title: string;
  userName;
  isShowName: boolean = true;
  newMessage = [
    { author: 'molly', message: 'you are welcome' },
    { author: 'molly', message: 'you are welcome' },
    { author: 'molly', message: 'you are welcome' },
  ];
  oldMessage = [
    { author: 'WiGPS', message: 'you are welcome' },
    { author: 'WiGPS', message: 'you are welcome' },
    { author: 'WiGPS', message: 'you are welcome' },
  ];
  //langue: any;
  eventdata: any;
  selectLangue: any = 'English';
  languageItems;
  subscription: Subscription;

  constructor(
    public titleSevice: GetTitleService,
    private translateService: TranslateService,
    private ConfigService: ConfigService,
    // private idmService: IdmService,
    // private authservice: AuthService,
  ) { }

  ngOnInit(): void {
    //user Name show or not,if from wigps then hide username
    this.userName = localStorage.getItem('userName');
    //this.isShowName =false;
    // this.subscription = this.idmService.receiveMsgSender.subscribe(
    //   (data: any) => {
    //     this.isShowName = false;// data.showTopBarUserName;
    //   }
    // );
    this.title = this.ConfigService.getSettings<string>('envirenment');
    this.languageItems = this.ConfigService.getSettings<String>('language');
    this.getLangLabel();
    //grt title data
    // this.titleSevice.title$.subscribe((title) => {
    //   this.title = title;
    // });
    // this.count=this.newMessage.length;
    this.oldMessage = this.newMessage.concat(this.oldMessage);
    //end get title data
  }
  toggleCollapsed(): void {
    this.isCollapsed = !this.isCollapsed;
    this.handleCollapsed.emit(this.isCollapsed);
  }
  logout() {
    // debugger

    // localStorage.removeItem('user_idm_token');
    // localStorage.removeItem('userInfo');
    // localStorage.removeItem('companyByPermission');
    // this.authservice.logout();
    // sessionStorage.clear(); // 清除所有session值
    // localStorage.clear();
    // this.clearAllCookie();
  }
  clearAllCookie() {
    const date = new Date();
    date.setTime(date.getTime() - 10000);
    const keys = document.cookie.match(/[^ =;]+(?=\=)/g);
    // console.log('需要删除的cookie名字：' + keys);
    if (keys) {
      for (let i = keys.length; i--;) {
        document.cookie =
          keys[i] + '=0; expire=' + date.toUTCString() + '; path=/';
      }
    }
  }

  // 切换语言
  switLanguage(languevalue: any) {
    this.translateService.use(languevalue);
    localStorage.setItem('language', languevalue);
    this.getLangLabel();
  }

  getLangLabel() {
    let lang = this.languageItems.find(
      (item) => item.value == localStorage.getItem('language')
    );
    if (lang) {
      this.selectLangue = lang.lable;
    }
  }

  ngOnDestroy(): void {
    //Called once, before the instance is destroyed.
    //Add 'implements OnDestroy' to the class.
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  // createBasicNotification(template: TemplateRef<{}>){
  //   let options={
  //     nzStyle: {
  //       width: '300px',
  //       top: '25px',
  //       right: '-10px',
  //       padding: 0
  //     },
  //     nzDuration: 0
  //   };
  //   this.notification.template(template,options);
  //   this.count=0;
  //   this.dot = false
  // }
  // onClick(){
  //   let message = [
  //     {author: 'molly',message: 'you are welcome'},
  //     {author: 'molly',message: 'you are welcome'},
  //     {author: 'molly',message: 'you are welcome'},
  //   ];
  //   this.oldMessage = this.oldMessage.concat(message)
  // }
}
