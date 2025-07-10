import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'permission-denied',
  template: `
    <h2 style="font-family: Roboto-Regular">{{'system.permission-denied'|translate}}</h2>
  `,
  styles: [``]
})
export class PermissionDeniedComponent implements OnInit {
  constructor(private translate: TranslateService) { }

  ngOnInit(): void { }
}
