import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SendmailComponent } from './sendmail.component';

const routes: Routes = [{ path: '', component: SendmailComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SendmailRoutingModule { }
