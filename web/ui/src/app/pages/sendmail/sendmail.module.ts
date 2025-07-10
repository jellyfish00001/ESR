import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SendmailRoutingModule } from './sendmail-routing.module';
import { SendmailComponent } from './sendmail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';


@NgModule({
  declarations: [
    SendmailComponent,
    
  ],
  imports: [
    CommonModule,
    SendmailRoutingModule,
    FormsModule,
    NgZorroAntdModule,
    ReactiveFormsModule,
  ]
})
export class SendmailModule { }
