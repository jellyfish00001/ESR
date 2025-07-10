import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
// import { AuthService } from 'src/app/shared/service/auth.service';

@Component({
  selector: 'app-page-not-found',
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.css']
})
export class PageNotFoundComponent implements OnInit {

  constructor(
    // private authservice: AuthService,
  ) { }

  ngOnInit() {
  }

  logout() {
    // window.location.href = '/';
    // this.authservice.logout();
    sessionStorage.clear();   // 清除所有session值
    // localStorage.clear();
    this.clearAllCookie();
  }
  clearAllCookie() {
    const date = new Date();
    date.setTime(date.getTime() - 10000);
    const keys = document.cookie.match(/[^ =;]+(?=\=)/g);
    // console.log('需要删除的cookie名字：' + keys);
    if (keys) {
      for (let i = keys.length; i--;) {
        document.cookie = keys[i] + '=0; expire=' + date.toUTCString() + '; path=/';
        // document.cookie = name + "=a; expires=" + date.toGMTString();
      }
    }
  }
}
