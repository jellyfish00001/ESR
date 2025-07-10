import { JudgeNullPipe } from './judge-null.pipe';
import { NgModule } from '@angular/core';



@NgModule({
  declarations: [
    JudgeNullPipe
  ],
  exports: [
    JudgeNullPipe,
  ]
})
export class CommonPipeModule { }
