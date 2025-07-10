import _ from "lodash";
import moment from "moment";

export class Util {

  public static getSession(key: string, defaultValue: string = null) {
    let ret = sessionStorage.getItem(key)
    if (!ret)
      return defaultValue
    return ret
  }


  public static initFormData(argData: any) {

    if (!argData || !argData.hasOwnProperty("employee_no"))
      return null

    let ret = new FormData()
    Object.keys(argData).forEach(d => {
      if (d == "files")
        argData[d].forEach(x => {
          ret.append(d, x)
        })
      else
        ret.append(d, argData[d])
    })

    return ret
  }

  public static checkRequiredFields(argData: any, argRequiredField: Array<string> = []): boolean {

    if (!argData)
      return false

    let keys = Object.keys(argData)
    for (let i in keys) {

      if (keys[i] in argRequiredField) {

        let v = argData[keys[i]].toString()
        if (!v || v.trim() == "") {
          return false
        }
      }
    }

    return true
  }

  public static bootstrapTableInModalHeight = window.innerHeight / 1.25 + 16

  public static formatterPattern = "YYYY/MM/DD"

  public static dateSlashPattern = "yyyy/MM/dd"

  public static dateTimeSlashPattern = "yyyy/MM/dd HH:mm:ss"

  public static formatterCurrentMonth = "YYYY-MM-01"

  public static apiPatternStart = "YYYY-MM-DD 00:00:00"

  public static apiPatternEnd = "YYYY-MM-DD 23:59:59"

  public static dateRangeMinDate = "2010-01-01"

  public static dateTimeNoDashFormatter = "YYYY-MM-DD HH:mm:ss"

  public static dateTimeFormatter = "YYYY-MM-DD_HH_mm_ss"

  public static dateFormatterDashYYYYMMDD = "YYYY-MM-DD"

  public static minDate = new Date()

  public static maxDate = new Date(new Date().getFullYear() + 1, 11, 31);

  public static dateSeperator = "~"

  public static dateRangeRegex = /\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/g

  public static changeTwoDecimal_f(x, y) {
    if (y == 0) {
      return Number.parseFloat(y).toFixed(2);
    } else {
      let result = ((x / y) * 100).toString();
      return Number.parseFloat(result).toFixed(2);
    }
  }

  public static convertDatetimeToMilliseconds(datetime: string): string {
    return _.replace(datetime, this.dateRangeRegex, (dt) => {
      return moment(dt).valueOf().toString();
    });
  }

  // 將時間區間改成起始日 00:00:00 -  結束日 23:59:59 的 millisecond
  public static convertDateStartAndEndTime(dateList: string[]): string[] {
    return [Util.convertDatetimeToMilliseconds(moment(dateList[0]).format('YYYY-MM-DD 00:00:00')),
    Util.convertDatetimeToMilliseconds(moment(dateList[1]).format('YYYY-MM-DD 23:59:59'))];
  }

  public static hasKeyAndValueInNestedObjectList(menus: any[], key: string, value: string): boolean {
    return _.some(menus, (menu) => {
      if (menu[key] === value) {
        return true;
      } else if (menu.children) {
        return this.hasKeyAndValueInNestedObjectList(menu.children, key, value);
      }
      return false;
    });
  }

}
