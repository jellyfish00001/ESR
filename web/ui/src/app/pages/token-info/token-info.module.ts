import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TokenInfoRoutingModule } from './token-info-routing.module';
import { TokenInfoComponent } from './token-info.component';
import { ShareModule } from 'src/app/shared/share-module.module';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
@NgModule({
  declarations: [TokenInfoComponent],
  imports: [
    ReactiveFormsModule,
    FormsModule,
    AngularMultiSelectModule,
    CommonModule,
    TokenInfoRoutingModule,
    ShareModule,
  ],
})
export class TokenInfoModule {}
