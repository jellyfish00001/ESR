import { AssignStep } from 'src/app/shared/models/assign-step';

export class DetailInfo {
  id: string;
  companycategory: string;
  category: string;
  expcode: string;
  expname: string;
  senarioname: string;
  keyword: string;
  acctcode: string;
  auditlevelcode: string;
  description: string;
  assignment: string;
  costcenter: string;
  pjcode: string;
  requiresinvoice: string;
  requiresattachment: string;
  attachmentname: string;
  isvatdeductable: string;
  canbypassfinanceapproval: string;
  departday: number;
  sectionday: number;
  calmethod: number;
  extraformcode: number;
  cuser: string;
  cdate: string;
  muser: string;
  mdate: string;
  datelevel: string;
  authorized: string;
  authorizer: string;
  sdate: string;
  edate: string;
  assignSteps: AssignStep[];
  descriptionnotice: string;
  attachmentnotice: string;
  requirespaperattachment: string;
}