import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/shared/service/common.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
  // isCollapsed: boolean;
  // width: number;
  public userName: any;
  public userId: any;
  constructor(private commonSrv: CommonService) { }

  // handleCollapsed(ev): void {
  //   this.isCollapsed = ev;
  // }
  ngOnInit() {
    // debugger
    // this.width = window.innerWidth > 580 ? 64 : 0;
    if(localStorage.getItem('intranet') == null){
      this.commonSrv.getNetWorkStatus();
    }
    if (localStorage.getItem('userInfo') == null || JSON.parse(localStorage.getItem('userInfo')).emplid != this.userId) {
      this.commonSrv.getEmployeeInfo();
    }
  }
}
