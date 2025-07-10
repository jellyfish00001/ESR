import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ201Component } from './rq201.component';

describe('RQ201Component', () => {
  let component: RQ201Component;
  let fixture: ComponentFixture<RQ201Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ201Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ201Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
