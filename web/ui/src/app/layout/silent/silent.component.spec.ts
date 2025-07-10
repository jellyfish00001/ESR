import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SilentComponent } from './silent.component';

describe('SilentComponent', () => {
  let component: SilentComponent;
  let fixture: ComponentFixture<SilentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SilentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SilentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
