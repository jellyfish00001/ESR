import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  private menuBurgerStatus$: BehaviorSubject<boolean> = new BehaviorSubject(false);

  constructor() { }

  public set menuStatus(isCollapsed: boolean) {
    this.menuBurgerStatus$.next(isCollapsed);
  }

  public get menuStatus(): boolean {
    return this.menuBurgerStatus$.getValue();
  }
}
