import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportSignedFormQueryDetailComponent } from './report-signed-form-query-detail.component';

describe('ReportSignedFormQueryDetailComponent', () => {
  let component: ReportSignedFormQueryDetailComponent;
  let fixture: ComponentFixture<ReportSignedFormQueryDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportSignedFormQueryDetailComponent ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportSignedFormQueryDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
