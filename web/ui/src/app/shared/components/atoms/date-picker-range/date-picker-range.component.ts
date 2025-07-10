import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import moment from 'moment';

@Component({
  selector: 'app-date-picker-range',
  templateUrl: './date-picker-range.component.html',
  styleUrls: ['./date-picker-range.component.scss'],
})
export class DatePickerRangeComponent implements OnInit {
  @Input() date = null;
  /**
   * 是否显示 time-picker
   *
   * @memberof DatePickerRangeComponent
   */
  @Input() showTime = false;
  @Output() dateRange = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  /**
   * 变更选择的时间区间
   *
   * @param {Date[]} result
   * @memberof DatePickerRangeComponent
   */
  onChange(result: Date[]): void {
    let dateRange: Array<string> = [];
    if (result.length == 0) {
      this.dateRange.emit(null);
    } else {
      if (this.showTime) {
        dateRange = [
          moment(result[0]).format('YYYY-MM-DD HH:mm:ss'),
          moment(result[1]).format('YYYY-MM-DD HH:mm:ss'),
        ];
      } else {
        dateRange = [
          moment(result[0]).format('YYYY-MM-DD'),
          moment(result[1]).format('YYYY-MM-DD'),
        ];
      }
      this.dateRange.emit(dateRange);
    }
  }
}
