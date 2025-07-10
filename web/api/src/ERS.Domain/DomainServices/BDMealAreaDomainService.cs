using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.BDMealArea;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Microsoft.AspNetCore.Http;
using Volo.Abp.ObjectMapping;
namespace ERS.DomainServices
{
    public class BDMealAreaDomainService : CommonDomainService, IBDMealAreaDomainService
    {
        private IBDMealAreaRepository _BDMealAreaRepository;
        private IObjectMapper _ObjectMapper;
        public BDMealAreaDomainService(
            IBDMealAreaRepository BDMealAreaRepository,
            IObjectMapper ObjectMapper
        )
        {
            _BDMealAreaRepository = BDMealAreaRepository;
            _ObjectMapper = ObjectMapper;
        }
        public async Task<Result<List<UploadBDMealAreaDto>>> BatchUploadBDMealArea(IFormFile excelFile, string userId)
        {
            Result<List<UploadBDMealAreaDto>> results = new()
            {
                data = new()
            };
            if (excelFile.ContentType == "application/vnd.ms-excel" || excelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                DataTable dt = NPOIHelper.GetDataTableFromExcel(excelFile, 1);
                var list = dt.Rows.Cast<DataRow>().Select((s, i) => new
                {
                    area = s[0].ToString().Trim(),
                    city = s[1].ToString().Trim(),
                    gotime = s[2].ToString().Trim(),
                    backtime = s[3].ToString().Trim(),
                    amount = s[4].ToString().Trim(),
                    currency = s[5].ToString().Trim()
                }).ToList();
                List<AddBDMealAreaDto> addList = new();
                for (int i = 0; i < list.Count; i++)
                {
                    AddBDMealAreaDto addBDMealAreaDto = new();
                    addBDMealAreaDto.area = list[i].area;
                    addBDMealAreaDto.city = list[i].city;
                    addBDMealAreaDto.amount = Convert.ToDecimal(list[i].amount);
                    addBDMealAreaDto.currency = list[i].currency;
                    addBDMealAreaDto.company = "WZS";
                    //出发时间处理
                    if (!String.IsNullOrEmpty(list[i].gotime))
                    {
                        //以"前"、"后"分段
                        string goFrontTime = list[i].gotime.Contains("前") ? list[i].gotime.Split("前").First() : String.Empty;
                        string goBackTime = list[i].gotime.Contains("后") ? list[i].gotime.Split("后").First() : String.Empty;
                        //前
                        if (!String.IsNullOrEmpty(goFrontTime))
                        {
                            //若有多段时间以“-”分割
                            if (goFrontTime.Contains("-"))
                            {
                                //14:00-19:00前
                                string[] goFrontTimeGroup = goFrontTime.Split("-");
                                addBDMealAreaDto.gotype1 = 1;
                                addBDMealAreaDto.gotype2 = -1;
                                addBDMealAreaDto.gotime1 = Convert.ToDateTime(goFrontTimeGroup[0]);
                                addBDMealAreaDto.gotime2 = Convert.ToDateTime(goFrontTimeGroup[1]);
                            }
                            //单段时间
                            if(!goFrontTime.Contains("-"))
                            {
                                addBDMealAreaDto.gotype1 = -1;
                                addBDMealAreaDto.gotype2 = 0;
                                addBDMealAreaDto.gotime1 = Convert.ToDateTime(goFrontTime);
                            }
                        }
                        //后
                        if (!String.IsNullOrEmpty(goBackTime))
                        {
                            addBDMealAreaDto.gotype1 = 1;
                            addBDMealAreaDto.gotype2 = 0;
                            addBDMealAreaDto.gotime1 = Convert.ToDateTime(goBackTime);
                        }
                    }
                    //回到时间处理
                    if (!String.IsNullOrEmpty(list[i].backtime))
                    {
                        string backFrontTime = list[i].backtime.Contains("前") ? list[i].backtime.Split("前").First() : String.Empty;
                        string returnBackTime = list[i].backtime.Contains("后") ? list[i].backtime.Split("后").First() : String.Empty;
                        if (!String.IsNullOrEmpty(backFrontTime))
                        {
                            if (backFrontTime.Contains("-"))
                            {
                                string[] backFrontTimeGroup = backFrontTime.Split("-");
                                addBDMealAreaDto.backtype1 = 1;
                                addBDMealAreaDto.backtype2 = -1;
                                addBDMealAreaDto.backtime1 = Convert.ToDateTime(backFrontTimeGroup[0]);
                                addBDMealAreaDto.backtime2 = Convert.ToDateTime(backFrontTimeGroup[1]);
                            }
                            if(!backFrontTime.Contains("-"))
                            {
                                addBDMealAreaDto.backtype1 = -1;
                                addBDMealAreaDto.backtype2 = 0;
                                addBDMealAreaDto.backtime1 = Convert.ToDateTime(backFrontTime);
                            }
                        }
                        if (!String.IsNullOrEmpty(returnBackTime))
                        {
                            addBDMealAreaDto.backtype1 = 1;
                            addBDMealAreaDto.backtype2 = 0;
                            addBDMealAreaDto.backtime1 = Convert.ToDateTime(returnBackTime);
                        }
                    }
                    addList.Add(addBDMealAreaDto);
                }
                List<BDMealArea> iBDMealAreaList = _ObjectMapper.Map<List<AddBDMealAreaDto>,List<BDMealArea>>(addList);
                var cities = iBDMealAreaList.GroupBy(w => w.city).Select(s => s.FirstOrDefault()).Select(g => g.city).ToList();
                string s = string.Join(",",cities.ToArray());
                await _BDMealAreaRepository.InsertManyAsync(iBDMealAreaList);
            }
            return results;
        }
    }
}