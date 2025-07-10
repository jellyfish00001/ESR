export interface CompanyInfo {
    id: string;
    companyCode: string;
    company: string;
    sapCompanyCode: string;
    companyDesc: string;
    abbr: string;
    curr: string;
    taxpayerNo: string;
    taxRate: number;
    area: string;
    timezone: number;
    timezoneName: string;
    creator: string;
    createDate: string;
    updateUser: string;
    updateDate: string;
    disabled: boolean;
}
export const TimeZoneInfo = [
    {
        key: 'timezone-zero',
        value: 0
    },
    {
        key: 'timezone-east-one',
        value: 1
    },
    {
        key: 'timezone-east-two',
        value: 2
    },
    {
        key: 'timezone-east-three',
        value: 3
    },
    {
        key: 'timezone-east-four',
        value: 4
    },
    {
        key: 'timezone-east-five',
        value: 5
    },
    {
        key: 'timezone-east-six',
        value: 6
    },
    {
        key: 'timezone-east-seven',
        value: 7
    },
    {
        key: 'timezone-east-eight',
        value: 8
    },
    {
        key: 'timezone-east-nine',
        value: 9
    },
    {
        key: 'timezone-east-ten',
        value: 10
    },
    {
        key: 'timezone-east-eleven',
        value: 11
    },
    {
        key: 'timezone-east-twelve',
        value: 12
    },
    {
        key: 'timezone-west-one',
        value: -1
    },
    {
        key: 'timezone-west-two',
        value: -2
    },
    {
        key: 'timezone-west-three',
        value: -3
    },
    {
        key: 'timezone-west-four',
        value: -4
    },
    {
        key: 'timezone-west-five',
        value: -5
    },
    {
        key: 'timezone-west-six',
        value: -6
    },
    {
        key: 'timezone-west-seven',
        value: -7
    },
    {
        key: 'timezone-west-eight',
        value: -8
    },
    {
        key: 'timezone-west-nine',
        value: -9
    },
    {
        key: 'timezone-west-ten',
        value: -10
    },
    {
        key: 'timezone-west-eleven',
        value: -11
    },
    {
        key: 'timezone-west-twelve',
        value: -12
    },
]

export const CompanyStatusInfo = [
    {
        key: 'company-status-enable',
        value: 1
    },
    {
        key: 'company-status-disenable',
        value: 0
    }  
]