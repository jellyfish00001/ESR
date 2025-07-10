import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Py001Component } from './py001.component';

describe('Py001Component', () => {
  let component: Py001Component;
  let fixture: ComponentFixture<Py001Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Py001Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Py001Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
