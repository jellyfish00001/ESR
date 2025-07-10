import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgxEchartsModule } from 'ngx-echarts';
import { NgZorroAntdModule } from 'src/app/ng-zorro-antd.module';
import { ChartOptions } from './classes';
import {
  ButtonComponent,
  ChartComponent,
  DatePickerRangeComponent,
  DatePickerRangeTitleComponent,
  DropDownComponent,
  InputComponent,
  InputTitleComponent,
  LabelComponent,
  SelectComponent,
  SelectTitleComponent,
  TableComponent,
  TitleComponent,
} from './components';
import { CommaLinebreakPipe } from './utils/comma-linebreak.pipe';
import { AbsolutenumPipe } from './utils/absolutenum.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgZorroAntdModule,
    NgxEchartsModule.forRoot({
      echarts: () => import('echarts'),
    }),
  ],
  declarations: [
    DatePickerRangeComponent,
    SelectComponent,
    TitleComponent,
    DatePickerRangeTitleComponent,
    SelectTitleComponent,
    ButtonComponent,
    TableComponent,
    LabelComponent,
    DropDownComponent,
    InputComponent,
    InputTitleComponent,
    ChartComponent,
    CommaLinebreakPipe,
    AbsolutenumPipe,
  ],
  exports: [
    DatePickerRangeComponent,
    SelectComponent,
    TitleComponent,
    DatePickerRangeTitleComponent,
    SelectTitleComponent,
    ButtonComponent,
    TableComponent,
    LabelComponent,
    DropDownComponent,
    InputComponent,
    InputTitleComponent,
    ChartComponent,
    CommaLinebreakPipe,
    AbsolutenumPipe
  ],
  providers: [ChartOptions],
})
export class ShareModule {}
