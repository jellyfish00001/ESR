import _ from "lodash";
import { Util } from "./util";


export class UserInfo {

  // private static instance: UserInfo = null
  private static instance: UserInfo = null

  public fullName: string
  public enName: string
  public cnName: string
  public email: string
  public employeeNo: string
  public plants: Set<string>

  public isAdmin: boolean = false
  public adminPermission: number = -1
  private authService: any

  constructor(_jsonData: any, _authService?: any) {
    this.fullName = _jsonData.ename
    this.enName = _jsonData.ename
    this.cnName = _jsonData.cname
    this.email = _jsonData.email

    if (localStorage.getItem("sessionChangeUserNo") == null) {
      this.employeeNo = _jsonData.emplid.toUpperCase();
      this.enName = _jsonData.ename

      //判斷最剛開始登入者是否為DcTeam member
      localStorage.setItem("sessionOriginUser", this.employeeNo);
      localStorage.setItem("sessionOriginEnName", this.enName);
      localStorage.setItem("sessionOriginEmail", this.email);
    } else {
      this.employeeNo = localStorage.getItem("sessionChangeUserNo") ? null : localStorage.getItem("sessionChangeUserNo")
      this.enName = localStorage.getItem("sessionChangeUserName") ? null : localStorage.getItem("sessionChangeUserName")
    }

    this.plants = new Set<string>()

    if (_authService)
      this.authService = _authService

    this.isAdmin = (Util.getSession("username") == this.employeeNo)
    let permission = Util.getSession("permission")
    this.adminPermission = permission ? parseInt(permission) : 1

  }

  public static getInstance(): UserInfo {

    if (_.isEmpty(UserInfo.instance)) {
      //throw new Error("UserInfo not loaded.")
      return null;
    }

    return UserInfo.instance
  }

  public static setInstance(_jsonData: any, _authService?: any) {
    UserInfo.instance = new UserInfo(_jsonData, _authService)
  }

  //切換使用者帳號
  public static changeInstance(changeEmployeeId: string, changeEmployeeName: string): UserInfo {
    // console.log("changeEmployeeId : ",changeEmployeeId)
    UserInfo.instance.employeeNo = changeEmployeeId;
    UserInfo.instance.enName = changeEmployeeName;
    localStorage.setItem('motp', "false");
    return UserInfo.instance
  }

  public async logout() {
    try {
      localStorage.removeItem("sessionChangeUserNo");
      localStorage.removeItem("sessionChangeUserName");
      localStorage.removeItem('motp');
      localStorage.removeItem('accessToken');
      console.log(window.location.origin);
      
      await this.authService.logout(window.location.origin)

    } catch (e) {
      alert("can not logout" + e)
    }
  }

  public static setMotp(motp: any) {
    localStorage.setItem('motp', motp);
  }
}
