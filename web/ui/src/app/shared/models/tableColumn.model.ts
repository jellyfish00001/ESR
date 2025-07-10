export interface TableColumnModel {
  /**
   * 栏位名
   *
   * @type {string}
   * @memberof TableColumnModel
   */
  title: string;
  /**
   * 栏位排序的 key
   *
   * @type {string}
   * @memberof TableColumnModel
   */
  columnKey: string;
  /**
   * 栏位宽度
   *
   * @type {string}
   * @memberof TableColumnModel
   */
  columnWidth: string;
  /**
   * 设置列内容的对齐方式
   *
   * @type {("left" | "right" | "center")}
   * @memberof TableColumnModel
   */
  align: "left" | "right" | "center";
  /**
   * 排序函数
   *
   * @param {*} a
   * @param {*} b
   * @return {*}  {(number | string | boolean)}
   * @memberof TableColumnModel
   */
  sortFn(a, b): number | string | boolean;
}
