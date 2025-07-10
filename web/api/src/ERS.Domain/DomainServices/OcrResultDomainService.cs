using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using Volo.Abp.ObjectMapping;

namespace ERS.DomainServices
{
    public class OcrResultDomainService : CommonDomainService, IOcrResultDomainService
    {
        private readonly IOcrResultRepository _ocrResultRepository;
        private readonly IObjectMapper _objectMapper;

        public OcrResultDomainService(
            IOcrResultRepository ocrResultRepository,
            IObjectMapper objectMapper)
        {
            _ocrResultRepository = ocrResultRepository;
            _objectMapper = objectMapper;
        }

        public async Task<Result<List<OcrResultDto>>> Search(Request<OcrResultDto> request)
        {
            Result<List<OcrResultDto>> result = new Result<List<OcrResultDto>>()
            {
                data = new List<OcrResultDto>()
            };

            if (request == null || request.data == null)
            {
                result.message = "illegal request";
                result.status = 2;
                return result;
            }

            List<OCRResults> query = (await _ocrResultRepository.WithDetailsAsync())
                .WhereIf(!string.IsNullOrEmpty(request.data.IdentificationNo), x => x.IdentificationNo.Contains(request.data.IdentificationNo))
                .WhereIf(!string.IsNullOrEmpty(request.data.InvoiceNo), x => x.InvoiceNo.Contains(request.data.InvoiceNo))
                .WhereIf(!string.IsNullOrEmpty(request.data.InvoiceCode), x => x.InvoiceCode.Contains(request.data.InvoiceCode))
                .WhereIf(request.data.InvoiceDate != null, x => x.InvoiceDate == request.data.InvoiceDate)
                .ToList();

            List<OcrResultDto> objMapList = _objectMapper.Map<List<OCRResults>, List<OcrResultDto>>(query);

            int pageIndex = request.pageIndex;
            int pageSize = request.pageSize;
            if (pageIndex < 1 || pageSize < 0)
            {
                pageIndex = 1;
                pageSize = 10;
            }
            int count = objMapList.Count;
            result.status = 1;
            result.data = objMapList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            result.total = count;
            return result;

        }

        public async Task<Result<string>> Create(OcrResultDto model, string userId)
        {
            Result<string> result = new Result<string>();

            if (string.IsNullOrEmpty(model.Id) || !Guid.TryParse(model.Id, out Guid guid))
            {
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            // Check if record with same identification number exists
            var exists = (await _ocrResultRepository.WithDetailsAsync())
                .Any(x => x.Id.ToString() == model.Id);

            if (exists)
            {
                result.message = L["SaveFailMsg"] + "：" + L["RecordAlreadyExists"];
                result.status = 2;
                return result;
            }

            var entity = _objectMapper.Map<OcrResultDto, OCRResults>(model);
            entity.cuser = userId;
            entity.cdate = DateTime.Now;
            entity.muser = userId;
            entity.mdate = DateTime.Now;

            await _ocrResultRepository.InsertAsync(entity);
            
            result.status = 1;
            result.message = L["SaveSuccessMsg"];
            result.data = entity.Id.ToString();
            
            return result;
        }

        public async Task<Result<string>> Update(OcrResultDto model, string userId)
        {
            Result<string> result = new Result<string>();
            
            if (string.IsNullOrEmpty(model.Id) || !Guid.TryParse(model.Id, out Guid guid))
            {
                result.message = "The parameter cannot be null.";
                result.status = 2;
                return result;
            }

            var entity = await _ocrResultRepository.FindAsync(x => x.Id == guid);
            if (entity != null)
            {
                // Update properties
                _objectMapper.Map(model, entity);
                entity.muser = userId;
                entity.mdate = DateTime.Now;

                await _ocrResultRepository.UpdateAsync(entity);

                result.status = 1;
                result.message = L["SaveSuccessMsg"];
            }
            
            return result;
        }

        public async Task<Result<string>> Delete(List<string> ids)
        {
            Result<string> result = new Result<string>();
            List<OCRResults> toRemove = new List<OCRResults>();
            
            foreach (string id in ids)
            {
                if (Guid.TryParse(id, out Guid guid))
                {
                    var entity = await _ocrResultRepository.FindAsync(x => x.Id == guid);
                    if (entity != null)
                    {
                        toRemove.Add(entity);
                    }
                }
            }
            
            if (toRemove.Count > 0)
            {
                await _ocrResultRepository.DeleteManyAsync(toRemove);
                result.status = 1;
                result.message = L["DeleteSuccess"];
            }
            
            return result;
        }
    }
}
