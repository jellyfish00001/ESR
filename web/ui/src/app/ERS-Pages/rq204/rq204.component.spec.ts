import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ204Component } from './rq204.component';

describe('RQ204Component', () => {
  let component: RQ204Component;
  let fixture: ComponentFixture<RQ204Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ204Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ204Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
