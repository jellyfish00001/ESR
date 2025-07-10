import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportAppliedFormQueryDetailComponent } from './report-applied-form-query-detail.component';

describe('ReportAppliedFormQueryDetailComponent', () => {
  let component: ReportAppliedFormQueryDetailComponent;
  let fixture: ComponentFixture<ReportAppliedFormQueryDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportAppliedFormQueryDetailComponent ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportAppliedFormQueryDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
