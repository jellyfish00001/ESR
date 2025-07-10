import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GetTitleService {
  public title$ = new Subject<string>();
  constructor() { }
  public setTitle(name: string): void {
    this.title$.next(name);
  }
}
