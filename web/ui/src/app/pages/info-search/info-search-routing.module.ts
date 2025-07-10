import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InfoSearchComponent } from './info-search.component';

const routes: Routes = [ { path: '', component: InfoSearchComponent },];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InfoSearchRoutingModule {}
