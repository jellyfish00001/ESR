import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IconsProviderModule } from 'src/app/icons-provider.module';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { ShareModule } from 'src/app/shared/share-module.module';
import { SearchConditionComponent } from './components/organsims';
import { InfoSearchRoutingModule } from './info-search-routing.module';
import { InfoSearchComponent } from './info-search.component';

@NgModule({
  imports: [
    InfoSearchRoutingModule,
    IconsProviderModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    NgZorroAntdModule,
    ShareModule,
  ],
  declarations: [InfoSearchComponent, SearchConditionComponent],
  exports: [],
})
export class InfoSearchModule {}
