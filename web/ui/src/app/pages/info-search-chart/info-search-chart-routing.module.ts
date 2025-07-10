import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InfoSearchChartComponent } from './info-search-chart.component';

const routes: Routes = [{ path: '', component: InfoSearchChartComponent }, ];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InfoSearchChartRoutingModule {}
