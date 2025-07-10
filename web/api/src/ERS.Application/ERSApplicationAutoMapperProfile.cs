using AutoMapper;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.Application.Contracts.DTO.Report;
using ERS.DTO;
using ERS.DTO.AppConfig;
using ERS.DTO.Application;
using ERS.DTO.BDCash;
using ERS.DTO.BDPaperSign;
using ERS.DTO.BDTreelevel;
using ERS.DTO.DataDictionary;
using ERS.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ERS;

/// <summary>
/// 主要目的是通过 AutoMapper 配置对象映射规则，简化 DTO 和实体之间的转换逻辑，提升代码的可维护性和开发效率。
/// 它是应用层中处理对象映射的重要工具
/// </summary>
public class ERSApplicationAutoMapperProfile : Profile
{
    public ERSApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<AppConfig, AppConfigDto>().ReverseMap();
        CreateMap<ERS.Common.Result<IList<string>>, Result<IList<string>>>().ReverseMap();
        CreateMap<CreateUpdateAppConfigDto, AppConfig>();
        CreateMap<CashHeadDto, CashHead>().ReverseMap();
        CreateMap<CashDetailDto, CashDetail>()
            .AfterMap((src, dest) => { dest.deptid = src.deptList != null && src.deptList.Count > 0 ? JsonConvert.SerializeObject(src.deptList) : src.deptid; });
        CreateMap<CashDetail, CashDetailDto>()
            .AfterMap((src, dest) => { dest.deptList = src.deptid.StartsWith("[") && src.deptid.EndsWith("]") ? JsonConvert.DeserializeObject<List<departCost>>(src.deptid) : null; });
        CreateMap<CashFileDto, CashFile>().ReverseMap();
        CreateMap<CashAmountDto, CashAmount>().ReverseMap(); 
        CreateMap<InvoiceDto, Invoice>()
            .ForMember(x => x.abnormalreason, y => { y.MapFrom(z => z.expdesc); })
            .ForMember(x => x.abnormalmsg, y => { y.MapFrom(z => z.expcode); }).ReverseMap();
        CreateMap<ReportDto, EFormHead>();
        CreateMap<ReportDto, CashHead>();
        CreateMap<ReportDto, CashDetail>();
        CreateMap<ReportDto, CashAmount>();
        CreateMap<BDCashDto,BDCashReturn>().ReverseMap();
        CreateMap<EmporgDto, EmpOrg>().ReverseMap();
        CreateMap<Finreview, FinReviewDto>().ReverseMap();
        CreateMap<BDPaperSign, AddPaperSignDto>().ReverseMap();
        CreateMap<BDPaperSign, EditPaperSignDto>().ReverseMap().ForMember(w => w.Id, opt => opt.Ignore());
        CreateMap<BDPaperSign, PaperSignDto>().ReverseMap();
        CreateMap<NickNameCommonDto, CustomerNickname>().ReverseMap();
        CreateMap<HelpManualDto, HelpManual>().ReverseMap();
        CreateMap<BDTreelevel, QueryBDTreelevelDto>()
            .ForMember(w => w.levelname, s => { s.MapFrom(g => g.levelname); })
            .ForMember(w => w.leveltwname, s => { s.MapFrom(g => g.leveltwname); })
            .ForMember(w => w.levelcnname, s => { s.MapFrom(g => g.levelcnname); })
            .ForMember(w => w.levelnum, s => { s.MapFrom(g => g.levelnum); });

        // 添加 DataDictionary 到 QueryDataDictionaryDto 的映射
        CreateMap<DataDictionary, QueryDataDictionaryDto>().ReverseMap();
    }
}
