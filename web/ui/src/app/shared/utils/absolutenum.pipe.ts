import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'absolutenum',
   // 非纯管道
  pure: false

})
export class AbsolutenumPipe implements PipeTransform {

transform(value: any, ...args: any[]): any {
    let plus;
    plus = Math.abs(value);
    return plus;
  }
}
