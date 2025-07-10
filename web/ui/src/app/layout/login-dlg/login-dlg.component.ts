import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
// import { AuthService } from 'src/app/shared/service/auth.service';
@Component({
  selector: 'app-login-dlg',
  templateUrl: './login-dlg.component.html',
  styleUrls: ['./login-dlg.component.scss'],
})
export class LoginDlgComponent implements OnInit {
  Langue: any;

  constructor(
    // private authService: AuthService,
     private router: Router
    ) {}

  ngOnInit(): void {
    // this.authService.login();
  }

  loginEvent() {
    // this.authService.login();
  }
}
