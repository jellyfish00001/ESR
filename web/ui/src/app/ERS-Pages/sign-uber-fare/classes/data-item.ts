export interface CashUberHead {
  emplid: string;
  rno: string;
  formCode: string;
  name: string;
  projectCode: string;
  businessTripNo: string;
  program: string;
}
export interface CashUberDetail {
  formCode: string;
  rno: string;
  item: string;
  startDate: string | null;      // DateTime? 推荐用 string | null
  destination: string;
  origin: string;
  status: string;
  amount: number | null;         // decimal? 推荐用 number | null
  reason: string;
  expCode: string;
  emplid: string;
  name: string;
  deptId: string;
}
