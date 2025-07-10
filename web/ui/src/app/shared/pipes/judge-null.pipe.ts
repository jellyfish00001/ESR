import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'judgeNull'
})
export class JudgeNullPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    if(!value && value !== 0) {
      return '--';
    }
    return value;
  }

}
