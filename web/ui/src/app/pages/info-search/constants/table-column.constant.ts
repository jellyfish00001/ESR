import { TableColumnModel } from 'src/app/shared/models';
import { DataItem } from '../classes';

export const Table_Column : TableColumnModel[] = [
  {
    title: 'Plant Code',
    columnKey: 'plantCode',
    columnWidth: '8.1%',
    align: 'left',
    sortFn: (a: DataItem, b: DataItem) =>
      a.plantCode.localeCompare(b.plantCode),
  },
  {
    title: 'Stage',
    columnKey: 'stage',
    columnWidth: '8.1%',
    align: 'left',
    sortFn: (a: DataItem, b: DataItem) => a.stage.localeCompare(b.stage),
  },
  {
    title: '班別',
    columnKey: 'dOrN',
    columnWidth: '7.2%',
    align: 'left',
    sortFn: (a: DataItem, b: DataItem) => a.dOrN.localeCompare(b.dOrN),
  },
  {
    title: 'Line',
    columnKey: 'line',
    columnWidth: '7.2%',
    align: 'left',
    sortFn: (a: DataItem, b: DataItem) => a.line.localeCompare(b.line),
  },
  {
    title: 'Module',
    columnKey: 'module',
    columnWidth: '11.7%',
    align: 'left',
    sortFn: (a: DataItem, b: DataItem) => a.module.localeCompare(b.module),
  },
  {
    title: '標準稼動率',
    columnKey: 'stdActionRate',
    columnWidth: '11.7%',
    align: 'right',
    sortFn: (a: DataItem, b: DataItem) => a.stdActionRate - b.stdActionRate,
  },
  {
    title: '每小時稼動率',
    columnKey: 'actionRate',
    columnWidth: '12.6%',
    align: 'right',
    sortFn: (a: DataItem, b: DataItem) => a.actionRate - b.actionRate,
  },
  {
    title: '差異',
    columnKey: 'gap',
    columnWidth: '8.1%',
    align: 'right',
    sortFn: (a: DataItem, b: DataItem) => a.gap - b.gap,
  },
  {
    title: '異常 累計次數',
    columnKey: 'exceptionCount',
    columnWidth: '9%',
    align: 'right',
    sortFn: (a: DataItem, b: DataItem) => a.exceptionCount - b.exceptionCount,
  },
  {
    title: '異常 累計時間',
    columnKey: 'exceptionTime',
    columnWidth: '10%',
    align: 'right',
    sortFn: (a: DataItem, b: DataItem) =>
      a.exceptionTime.localeCompare(b.exceptionTime),
  },
];
