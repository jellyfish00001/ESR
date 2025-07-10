using System.Collections.Generic;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.Communication;
using System;
using System.Linq;
using Volo.Abp.Application.Services;
using ERS.Application.Contracts.DTO.EmployeeInfo;
using ERS.IRepositories;
using ERS.Application.Contracts.DTO.Employee;
using ERS.Application.Contracts.DTO.Emporg;
using ERS.Entities;
using ERS.IDomainServices;
namespace ERS.Application.Services
{
    public class EmployeeInfoService : ApplicationService, IEmployeeInfoService
    {
        private IEmployeeInfoRepository _EmployeeInfoRepository;
        private ICompanyDomainService _CompanyDomainService;
        private IEmporgService _emporgService;

        public EmployeeInfoService(IEmployeeInfoRepository EmployeeInfoRepository,ICompanyDomainService CompanyDomainService, IEmporgService emporgService)
        {
            _EmployeeInfoRepository = EmployeeInfoRepository;
            _CompanyDomainService = CompanyDomainService;
            _emporgService = emporgService;
        }

        public async Task<EmployeeDto> GetEmployeeAsync(string user)
        {
            EmployeeDto employeeDto = await QueryEmployeeAsync(user);
            return employeeDto;
        }
        public async Task<EmployeeDto> QueryEmployeeAsync(string user)
        {
            EmployeeDto employeeDto = new EmployeeDto();
            //查询返回员工信息
            EmployeeInfo employeeInfo = await _EmployeeInfoRepository.QueryByEmplid(user);
            if (employeeInfo == null) return employeeDto;
            //查询PJccode是否存在
            employeeDto.emplid = employeeInfo.emplid;
            employeeDto.deptid = employeeInfo.deptid;
            employeeDto.cname = employeeInfo.name;
            employeeDto.ename = employeeInfo.name_a;
            employeeDto.email = employeeInfo.email_address_a;
            employeeDto.phone = employeeInfo.phone_a;
            employeeDto.isaccount = true ;
            employeeDto.company = employeeInfo.company;
            return employeeDto;
        }
        public async Task<List<EmployeeInfoDto>> GetEmployeeInfos(string keyword)
        {
            Console.WriteLine("keyword: " + keyword);
            Console.WriteLine("get data: " + await _EmployeeInfoRepository.QueryByEmplidOrEngName(keyword));

            return (await _EmployeeInfoRepository.QueryByEmplidOrEngName(keyword)).Select(s => new EmployeeInfoDto
            {
                emplid = s.emplid,
                name = s.name,
                nameA = s.name_a,
            }).ToList();
        }
    }
}