export interface OverdueChargeAgainstDetail {
    companyCode: string;
    advanceFundRno: string;
    digest: string;
    appliedAmt: number;
    notChargeAgainstAmt: number;
    openDays: number;
    delayTimes: number;
    advanceDate: Date;
    disabled: boolean;
    cdate: string;
}

export interface DelayInfo {
    id: number;
    rno: string;
    digest: string;
    appliedAmt: number;
    notChargeAgainstAmt: number;
    scheduledDebitDate: string;
    openDays: number;
    delayChargeAgainstDays: number;
    afterDate: string;
    delayReason: string;
    disabled: boolean;
    cdate: string;
}