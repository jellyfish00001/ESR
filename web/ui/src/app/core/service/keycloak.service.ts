import { Injectable } from '@angular/core';
import KeyCloak from 'keycloak-js';
// import { ConfigService } from 'src/app/shared/service/config.service';

@Injectable({
  providedIn: 'root'
})
export class KeycloakService {

  keyCloak: any;

  user = {};

  constructor() { }
  init() {
    return new Promise((resolve, reject) => {
      if (!this.keyCloak) {
        let iniOption = {
          url: 'https://keycloak-prd.wistron.com/auth',
          realm: 'k8sprdwzsi40',
          clientId: 'wigps',
          onLoad: 'login-required'
        };
        this.keyCloak = KeyCloak(iniOption);
        this.keyCloak.init({ onLoad: 'login-required', "checkLoginIframe": false }).success(
          auth => {
            if (auth) {
              resolve('');
            } else {
              reject('');
            }
          }
        );
      } else resolve('');
    });
  }

  getKeycloak() {
    return this.keyCloak;
  }

  logout(): Promise<any> {
    return new Promise((resolve, reject) => {
      try {
        //localStorage.removeItem(LOCALSTORAGE.KEYS.USER);
        this.keyCloak.logout();
        resolve('');
      } catch (error) {
        this.keyCloak
          .init({
            onLoad: 'login-required',
            checkLoginIframe: false,
            promiseType: 'native'
          });
        this.keyCloak.logout();
        reject();
      }
    });
  }

}
