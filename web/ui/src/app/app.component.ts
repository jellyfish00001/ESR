import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { MatomoInjector } from 'ngx-matomo';
// import { IdmService } from './shared/service/idm.service';
import { TranslateService } from '@ngx-translate/core';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { ConfigService } from '@ngx-config/core';
import { ConfigLoaderService } from './shared/service/config-loader-service';
// // import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'ERS';
  languageItems;

  constructor(
    private titleService: Title,
    private readonly matomoInjector: MatomoInjector,
    private translateService: TranslateService,
    // private EnvironmentconfigService: EnvironmentconfigService,
    private ConfigService: ConfigLoaderService,
    // private idmService: IdmService,
    // private authService: AuthService
  ) {
    this.titleService.setTitle(this.title);
    this.initMatomo();
  }

  async ngOnInit() {
    //this.idmService.getWigps();
    let currentLang = localStorage.getItem('language') || this.translateService.getBrowserCultureLang();
    this.languageItems = this.ConfigService.getSettings<String>('language');
    let langs = this.languageItems.map(o=>{return o.value});
    if (!langs.includes(currentLang)){
      if (currentLang.startsWith("zh")){
        currentLang = "zh_TW";
      }
      else{
        currentLang = "en";
      }
    }
    this.translateService.use(currentLang);
    // this.authService.login();
  }

  initMatomo(): void {
    // this.matomoInjector.init(`https://matomo.wistron.com/`, 99999);
  }
}
