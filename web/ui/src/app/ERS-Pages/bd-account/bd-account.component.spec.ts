import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Bd05Component } from './bd05.component';

describe('Bd05Component', () => {
  let component: Bd05Component;
  let fixture: ComponentFixture<Bd05Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Bd05Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Bd05Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
