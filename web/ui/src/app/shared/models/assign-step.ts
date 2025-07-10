export interface AssignStep {
  name: string;
  position: string;
  approverList: Approver[];
}

export interface Approver {
  emplid: string; // 工號
  name: string; // 中文名
  nameA: string; // 英文名
  display: string; // 顯示Label 顯示為英文名(若無，則顯示中文名)
}
