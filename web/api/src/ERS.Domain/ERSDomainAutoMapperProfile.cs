using AutoMapper;
using ERS.DTO;
using ERS.DTO.Account;
using ERS.DTO.Application;
using ERS.DTO.Approval;
using ERS.DTO.ApprovalFlow;
using ERS.DTO.Auditor;
using ERS.DTO.BDAccount;
using ERS.DTO.BDCompanyCategory;
using ERS.DTO.BDExp;
using ERS.DTO.BDExpenseDept;
using ERS.DTO.BDExpenseSenario;
using ERS.DTO.BDInvoiceFolder;
using ERS.DTO.BDInvoiceRail;
using ERS.DTO.BDInvoiceType;
using ERS.DTO.BDMealArea;
using ERS.DTO.BDSignlevel;
using ERS.DTO.BDTicketRail;
using ERS.DTO.BDVender;
using ERS.DTO.BDVirtualDepartments;
using ERS.DTO.CashCarryDetail;
using ERS.DTO.Company;
using ERS.DTO.FinApprover;
using ERS.DTO.Invoice;
using ERS.DTO.PapreSign;
using ERS.DTO.Payment;
using ERS.DTO.Proxy;
using ERS.Entities;
using ERS.Entities.Payment;
using ERS.Model;

namespace ERS
{
    public class ERSDomainAutoMapperProfile : Profile
    {
        public ERSDomainAutoMapperProfile()
        {
            CreateMap<ApprovalFlowDto, ApprovalFlow>().ReverseMap();
            CreateMap<CashDetailDto, CashDetail>().ReverseMap();
            CreateMap<CashHeadDto, CashHead>().ReverseMap();
            CreateMap<CashAmountDto, CashAmount>().ReverseMap();
            CreateMap<EFormHeadDto, EFormHead>().ReverseMap();
            CreateMap<QueryPostingDto, Queryposting>().ReverseMap();
            CreateMap<SignlogDto, EFormSignlog>().ReverseMap();
            CreateMap<AlistDto, EFormAlist>().ReverseMap();
            CreateMap<CarryDetailDto, CashCarrydetail>().ReverseMap();
            CreateMap<Finreview, FinReviewDto>().ReverseMap();
            CreateMap<PaymentDetailDto,CashPaymentDetail>().ReverseMap();
            CreateMap<BDExpFormDto, BdExp>().ReverseMap();
            CreateMap<AddBDExpDto, BdExp>().ReverseMap();
            CreateMap<BDExpExcelDto, BdExp>().ReverseMap();
            CreateMap<BDSenarioDto, BDExpenseSenario>().ReverseMap();
            CreateMap<BDSenarioOptionDto, BDExpenseSenario>().ReverseMap();
            CreateMap<ExtraStepsDto, ExtraSteps>().ReverseMap();
            CreateMap<QueryBDAcctDto, BdAccount>().ReverseMap();
            CreateMap<BDAccountParamDto, BdAccount>().ReverseMap();
            CreateMap<QueryBDSignlevelDto, BDSignlevel>().ReverseMap();
            CreateMap<BDSignlevelParamDto, BDSignlevel>().ReverseMap();
            CreateMap<QueryBDExpenseDeptDto, BDExpenseDept>().ReverseMap();
            CreateMap<QueryBDVirtualDepartmentsDto, BDExpenseDept>().ReverseMap();
            CreateMap<Finreview, FinApproverDto>().ReverseMap();
            CreateMap<BDCompanyCategory, ComInfoDto>().ReverseMap();
            CreateMap<Finreview, FinApproverParamsDto>().ReverseMap();
            CreateMap<Finreview, AddFinApproverDto>().ReverseMap();
            CreateMap<EFormAudit, AuditorParamsDto>().ReverseMap();
            CreateMap<BDForm, BDFormDto>().ReverseMap();
            CreateMap<BDInvoiceType, BDInvoiceTypeDto>().ReverseMap();
            CreateMap<BDInvoiceType, AddBDInvTypeDto>().ReverseMap();
            CreateMap<InvTypeCheckDto, EditBDInvTypeDto>().ReverseMap();
            CreateMap<InvTypeCheckDto, AddBDInvTypeDto>().ReverseMap();
            CreateMap<InvoiceTypeDto, BDInvoiceType>().ReverseMap();
            //CreateMap<EditBDInvoiceFolderDto, BDInvoiceFolder>().ReverseMap();
            CreateMap<Invoice, UpdatePayStatDto>();
            CreateMap<SuperCategory, MealCostDto>();
            CreateMap<BDTicketRail, BDTicketRailDto>().ReverseMap();
            CreateMap<BDTicketRail, AddBDTicketRailDto>().ReverseMap();
            CreateMap<BDInvoiceRail, BDInvoiceRailDto>().ReverseMap();
            CreateMap<BDInvoiceRail, AddBDInvoiceRailDto>().ReverseMap();
            CreateMap<BDInvoiceFolder, BDInvoiceFolderDto>()
            .ForMember(x => x.curr, y => { y.MapFrom(z => z.currency); })
            .ReverseMap();
            CreateMap<InvoiceDto, BDInvoiceFolder>()
            .ForMember(w => w.sellername, s => { s.MapFrom(g => g.salesname); })
            .ForMember(w => w.sellertaxid, s => { s.MapFrom(g => g.salestaxno); })
            .ForMember(w => w.buyertaxid, s => { s.MapFrom(g => g.buyertaxno); })
            //.ForMember(w => w.amount, s => { s.MapFrom(g => g.tlprice); })
            .ForMember(w => w.untaxamount, s => { s.MapFrom(g => g.amount); })
            .ForMember(w => w.verifytype, s => { s.MapFrom(g => g.verifyStateDesc); })
            .ForMember(w => w.invtype, s => { s.MapFrom(g => g.invdesc); })
            .ForMember(w => w.currency, s => { s.MapFrom(g => g.curr); })
            //.ForMember(w => w.existautopa, s => { s.MapFrom(g => g.existautopa); })
            .ForMember(w => w.abnormalreason, s => { s.MapFrom(g => g.expdesc); })
            .AfterMap((src, dest) =>
            {
                dest.paytype = src.paymentStatDesc == "已請款" ? "requested" : (src.paymentStatDesc == "待請款" ? "unrequested" : (src.paymentStatDesc == "已入賬" ? "recorded" : ""));
            })
            .ReverseMap();
            CreateMap<BDInvoiceFolder, UnpaidInvInfoDto>()
            .ForMember(w => w.collectionName, s => { s.MapFrom(g => g.sellername); })
            .ForMember(w => w.collectionNo, s => { s.MapFrom(g => g.sellertaxid); })
            .ForMember(w => w.paymentNo, s => { s.MapFrom(g => g.buyertaxid); })
            .ForMember(w => w.paymentName, s => { s.MapFrom(g => g.buyername); })
            .ForMember(w => w.oamount, s => { s.MapFrom(g => g.amount); })
            .ForMember(w => w.amount, s => { s.MapFrom(g => g.untaxamount); })
            .ForMember(w => w.verifyStateDesc, s => { s.MapFrom(g => g.verifytype); })
            .ForMember(w => w.invdesc, s => { s.MapFrom(g => g.invtype); })
            .ForMember(w => w.curr, s => { s.MapFrom(g => g.currency); })
            .ForMember(w => w.expdesc, s => { s.MapFrom(g => g.abnormalreason); })
            .AfterMap((src, dest) =>
            {
                dest.invstat = src.verifytype == "Lock" ? "Lock" : "N";
                dest.paymentStatDesc = src.paytype == "requested" ? "已請款" : (src.paytype == "unrequested" ? "待請款" : src.paytype == "recorded" ? "已入賬" : "");
            })
            .ReverseMap();
            CreateMap<SignForm, SignlogDto>()
                .ForMember(x => x.rno, y => { y.MapFrom(z => z.formno); })
                .ForMember(x => x.astepname, y => { y.MapFrom(z => z.step); })
                .ForMember(x => x.aemplid, y => { y.MapFrom(z => z.signer_emplid); })
                .ForMember(x => x.aname, y => { y.MapFrom(z => z.signer_cname); })
                .ForMember(x => x.aename, y => { y.MapFrom(z => z.signer_ename); })
                .ForMember(x => x.adate, y => { y.MapFrom(z => z.sign_date); })
                .ForMember(x => x.aresult, y => { y.MapFrom(z => z.sign_result); })
                .ForMember(x => x.aremark, y => { y.MapFrom(z => z.sign_remark); })
                .ForMember(x => x.step, y => { y.MapFrom(z => z.seq); })
                .AfterMap((src, dest) =>
                {
                    // dest.aresult = src.sign_result == "Approve" ? "A" : (src.sign_result == "Return" ? "R" : "");
                    //dest.astepname = !String.IsNullOrEmpty(src.step_activity) && String.IsNullOrEmpty(src.muser) ? src.step_activity : src.step;
                })
                ;
            CreateMap<SignForm, AlistDto>()
                .ForMember(x => x.rno, y => { y.MapFrom(z => z.formno); })
                .ForMember(x => x.stepname, y => { y.MapFrom(z => z.step); })
                .ForMember(x => x.cemplid, y => { y.MapFrom(z => z.signer_emplid); })
                .ForMember(x => x.deptid, y => { y.MapFrom(z => z.signer_deptid); })
                .ForMember(x => x.adate, y => { y.MapFrom(z => z.sign_date); })
                .ForMember(x => x.formcode, y => { y.MapFrom(z => z.form_code); })
                .ForMember(x => x.step, y => { y.MapFrom(z => z.seq); })
                .AfterMap((src, dest) =>
                {
                    dest.approval = src.signer_emplid + "/" + src.signer_ename;
                    dest.status = src.status == "Finish" ? "F" : "P";
                    //dest.stepname = !String.IsNullOrEmpty(src.step_activity) && String.IsNullOrEmpty(src.muser) ? src.step_activity : src.step;
                })
            ;
            CreateMap<CashCarrydetail, CarryDetailReportDto>()
            .AfterMap((src, dest) =>
            {
                dest.postdate = src.postdate.ToString("yyyyMMdd");
                dest.docdate = src.docdate.ToString("yyyyMMdd");
                dest.baslindate = src.baslindate.ToString("yyyyMMdd");
            })
            .ReverseMap();
            CreateMap<ProxyCash, ProxyCashDto>().ReverseMap();
            CreateMap<BDMealArea, AddBDMealAreaDto>().ReverseMap();
            CreateMap<CashCarrydetail, SACarryDetailReportDto>().AfterMap((src, dest) =>
            {
                dest.postdate = src.postdate.ToString("yyyyMMdd");
                dest.docdate = src.docdate.ToString("yyyyMMdd");
            })
            .ReverseMap();

            CreateMap<UpdatePayStatDto, ERSInvDto>()
            .ForMember(w => w.Invcode, y => { y.MapFrom(z => z.invcode); })
            .ForMember(w => w.Invno, y => { y.MapFrom(z => z.invno); });

            CreateMap<Invoice, ERSInvDto>()
            .ForMember(w => w.Invcode, y => { y.MapFrom(z => z.invcode); })
            .ForMember(w => w.Invno, y => { y.MapFrom(z => z.invno); });

            CreateMap<BDCompanyCategory, BDCompanyCategoryParamDto>().ReverseMap();
            CreateMap<BDCompanyCategoryParamDto, BDCompanyCategory>().ReverseMap();
            CreateMap<BDVender, BDVenderParamDto>().ReverseMap();
            CreateMap<BDCompanySite, BDCompanySiteParamDto>().ReverseMap();
            CreateMap<CashUberHead, CashUberHeadDto>().ReverseMap();
            CreateMap<CashUberDetail, CashUberDetailDto>().ReverseMap();
            CreateMap<CashCarrydetail, CashCarryDetailDto>().ReverseMap();
        }
    }
}
