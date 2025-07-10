import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TokenInfoComponent } from './token-info.component';

const routes: Routes = [ { path: '', component: TokenInfoComponent },];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TokenInfoRoutingModule { }
