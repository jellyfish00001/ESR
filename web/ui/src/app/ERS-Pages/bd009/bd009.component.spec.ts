import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BD009Component } from './bd009.component';

describe('Bd009Component', () => {
  let component: BD009Component;
  let fixture: ComponentFixture<BD009Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BD009Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BD009Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
