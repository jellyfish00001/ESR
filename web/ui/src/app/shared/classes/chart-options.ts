import * as echarts from 'echarts';
import {
  VERTICAL_BAR_COLOR,
  DATAZOOM_FILLERCOLOR,
  LEGEND_COLOR,
  SPLITLINE_COLOR,
  STACKBAR_BG_COLOR,
  STACK_COLOR,
  XAXISLABEL_COLOR,
  YAXISLABEL_COLOR,
  HORIZONTAL_BAR_COLOR,
} from '../constants';
import { BarChartOrientType, DataZoomOrientType, StackType } from '../models';

export class ChartOptions {
  /**
   * 绘制图形
   *
   * @param {*} chartId
   * @param {*} option
   * @memberof ChartOptions
   */
  setChart(chartId, option) {
    if (!Object.keys(option).length) return;
    let myChart = echarts.init(
      <HTMLDivElement>document.getElementById(chartId)
    );
    myChart.setOption(option);
    myChart.resize();
  }

  /**
   * 设置缩放区域
   *
   * @param {*} option
   * @param {string | number} startValue 缩放区域起始数值
   * @param {string | number} endValue 缩放区域结束数值
   * @param {DataZoomOrientType} orient 布局方式，默认为横向
   * @return {*}  {option}
   * @memberof ChartOptions
   */
  setDataZoom(
    option,
    startValue: string | number,
    endValue: string | number,
    orient: DataZoomOrientType = 'horizontal'
  ): Object {
    if (Object.keys(option).length) {
      option.dataZoom = {
        type: 'slider',
        orient: orient,
        startValue: startValue,
        endValue: endValue,
        maxValueSpan: 10, //限制窗口大小的最大类目
        fillerColor: DATAZOOM_FILLERCOLOR, //选中范围的填充颜色
        showDetail: false, //是否显示detail，即拖拽时候显示详细数值信息
        showDataShadow: false, //是否显示数据阴影
        zoomLock: true, //是否锁定选择区域的大小(设置为 true 则锁定，即只能平移，不能缩放)
        brushSelect: false, //是否开启刷选功能(按住鼠标左键后框选出选中部分)
      };
      if (orient === 'vertical') {
        option.dataZoom.width = 10;
      } else {
        option.dataZoom.height = 10;
      }
    }
    return option;
  }

  /**
   * 获取圆环图的 option 参数
   *
   * @param {string} graphicName graphic Name
   * @param {string} graphicValue graphic Value
   * @param {any[]} dataList
   * @param {string[]} colors
   * @return {*}  {option}
   * @memberof ChartOptions
   */
  getDoughnutChartOption(
    graphicName: string,
    graphicValue: string,
    dataList: any[],
    colors: string[]
  ): Object {
    let option = {
      color: colors,
      tooltip: {
        trigger: 'item',
        textStyle: { color: '#000', fontSize: '16' },
        backgroundColor: '#fff',
        extraCssText: 'min-width: 200px',
        enterable: true,
        position(pos, params, dom, rect, size) {
          // 鼠标在左侧时 tooltip 显示到右侧，鼠标在右侧时 tooltip 显示到左侧。
          const obj = { top: null, left: null };
          if (pos[0] < size.viewSize[0] / 2) {
            obj.left = pos[0];
          } else {
            obj.left = pos[0] - size.viewSize[0] / 2 - 5;
          }
          obj.top = pos[1];
          return obj;
        },
      },
      graphic: [
        {
          // 图形中间文字 Name
          type: 'text',
          left: 'center',
          top: '45%',
          style: {
            text: graphicName,
            textAlign: 'center',
            fill: 'rgba(255, 255, 255, 0.65)',
            fontSize: 16,
          },
        },
        {
          // 图形中间文字 Value
          type: 'text',
          left: 'center',
          top: '55%',
          style: {
            text: graphicValue,
            textAlign: 'center',
            fill: 'rgba(255, 255, 255, 0.90)',
            fontSize: 18,
          },
        },
      ],
      series: [
        {
          name: 'myChart',
          type: 'pie',
          radius: ['70%', '90%'],
          label: {
            show: false,
          },
          emphasis: {
            scale: false,
          },
          data: dataList,
        },
      ],
    };
    return option;
  }

  /**
   * 获取直方图的 option 参数
   *
   * @param {string} axisName 坐标轴名称，与 orient 相关，默认为 y 轴名称
   * @param {Object[]} dataSource 数据集合(echarts 的 dataset.source)
   * @param {BarChartOrientType} orient 直方图显示方向，默认为纵向
   * @return {*}  {option}
   * @memberof ChartOptions
   */
  getBarChartOption(
    axisName: string,
    dataSource: Object[],
    orient: BarChartOrientType = 'vertical'
  ): Object {
    let xAxisInfo = {
      type: 'category',
      axisTick: {
        show: false,
      },
      axisLabel: {
        color: XAXISLABEL_COLOR,
      },
    };
    let yAxisInfo = {
      name: axisName,
      type: 'value',
      axisLabel: {
        color: YAXISLABEL_COLOR,
      },
      nameTextStyle: {
        color: YAXISLABEL_COLOR,
      },
      splitLine: {
        lineStyle: {
          color: SPLITLINE_COLOR,
        },
      },
    };
    let option = {
      color: '',
      dataset: {
        source: dataSource,
      },
      xAxis: {},
      yAxis: {},
      series: [
        {
          type: 'bar',
          seriesLayoutBy: 'row',
          barWidth: 24,
        },
      ],
    };
    if (orient === 'horizontal') {
      option.color = HORIZONTAL_BAR_COLOR;
      option.xAxis = yAxisInfo;
      option.yAxis = xAxisInfo;
      option.series[0].barWidth = 20;
    } else {
      option.color = VERTICAL_BAR_COLOR;
      option.xAxis = xAxisInfo;
      option.yAxis = yAxisInfo;
      option.series[0].barWidth = 24;
    }
    return option;
  }

  /**
   * 获取堆叠直方/折线图的 option 参数
   *
   * @param {StackType} type 堆叠图的类型
   * @param {string} yAxisName y轴名称
   * @param {Object[]} dataSource 数据集合(echarts 的 dataset.source)
   * @return {*}  {option}
   * @memberof ChartOptions
   */
  getStackChartOption(
    type: StackType,
    yAxisName: string,
    dataSource: Object[]
  ): Object {
    let leng = dataSource.length;
    let stackSeries = [];
    if (leng > 1) {
      for (let index = 0; index < leng - 1; index++) {
        if (type === 'bar') {
          stackSeries.push({
            type: type,
            seriesLayoutBy: 'row',
            stack: 'stackName',
            barWidth: 24,
            showBackground: true,
            backgroundStyle: {
              color: STACKBAR_BG_COLOR,
            },
          });
        } else if (type === 'line') {
          stackSeries.push({
            type: type,
            seriesLayoutBy: 'row',
            stack: 'stackName',
            symbol: 'circle',
            symbolSize: 10,
          });
        }
      }
    }
    let option = {
      color: STACK_COLOR,
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          // 坐标轴指示器，坐标轴触发有效
          type: 'shadow', // 默认为直线，可选为：'line' | 'shadow'
        },
      },
      legend: {
        selectedMode: false,
        textStyle: {
          color: LEGEND_COLOR,
        },
      },
      dataset: {
        source: dataSource,
      },
      xAxis: {
        type: 'category',
        axisTick: {
          show: false,
        },
        axisLabel: {
          color: XAXISLABEL_COLOR,
        },
      },
      yAxis: {
        type: 'value',
        name: yAxisName,
        axisLabel: {
          color: YAXISLABEL_COLOR,
        },
        nameTextStyle: {
          color: YAXISLABEL_COLOR,
        },
        splitLine: {
          lineStyle: {
            color: SPLITLINE_COLOR,
          },
        },
      },
      series: stackSeries,
    };
    return option;
  }
}
