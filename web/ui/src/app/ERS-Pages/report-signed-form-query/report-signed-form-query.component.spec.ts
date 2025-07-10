import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportSignedFormQueryComponent } from './report-signed-form-query.component';

describe('ReportSignedFormQueryComponent', () => {
  let component: ReportSignedFormQueryComponent;
  let fixture: ComponentFixture<ReportSignedFormQueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReportSignedFormQueryComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportSignedFormQueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
