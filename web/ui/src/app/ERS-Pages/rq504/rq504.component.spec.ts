import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ504Component } from './rq504.component';

describe('RQ504Component', () => {
  let component: RQ504Component;
  let fixture: ComponentFixture<RQ504Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ504Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ504Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
