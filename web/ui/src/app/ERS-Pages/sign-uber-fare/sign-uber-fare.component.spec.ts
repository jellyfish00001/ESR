import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SignUberFareComponent } from './sign-uber-fare.component';

describe('sign-uber-fare', () => {
  let component: SignUberFareComponent;
  let fixture: ComponentFixture<SignUberFareComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SignUberFareComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SignUberFareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
