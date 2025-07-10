import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { RouterModule } from '@angular/router';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { TopNavbarComponent } from './components/top-navbar/top-navbar.component';
// import { SideNavbarComponent } from './components/side-navbar/side-navbar.component';
import { LayoutComponent } from './layout.component';
// import { SilentComponent } from './silent/silent.component';
// import { LoginDlgComponent } from './login-dlg/login-dlg.component';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgZorroAntdModule } from '../ng-zorro-antd.module';
import { NgxEchartsModule } from 'ngx-echarts';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { HeaderComponent } from "./header/header.component";
import { MenuComponent } from "./menu/menu.component";

export function createTranslateHttpLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}


@NgModule({
  declarations: [
    LayoutComponent,
    MenuComponent,
    HeaderComponent
    // TopNavbarComponent,
    // SideNavbarComponent,
    // SilentComponent,
    // LoginDlgComponent,

  ],
  imports: [
    CommonModule,
    NzLayoutModule,
    RouterModule,
    NzIconModule,
    NzMenuModule,
    NzDropDownModule,
    NzBadgeModule,
    //I18N
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateHttpLoader,
        deps: [HttpClient],
      },
    }),
    // ERSPagesModule,
    NgZorroAntdModule
  ],
  providers: [
    NgZorroAntdModule,
    NgxEchartsModule,
  ],
  exports: [LayoutComponent]
})
export class LayoutModule { }
