import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ104Component } from './rq104.component';

describe('RQ104Component', () => {
  let component: RQ104Component;
  let fixture: ComponentFixture<RQ104Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ104Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ104Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
