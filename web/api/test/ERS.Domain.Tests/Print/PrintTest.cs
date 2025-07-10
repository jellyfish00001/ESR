using ERS.IDomainServices;

namespace ERS.Print
{
    public class PrintTest : ERSDomainTestBase
    {
        private IPrintDomainService _printdomainService;

        public PrintTest()
        {
            _printdomainService = GetRequiredService<IPrintDomainService>();
        }

        // [Fact]
        // public async void Cash1PrintTest()
        // {
        //     string rno = "E22083000010";
        //     string rno1 = "E22082600005";
        //     string rno2 = "C1108120010";
        //     string rno3 = "B2012170001";
            
            
        //     var result = await _printdomainService.CateringGuestsPrintAsync(rno);
        //     var result1 = await _printdomainService.EntertainmentExpPrintAsync(rno1);
        //     var result2 = await _printdomainService.GENCommExpPrintAsync(rno2);
        //     var result3 = await _printdomainService.BatchReimbursementPrintAsync(rno3);
        //     Console.WriteLine(result);
        //     Console.WriteLine(result1);
        //     Console.WriteLine(result3);
        // }

        //     //PuppeteerSharp html转pdf
        //     // var browserFetcher = new BrowserFetcher();
        //     // string outputFile = Path.Combine(Path.Combine("Files", "Print", "Output", "Print.pdf"));
        //     // await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        //     // var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //     // {
        //     //     Headless = true
        //     // });
        //     // var page = await browser.NewPageAsync();
        //     // await page.SetContentAsync(result);
        //     // var resultpdf = await page.GetContentAsync();
        //     // await page.PdfAsync(outputFile, new PdfOptions
        //     //     {
        //     //         Format = PaperFormat.A4,
        //     //         MarginOptions = new MarginOptions
        //     //         {
        //     //             Top = "20px",
        //     //             Right = "20px",
        //     //             Bottom = "0px",
        //     //             Left = "20px"
        //     //         },
        //     //         Landscape = true
        //     //     });
        //     // Console.WriteLine(resultpdf);

        //     // var doc = new HtmlToPdfDocument()
        //     // {
        //     //     GlobalSettings = {
        //     //         ColorMode = ColorMode.Color,
        //     //         Orientation = Orientation.Landscape,
        //     //         PaperSize = PaperKind.A4,
        //     //     },
        //     //     Objects = {
        //     //         new ObjectSettings() {
        //     //         PagesCount = true,
        //     //         HtmlContent = result,
        //     //         WebSettings = { DefaultEncoding = "utf-8" }
        //     //         }
        //     //     }
        //     // };

        //     // var converter = new BasicConverter(new PdfTools());
        //     // byte[] pdf = converter.Convert(doc);
        // }
    }
}