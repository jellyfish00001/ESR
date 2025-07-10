import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BdExpenseSenarioComponent } from './bd-expense-senario.component';

describe('BdExpenseSenarioComponent', () => {
  let component: BdExpenseSenarioComponent;
  let fixture: ComponentFixture<BdExpenseSenarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BdExpenseSenarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BdExpenseSenarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
