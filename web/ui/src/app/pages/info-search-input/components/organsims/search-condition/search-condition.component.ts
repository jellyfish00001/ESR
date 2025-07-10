import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SearchSelectModel } from 'src/app/shared/models';

@Component({
  selector: 'app-search-condition',
  templateUrl: './search-condition.component.html',
  styleUrls: ['./search-condition.component.scss'],
})
export class SearchConditionComponent implements OnInit {
  @Output() searchList = new EventEmitter<any>();
  selectedValueList = [];

  constructor() {}

  ngOnInit(): void {
  }

  enterSearch(value) {
    this.searchList.emit(value);
  }
}
