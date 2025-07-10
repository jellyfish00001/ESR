import { Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'src/app/shared/classes';
import { BarChartOrientType, DataZoomOrientType } from 'src/app/shared/models';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.scss'],
})
export class BarChartComponent implements OnInit {
  @Input() id: string = 'main';
  @Input() axisName: string;
  @Input() barChartOrient: BarChartOrientType = 'vertical';
  @Input() dataZoomOrient: DataZoomOrientType = 'horizontal';
  @Input() startValue: string | number;
  @Input() endValue: string | number;
  @Input() seriesData = [];
  option;
  constructor(private chartOption: ChartOptions) {}

  ngOnInit(): void {
    let option = this.chartOption.getBarChartOption(
      this.axisName,
      this.seriesData,
      this.barChartOrient,
    );
    this.option = this.chartOption.setDataZoom(
      option,
      this.startValue,
      this.endValue,
      this.dataZoomOrient,
    );
  }
}
