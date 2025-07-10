import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-input-title',
  templateUrl: './input-title.component.html',
  styleUrls: ['./input-title.component.scss'],
})
export class InputTitleComponent implements OnInit {
  @Input() title;
  @Input() placeholder;
  @Input() value;
  @Output() inputValue = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  inputChange(input) {
    this.value = input;
    this.inputValue.emit(input);
  }
}
