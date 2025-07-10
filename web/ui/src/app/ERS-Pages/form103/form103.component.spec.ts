import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FORM104Component } from './form104.component';

describe('FORM104Component', () => {
  let component: FORM104Component;
  let fixture: ComponentFixture<FORM104Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FORM104Component]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FORM104Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
