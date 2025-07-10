import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BdvirtualdepartmentComponent } from './bdvirtualdepartment.component';

describe('BdexpensedeptComponent', () => {
  let component: BdvirtualdepartmentComponent;
  let fixture: ComponentFixture<BdvirtualdepartmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BdvirtualdepartmentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BdvirtualdepartmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
