import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TableColumnModel } from 'src/app/shared/models';

@Component({
  selector: 'app-checkbox-table',
  templateUrl: './checkbox-table.component.html',
  styleUrls: ['./checkbox-table.component.scss'],
})
export class CheckboxTableComponent implements OnInit {
  /**
   * table 栏位属性设置
   *
   * @type {TableColumnModel[]}
   * @memberof CheckboxTableComponent
   */
  @Input() listTableColumn: TableColumnModel[] = [];
  /**
   * table data
   *
   * @memberof CheckboxTableComponent
   */
  @Input() listTableData = [];
  /**
   * 可操作的功能按钮清单
   *
   * @memberof CheckboxTableComponent
   */
  @Input() width = '';
  /**
   * table width
   *
   * @memberof CheckboxTableComponent
   */
  @Input() scroll: object
  /**
   * 可滑动
   *
   * @memberof CheckboxTableComponent
   */
  @Input() total = 0
  /**
   * table item total
   *
   * @memberof CheckboxTableComponent
   */
  @Input() showEditButton = true
  /**
   * 是否显示编辑按钮
   *
   * @memberof CheckboxTableComponent
   */
  @Input() editRow(id: any) { }
  /**
   * 编辑方法
   *
   * @memberof CheckboxTableComponent
   */
   @Input() deleteRow(id: any) { }
   /**
    * 编辑方法
    *
    * @memberof CheckboxTableComponent
    */
   @Input() showDetailCol = true
   /**
    * 显示明细列
    *
    * @memberof CheckboxTableComponent
    */
    @Input() detailColName ='Detail'
    /**
     * 明细列名
     *
     * @memberof CheckboxTableComponent
     */
     @Input() detailCellName ='detail'
     /**
      * 明细单元格link名
      *
      * @memberof CheckboxTableComponent
      */
  @Input() listOfAction: string[] = [];
  @Output() selectAction = new EventEmitter<any>();
  selectData;
  constructor() { }

  ngOnInit(): void { }

  hover(data) {
    this.selectData = data;
  }

  getDropDownIndex(index) {
    this.selectAction.emit({
      data: this.selectData,
      actionIndex: index,
    });
  }

  checked = false;
  addloading = false;
  editloading = false;
  deleteloading = false;
  indeterminate = false;
  listOfCurrentPageData: any[] = [];
  setOfCheckedId = new Set<any>();
  updateCheckedSet(id: any, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onCurrentPageDataChange(listOfCurrentPageData: any): void {
    this.listOfCurrentPageData = listOfCurrentPageData;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const listOfEnabledData = this.listOfCurrentPageData.filter(({ disabled }) => !disabled);
    this.checked = listOfEnabledData.every(({ id }) => this.setOfCheckedId.has(id));
    this.indeterminate = listOfEnabledData.some(({ id }) => this.setOfCheckedId.has(id)) && !this.checked;
  }

  onItemChecked(id: any, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    this.listOfCurrentPageData.filter(({ disabled }) => !disabled).forEach(({ id }) => this.updateCheckedSet(id, checked));
    this.refreshCheckedStatus();
  }



}
