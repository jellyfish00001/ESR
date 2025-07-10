import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RQ701Component } from './rq701.component';

describe('RQ701Component', () => {
  let component: RQ701Component;
  let fixture: ComponentFixture<RQ701Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RQ701Component ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RQ701Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
