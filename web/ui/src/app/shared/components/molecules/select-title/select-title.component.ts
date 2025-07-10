import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-select-title',
  templateUrl: './select-title.component.html',
  styleUrls: ['./select-title.component.scss'],
})
export class SelectTitleComponent implements OnInit {
  @Input() title;
  @Input() listOfOption;
  @Input() defaultValue;
  @Output() selectValue = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  getSelectValue(value) {
    this.selectValue.emit(value);
  }
}
