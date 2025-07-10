import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[appTransToRed]'
})
export class TransToRedDirective implements AfterViewInit {
  @Input() appTransToRed: object;
  constructor(
    private el: ElementRef
  ) { }
  ngAfterViewInit(): void {
   if (this.appTransToRed) {
    this.el.nativeElement.style.color = '#FF4339';
   } else {
    this.el.nativeElement.style.color = 'rgba(255, 255, 255, 0.9)';
   }
  }
}
