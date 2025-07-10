import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SearchSelectModel } from 'src/app/shared/models';

@Component({
  selector: 'app-search-condition',
  templateUrl: './search-condition.component.html',
  styleUrls: ['./search-condition.component.scss'],
})
export class SearchConditionComponent implements OnInit {
  @Input() nzOptionSelectList: SearchSelectModel[] = [];
  @Input() date = null;
  @Output() searchList = new EventEmitter<any>();
  listOfAction = ['a1', 'a2'];
  searchInfo = {
    date: this.date,
    select: [],
    actionIndex: null,
    keyWord: null
  }
  constructor() {}

  ngOnInit(): void {
    this.nzOptionSelectList.forEach((item) => {
      this.searchInfo.select.push({
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
    this.searchInfo.select[index].value = value;
  }

  enterSearch(keyWord) {
    this.searchInfo.keyWord = keyWord;
    this.searchList.emit(this.searchInfo);
  }

  getDropDownIndex(index) {
    this.searchInfo.actionIndex = index;
  }
}
