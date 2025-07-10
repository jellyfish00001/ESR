import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
})
export class InputComponent implements OnInit {
  @Input() placeholder = 'placeholder';
  @Input() value;
  @Output() inputValue = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  inputChange(input) {
    this.inputValue.emit(input);
  }
}
