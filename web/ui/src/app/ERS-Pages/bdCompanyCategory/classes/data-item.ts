export interface CompanyInfo {
  Id: string;
  CompanyCategory: string;
  CompanyDesc: string;
  CompanySap: string;
  Stwit: string;
  BaseCurrency: string;
  IdentificationNo: string;
  IncomeTaxRate: string;
  Vatrate: string;
  Status:string;
  Area: string;
  TimeZone: string;
  // Site: string;
  // Company: string;
  CompanySite: CompanySite[];
}

export class CompanySite {
  Id:string;
  seq: number;
  CompanyCategory: string;
  Company: string;
  Site: string;
}