
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDVender;
using Volo.Abp.ObjectMapping;

namespace ERS.DomainServices
{
    public class BDVenderDomainService : CommonDomainService, IBDVenderDomainService
    {
        private IBDVenderRepository _iBDVenderRepository;
        private IObjectMapper _iObjectMapper;

        public BDVenderDomainService(IBDVenderRepository iBDVenderRepository,
            IObjectMapper iObjectMapper)
        {
            _iBDVenderRepository = iBDVenderRepository;
            _iObjectMapper = iObjectMapper;
        }

        public async Task<Result<string>> Create(BDVenderParamDto model, string userId)
        {
            Result<string> result = new Result<string>();
            if (string.IsNullOrEmpty(model.UnifyCode))
            {
                result.message = L["SaveFailMsg"] + "：" + L["UnifiedNoNotEmpty"];
                result.status = 2;
                return result;
            }
            List<BDVender> ls = (await _iBDVenderRepository.WithDetailsAsync()).Where(x => x.VenderCode == model.VenderCode).ToList();//await _supplierRepository.FindAsync(x => x.unifiedNo == model.unifiedNo || x.supplierNo == model.supplierNo);
            if (ls.Count == 0)
            {
                BDVender en = new BDVender();
                en.UnifyCode = model.UnifyCode;
                en.VenderCode = model.VenderCode;
                en.VenderName = model.VenderName;
                en.company = "All";
                en.mdate = System.DateTime.Now;
                en.muser = userId;
                await _iBDVenderRepository.InsertAsync(en);
                result.status = 1;
                result.message = L["SaveSuccessMsg"];
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["VenderCodeExist"];
                result.status = 2;
            }
            return result;
        }

        public async Task<Result<List<BDVenderParamDto>>> Search(Request<BDVenderParamDto> request)
        {
            List<BDVender> ls = (await _iBDVenderRepository.WithDetailsAsync()).
                WhereIf(!string.IsNullOrEmpty(request.data.UnifyCode),e => e.UnifyCode.Contains(request.data.UnifyCode)).
                WhereIf(!string.IsNullOrEmpty(request.data.VenderCode), e => e.VenderCode.Contains(request.data.VenderCode)).
                WhereIf(!string.IsNullOrEmpty(request.data.VenderName), e => e.VenderName.Contains(request.data.VenderName))
                .ToList();
            List<BDVenderParamDto> res = _iObjectMapper.Map<IList<BDVender>, IList<BDVenderParamDto>>(ls).ToList();

            //foreach (BDVender supplier in ls)
            //{
            //    BDVenderParamDto model = new BDVenderParamDto();
            //    model.venderCode = supplier.venderCode;
            //    model.venderName = supplier.venderName;
            //    model.unifyCode = supplier.unifyCode;
            //    model.id = supplier.Id.ToString();
            //    res.Add(model);
            //}
            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = res.Count;
            Result<List<BDVenderParamDto>> result = new Result<List<BDVenderParamDto>>()
            {
                data = new List<BDVenderParamDto>()
            };
            result.data = res.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;
        }

        public async Task<Result<string>> UpdateSupplierInfo(BDVenderParamDto en, string userId)
        {
            Result<string> result = new Result<string>();
            bool isDuplicate = false;
            List<BDVender> ls = (await _iBDVenderRepository.WithDetailsAsync()).Where(x => x.VenderCode == en.VenderCode).ToList();
            foreach (BDVender supplier in ls)
            {
                if (!supplier.Id.ToString().Equals(en.Id))
                {
                    isDuplicate = true;
                    break;
                }
            }

            if(!isDuplicate) {
                BDVender oldData = await _iBDVenderRepository.FindAsync(x => en.Id.Equals(x.Id.ToString()));
                if (oldData != null)
                {
                    oldData.UnifyCode = en.UnifyCode;
                    oldData.VenderCode = en.VenderCode;
                    oldData.VenderName = en.VenderName;
                    oldData.mdate = System.DateTime.Now;
                    oldData.muser = userId;
                    await _iBDVenderRepository.UpdateAsync(oldData);
                    result.status = 1;
                    result.message = L["SaveSuccessMsg"];
                }
                else
                {
                    result.message = L["SaveFailMsg"] + "：" + L["VenderNotExist"];
                    result.status = 2;

                }
            }
            else
            {
                result.message = L["SaveFailMsg"] + "：" + L["VenderCodeExist"];
                result.status = 2;
            }
                
            
            
            return result;
        }

        public async Task<Result<string>> DeleteSupplier(List<string> ids)
        {
            Result<string> result = new Result<string>();
            List<BDVender> toRemove = new List<BDVender>();
            foreach (string id in ids)
            {
                BDVender en = await _iBDVenderRepository.FindAsync(x => x.Id.ToString() == id);
                if (en != null)
                    toRemove.Add(en);
            }
            if (toRemove.Count > 0)
                await _iBDVenderRepository.DeleteManyAsync(toRemove);
            result.status = 1;
            return result;
        }

        public async Task<Result<List<BDVenderParamDto>>> Download(Request<BDVenderParamDto> request)
        {
            List<BDVender> ls = (await _iBDVenderRepository.WithDetailsAsync()).
                WhereIf(!string.IsNullOrEmpty(request.data.UnifyCode), e => e.UnifyCode.Contains(request.data.UnifyCode)).
                WhereIf(!string.IsNullOrEmpty(request.data.VenderCode), e => e.VenderCode.Contains(request.data.VenderCode)).
                WhereIf(!string.IsNullOrEmpty(request.data.VenderName), e => e.VenderName.Contains(request.data.VenderName))
                .ToList();
            List<BDVenderParamDto> res = new List<BDVenderParamDto>();
            foreach (BDVender vender in ls)
            {
                BDVenderParamDto model = new BDVenderParamDto();
                model.VenderCode = vender.VenderCode;
                model.VenderName = vender.VenderName;
                model.UnifyCode = vender.UnifyCode;
                model.Id = vender.Id.ToString();
                res.Add(model);
            }
            int count = res.Count;
            Result<List<BDVenderParamDto>> result = new Result<List<BDVenderParamDto>>()
            {
                data = new List<BDVenderParamDto>()
            };
            result.data = res;
            result.total = count;
            return result;
        }

        /// <summary>
        /// 查找所有供应商unify_code和vender_name信息
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<BDVenderParamDto>>> GetAllVendors()
        {
            List<BDVender> ls = (await _iBDVenderRepository.WithDetailsAsync()).
                                 OrderBy(e => e.VenderName)
                                 .ToList();
            List<BDVenderParamDto> res = _iObjectMapper.Map<IList<BDVender>, IList<BDVenderParamDto>>(ls).ToList();
            Result<List<BDVenderParamDto>> result = new Result<List<BDVenderParamDto>>()
            {
                data = res,
                total = res.Count
            };
            return result;
        }
    }
}