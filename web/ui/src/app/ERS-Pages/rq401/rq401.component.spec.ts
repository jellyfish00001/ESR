import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ401Component } from './rq401.component';

describe('RQ401Component', () => {
  let component: RQ401Component;
  let fixture: ComponentFixture<RQ401Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ401Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ401Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
