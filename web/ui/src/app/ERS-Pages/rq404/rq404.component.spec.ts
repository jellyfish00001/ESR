import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ404Component } from './rq404.component';

describe('RQ404Component', () => {
  let component: RQ404Component;
  let fixture: ComponentFixture<RQ404Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ404Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ404Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
