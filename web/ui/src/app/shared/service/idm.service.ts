import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from '@ngx-config/core';
import { TranslateService } from '@ngx-translate/core';
import { ReplaySubject } from 'rxjs';
import { AuthService } from './auth.service';
import { EnvironmentconfigService } from './environmentconfig.service';
@Injectable({
  providedIn: 'root',
})
export class IdmService {
  receiveMsgSender: ReplaySubject<any> = new ReplaySubject<any>(1);

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private idmConfig: EnvironmentconfigService,
    private configService: ConfigService,
    private translateService: TranslateService
  ) {}
  getAuthority(): Promise<string[]> {
    return new Promise((resolve, reject) => {
      let auth = [];
      this.http
        .get(
          `${this.idmConfig.authConfig.api_url}/PortalAPI/User/GetUserAclByClient`,
          {
            headers: {
              authorization: '' + this.authService.authorizationHeader,
            },
            params: { clientId: 'ers' },
          }
        )
        .subscribe((res) => {
          for (let i in res) {
            auth[i] = res[i].permissionRequired;
          }
          resolve(auth);
        });
    });
  }

  public getWigps() {
    const WiGPS_URL = this.configService.getSettings<string>('WiGPS_URL');
    top.postMessage('ok', WiGPS_URL);
    window.addEventListener('message', (event) => {
      if (event.origin !== WiGPS_URL) {
        return;
      } else {
        this.receiveMsg(event.data);
      }
    });
  }

  private receiveMsg(data) {
    this.receiveMsgSender.next(data);
    this.translateService.use(data.lang);
    localStorage.setItem('language', data.lang);
  }
}
