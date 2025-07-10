import { Injectable } from '@angular/core';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { URLConst } from 'src/app/shared/const/url.const';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver';

const getBase64 = (file: File): Promise<string | ArrayBuffer | null> =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });
@Injectable({
  providedIn: 'root',
})
export class CommonService {
  constructor(
    private Service: WebApiService,
    private EnvironmentconfigService: EnvironmentconfigService,
    public domSanitizer: DomSanitizer,
    public translate: TranslateService,
    private router: Router,
    private message: NzMessageService
  ) { }

  private _userInfo: any;
  private _companyByPermission: any;

  public set setUserData(info: any) {
    this._userInfo = info;
  }

  public set setCompanyData(data: any) {
    this._companyByPermission = data;
  }

  public get getUserInfo() {
    if (localStorage.getItem('userInfo') != null) {
      this.setUserData = JSON.parse(localStorage.getItem('userInfo'));
    }
    if (!!this._userInfo) {
      this._userInfo['isMobile'] = this.CheckIsMobile();
    }
    return this._userInfo;
  }
  public get getUserCompany() {
    if (localStorage.getItem('userInfo') != null) {
      var temp = JSON.parse(localStorage.getItem('userInfo'));
      return temp.company;
    }
    return null;
  }

  public get getCompanyQueryOptionsByPermission() {
    if (localStorage.getItem('companyByPermission') != null) {
      this.setCompanyData = JSON.parse(
        localStorage.getItem('companyByPermission')
      );
    }
    // let options = this._companyByPermission;
    // if (options.length > 1) { options.push(''); }
    // options = options.sort((a, b) => a.localeCompare(b));
    // return options;
    return this._companyByPermission;
  }

  public get getCompanyAddOptionsByPermission() {
    if (localStorage.getItem('companyByPermission') != null) {
      this.setCompanyData = JSON.parse(
        localStorage.getItem('companyByPermission')
      );
    }
    return this._companyByPermission;
  }

  changeDomain(url: string): string {
    if (this.EnvironmentconfigService.authConfig.ismicroservices) {
      if (url.startsWith('https://ers-ww-web.k8s-prd.k8s.wistron.com/')) {
        url = url.replace(
          'https://smarters.wistron.com',
          this.EnvironmentconfigService.authConfig.webUrl
        );
      }
      if (url.startsWith('https://ers-ww-web.k8s-qas.k8s.wistron.com/')) {
        url = url.replace(
          'https://smartersq.wistron.com',
          this.EnvironmentconfigService.authConfig.webUrl
        );
      }
      if (url.startsWith('https://ers-ww-web.k8s-dev.k8s.wistron.com')) {
        url = url.replace(
          'https://smartersd.wistron.com',
          this.EnvironmentconfigService.authConfig.webUrl
        );
      }
    }
    return url;
  }

  getEmployeeInfo(): any {
    // company、emplid、deptid、cname、phone、curr、costdeptid、isaccount、timezone、isproxy、proxylist
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl +
      URLConst.GetEmployeeInfo,
      null
    ).subscribe((res) => {
      let result;
      if (res && res.status === 200) {
        localStorage.setItem('userInfo', JSON.stringify(res.body));
        result = res.body;
      }
      return result;
    });
  }

  getNetWorkStatus(): any {
    this.Service.doGet(URLConst.GetNetWorkStatus, null).subscribe((res) => {
      let result;
      if (res && res.status === 200) {
        localStorage.setItem('intranet', res.body);
        result = res.body;
      }
      return result;
    });
  }

  getEmployeeInfoById(userId: string): Observable<any> {
    // company、emplid、deptid、cname、phone、curr、costdeptid、isaccount、timezone、isproxy、proxylist
    return this.Service.doGet(
      URLConst.GetEmployeeInfoById + `${userId}`,
      null
    ).pipe(
      map((res) => {
        if (res && res.status === 200) {
          return res.body;
        } else {
          return null;
        }
      })
    );
  }

  getAllCompanys(): Observable<string[]> {
    return this.Service.doGet(URLConst.GetCompany, null).pipe(
      map((res) => {
        let result = [];
        if (res && res.status === 200 && !!res.body) {
          if (res.body.status == 1) {
            result = res.body.data;
          } else {
            this.message.error(res.body.message, { nzDuration: 6000 });
          }
        } else {
          this.message.error(
            res.message ?? this.translate.instant('server-error'),
            { nzDuration: 6000 }
          );
        }
        return result;
      })
    );
  }

  getOthersCompanys(): Observable<string[]> {
    return this.Service.doGet(URLConst.GetOthersCompany, null).pipe(
      map((res) => {
        let result = [];
        if (res && res.status === 200 && !!res.body) {
          result = res.body;
        } else {
          this.message.error(
            res.message ?? this.translate.instant('server-error'),
            { nzDuration: 6000 }
          );
        }
        return result;
      })
    );
  }

  getApplyQueryFormCompanys(): Observable<string[]> {
    return this.Service.doGet(URLConst.GetCompanyByArea, null).pipe(
      map((res) => {
        let result = [];
        if (res && res.status === 200 && !!res.body) {
          if (res.body.status == 1) {
            result = res.body.data;
          } else {
            this.message.error(res.body.message, { nzDuration: 6000 });
          }
        } else {
          this.message.error(
            res.message ?? this.translate.instant('server-error'),
            { nzDuration: 6000 }
          );
        }
        return result;
      })
    );
  }


  getManagerList(userId: string): Observable<string[]> {
    return this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetManager,
      { user: userId }
    ).pipe(
      map((res) => {
        if (res && res.status === 200) {
          return res.body;
        } else {
          return [];
        }
      })
    );
  }

  getEmployeeList(userId: string): Observable<string[]> {
    return this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetManager,
      { user: userId }
    ).pipe(
      map((res) => {
        if (res && res.status === 200) {
          return res.body.map((o) => {
            return {
              emplid: o.emplid,
              name: o.name,
              label: o.emplid + '/' + o.name,
            };
          });
        } else {
          return [];
        }
      })
    );
  }

  async getPicBase64(file: File): Promise<any> {
    return await getBase64(file);
  }

  getFileUrl(file: File): SafeResourceUrl {
    const fileType = file.type;
    const blob = new Blob([file], { type: fileType || 'application/*' });
    let fileUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
      window.URL.createObjectURL(blob)
    );
    return fileUrl;
  }

  FormatString(str: string, ...val: string[]) {
    for (let index = 0; index < val.length; index++) {
      str = str.replace(`{${index}}`, val[index]);
    }
    return str;
  }

  getFileData = (url, fileName, filetype): Promise<File> =>
    new Promise((resolve, reject) => {
      if (!!filetype) {
        var xhr = new XMLHttpRequest();
        var blob = null;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url);
        xhr.setRequestHeader('Accept', filetype);
        xhr.responseType = 'blob';
        xhr.onload = () => {
          if (xhr.status === 200) {
            blob = xhr.response;
            let file = new File([blob], fileName, { type: filetype });
            resolve(file);
          }
        };
        xhr.send();
      }
    });

  getFile(url, callback) {
    var blob = null;
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.setRequestHeader('Accept', 'image/jpeg');
    xhr.responseType = 'blob';
    xhr.onload = () => {
      if (xhr.status === 200) {
        blob = xhr.response;
        let imgFile = new File([blob], 'imageName', { type: 'image/jpeg' });
        console.log(imgFile);
        callback.call(this, imgFile);
      }
    };
    xhr.send();
  }
  SendMobileSignXMLData(rno: string) {
    let url = URLConst.SendMobileSignApi;
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl + url + `?rno=${rno}`,
      {}
    ).subscribe((res) => {
      console.log('抛手机签核结果' + res);
    });
  }
  Approval(data, preUrl, formData: any = null) {
    let url = URLConst.SignApi;
    let param = data;
    if (!!formData) {
      url = URLConst.Approval704;
      param = formData;
    }
    this.Service.Put(
      this.EnvironmentconfigService.authConfig.ersUrl + url,
      param
    ).subscribe((res) => {
      if (res != null && res.status === 200) {
        var result = res.body;
        if (result.status == 1) {
          this.message.success(this.translate.instant('submit-successfully'));
          if (!!preUrl) {
            this.router.navigateByUrl('ers/' + preUrl);
          } else {
            this.router.navigateByUrl('ers/form101');
          }
        } else {
          this.message.error(result.message, { nzDuration: 5000 });
        }
      } else {
        this.message.error(res.message, { nzDuration: 5000 });
      }
    });
  }
  Reject(data, preUrl) {
    this.Service.Put(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.RejectApi,
      data
    ).subscribe((res) => {
      if (res != null && res.status === 200) {
        var result = res.body;
        if (result.status == 1) {
          this.message.success(this.translate.instant('submit-successfully'));
          if (!!preUrl) {
            this.router.navigateByUrl('ers/' + preUrl);
          } else {
            this.router.navigateByUrl('ers/form101');
          }
        } else {
          this.message.error(result.message, { nzDuration: 5000 });
        }
      } else {
        this.message.error(res.message, { nzDuration: 5000 });
      }
    });
  }
  Transform(data, preUrl) {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
      URLConst.TransformSignApi,
      data
    ).subscribe((res) => {
      if (res != null && res.status === 200) {
        var result = res.body;
        if (result.status == 1) {
          this.message.success(this.translate.instant('submit-successfully'));
          if (!!preUrl) {
            this.router.navigateByUrl('ers/' + preUrl);
          } else {
            this.router.navigateByUrl('ers/form101');
          }
        } else {
          this.message.error(result.message, { nzDuration: 5000 });
        }
      } else {
        this.message.error(res.message, { nzDuration: 5000 });
      }
    });
  }
  ExportDataToExcel(header: string[], body: any, filename) {
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('sheet1');
    worksheet.addRow(header);
    for (let item of body) {
      worksheet.addRow(Object.values(item));
    }
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });
      saveAs(blob, filename);
    });
  }

  CheckIsMobile() {
    var isMobile = false;
    var userAgentInfo = navigator.userAgent;
    var Agents = [
      'Android',
      'iPhone',
      'SymbianOS',
      'Windows Phone',
      'iPad',
      'iPod',
    ];
    for (var v = 0; v < Agents.length; v++) {
      if (userAgentInfo.indexOf(Agents[v]) > 0) {
        isMobile = true;
        break;
      }
    }
    return isMobile;
  }

  formatAmountWithThousandsSeparator(amount) {
    // 转换金额为字符串
    var amountStr = amount.toString();

    // 检查是否存在小数部分
    var decimalSeparatorIndex = amountStr.indexOf(".");
    var hasDecimal = decimalSeparatorIndex !== -1;

    // 获取整数部分
    var integerPart = hasDecimal ? amountStr.substr(0, decimalSeparatorIndex) : amountStr;

    // 格式化整数部分，添加千分位分隔符
    integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    // 如果存在小数部分，则拼接整数部分和小数部分
    var formattedAmount = hasDecimal
      ? integerPart + amountStr.substr(decimalSeparatorIndex)
      : integerPart;

    return formattedAmount;
  }

  //获取本地化名称
  getLocalizedName(nameEn: any, nameZhcn: any, nameZhtw: any, nameEs: any, nameVn: any, nameCz: any): string {
    const currentLang = this.translate.currentLang;

    switch (currentLang.toLocaleLowerCase()) {
      case 'zh_cn':
        return nameZhcn || nameEn;
      case 'zh_tw':
        return nameZhtw || nameEn;
      case 'es': // 西班牙语
        return nameEs || nameEn;
      case 'vi': // 越南语
        return nameVn || nameEn;
      case 'cs': // 捷克语
        return nameCz || nameEn;
      default:
        return nameEn; // 默认英文
    }
  }

}
