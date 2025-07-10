using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
namespace ERS.DomainServices
{
    public class EmpChsDomainService : CommonDomainService, IEmpChsDomainService
    {
        private IEmpchsRepository _EmpchsRepository;
        private ICompanyRepository _CompanyRepository;
        private IEmployeeRepository _EmployeeRepository;
        public EmpChsDomainService(
            IEmpchsRepository EmpchsRepository,
            ICompanyRepository CompanyRepository,
            IEmployeeRepository EmployeeRepository
        )
        {
            _EmpchsRepository = EmpchsRepository;
            _CompanyRepository = CompanyRepository;
            _EmployeeRepository = EmployeeRepository;
        }
        public async Task<Result<string>> UploadBankAccountName(IFormFile excelFile)
        {
            Result<string> result = new();
            DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
            List<Empchs> query = (await _EmpchsRepository.GetAllEmpchsList()).ToList();
            var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
            {
                emplid = s[0].ToString(),
                accountname = s[1].ToString()
            }).ToList();
            var emplInfo = (await _EmployeeRepository.WithDetailsAsync()).Where(w => list.Select(s => s.emplid).Contains(w.emplid)).ToList();
            var comInfo = (await _CompanyRepository.WithDetailsAsync()).ToList();
            if (list.Count == 0)
            {
                result.message = L["ExcelEmpty"];
                return result;
            }
            List<Empchs> newEmpchsList = new();
            List<Empchs> updateEmpchsList = new();
            if (excelFile.FileName.ToLower().EndsWith(".xls") || excelFile.FileName.ToLower().EndsWith(".xlsx"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string empCompany = emplInfo.Where(s => s.emplid == list[i].emplid).FirstOrDefault()?.company;
                    string company = comInfo.Where(w => w.CompanyCategory == empCompany).FirstOrDefault()?.company;
                    List<Empchs> existQuery = query.Where(w => w.emplid == list[i].emplid).ToList();
                    if (existQuery.Count > 0)
                    {
                        for (int j = 0; j < existQuery.Count; j++)
                        {
                            existQuery[j].scname = list[i].accountname;
                            existQuery[j].mdate = System.DateTime.Now;
                            existQuery[j].muser = "System";
                            existQuery[j].company = !string.IsNullOrEmpty(company) ? company : "ALL";
                        }
                        updateEmpchsList.AddRange(existQuery);
                    }
                    else
                    {
                        Empchs newEmpchs = new();
                        newEmpchs.emplid = list[i].emplid;
                        newEmpchs.scname = list[i].accountname;
                        newEmpchs.cdate = System.DateTime.Now;
                        newEmpchs.cuser = "System";
                        newEmpchs.company = !string.IsNullOrEmpty(company) ? company : "ALL";
                        newEmpchsList.Add(newEmpchs);
                    }
                }
                await _EmpchsRepository.InsertManyAsync(newEmpchsList);
                await _EmpchsRepository.UpdateManyAsync(updateEmpchsList);
            }
            return result;
        }
    }
}