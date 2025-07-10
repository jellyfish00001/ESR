import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InfoSearchInputComponent } from './info-search-input.component';

const routes: Routes = [ { path: '', component: InfoSearchInputComponent },];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InfoSearchInputRoutingModule {}
