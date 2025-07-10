import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TableColumnModel } from 'src/app/shared/models';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
})
export class TableComponent implements OnInit {
  /**
   * table 栏位属性设置
   *
   * @type {TableColumnModel[]}
   * @memberof TableComponent
   */
  @Input() listOfColumn: TableColumnModel[] = [];
  /**
   * table data
   *
   * @memberof TableComponent
   */
  @Input() listOfData = [];
  /**
   * 可操作的功能按钮清单
   *
   * @memberof TableComponent
   */
  @Input() listOfAction: string[] = [];
  @Input() showPagination: boolean = true;
  /**
   * 是否分页
   *
   * @memberof TableComponent
   */
  @Input() scroll: object = {};
  /**
   * 固定长宽
   *
   * @memberof TableComponent
   */
  @Output() selectAction = new EventEmitter<any>();
  selectData;
  //根据不同的页面客制组件
  @Input() PageKey: string = '';

  constructor(
    public translate: TranslateService,
  ) { }

  ngOnInit(): void { }

  hover(data) {
    this.selectData = data;
  }

  getDropDownIndex(index) {
    console.log('index',index)
    this.selectAction.emit({
      data: this.selectData,
      actionIndex: index,
    });
  }

  getColumnName(rawName: string): string {
    let name = rawName;
    let index = rawName.indexOf('(')
    if (index !== -1)
      name = this.translate.instant(rawName.substring(0, index)) + rawName.substring(index);
    else name =this.translate.instant(rawName);
    return name;
  }
}
