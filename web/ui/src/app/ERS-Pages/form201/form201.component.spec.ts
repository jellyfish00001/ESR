import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FORM201Component } from './form201.component';

describe('FORM201Component', () => {
  let component: FORM201Component;
  let fixture: ComponentFixture<FORM201Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FORM201Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FORM201Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
