import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpResponse, HttpRequest, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
// import { AuthService } from './auth.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { EnvironmentconfigService } from './environmentconfig.service';
import { filter } from 'rxjs/operators';

@Injectable(
  { providedIn: 'root' }
)
export class WebApiService {
  // userLang = this.authService.userInfo.lang;
  // token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken') || '';
  constructor(private httpClient: HttpClient,
    // // private authService: AuthService
    private EnvironmentconfigService: EnvironmentconfigService,
    private messageService: NzMessageService) {
    /* 总结
    在 Angular 2 中 constructor 一般用于依赖注入或执行简单的数据初始化操作，
    ngOnInit 钩子主要用于执行组件的其它初始化操作或获取组件输入的属性值
    */

  }
  private getToken(): string {
    return localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken') || '';
  }
  private getlanguage(): string {
    const lang = localStorage.getItem('i18n') || sessionStorage.getItem('i18n') || 'zh_TW';
    //因为后端目前只有zh.json和en.json两种语言资源文件，所以中文统一返回zh
    if (lang === 'zh_CN') {
      return 'zh-CN';
    }
    if (lang === 'zh_TW') {
      return 'zh-TW';
    }
    return lang;
  }

  /**
   * Return an object of json
   * param url
   * param data
   * returns {Observable<{}>}
   */
  /**
  doPost(url: string, data: { pageindex: any; pagesize: any; orderby: any; data: any; }): Observable<any> {
      const lang = this.authService.userInfo.lang;
      const siteCode = this.authService.userInfo.sitecode;
      const timeZone = this.authService.userInfo.timezone;
      const pageIndex = data.pageindex ? data.pageindex : 0;
      const pageSize = data.pagesize ? data.pagesize : 0;
      const orderBy = data.orderby ? data.orderby : '';
      const postData = data.data ? data.data : data;

      const body: any = {
          userLang: lang, userSiteCode: siteCode, userTimeZone: timeZone,
          pageindex: pageIndex, pageSize: pageSize, orderby: orderBy, data: postData
      };
      const _headers = new HttpHeaders()
          .set('Content-Type', 'application/json; charset=utf-8')
          .set('Authorization', 'Bearer ' + this.authService.userInfo.token);
      const options = { headers: _headers };
      return this.httpClient.post(url, body, options);
  }
*/

  /**
   * Return an object of json
   * param url
   * param data
   * param isDownload:判断是否是下载
   * returns {Observable<{}>}
   */
  doPost(url, data, isDownload?: boolean): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }
    // if (!this.authService.userInfo.emplid) {
    //     this.messageService.warning(this.translate.instant('OS.PleaseSelectCompanyFirst'));
    //     return new Observable();
    // }
    // const lang = (sessionStorage.getItem("wmsmlang") == undefined ? this.authService.userInfo.lang : sessionStorage.getItem("wmsmlang"));
    // const siteCode = this.authService.userInfo.sitecode;
    // const timeZone = this.authService.userInfo.timezone;
    // const pageIndex = data.pageIndex ? data.pageIndex : 0;
    // const pageSize = data.pageSize ? data.pageSize : 0;
    // const orderBy = data.orderby ? data.orderby : '';
    // const postData = data.data ? data.data : data;
    // // flag:imbd002,003等几个程序需要
    // const flag = data.flag ? data.flag : '';

    // const body: any = {
    //     userLang: lang, userSiteCode: siteCode, userTimeZone: timeZone,
    //     pageindex: pageIndex, pageSize: pageSize, orderby: orderBy, data: postData,
    //     flag: flag
    // };

    const _headers = new HttpHeaders()
      .set('Content-Type', 'application/json; charset=utf-8')
      .set('Accept-Language', this.getlanguage()) // 设置语言头部
      .set('Authorization', this.getToken()) //'Bearer ' + this.authService.userInfo.token
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString());
    let options;
    if (isDownload) {
      options = { headers: _headers, responseType: 'blob' };
    } else {
      options = { headers: _headers };
    }
    return this.httpClient.post(url, data, options);
  };

  Post(url: string, data: any): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }

    const _headers = new HttpHeaders()
      .set('Authorization', this.getToken())
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString())
      .set('Accept-Language', this.getlanguage()); // 设置语言头部
    const req = new HttpRequest('POST', url, data, { headers: _headers });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse));
  };

  PostDB(url: string, data: any, token: any): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }

    const _headers = new HttpHeaders()
      .set('Authorization', this.getToken())
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString())
      .set('Accept-Language', this.getlanguage()); // 设置语言头部
    const req = new HttpRequest('POST', url, data, { headers: _headers });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse));
  };

  doGet(url: string, data: any): Observable<any> {

    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }

    const _headers = new HttpHeaders()
      .set('Authorization', this.getToken())
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString())
      .set('Accept-Language', this.getlanguage()); // 设置语言头部
    url = this.setUrlQuery(url, data);
    const req = new HttpRequest('GET', url, null, { headers: _headers });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse));
  }

  Put(url: string, data: any): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }
    const _headers = new HttpHeaders()
      .set('Authorization', this.getToken())
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString())
      .set('Accept-Language', this.getlanguage()); // 设置语言头部
    const req = new HttpRequest('PUT', url, data, { headers: _headers });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse));
  };

  Delete(url: string, data: any): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }
    const _headers = new HttpHeaders()
      .set('Authorization', this.getToken())
      .set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString())
      .set('Accept-Language', this.getlanguage()); // 设置语言头部
    const req = new HttpRequest('DELETE', url, data, { headers: _headers });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse));
  };
  protected _Download(url: string, data: any): Observable<any> {
    if (!url.startsWith('http')) {
      url = this.EnvironmentconfigService.authConfig.ersUrl + url;
    }
    const _headers = new HttpHeaders().set('Authorization', this.getToken()).set('responseType', 'arraybuffer').set('timezone', (0 - new Date().getTimezoneOffset() / 60).toString()).set('Accept-Language', this.getlanguage()); // 设置语言头部
    const req = new HttpRequest('POST', url, data, { headers: _headers, responseType: 'arraybuffer' });
    return this.httpClient.request(req).pipe(filter(e => e instanceof HttpResponse))
  }

  Download(url: string, data: any, fileName: string): void {
    this._Download(url, data).subscribe((res) => {
      const blob = new Blob([res.body], { type: '.csv,application/vnd.ms-excel' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      a.parentNode!.removeChild(a);
    })
  }

  setUrlQuery(url: string, data: any) {
    if (data) {
      let queryArr = [];
      for (const key in data) {
        if (data.hasOwnProperty(key)) {
          queryArr.push(`${key}=${data[key]}`)
        }
      }
      if (url.indexOf('?') !== -1) {
        url = `${url}&${queryArr.join('&')}`
      } else {
        url = `${url}?${queryArr.join('&')}`
      }
    }
    return url;
  }
}
