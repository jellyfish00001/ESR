import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-date-picker-range-title',
  templateUrl: './date-picker-range-title.component.html',
  styleUrls: ['./date-picker-range-title.component.scss'],
})
export class DatePickerRangeTitleComponent implements OnInit {
  @Input() title;
  @Input() date = null;
  @Input() showTime = false;
  @Output() dateRange = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  getDateRange(date) {
    this.dateRange.emit(date);
  }
}
