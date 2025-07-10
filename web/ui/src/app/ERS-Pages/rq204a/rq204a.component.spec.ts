import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ204AComponent } from './rq204a.component';

describe('RQ204AComponent', () => {
  let component: RQ204AComponent;
  let fixture: ComponentFixture<RQ204AComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RQ204AComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RQ204AComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
