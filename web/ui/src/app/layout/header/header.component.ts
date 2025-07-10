import { Component, OnInit } from '@angular/core';
// import { groupList } from 'src/app/common/constant';
import { TranslateService } from '@ngx-translate/core';
import _ from 'lodash';
import { UserInfo } from 'src/app/common/user-info';
import { LayoutService } from '../layout.service';
import { HeaderService } from './header.service';
import { HttpClient, HttpParams } from '@angular/common/http';


// import { AADInitService } from 'src/app/shared/service/aad-init.service';


interface LanguageSetting {
  locateName: string,
  displayName: string,
  imageSource: string,
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  canChangeUser = false;

  // groupList = groupList;
  languageSettingList: LanguageSetting[] = [
    {
      locateName: 'zh_CN',
      displayName: '简',
      imageSource: '../../../assets/images/Icon/ic_ch.svg',
    },
    {
      locateName: 'zh_TW',
      displayName: '繁',
      imageSource: '../../../assets/images/Icon/ic_tw.svg',
    },
    {
      locateName: 'en',
      displayName: 'en',
      imageSource: '../../../assets/images/Icon/ic_en.svg',
    },
  ]


  // -----IDM信息
  // userName: any;
  // nameChar: any;
  // subscription: Subscription;

  constructor(
    private translate: TranslateService,
    private layoutService: LayoutService,
    public headerService: HeaderService,
  ) {

    this.translate.use(this.headerService.language);
  }

  ngOnInit() {

  }

  signout() {
    UserInfo.getInstance().logout();
  }

  setI18n(language: any) {
    this.translate.use(language);
    // 记录当前设置的语言
    localStorage.setItem('language', language);
    localStorage.setItem('i18n', language);
    this.headerService.language = language;
  }

  toggleCollapsed(): void {
    this.layoutService.menuStatus = !this.layoutService.menuStatus;
  }

  get languageSetting(): LanguageSetting {
    return _.find(this.languageSettingList, (languageSetting: LanguageSetting) => languageSetting.locateName === this.headerService.language);
  }


  // async isSwitchGroupMember() {
  //   const params = new HttpParams().set('groupName', this.groupList.Switch_Account_Team).set('userId', localStorage.getItem('sessionOriginUser'));
  //   await this.httpClient.get<any>(this.PERMISSION + 'isGroupMember', {params: params}).toPromise().then
  //   (
  //     async res => {
  //       if (res.code === 0) {
  //         this.canChangeUser = res.data;
  //       }
  //     }
  //   )

  //   return false;
  // }

}
