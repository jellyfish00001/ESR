import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IconsProviderModule } from 'src/app/icons-provider.module';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { ShareModule } from 'src/app/shared/share-module.module';
import { BarChartComponent, DoughnutComponent, SearchConditionComponent, StackChartComponent } from './components';
import { InfoSearchChartRoutingModule } from './info-search-chart-routing.module';
import { InfoSearchChartComponent } from './info-search-chart.component';

@NgModule({
  imports: [
    InfoSearchChartRoutingModule,
    IconsProviderModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    NgZorroAntdModule,
    ShareModule,
  ],
  declarations: [
    InfoSearchChartComponent,
    SearchConditionComponent,
    DoughnutComponent,
    BarChartComponent,
    StackChartComponent,
  ],
  exports: [],
})
export class InfoSearchChartModule {}
