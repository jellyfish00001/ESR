import { Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'src/app/shared/classes';
import { StackType } from 'src/app/shared/models';

@Component({
  selector: 'app-stack-chart',
  templateUrl: './stack-chart.component.html',
  styleUrls: ['./stack-chart.component.scss'],
})
export class StackChartComponent implements OnInit {
  @Input() id: string = 'main';
  @Input() type: StackType = 'bar';
  @Input() yAxisName: string;
  @Input() seriesData = [];
  option;
  constructor(private chartOption: ChartOptions) {}

  ngOnInit(): void {
    this.option = this.chartOption.getStackChartOption(
      this.type,
      this.yAxisName,
      this.seriesData
    );
  }
}
