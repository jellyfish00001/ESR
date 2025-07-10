import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportUberTransactionalQueryComponent } from './report-uber-transactional-query.component';

describe('FORM104Component', () => {
  let component: ReportUberTransactionalQueryComponent;
  let fixture: ComponentFixture<ReportUberTransactionalQueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReportUberTransactionalQueryComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportUberTransactionalQueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
