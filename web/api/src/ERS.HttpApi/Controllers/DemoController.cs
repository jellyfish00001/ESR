using ERS.Context;
using ERS.DTO;
using ERS.DTO.AppConfig;
using ERS.DTO.Payment;
using ERS.Localization;
using ERS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERS.Controllers;
/* Inherit your controllers from this class.
 */
#if DEBUG
[AllowAnonymous]
#endif
public class DemoController : BaseController
{
    private IDemoService _IDemoService;
    private IPaymentService _PaymentService;
    private IAppConfigService _AppConfigService;
    public DemoController(IDemoService DemoService, IAppConfigService AppConfigService, IPaymentService PaymentService)
    {
        _IDemoService = DemoService;
        LocalizationResource = typeof(ERSResource);
        _AppConfigService = AppConfigService;
        _PaymentService = PaymentService;
    }
    // [HttpGet("test1")]
    // public string test()
    // {
    //     return "123" + L["Welcome"] + _IDemoService.get().Result; ;
    // }
    [HttpGet("test2")]
    public async Task test2(string rno) {
        var ererer = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
        var erer = RequestContext.Current.userid;
        var wer = DateTime.Now;
    }
    [HttpPost("scan")]
    public async Task<Result<IList<string>>> scan(IFormFile file) => await _IDemoService.scan(file);
    [HttpPost("testSendPaymentEmail")]
    public async Task testSendPaymentEmail(string emplid, string sysno) => await _PaymentService.SendMailToUserForPayment(emplid, sysno);
}
