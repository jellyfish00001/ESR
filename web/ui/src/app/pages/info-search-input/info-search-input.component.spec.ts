import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoSearchInputComponent } from './info-search-input.component';

describe('InfoSearchInputComponent', () => {
  let component: InfoSearchInputComponent;
  let fixture: ComponentFixture<InfoSearchInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InfoSearchInputComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoSearchInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
