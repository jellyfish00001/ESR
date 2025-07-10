import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoSearchChartComponent } from './info-search-chart.component';

describe('InfoSearchChartComponent', () => {
  let component: InfoSearchChartComponent;
  let fixture: ComponentFixture<InfoSearchChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InfoSearchChartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoSearchChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
