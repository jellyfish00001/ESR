import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IconsProviderModule } from 'src/app/icons-provider.module';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { ShareModule } from 'src/app/shared/share-module.module';
import { SearchConditionComponent } from './components';
import { InfoSearchInputRoutingModule } from './info-search-input-routing.module';
import { InfoSearchInputComponent } from './info-search-input.component';

@NgModule({
  imports: [
    InfoSearchInputRoutingModule,
    IconsProviderModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    NgZorroAntdModule,
    ShareModule,
  ],
  declarations: [InfoSearchInputComponent, SearchConditionComponent],
  exports: [],
})
export class InfoSearchInputModule {}
