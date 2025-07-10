import _ from "lodash";

export const sortArray = (key) => {
  return (a, b) => {
    if (typeof a[key] == 'string') {
      return a[key].localeCompare(b[key]);
    } else if (typeof a[key] == 'object' && a[key] instanceof Date) {
      return a[key].getTime() - b[key].getTime();
    } else if (typeof a[key] == 'number') {
      return a - b;
    }
  }
}

export class ArrayUtil {
  // 尋找是否有重複的物件
  public static hasDuplicateObjects<T>(arr: T[], obj: T): boolean {
    return !_.isEmpty(_.find(arr, obj));
  }
}
