import { Component, OnInit } from '@angular/core';
import { ConfigService } from '@ngx-config/core';
import { User } from 'oidc-client';
// import { AuthService } from '../shared/service/auth.service';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.scss'],
})
export class CallbackComponent implements OnInit {
  // settingSrv: any;
  constructor(
    // // private authService: AuthService,
    private ConfigService: ConfigService,
    private router: Router,
    private EnvironmentconfigService: EnvironmentconfigService
  ) { }

 async ngOnInit() {
  // debugger;
  // this.authService.loginCallBack().subscribe((user: User) => {
  //   if (user) {
  //     console.log("user: ",user);
  //     localStorage.setItem(this.authService.userKey, user.toStorageString());
  //     // debugger;
  //     sessionStorage.setItem('user_idm_token', JSON.stringify(user));
  //     localStorage.setItem('user_idm_token', JSON.stringify(user));
  //     this.router.navigateByUrl(user.state);
  //       // window.location.replace(this.EnvironmentconfigService.authConfig.webUrl + '/#/pages');
  //       // let roleUrl = this.EnvironmentconfigService.authConfig.webUrl + '/#/pages';
  //       // localStorage.setItem('url', roleUrl);
  //     }
  //   });
  }
}
