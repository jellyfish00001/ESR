import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BdCompanyCategoryComponent } from './bdCompanyCategory.component';

describe('BdCompanyCategoryComponent', () => {
  let component: BdCompanyCategoryComponent;
  let fixture: ComponentFixture<BdCompanyCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BdCompanyCategoryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BdCompanyCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
