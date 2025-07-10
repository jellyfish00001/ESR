import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'note-table',
  templateUrl: './note-table.component.html',
})
export class NoteTableTemplateComponent implements OnInit {
  @Input() curr: any;
  @Input() amountList: any = { insideSupervisor: 0, insideElse: 0, outsideSupervisor: 0, outsideElse: 0 };

  constructor(
  ) { }

  ngOnInit(): void {
  }
}
