import { Component, OnInit } from '@angular/core';
import { UserManager } from 'oidc-client';
import { EnvironmentconfigService } from 'src/app/shared/service/environmentconfig.service';


@Component({
  selector: 'app-silent',
  templateUrl: './silent.component.html',
  styleUrls: ['./silent.component.scss']
})
export class SilentComponent implements OnInit {
  // private manager: UserManager = new UserManager(this.environment.authConfig);

  constructor( private environment: EnvironmentconfigService) { }

  ngOnInit(): void {
    // this.manager.signinSilentCallback();
  }
}
