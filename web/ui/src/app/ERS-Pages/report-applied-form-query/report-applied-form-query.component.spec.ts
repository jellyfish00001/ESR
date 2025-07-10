import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportAppliedFormQueryComponent } from './report-applied-form-query.component';

describe('ReportAppliedFormQueryComponent', () => {
  let component: ReportAppliedFormQueryComponent;
  let fixture: ComponentFixture<ReportAppliedFormQueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReportAppliedFormQueryComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportAppliedFormQueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
