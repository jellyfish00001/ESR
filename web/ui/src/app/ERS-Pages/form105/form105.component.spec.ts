import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FORM105Component } from './form105.component';

describe('FORM105Component', () => {
  let component: FORM105Component;
  let fixture: ComponentFixture<FORM105Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FORM105Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FORM105Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
