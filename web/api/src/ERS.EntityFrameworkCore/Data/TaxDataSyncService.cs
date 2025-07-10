using Abp.Application.Services;
using CsvHelper;
using CsvHelper.Configuration;
using ERS.Entities;
using ERS.IDomainServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ERS.IRepositories;
using System.Linq;
using ERS.EntityFrameworkCore;
using EFCore.BulkExtensions;


namespace ERS.Data
{
    public class TaxDataSyncService : ApplicationService, ITaxDataSyncService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private ERSDbContext _eRSDbContext;
        private ICorporateRegistrationRepository _iCorporateRegistrationRepository;
        public TaxDataSyncService(
            ICorporateRegistrationRepository iCorporateRegistrationRepository, ERSDbContext eRSDbContext)
        {
            _iCorporateRegistrationRepository = iCorporateRegistrationRepository;
            _eRSDbContext = eRSDbContext;
        }

        public async Task DownloadTaxDataAndSync()
        {
            string csvContent = null;
            string url = "https://eip.fia.gov.tw/data/BGMOPEN1.csv";
            try
            {
                Console.WriteLine("sync tax data job start");

                csvContent = await httpClient.GetStringAsync(url);
                ConvertDataAndSyncDB(csvContent);

                Console.WriteLine("sync tax data job end");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error updating tax data: " + e.Message);
            }
        }

        public async void ConvertDataAndSyncDB(string csvContent)
        {   //CorporateRegistration
            using (var reader = new StringReader(csvContent))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                List<CorporateRegistration> ls = new List<CorporateRegistration>();
                csv.Read();//这步不能省略，否则表头会被当成资料读出来
                csv.ReadHeader();
                var updateTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                while (csv.Read())
                {
                    CorporateRegistration corporateRegistration = new CorporateRegistration();
                    corporateRegistration.unifiedNo = csv.GetField(1);
                    corporateRegistration.name = csv.GetField(3);
                    corporateRegistration.usesInvoice = csv.GetField(7);
                    corporateRegistration.updateTime = updateTime;
                    ls.Add(corporateRegistration);
                }
                //sync data to DB
                if (ls.Count > 0)
                {
                    //先删后增
                    List<CorporateRegistration> oldData = (await _iCorporateRegistrationRepository.WithDetailsAsync()).ToList();
                    //Console.WriteLine("oldData: " + oldData);
                    if (oldData.Count > 0) {
                        await _eRSDbContext.BulkDeleteAsync(oldData);
                    }
                    await _eRSDbContext.BulkInsertAsync(ls);
                }
            }
        }

        //测试本地CSV用
        public async void TestReadCsvFile()
        {
            string filePath = "F:\\rpa\\BGMOPEN1.csv";
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Object>();
                csv.Read();//这步不能省略，否则表头会被当成资料读出来
                csv.ReadHeader();
                var updateTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                List<CorporateRegistration> ls = new List<CorporateRegistration>();
                while (csv.Read())
                {
                    if (csv.GetField(1) == "") 
                    {
                        continue;
                    }
                    CorporateRegistration corporateRegistration = new CorporateRegistration();
                    corporateRegistration.unifiedNo = csv.GetField(1);
                    corporateRegistration.name = csv.GetField(3);
                    corporateRegistration.usesInvoice = csv.GetField(7);
                    corporateRegistration.updateTime = updateTime;
                    corporateRegistration.company = "WHQ";
                    ls.Add(corporateRegistration);
                }
                if (ls.Count > 0)
                {
                    //先删后增
                    List<CorporateRegistration> oldData = (await _iCorporateRegistrationRepository.WithDetailsAsync()).ToList();
                    //Console.WriteLine("oldData: " + oldData);
                    if (oldData.Count > 0)
                    {
                        await _eRSDbContext.BulkDeleteAsync(oldData);
                    }
                    await _eRSDbContext.BulkInsertAsync(ls);
                }
            }
        }

    }
}