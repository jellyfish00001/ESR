import { Injectable } from '@angular/core';
import { WebStorageStateStore } from 'oidc-client';
import { ConfigLoaderService } from './config-loader-service';
@Injectable({
  providedIn: 'root'
})
export class EnvironmentconfigService {
  authConfig: any = {};
  constructor(private ConfigService: ConfigLoaderService) {
    this.ConfigService.loadConfig().subscribe((res) => {
      // console.log('3-config', res);
      this.authConfig = {
        language: this.ConfigService.getSettings<string[]>('language') || ['zh-CN'],
        env: this.ConfigService.getSettings<string>('env') || 'Dev',
        ismicroservices: window.location.host.includes("wzsapi") ? true : false,
        webUrl: window.location.host.includes("wzsapi") ? window.location.protocol + '//' + window.location.host + this.ConfigService.getSettings<string>('RelativePath') : window.location.protocol + '//' + window.location.host,
        userStore: new WebStorageStateStore({ store: window.localStorage }),
        ersUrl: this.ConfigService.getSettings<string>('ersUrl') ? this.ConfigService.getSettings<string>('ersUrl') : (window.location.host.includes("wzsapi") ? window.location.protocol + '//' + window.location.host + this.ConfigService.getSettings<string>('RelativePath') : window.location.protocol + '//' + window.location.host),
        BPMProxy: this.ConfigService.getSettings<string>('BPMProxy'),
        BPMSignLevel1: this.ConfigService.getSettings<string>('BPMSignLevel1'),
        BPMSignLevel2: this.ConfigService.getSettings<string>('BPMSignLevel2'),
        envirenment: this.ConfigService.getSettings<string>('envirenment'),
        // api_url: this.ConfigService.getSettings<string>('api_url'),
      }
    });
   }


}
