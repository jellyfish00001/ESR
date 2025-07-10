import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ404AComponent } from './rq404a.component';

describe('RQ404AComponent', () => {
  let component: RQ404AComponent;
  let fixture: ComponentFixture<RQ404AComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ404AComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ404AComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
