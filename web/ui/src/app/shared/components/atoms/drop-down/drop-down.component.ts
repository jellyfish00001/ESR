import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-drop-down',
  templateUrl: './drop-down.component.html',
  styleUrls: ['./drop-down.component.scss'],
})
export class DropDownComponent implements OnInit {
  /**
   * 下拉菜单清单
   *
   * @type {string[]}
   * @memberof DropDownComponent
   */
  @Input() listOfAction: string[] = [];
  @Output() selectAction = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {}

  click(index) {
    this.selectAction.emit(index);
  }
}
