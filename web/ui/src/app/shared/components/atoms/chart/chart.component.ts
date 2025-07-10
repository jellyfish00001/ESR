import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'src/app/shared/classes';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss'],
})
export class ChartComponent implements OnInit, AfterViewInit {
  @Input() id: string = 'main';
  @Input() option;

  constructor(private chartOption: ChartOptions) {}

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    //Called after ngAfterContentInit when the component's view has been initialized. Applies to components only.
    //Add 'implements AfterViewInit' to the class.
    this.chartOption.setChart(this.id, this.option);
  }
}
