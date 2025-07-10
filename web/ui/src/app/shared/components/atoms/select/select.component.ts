import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss'],
})
export class SelectComponent implements OnInit {
  /**
   * 下拉选单清单
   *
   * @memberof SelectComponent
   */
  @Input() listOfOption = ['Option 01', 'Option 02'];
  @Input() selectedValue = null;
  @Output() select = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {}

  selectChange(select) {
    this.select.emit(select);
  }
}
