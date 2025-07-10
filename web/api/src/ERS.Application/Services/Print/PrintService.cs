using DinkToPdf;
using DinkToPdf.Contracts;
using ERS.DomainServices;
using ERS.DTO;
using ERS.DTO.PapreSign;
using ERS.DTO.Print;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.Localization;
using ERS.Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
namespace ERS.Services.Print
{
    public class PrintService : ApplicationService, IPrintService
    {
        private IPrintDomainService _printDomainService;
        private IEFormHeadRepository _eformheadRepository;
        private IObjectMapper _objectMapper;
        private IMinioRepository _minioRepository;
        private IMinioDomainService _minioDomainService;
        private ICashFileRepository _cashFileRepository;
        public PrintService(IPrintDomainService printDomainService, IObjectMapper objectMapper, IEFormHeadRepository eformheadRepository, IMinioRepository minioRepository, ICashFileRepository cashFileRepository, IMinioDomainService minioDomainService)
        {
            _printDomainService = printDomainService;
            _eformheadRepository = eformheadRepository;
            _objectMapper = objectMapper;
            _minioRepository = minioRepository;
            _cashFileRepository = cashFileRepository;
            LocalizationResource = typeof(ERSResource);
            _minioDomainService = minioDomainService;
        }

        public Result<string> StoreFormDetails(List<string> rnolist, string token,string userId)
        {
            string area =  _minioDomainService.GetMinioArea(userId).Result;
            rnolist.ForEach(async rno =>
            {
                List<string> rnoList = new List<string> { rno };
                var htmlResult = await GetPrintAsync(rnoList, token);
                if (htmlResult == null || string.IsNullOrEmpty(htmlResult.data))
                {
                    return;
                }
                // Convert HTML to PDF
                var pdfDocument = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = htmlResult.data,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                        }
                    }
                };
                IConverter _converter = new SynchronizedConverter(new PdfTools());
                byte[] pdf = _converter.Convert(pdfDocument);
                string Taday = DateTime.Now.ToString("yyyyMM");
                // Upload PDF to MinIO
                string fileName = rno + ".pdf";
                string objectName = Taday + "/" + rno + "/" + fileName;
                using (var stream = new MemoryStream(pdf))
                {
                    await _minioRepository.PutObjectAsync(objectName, stream, "application/pdf",area);
                }
                EFormHead eFormHead = await _eformheadRepository.GetByNo(rno);
                CashFile cashFile = new CashFile
                {
                    rno = rno + "-FORM",
                    company = eFormHead.company,
                    seq = 1,
                    item = 1,
                    category = "FORM",//类型
                    filetype = "application/pdf",//类型名称
                    path = objectName,//存储路径
                    filename = fileName,//文件名称
                    formcode = eFormHead.formcode,
                    tofn = fileName,//原名称
                    ishead = "N",//是否为head附件
                    cdate = System.DateTime.Now,
                    cuser = "system"
                };
                await _cashFileRepository.InsertAsync(cashFile);
            });
            return new Result<string> { message = "PDF file generated and uploaded to MinIO successfully." };
        }
        public async Task<Result<string>> GetPrintAsync(List<string> rnolist, string token)
        {
            StringBuilder content = new StringBuilder(); //接收domain传来的填充完数据的html片段
            Result<string> results = new Result<string>();
            string contentresult = "";
            string htmlframe = @"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <meta charset='UTF-8'>
                                    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                    <title>Document</title>
                                    <style>
                                        html {
                                            height: 100%;
                                            width: 100%;
                                        }
                                        body {
                                            height: 100%;
                                            width: 100%;
                                            margin: 0;
                                        }
                                        table {
                                            width: 90%;
                                            font-family: Simsun;
                                            font-size: 10pt;
                                        }
                                        table td {
                                            word-wrap: break-word;
                                            width: auto !important;
                                        }
                                    </style>
                                </head>
                                <body></body>
                                </html>";
            string result = "";
            List<string> actualrnolist = new List<string>();
            foreach (var rno in rnolist)
            {
                var efhresult = _eformheadRepository.GetByNo(rno);
                if (efhresult.Result != null)
                {
                    actualrnolist.Add(rno);
                }
            }
            actualrnolist = actualrnolist.Distinct().ToList();
            foreach (var rno in actualrnolist)
            {
                EFormHead efhead = await _eformheadRepository.GetByNo(rno.Trim());
                EFormHeadDto efhdata = _objectMapper.Map<EFormHead, EFormHeadDto>(efhead);
                if (!efhdata.apid.EndsWith("A") && efhdata.formcode == "CASH_2")
                {
                    content.Append(await _printDomainService.EntertainmentExpPrintAsync(rno, token));
                }
                else if (efhdata.apid.EndsWith("A") && efhdata.formcode == "CASH_2")
                {
                    content.Append(await _printDomainService.CateringGuestsPrintAsync(rno, token));
                }
                else if (efhdata.formcode == "CASH_3")
                {
                    content.Append(await _printDomainService.AdvancePaymentPrintAsync(rno, token));
                }
                else if (efhdata.formcode == "CASH_1")
                {
                    content.Append(await _printDomainService.GENCommExpPrintAsync(rno, token));
                }
                else if (efhdata.formcode == "CASH_4")
                {
                    content.Append(await _printDomainService.BatchReimbursementPrintAsync(rno, token));
                }
                else if (efhdata.formcode == "CASH_6")
                {
                    content.Append(await _printDomainService.ReturnTaiwanMeetingPrint(rno, token));
                }
                else if (efhdata.formcode == "CASH_X")
                {
                    content.Append(await _printDomainService.PayrollRequestPrintAsync(rno, token));
                }
            }
            contentresult = content.ToString();
            result = htmlframe.Replace("<body>", "<body>" + contentresult);
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            result = Regex.Replace(result, @"""(.+?)""", m => "" + m.Groups[1].Value + "");
            results.data = result;
            return results;
        }
        public async Task<Result<List<PrintDto>>> GetQueryPagePrint(Request<PrintQueryDto> request)
        {
            return await _printDomainService.QueryPagePrint(request);
        }
    }
}