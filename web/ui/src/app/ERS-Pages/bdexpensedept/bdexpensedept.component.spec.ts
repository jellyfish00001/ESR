import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BdexpensedeptComponent } from './bdexpensedept.component';

describe('BdexpensedeptComponent', () => {
  let component: BdexpensedeptComponent;
  let fixture: ComponentFixture<BdexpensedeptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BdexpensedeptComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BdexpensedeptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
