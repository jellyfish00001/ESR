import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import moment from 'moment';
import { SearchSelectModel } from 'src/app/shared/models';
import { GetTitleService } from 'src/app/shared/service';
import {
  DORN_LIST,
  LINE_LIST,
  MODULE_LIST,
  PLANTCODE_LIST,
  STAGE_LIST,
  Table_Column,
} from './constants';

@Component({
  selector: 'app-info-search',
  templateUrl: './info-search.component.html',
  styleUrls: ['./info-search.component.scss'],
})
export class InfoSearchComponent implements OnInit {
  dateStr = null;
  listOfColumn = Table_Column;
  listOfData;
  listOfAction = ['查看詳細', '下載附件'];

  nzOptionSelectList: SearchSelectModel[] = [
    {
      label: 'Plant Code',
      optionalValue: ['All', ...PLANTCODE_LIST],
      default: 'All',
    },
    {
      label: 'Stage',
      optionalValue: ['All', ...STAGE_LIST],
      default: 'All',
    },
    {
      label: 'Line',
      optionalValue: ['All', ...LINE_LIST],
      default: 'All',
    },
    {
      label: '班別',
      optionalValue: ['All', ...DORN_LIST],
      default: 'All',
    },
  ];

  constructor(
    public activatedRoute: ActivatedRoute,
    public titleService: GetTitleService
  ) {}

  ngOnInit(): void {
    const snpData = this.activatedRoute.snapshot.data;
    this.titleService.setTitle(snpData?.title);
    this.iniListOfData();
  }

  iniListOfData() {
    const data = [];
    const plantCodeList = PLANTCODE_LIST;
    const stageList = STAGE_LIST;
    const dOrNList = DORN_LIST;
    const moduleList = MODULE_LIST;
    for (let i = 0; i < 100; i++) {
      let stdActionRate = Math.round(Math.random() * 100);
      let actionRate = Math.round(Math.random() * 100);
      data.push({
        plantCode:
          plantCodeList[Math.floor(Math.random() * plantCodeList.length)],
        stage: stageList[Math.floor(Math.random() * stageList.length)],
        dOrN: dOrNList[Math.floor(Math.random() * dOrNList.length)],
        line: `SA${Math.round(Math.random() * 10)}`,
        module: moduleList[Math.floor(Math.random() * moduleList.length)],
        stdActionRate: stdActionRate,
        actionRate: actionRate,
        gap: actionRate - stdActionRate,
        exceptionCount: Math.round(Math.random() * 10),
        exceptionTime: moment().format('hh:mm:ss'),
      });
    }
    this.listOfData = data;
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
