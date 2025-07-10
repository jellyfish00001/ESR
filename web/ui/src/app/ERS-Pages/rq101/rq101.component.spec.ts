import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ101Component } from './rq101.component';

describe('RQ101Component', () => {
  let component: RQ101Component;
  let fixture: ComponentFixture<RQ101Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ101Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ101Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
