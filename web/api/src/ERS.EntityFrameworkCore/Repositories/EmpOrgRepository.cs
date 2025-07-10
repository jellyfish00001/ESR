using ERS.Entities;
using ERS.EntityFrameworkCore;
using ERS.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ERS.Repositories
{
    public class EmpOrgRepository : EfCoreRepository<ERSDbContext, EmpOrg, Guid>, IEmpOrgRepository
    {
        public EmpOrgRepository(IDbContextProvider<ERSDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }


        public async Task<IList<EmpOrg>> GetDeptidList(string company)
        {
            return (await GetDbSetAsync()).Where(b =>b.company==company).AsNoTracking().ToList();
        }
        public async Task<IList<EmpOrg>> GetDeptidList(int number)
        {
            return (await GetDbSetAsync()).Take(number).AsNoTracking().ToList();
        }

        public async Task<EmpOrg> GetDeptid(string deptid, string company) => (await GetDbSetAsync()).Where(b => b.deptid == deptid && b.company == company).AsNoTracking().FirstOrDefault();

        public async Task<EmpOrg> GetDeptid(string deptid) => (await GetDbSetAsync()).Where(b => b.deptid == deptid).AsNoTracking().FirstOrDefault();

        public async Task<IEnumerable<EmpOrg>> QueryVagues(string deptid, string company)
        {
            return await (await GetDbSetAsync()).Where(b => b.company == company && b.tree_level_num <= 7 && b.flag == 0 && b.deptid.Contains(deptid)).AsNoTracking().OrderBy(b => b.deptid).Take(10).ToListAsync();
        }

        public async Task<IEnumerable<EmpOrg>> QueryVagues(string deptid)
        {
            return await (await GetDbSetAsync()).Where(b => b.tree_level_num <= 7 && b.flag == 0 && b.deptid.Contains(deptid.ToUpper())).AsNoTracking().OrderBy(b => b.deptid).Distinct().Take(10).ToListAsync();
        }
        public async Task<string> GetCostDeptid(string deptid, string company)
        {

            EmpOrg empOrg = await this.GetDeptid(deptid);
            string _deptid = empOrg.deptid;
            if (empOrg.tree_level_num > 7)
            {
                _deptid = await GetCostDeptid(empOrg.uporg_code_a, company);
            }
            return _deptid;
        }
        public async Task<string> GetPlantByDeptid(string deptid)
        {
            return await (await GetDbSetAsync()).Where(b => b.deptid == deptid).AsNoTracking().Select(i => i.plant_id_a).FirstOrDefaultAsync();
        }

        public async Task<string> GetBGByDeptid(string deptid)
        {
            EmpOrg empOrg = await this.GetDeptid(deptid);
            string _emplid = empOrg.manager_id;
            if (empOrg.tree_level_num > 2)
                _emplid = await GetBGByDeptid(empOrg.uporg_code_a);
            return _emplid;
        }

        public async Task<string> GetCompanyCodeByDeptid(string deptid)
        {
            return await (await GetDbSetAsync()).Where(w => w.deptid == deptid).AsNoTracking().Select(s => s.company).FirstOrDefaultAsync();
        }
    }
}
