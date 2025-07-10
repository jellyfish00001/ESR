import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SearchSelectModel } from 'src/app/shared/models';

@Component({
  selector: 'app-search-condition',
  templateUrl: './search-condition.component.html',
  styleUrls: ['./search-condition.component.scss'],
})
export class SearchConditionComponent implements OnInit {
  @Input() nzOptionSelectList:SearchSelectModel[] = [];
  @Input() date = null;
  @Output() searchList = new EventEmitter<any>();
  selectedValueList = [];

  constructor() {}

  ngOnInit(): void {
    this.nzOptionSelectList.forEach((item) => {
      this.selectedValueList.push({
        label: item.label,
        value: item.default,
        default: item.default,
      });
    });
  }

  getDateRange(date) {
    this.date = date;
  }

  getSelectValue(value, index) {
    this.selectedValueList[index].value = value;
  }

  enterSearch() {
    this.searchList.emit({
      date: this.date,
      select: this.selectedValueList,
    });
  }
}
