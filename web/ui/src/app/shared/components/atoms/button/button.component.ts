import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
})
export class ButtonComponent implements OnInit {
  @Input() title;
  @Output() submitted = new EventEmitter<void>();
  constructor() {}

  ngOnInit(): void {}

  click() {
    this.submitted.emit();
  }
}
