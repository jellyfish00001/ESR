import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Py002Component } from './py002.component';

describe('Py002Component', () => {
  let component: Py002Component;
  let fixture: ComponentFixture<Py002Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Py002Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Py002Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
