import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'commaLinebreak',
  // 非纯管道
  pure: false,
})
export class CommaLinebreakPipe implements PipeTransform {

  transform(value: any, ...args: any[]): any {   

    let reg = /[,，]/g;
    let linedata = value.replace(reg, ',');
    let lines: any[] = linedata.split(',');
    return lines;
  }


}
