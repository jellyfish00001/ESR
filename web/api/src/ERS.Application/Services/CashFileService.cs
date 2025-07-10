using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ERS.Application.Contracts.DTO.CashFile;
using ERS.Domain.IDomainServices;
using ERS.Entities;
using ERS.Minio;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
using ERS.IDomainServices;
namespace ERS.Application.Services
{
    public class CashFileService : ApplicationService, ICashFileService
    {
        private ICashFileDomainService _cashFileDomainService;
        private IMinioRepository _minioRepository;
        private IMinioDomainService _minioDomainService;
        public CashFileService(ICashFileDomainService cashFileDomainService, IMinioRepository minioRepository, IMinioDomainService minioDomainService)
        {
            _cashFileDomainService = cashFileDomainService;
            _minioRepository = minioRepository;
            _minioDomainService = minioDomainService;
        }
        public async Task<string> add(IFormCollection formCollection, string user)
        {
            string area = await _minioDomainService.GetMinioArea(user);
            List<CashFile> cashes = new List<CashFile>();
            string rno = formCollection["rno"].ToString();//获取单号
            string company = formCollection["company"].ToString();//获取公司别
            string formcode = formCollection["formcode"].ToString();//获取fromcode
            string item = formCollection["item"].ToString();//获取单据item
            string ishead = formCollection["ishead"].ToString();//是否为head附件
            string category = formCollection["category"].ToString();//获取类型
            string filetype = formCollection["filetype"].ToString();//获取类型名称
            string Taday = DateTime.Now.ToString("yyyyMM");
            int i = 1;//附件中的item
            var files = formCollection.Files;
            foreach (var file in files)
            {
                string objectName = Taday + "/" + rno + "/" + file.FileName;
                using (var steam = file.OpenReadStream())
                {
                    await _minioRepository.PutObjectAsync(objectName, steam, file.ContentType,area);
                }
                CashFile cashFile = new CashFile();
                cashFile.rno = rno;
                cashFile.company = company;
                cashFile.seq = Convert.ToInt32(item);
                cashFile.item = i;
                cashFile.category = category;//类型
                cashFile.filetype = filetype;//类型名称
                cashFile.path = objectName;//存储路径
                cashFile.filename = file.FileName;//文件名称
                cashFile.formcode = formcode;
                cashFile.tofn = file.FileName;//原名称
                cashFile.ishead = ishead;//是否为head附件
                cashFile.cdate = System.DateTime.Now;
                cashFile.cuser = user;
                cashes.Add(cashFile);
                i++;
            }
            return await _cashFileDomainService.add(cashes);
        }
        public async Task<string> delete(CashFileDto cash, string user)
        {
            try{
                string area = await _minioDomainService.GetMinioArea(user);
                CashFile cashFile = new CashFile();
                cashFile.rno = cash.rno;
                cashFile.seq = cash.seq;
                cashFile.item = cash.item;
                cashFile.formcode = cash.formcode;
                cashFile.company = cash.company;
                CashFile _cashFile   = await _cashFileDomainService.query(cashFile);
               await _minioRepository.RemoveObjectAsync(_cashFile.path,area);
               return "success";
            }catch(Exception e)
            {
            return e.Message;
            }
        }
    }
}