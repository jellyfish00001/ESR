import { DOUGHNUT_COLOR, STACK_COLOR } from 'src/app/shared/constants';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchSelectModel } from 'src/app/shared/models';
import { GetTitleService } from 'src/app/shared/service';
import { MODEL_LIST, PRODUCT_LIST, STAGE_LIST, STATUS_LIST } from './constants';

@Component({
  selector: 'app-info-search-chart',
  templateUrl: './info-search-chart.component.html',
  styleUrls: ['./info-search-chart.component.scss'],
})
export class InfoSearchChartComponent implements OnInit {
  dateStr = null;

  nzOptionSelectList: SearchSelectModel[] = [
    {
      label: 'Status',
      optionalValue: ['All', ...STATUS_LIST],
      default: 'All',
    },
    {
      label: '客戶產品別',
      optionalValue: ['All', ...PRODUCT_LIST],
      default: 'All',
    },
    {
      label: 'Model name',
      optionalValue: ['All', ...MODEL_LIST],
      default: 'All',
    },
    {
      label: '站別',
      optionalValue: ['All', ...STAGE_LIST],
      default: 'All',
    },
  ];

  doughnutList = [
    {
      id: 'A1',
      graphicName: 'OOB',
      graphicValue: '6243',
      dataList: [
        { value: 1048, name: '搜索引擎' },
        { value: 735, name: '直接访问' },
        { value: 580, name: '邮件营销' },
        { value: 484, name: '联盟广告' },
        { value: 300, name: '视频广告' },
      ],
      colors: DOUGHNUT_COLOR,
    },
    {
      id: 'A2',
      graphicName: 'OOB2',
      graphicValue: '6243',
      dataList: [
        { value: 348, name: '搜索引擎' },
        { value: 1035, name: '直接访问' },
        { value: 580, name: '邮件营销' },
        { value: 484, name: '联盟广告' },
      ],
      colors: STACK_COLOR,
    },
  ];

  barChartList = [{
    id: 'barChart_vertical',
    axisName: '費用(RMB)',
    barChartOrient:'vertical',
    dataZoomOrient:'horizontal',
    startValue: 1,
    endValue: 10,
    seriesData: [
      [ 'label',
        'Mon',
        'Tue',
        'Wed',
        'Thu',
        'Fri',
        'Sat',
        'Sun',
        'M1',
        'M2',
        'M3',
        'M4',
      ],
      ['value',120, 200, 150, 80, 70, 110, 130, 200, 150, 80, 70],
    ],
  },{
    id: 'barChart_horizontal',
    axisName: '費用(RMB)',
    barChartOrient:'horizontal',
    dataZoomOrient:'vertical',
    startValue: 0,
    endValue: 4,
    seriesData: [
      [ 'label',
        'Mon',
        'Tue',
        'Wed',
        'Thu',
        'Fri',
        'Sat',
        'Sun',
        'M1',
        'M2',
        'M3',
        'M4',
      ],
      ['value',120, 200, 150, 80, 70, 110, 130, 200, 150, 80, 70],
    ],
  }];

  stackChartList = [
    {
      id: 'stackBarChart',
      type: 'bar',
      yAxisName: '故障次數',
      seriesData: [
        ['搜索引擎', '周一', '周二', '周三', '周四', '周五', '周六', '周日'],
        ['百度', 620, 732, 701, 734, 1090, 1130, 1120],
        ['谷歌', 120, 132, 101, 134, 290, 230, 220],
        ['必应', 60, 72, 71, 74, 190, 130, 110],
        ['其他', 62, 82, 91, 84, 109, 110, 120],
      ],
    },
    {
      id: 'stackLineChart',
      type: 'line',
      yAxisName: 'Issue停機時間(hr)',
      seriesData: [
        ['搜索引擎', '周一', '周二', '周三', '周四', '周五', '周六', '周日'],
        ['百度', 620, 732, 701, 734, 1090, 1130, 1120],
        ['谷歌', 120, 132, 101, 134, 290, 230, 220],
        ['必应', 60, 72, 71, 74, 190, 130, 110],
        ['其他', 62, 82, 91, 84, 109, 110, 120],
      ],
    },
  ];

  constructor(
    public activatedRoute: ActivatedRoute,
    public titleService: GetTitleService
  ) {}

  ngOnInit(): void {
    const snpData = this.activatedRoute.snapshot.data;
    this.titleService.setTitle(snpData?.title);
  }

  getSearchList(searchInfo) {
    this.dateStr = searchInfo.date?.join('~');
    console.log('searchInfo:', searchInfo);
  }

  export() {
    console.log('export');
  }

  getSelectAction(data) {
    console.log('table action', data);
  }
}
