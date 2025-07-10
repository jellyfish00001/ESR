import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ501Component } from './rq501.component';

describe('RQ501Component', () => {
  let component: RQ501Component;
  let fixture: ComponentFixture<RQ501Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ501Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ501Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
