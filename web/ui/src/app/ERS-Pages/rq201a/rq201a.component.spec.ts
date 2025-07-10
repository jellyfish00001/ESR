import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ201AComponent } from './rq201a.component';

describe('RQ201AComponent', () => {
  let component: RQ201AComponent;
  let fixture: ComponentFixture<RQ201AComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ201AComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ201AComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
