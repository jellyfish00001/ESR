import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ704Component } from './rq704.component';

describe('RQ704Component', () => {
  let component: RQ704Component;
  let fixture: ComponentFixture<RQ704Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ704Component ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RQ704Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
