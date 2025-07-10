import { TransToRedDirective } from './trans-to-red.directive';
import { NgModule } from '@angular/core';



@NgModule({
  declarations: [
    TransToRedDirective
  ],
  exports: [
    TransToRedDirective
  ]
})
export class CommonDirectiveModule { }
