import { LayoutModule } from './layout/layout.module';
import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, Inject, LOCALE_ID, NgModule } from '@angular/core';
import { DecimalPipe, registerLocaleData, LocationStrategy, HashLocationStrategy, APP_BASE_HREF } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule, HttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
// animations
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// i18n
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NZ_I18N, zh_CN } from 'ng-zorro-antd/i18n';
import zh from '@angular/common/locales/zh';
// NG-ZORRO---NgZorroAntdModule废弃,需要一个个模块的引入,故统一在shareModule引入Zorro需要依赖module
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { ShareModule } from 'src/app/shared/share-module.module';
import { IconsProviderModule } from './icons-provider.module';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
// import { NoopInterceptor } from './shared/service/noop-interceptor';
import { NgxEchartsModule } from 'ngx-echarts';
import * as echarts from 'echarts';
import { MatomoModule } from 'ngx-matomo';
import { environment } from 'src/environments/environment';
import { ConfigLoader, ConfigModule, ConfigService } from '@ngx-config/core';
import { ConfigHttpLoader } from '@ngx-config/http-loader';
import { CryptoService } from 'src/app/shared/service/crypto.service';
// import { CallbackComponent } from './callback/callback.component';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
import { LoginModule } from './pages/login/login.module';

import { MsalModule, MsalRedirectComponent, MsalInterceptor, MsalGuard, MsalBroadcastService, MsalService, MSAL_INSTANCE} from '@azure/msal-angular';
import { PublicClientApplication, InteractionType } from '@azure/msal-browser';
import { AADInitService } from './shared/service/aad-init.service';
import { ConfigLoaderService } from './shared/service/config-loader-service';
import { CodeHttpInterceptor } from './core/interceptors/code-http-interceptor';
// import { MSAL_GUARD_CONFIG, MsalGuardConfiguration } from '@azure/msal-angular';
// import { catchError, tap, throwError } from 'rxjs';
const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

// const clientId = "7e520eb7-5705-403b-a7f3-3a9542f5bae9";// 替换为你的 AAD 应用程序的客户端 ID
// const authority = "https://login.microsoftonline.com/de0795e0-d7c0-4eeb-b9bb-bc94d8980d3b";// 替换为你的租户 ID

registerLocaleData(zh);
export function configFactory(http: HttpClient): ConfigLoader {
  return new ConfigHttpLoader(http, './assets/config.json'); // API ENDPOINT
}
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function AppConfigLoaderFactory(configLoaderService: ConfigLoaderService): () => Promise<void> {
  return () => {
    return configLoaderService.loadConfig().toPromise().then(config => {
      (window as any).__APP_CONFIG__ = config; // 全局变量
    }).catch(error => {
      console.error('Failed to load configuration:', error);
      return Promise.reject(error);
    });
  };
}
// export function MSALInstanceFactory(): PublicClientApplication {
//   const config = (window as any).__APP_CONFIG__;
//   if (!config) {
//     throw new Error('Config is undefined. Ensure APP_INITIALIZER has loaded the configuration.');
//   }
//   return new PublicClientApplication({
//     auth: {
//       clientId: (window as any).__APP_CONFIG__.aad.clientId,
//       authority:(window as any).__APP_CONFIG__.aad.authority,
//       redirectUri: '/',
//       postLogoutRedirectUri: '/login',
//       navigateToLoginRequestUrl: false,
//     },
//     cache: {
//       cacheLocation: 'localStorage',
//       storeAuthStateInCookie: false, // 针对旧版浏览器
//     },
//   });
// }
@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    AngularMultiSelectModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    // LayoutModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    NgxEchartsModule.forRoot({ // 必须要静态注入，子模块可直接 import 即可
      echarts
    }),
    NgZorroAntdModule,
    ShareModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
    MatomoModule,
    ConfigModule.forRoot({
      provide: ConfigLoader,
      useFactory: (configFactory),
      deps: [HttpClient]
    }),
    MsalModule.forRoot(new PublicClientApplication({
      auth: {
        clientId: '7e520eb7-5705-403b-a7f3-3a9542f5bae9', // 替换为你的 AAD 应用程序的客户端 ID
        authority: 'https://login.microsoftonline.com/de0795e0-d7c0-4eeb-b9bb-bc94d8980d3b' , // 从全局配置中获取 // 替换为你的租户 ID
        redirectUri: '/',
        postLogoutRedirectUri: '/login',
        navigateToLoginRequestUrl: false,
      },
      cache: {
        cacheLocation: 'localStorage',
        storeAuthStateInCookie: isIE, // Set to true for Internet Explorer 11
      }
    }), {
      interactionType: InteractionType.Redirect, // MSAL Guard Configuration
    },
      {
        interactionType: InteractionType.Redirect,
        protectedResourceMap: new Map([
          ["*", ["user.read"]],    // url都会塞入authorization
        ]),
      }
    )
  ],
  providers: [
    // {
    //   provide: APP_INITIALIZER,
    //   useFactory: AppConfigLoaderFactory,
    //   deps: [ConfigLoaderService],
    //   multi: true
    // },
    {
      provide: APP_INITIALIZER,
      useFactory: (aadInit: AADInitService) => () => aadInit.initData(),
      deps: [AADInitService],
      multi: true,
    },

    // MSAL instance configuration based on loaded config
    // {
    //   provide: MSAL_INSTANCE,
    //   useFactory: MSALInstanceFactory,
    //   // deps: [AADInitService]
    // },
    // {
    //   provide: MSAL_GUARD_CONFIG,
    //   useFactory: () => {
    //     return {
    //       interactionType: InteractionType.Redirect, // MSAL Guard Configuration
    //       protectedResourceMap: new Map([
    //         ["*", ["user.read"]], // 保护的资源和作用域
    //       ]),
    //     };
    //   },
    // },
    MsalGuard,
    MsalService,
    MsalBroadcastService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CodeHttpInterceptor,
      multi: true,
    },
    { provide: NZ_I18N, useValue: zh_CN },
    DecimalPipe,
    { provide: CryptoService },
    { provide: LocationStrategy, useClass: HashLocationStrategy }
  ],
  bootstrap: [AppComponent,MsalRedirectComponent]
})
export class AppModule {
  constructor(private i18n: TranslateService, @Inject(LOCALE_ID) locale: string) {
    if (environment.supportedLocale.indexOf(locale) === -1) {
      if (localStorage.getItem('language')) {
        locale = localStorage.getItem('language');
      } else {
        locale = "zh_TW";//"language":[{"lable":"English","value":"en"},{"lable":"简体中文","value":"zh_CN"},{"lable":"繁體中文","value":"zh_TW"}] 初始化
      }
    }
    this.i18n.use(locale);
  }
}
