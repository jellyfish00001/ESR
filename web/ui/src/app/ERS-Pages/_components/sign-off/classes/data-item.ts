export interface SignLogList {
  step: string;
  signUser: string;
  signDate: string;
  status: string;
  remark: string;
}
export interface SignList {
  step: string;
  signUser: string;
  status: string;
  company: string;
}

export interface ApprovalParams {
  rno: string;
  approverEmplid: string;
  inviteEmplid: string;
  inviteMethod: number; // -1:前置邀簽 1: 後置邀簽
  // paperSign: boolean;
  comment: string;
}

