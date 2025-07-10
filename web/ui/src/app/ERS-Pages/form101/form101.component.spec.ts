import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Form101Component } from './form101.component';

describe('Form101Component', () => {
  let component: Form101Component;
  let fixture: ComponentFixture<Form101Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Form101Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Form101Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
