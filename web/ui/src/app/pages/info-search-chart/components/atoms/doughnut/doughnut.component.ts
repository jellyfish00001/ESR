import { Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'src/app/shared/classes';

@Component({
  selector: 'app-doughnut',
  templateUrl: './doughnut.component.html',
  styleUrls: ['./doughnut.component.less'],
})
export class DoughnutComponent implements OnInit {
  @Input() id: string = 'main';
  @Input() graphicName: string;
  @Input() graphicValue: string;
  @Input() dataList = [];
  @Input() colors: string[] = [];
  option;
  constructor(private chartOption: ChartOptions) {}

  ngOnInit(): void {
    this.option = this.chartOption.getDoughnutChartOption(
      this.graphicName,
      this.graphicValue,
      this.dataList,
      this.colors
    );
  }
}
