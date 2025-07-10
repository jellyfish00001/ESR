using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERS.DTO;
using ERS.DTO.ApprovalFlow;
using ERS.DTO.BDExpenseSenario;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using Volo.Abp.ObjectMapping;
using System.Data;
using ERS.Attribute;
using ERS.DTO.BDExp;
using Microsoft.EntityFrameworkCore;

namespace ERS.DomainServices
{
    public class ApprovalFlowDomainService : CommonDomainService, IApprovalFlowDomainService
    {
        private IApprovalFlowRepository _ApprovalFlowRepository;
        private IAppConfigRepository _AppConfigRepository;
        private IApprovalAssignedApproverRepository _ApprovalAssignedApproverRepository;
        private IBDTreelevelRepository _BDTreelevelRepository;
        private IBDExpRepository _BDExpRepository;
        private IBDSignlevelRepository _BDSignlevelRepository;
        private IBDExpenseSenarioRepository _BDSenarioRepository;
        private IDataDictionaryRepository _DataDictionaryRepository;
        private IEmployeeInfoRepository _EmployeeInfoRepository;
        private IEmpOrgRepository _EmpOrgRepository;
        private IEFormAuditRepository _EFormAuditRepository;
        private IFinreviewRepository _FinreviewRepository;
        private IObjectMapper _ObjectMapper;

        public ApprovalFlowDomainService(
          IApprovalFlowRepository iApprovalFlowRepository,
          IAppConfigRepository iAppConfigRepository,
          IApprovalAssignedApproverRepository iApprovalAssignedApproverRepository,
          IBDTreelevelRepository iBDTreelevelRepository,
          IBDSignlevelRepository iBDSignlevelRepository,
          IBDExpRepository iBDExpRepository,
          IBDExpenseSenarioRepository iBDSenarioRepository,
          IDataDictionaryRepository iDataDictionaryRepository,
          IEmployeeInfoRepository iEmployeeInfoRepository,
          IEmpOrgRepository iEmpOrgRepository,
          IEFormAuditRepository iEFormAuditRepository,
          IFinreviewRepository iFinreviewRepository,
          IObjectMapper iObjectMapper
        ){
          _ApprovalFlowRepository = iApprovalFlowRepository;
          _AppConfigRepository = iAppConfigRepository;
          _ApprovalAssignedApproverRepository = iApprovalAssignedApproverRepository;
          _BDTreelevelRepository = iBDTreelevelRepository;
          _BDSignlevelRepository = iBDSignlevelRepository;
          _BDExpRepository = iBDExpRepository;
          _BDSenarioRepository = iBDSenarioRepository;
          _DataDictionaryRepository = iDataDictionaryRepository;
          _EmployeeInfoRepository = iEmployeeInfoRepository;
          _EmpOrgRepository = iEmpOrgRepository;
          _EFormAuditRepository = iEFormAuditRepository;
          _FinreviewRepository = iFinreviewRepository;
          _ObjectMapper = iObjectMapper;
        }

        public async Task<Result<ApprovalFlowDto>> GetApprovalFlowById(Guid? id)
        {
            Result<ApprovalFlowDto> result = new Result<ApprovalFlowDto>();
            ApprovalFlow appFlow = await _ApprovalFlowRepository.GetApprovalFlowById(id);
            if (appFlow == null)
            {
              result.status = 2;
              result.message = L["ApprovalFlowNotFound"];
              return result;
            }

            result.data = _ObjectMapper.Map<ApprovalFlow, ApprovalFlowDto>(appFlow);

            return result;
        }

        public async  Task<Result<List<ApprovalFlowDto>>> GetApprovalFlowByRno(string rno)
        {
            //Rno是報銷單號
            Result<List<ApprovalFlowDto>> result = new Result<List<ApprovalFlowDto>>();

            List<ApprovalFlow> appFlow = await _ApprovalFlowRepository.GetApprovalFlowByRno(rno);
            result.status = 1;
            if (appFlow == null || appFlow.Count == 0)
            {   
              result.status = 2;             
              result.message = L["ApprovalFlowNotFound"];
              return result;
            }
            
            result.data = _ObjectMapper.Map<List<ApprovalFlow>, List<ApprovalFlowDto>>(appFlow);
            return result;
        }

        // Uber起單
        public async Task<Result<string>> ApplyUberApprovalFlow(CashHead cashHead,  List<CashDetail> cashDetails)
        {
            Result<string> result = new Result<string>();            
            List<ApprovalFlow> totalFlow = new List<ApprovalFlow>();   //欲儲存的流程

            //檢查rno是否存在有效流程
            bool isOngoing = CheckIsFlowOnGoing(cashHead.rno).Result;
            if (isOngoing) return errorResult(result, String.Format(L["ApprovalFlowAlreadyExists"], cashHead.rno));

            //狀況1: 交通車滿載，只要總務簽核
            if(cashHead.Program=="交通車滿載"){
              //撈取總務簽核人
              AppConfig generalAffair = (await _AppConfigRepository.WithDetailsAsync())
                                    .FirstOrDefault(a => a.key == "Uber_SignID" && a.company == cashHead.company);
              
              if(generalAffair == null || string.IsNullOrEmpty(generalAffair.value)){
                return errorResult(result, L["UberSignIDNotFound"]);
              }
              ApprovalFlow newFlow = new ApprovalFlow();   //欲儲存的流程
              Guid newId = Guid.NewGuid(); //產生新的ID  
              newFlow.SetId(newId); //設定ID
              newFlow.rno = cashHead.rno;  //報銷單號
              newFlow.step = GetMaxStep(cashHead.rno).Result+1;         //簽核步驟順序
              newFlow.stepname = "GeneralAffairs";  //簽核步驟名稱
              newFlow.aassignedemplid = generalAffair.value;   //總務簽核人工號
              newFlow.assignedapprovername = _EmployeeInfoRepository.QueryByEmplidOrEngName(generalAffair.value).Result.FirstOrDefault().name_a;  //簽核人姓名
              newFlow.assigneddeptid = _EmployeeInfoRepository.QueryByEmplidOrEngName(generalAffair.value).Result.FirstOrDefault().deptid;  //簽核人部門代碼
              newFlow.status = "P";  //處理中
              newFlow.company = cashHead.company;  //簽核公司別

              //將可簽核人員放到assigned approver表中              
              ApprovalAssignedApprover approver = new ApprovalAssignedApprover();
              approver.flow_id = newFlow.Id; //流程ID
              approver.approver_emplid = newFlow.aassignedemplid; //簽核人工號
              approver.approver_name = newFlow.assignedapprovername; //簽核人姓名
              approver.approver_deptid = newFlow.assigneddeptid; //簽核人部門代碼
              approver.company = newFlow.company; //簽核公司別
              await _ApprovalAssignedApproverRepository.InsertAsync(approver, true); //儲存應該簽核人員              
              
              try{
                await _ApprovalFlowRepository.InsertAsync(newFlow, true); //儲存流程
              }
              catch (Exception ex) {
                return errorResult(result, ex.Message);
              }
            }
            //狀況2:非交通車滿載，申請人以及核決主管簽核，核決權限固定抓A1
            else{
              ApprovalFlow newFlow = new ApprovalFlow();   //欲儲存的流程
              decimal step = GetMaxStep(cashHead.rno).Result; //取得當前最大步驟
              //申請人
              string approverName = _EmployeeInfoRepository.QueryByEmplidOrEngName(cashHead.cuser).Result.FirstOrDefault().name_a; //申請人姓名
              string approverDept = _EmployeeInfoRepository.QueryByEmplidOrEngName(cashHead.cuser).Result.FirstOrDefault().deptid; //申請人部門代碼
              newFlow.rno = cashHead.rno;  //報銷單號
              newFlow.step = ++step;         //簽核步驟順序
              newFlow.stepname = "UberApplyer";  //簽核步驟名稱
              newFlow.aassignedemplid = cashHead.cuser;   //申請人工號
              newFlow.assignedapprovername = approverName;  //簽核人姓名
              newFlow.assigneddeptid = approverDept;  //簽核人部門代碼
              newFlow.status = "P";  //處理中
              newFlow.company = cashHead.company;  //簽核公司別
              newFlow.nextflowid = Guid.Empty; //下一個流程ID設為空
              totalFlow.Add(newFlow);

              //核決主管
              decimal totalAmount = (decimal)cashDetails.Sum(d => d.amount);
              BDSignlevel applicableLevels = (await _BDSignlevelRepository.WithDetailsAsync())
                    .Where(sl => sl.item == "A1" && sl.money < totalAmount && sl.company == cashHead.company)
                    .ToList()
                    .OrderBy(sl => int.Parse(sl.signlevel))  // 按照 signlevel 升序排列
                    .First();
              SignDept dept = new SignDept(){
                deptid = approverDept,
                signitem = "A1",
                signlevel = applicableLevels.signlevel,
                amount = (decimal)cashDetails.Sum(d => d.amount) //計算總金額
              };
              List<SignManager> signManager = new List<SignManager>();
              signManager = await ProcessSignManager(dept);
              if(signManager != null && signManager.Count >0 ){
                foreach(SignManager manager in signManager){
                  newFlow = new ApprovalFlow();
                  newFlow.rno = cashHead.rno;  //報銷單號
                  newFlow.step = ++step;         //簽核步驟順序
                  newFlow.stepname = (await _BDTreelevelRepository.WithDetailsAsync()).Where(l => l.levelnum == int.Parse(manager.signlevel)).Select(l => l.levelname).FirstOrDefault();  //簽核步驟名稱
                  newFlow.aassignedemplid = manager.managerid;   //簽核人工號
                  newFlow.assignedapprovername = _EmployeeInfoRepository.QueryByEmplidOrEngName(manager.managerid).Result.FirstOrDefault().name_a;  //簽核人姓名
                  newFlow.assigneddeptid = manager.deptid;  //簽核人部門代碼
                  newFlow.status = "N";  //處理中
                  newFlow.company = cashHead.company;  //簽核公司別
                  totalFlow.Add(newFlow);
                }
              }
            }
            if(totalFlow.Count > 0){
              try{
                //level1 填寫Id和next_flow_id
                int count=0;
                Guid lastId = Guid.Empty;
                foreach(ApprovalFlow flow in totalFlow.OrderByDescending(a => a.step)){
                  Guid newId = Guid.NewGuid(); //產生新的ID  
                  flow.SetId(newId); //設定ID
                  if(count == 0){
                    flow.nextflowid = Guid.Empty;
                  }
                  else{
                    flow.nextflowid = lastId;
                  }
                  lastId = flow.Id;
                  count++;
                }
                await _ApprovalFlowRepository.InsertManyAsync(totalFlow, true); //儲存所有流程

                //將可簽核人員放到assigned approver表中
                foreach(ApprovalFlow mainFlow in totalFlow){
                  ApprovalAssignedApprover approver = new ApprovalAssignedApprover();
                  approver.flow_id = mainFlow.Id; //流程ID
                  approver.approver_emplid = mainFlow.aassignedemplid; //簽核人工號
                  approver.approver_name = mainFlow.assignedapprovername; //簽核人姓名
                  approver.approver_deptid = mainFlow.assigneddeptid; //簽核人部門代碼
                  approver.company = mainFlow.company; //簽核公司別
                  await _ApprovalAssignedApproverRepository.InsertAsync(approver, true); //儲存應該簽核人員
                }
              }
              catch (Exception ex) {
                return errorResult(result, ex.Message);
              }
            }
            
            result.status = 1;
            result.message = L["ApplyUberApprovalFlowSuccess"];
            return result;
        }

        public async Task<Result<string>> ApplyApprovalFlow(CashHead cashHead,  List<CashDetail> cashDetails)
        {
            //簽核三階段(step name)：Payee, AddSignBefore, Supervisor, AddSignAfter, Finance；各階段內，全都要approve才會往下個階段簽
            Result<string> result = new Result<string>();
            ApprovalFlow newFlow = new ApprovalFlow();   //欲儲存的流程
            ApprovalFlow savedFlow = new ApprovalFlow(); //剛儲存的流程
            List<ApprovalFlow> totalFlow = new List<ApprovalFlow>();   //欲儲存的流程

            //level1 *****根據rno查詢主單資訊是否合規*****
            //1.檢查cashHead和cashDetail
            if (cashHead == null || cashDetails == null || cashDetails.Count == 0) return errorResult(result, L["RnoHaveNoData"]);
            string rno = cashHead.rno;
            Guid lastFlowId = Guid.Empty;
            decimal maxStep = GetMaxStep(cashHead.rno).Result;
            decimal step = maxStep;
            cashDetails = cashDetails.OrderBy(cashDetail => cashDetail.seq).ToList();//按照seq排序

            //2.檢查rno是否已產生流程
            bool isOngoing = CheckIsFlowOnGoing(cashHead.rno).Result;
            if (isOngoing) return errorResult(result, String.Format(L["ApprovalFlowAlreadyExists"], cashHead.rno));

            //3. 取得emp_org資料
            //List<EmpOrg> empOrg = (await _EmpOrgRepository.WithDetailsAsync()).ToList();

            //4. 取得EmployeeInfo資料
            //List<EmployeeInfo> empInfo = (await _EmployeeInfoRepository.WithDetailsAsync()).ToList();

            //5.取得TreeLevel資料
            List<BDTreelevel> treeLevel = (await _BDTreelevelRepository.WithDetailsAsync()).ToList();

            //6.取得Auditor資料
            List<EFormAudit> auditor = (await _EFormAuditRepository.WithDetailsAsync()).ToList();

            //7.取得Finreview資料
            List<Finreview> finReview = (await _FinreviewRepository.WithDetailsAsync()).ToList();
            
            //8.取得DataDictionary設定資料
            //List<DataDictionary> dd = (await _DataDictionaryRepository.WithDetailsAsync()).Where(w => w.Category == "sign_step_name").ToList();

            //level1 *****收款人*****
            //如果申請人和收款人不同，需要新增收款人簽核
            if(cashHead.formcode == "CASH_4"){//批量報銷，受款人要抓detail的payeeid
              foreach(CashDetail detail in cashDetails){
                if(cashHead.cuser != detail.payeeid){
                  newFlow = SetPayeeFlow(true, cashHead, detail, ++step);
                  totalFlow.Add(newFlow);
                }
              }
            }
            else{//一般報銷
              if(cashHead.cuser !=cashHead.payeeId){
                newFlow = SetPayeeFlow(false, cashHead, cashDetails[0], ++step);
                totalFlow.Add(newFlow);
              }
            }

            //level1 算出這張單需要簽核的部門及金額
            List<BDExpenseSenario> allBdSenario = (await _BDSenarioRepository.WithDetailsAsync()).Where(e => e.companycategory == cashHead.company).ToList();
            List<BDSenarioDto> allBdSenarioDto = new List<BDSenarioDto>();
            foreach (BDExpenseSenario senario in allBdSenario){
                BDSenarioDto dto = await _BDSenarioRepository.GetBDSenarioById(senario.Id);
              if (dto != null) allBdSenarioDto.Add(dto);
            }
            if (!allBdSenarioDto.Any()) return errorResult(result, L["BDSenarioNotFound"]);
            List<BDSignlevel> allSignLevels = (await _BDSignlevelRepository.WithDetailsAsync()).ToList();
            List<SignDept> signDept = new List<SignDept>();
            signDept = ProcessSignDept(cashHead, cashDetails, allBdSenarioDto, allSignLevels);

            //level1 報銷情景(主管前)
            foreach(CashDetail detail in cashDetails){
              //取得報銷情景
              List<ExtraStepsDto> addSignBefore = allBdSenarioDto.Where(s=>s.extraSteps != null && s.extraSteps.Count >0)
                                                              .SelectMany(s => s.extraSteps)
                                                              .Where(e => e.Position == "Before")
                                                              .ToList();
              if(addSignBefore != null && addSignBefore.Count>0){
                foreach(ExtraStepsDto addSign in addSignBefore){
                  ApprovalFlow checkFlow = totalFlow.FirstOrDefault(a => a.assigneddeptid == addSign.ApproverEmplid && a.stepname == "AddSignBefore");  
                  if(addSign.ApproverEmplid !=null && addSign.ApproverEmplid != "" && checkFlow == null){
                    newFlow = SetAddSignFlow("AddSignBefore", cashHead, addSign, ++step); //填寫流程內容
                    totalFlow.Add(newFlow);
                  }
                }
              }
            }

            //level1 *****簽核主管*****
            foreach(SignDept dept in signDept){
              //根據cash detail統計完的部門，設定該部門的簽核主管
              List<SignManager> signManager = new List<SignManager>();
              signManager = await ProcessSignManager(dept);
              if(signManager != null && signManager.Count >0 ){
                foreach(SignManager manager in signManager){
                  newFlow = SetManagerFlow(treeLevel, cashHead, manager, ++step); //設定主管簽核流程
                  totalFlow.Add(newFlow);
                }
              }
            }
            totalFlow = AdjustManagerFlowStep(totalFlow, treeLevel, ref step, maxStep);//
            
            //level1 加簽Autitor
            totalFlow = SetAuditorFlow(totalFlow, cashHead, auditor, ref step);

            //level1 報銷情景(主管後)
            foreach(CashDetail detail in cashDetails){
              //取得報銷情景
               List<ExtraStepsDto> addSignAfter = allBdSenarioDto.Where(s=>s.extraSteps != null && s.extraSteps.Count >0)
                                                                 .SelectMany(s => s.extraSteps)
                                                                 .Where(e => e.Position == "After")
                                                                 .ToList();
              if(addSignAfter != null && addSignAfter.Count>0){
                foreach(ExtraStepsDto addSign in addSignAfter){
                  ApprovalFlow checkFlow = totalFlow.FirstOrDefault(a => a.aassignedemplid == addSign.ApproverEmplid && a.stepname == "AddSignAfter");  
                  if(addSign.ApproverEmplid !=null && addSign.ApproverEmplid != "" && checkFlow == null){
                    newFlow = SetAddSignFlow("AddSignAfter", cashHead, addSign, ++step); //填寫流程內容
                    totalFlow.Add(newFlow);
                  }
                }
              }
            }

            //level1 *****會計*****
            SignDept maxAmountDept = signDept.OrderByDescending(a => a.amount).FirstOrDefault();
            totalFlow = await SetFinFlow(totalFlow, cashHead, maxAmountDept, finReview, ++step);
            if(!totalFlow.Any()){
              return errorResult(result, L["Rv1NotExist"]);
            }

            //level1 填寫Id和next_flow_id
            int count=0;
            Guid lastId = Guid.Empty;
            decimal minStep = totalFlow.Min(s=>s.step);
            foreach(ApprovalFlow flow in totalFlow.OrderByDescending(a => a.step)){
              Guid newId = Guid.NewGuid(); //產生新的ID  
              flow.SetId(newId); //設定ID
              flow.status = flow.step == minStep ? "P" : "N"; //Step最小的設為P，其他為N
              if(count == 0){
                
              }
              else{
                flow.nextflowid = lastId;
              }
              lastId = flow.Id;
              count++;
            }
            await _ApprovalFlowRepository.InsertManyAsync(totalFlow, true);//儲存所有流程
            
            foreach(ApprovalFlow mainFlow in totalFlow){
              Console.WriteLine("$Step:{mainFlow.step}");
              string[] cemplids = mainFlow.aassignedemplid.Split(';', StringSplitOptions.RemoveEmptyEntries);
              string[] approvernames = mainFlow.assignedapprovername.Split(';', StringSplitOptions.RemoveEmptyEntries);
              string[] deptids = mainFlow.assigneddeptid.Split(';', StringSplitOptions.RemoveEmptyEntries);

              for (int i = 0; i < cemplids.Length; i++){
                //if(string.IsNullOrEmpty(cemplids[i])) continue; //如果工號是空的，則跳過
                ApprovalAssignedApprover approver = new ApprovalAssignedApprover();
                approver.flow_id = mainFlow.Id; //流程ID
                approver.approver_emplid = cemplids[i]; //簽核人工號
                approver.approver_name = approvernames[i]; //簽核人姓名
                approver.approver_deptid = deptids[i]; //簽核人部門代碼
                approver.company = mainFlow.company; //簽核公司別
                await _ApprovalAssignedApproverRepository.InsertAsync(approver, true); //儲存應該簽核人員
              }
              //ApprovalAssignedApprover approver = new ApprovalAssignedApprover(); //儲存應該簽核人員
            }

            result.status = 1;
            result.message = L["ApplyApprovalFlowSuccess"];
            return result;
        }

        //取得目前應該簽核的流程
        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlow(string rno){
            Result<List<ApprovalFlowDto>> result = new Result<List<ApprovalFlowDto>>();

            List<ApprovalFlow> flows = (await _ApprovalFlowRepository.WithDetailsAsync())
                              .Where(f => f.rno == rno)
                              .ToList();
            if (!flows.Any()){
              result.status = 2;
              result.message = L["ApprovalFlowNotFound"];
              return result;
            }

            decimal minStep = (await _ApprovalFlowRepository.WithDetailsAsync())
                                                .Where(f => f.rno == rno && f.status == "P")
                                                .Min(f => f.step);

            List<ApprovalFlow> currentFlow = (await _ApprovalFlowRepository.WithDetailsAsync())
                                                .Where(f => f.rno == rno && f.status == "P" && f.step == minStep)
                                                .ToList();

            if (currentFlow == null || currentFlow.Count == 0)
            {
                result.status = 2;
                result.message = L["ApprovalFlowNotFound"];
                return result;
            }

            List<ApprovalFlowDto> finalFlows = _ObjectMapper.Map<List<ApprovalFlow>, List<ApprovalFlowDto>>(currentFlow);
            foreach (ApprovalFlowDto flow in finalFlows)
            {
              //取得簽核人員工號
              List<ApprovalAssignedApprover> assignedApprovers = (await _ApprovalAssignedApproverRepository.WithDetailsAsync())
                                              .Where(a => a.flow_id == flow.Id)
                                              .ToList();
              if(assignedApprovers != null && assignedApprovers.Count > 0){
                  flow.assignedEmplids = assignedApprovers.Select(a => $"{a.approver_emplid}/{a.approver_name}").ToList();
              }
            }

            result.status = 1;
            result.data = finalFlows;

            return result;
        }

        //取得目前完整的簽核流程列表
        public async Task<Result<List<ApprovalFlowDto>>> GetCurrentApprovalFlowList(string rno){
            Result<List<ApprovalFlowDto>> result = new Result<List<ApprovalFlowDto>>();

            decimal maxStep = GetMaxStepWithX(rno).Result; //取得最大步驟

            List<ApprovalFlow> flows = (await _ApprovalFlowRepository.WithDetailsAsync())
                              .Where(f => f.rno == rno && f.step > maxStep).OrderBy(f=> f.step)
                              .ToList();
            if (!flows.Any()){
              result.status = 2;
              result.message = L["ApprovalFlowNotFound"];
              return result;
            }

            List<ApprovalFlowDto> finalFlows = _ObjectMapper.Map<List<ApprovalFlow>, List<ApprovalFlowDto>>(flows);
            foreach (ApprovalFlowDto flow in finalFlows)
            {
              //取得簽核人員工號
              List<ApprovalAssignedApprover> assignedApprovers = (await _ApprovalAssignedApproverRepository.WithDetailsAsync())
                                              .Where(a => a.flow_id == flow.Id)
                                              .ToList();
              if(assignedApprovers != null && assignedApprovers.Count > 0){
                  flow.assignedEmplids = assignedApprovers.Select(a => $"{a.approver_emplid}/{a.approver_name}").ToList();
              }
            }

            result.status = 1;
            result.data = finalFlows;

            return result;
        }

        public async Task<Result<string>> ForwardApprovalFlow(ProcessFlowDto request)
        {
            //Todo:轉單/邀簽功能
            await _ApprovalFlowRepository.InsertManyAsync(null, true);//儲存所有流程
            return null;
        }

        public async Task<Result<string>> DeleteApprovalFlow(ProcessFlowDto request)
        {
            Result<string> result = new Result<string>();
            if (request == null || string.IsNullOrEmpty(request.rno))
            {
                return errorResult(result, L["InvalidRequest"]);
            }

            Guid flowId = Guid.Empty;

            await _ApprovalFlowRepository.DeleteAsync(flowId, true); //刪除流程

            return null;
        }        
        
        //同意簽核
        public async Task<Result<string>> ApproveApprovalFlow(ProcessFlowDto request, string userId)
        {
            Result<string> result = new Result<string>();
            List<ApprovalFlow> allFlows = (await _ApprovalFlowRepository.WithDetailsAsync())
                                          .Where(f => f.rno == request.rno && (f.status == "P" || f.status == "N")) //取得正在處理和尚未處理的流程
                                          .ToList();
            ApprovalFlow currentFlow = allFlows.FirstOrDefault(f => f.aassignedemplid == userId && f.status == "P"); //根據登入人，在P的流程中找到當前流程
            if(currentFlow is null){
              return errorResult(new Result<string>(), L["notTheCurrentSigner"]);
            }

            Guid flowId = currentFlow.Id; //取得當前流程ID
            //取得簽核人相關資訊
            ApprovalAssignedApprover approver = (await _ApprovalAssignedApproverRepository.WithDetailsAsync())
                                          .FirstOrDefault(a => a.flow_id == flowId);

            ApprovalFlow flow = allFlows.FirstOrDefault(f=>f.Id == flowId);
            if (flow == null){
              return errorResult(new Result<string>(), L["ApprovalFlowNotFound"]);
            }
            flow.approveremplid = userId; //設定簽核人工號
            flow.approvername = approver.approver_name; //設定簽核人姓名
            flow.approverdeptid = approver.approver_deptid; //設定簽核人部門代碼
            flow.mdate = DateTime.Now; //設定簽核日期
            flow.status = "A"; //設定為已簽核
            flow.approvedate = DateTime.Now; //設定簽核日期
            flow.comment = request.comment; //設定簽核意見
            await _ApprovalFlowRepository.UpdateAsync(flow, true);
            result.message = L["signSucess"];

            //更新下一步流程為P
            decimal flowStep = flow.step; //取得當前流程步驟
            List<ApprovalFlow> sameFlow = allFlows.Where(f => f.step == flowStep && f.Id != flowId).ToList(); //取得其他的同一步驟流程
            if(sameFlow.All(f => f.status == "A")){
              //如果同一步驟的所有流程都已簽核，則將下一個流程設為P
              List<ApprovalFlow> nextFlows = allFlows.Where(f => f.step == flowStep + 1).ToList();
              if (nextFlows == null){
                result.message = L["FlowFinished"];
              }
              else{
                nextFlows.ForEach(f=>f.status = "P"); //將下一step流程設為P
                await _ApprovalFlowRepository.UpdateManyAsync(nextFlows, true);
              }
            }            
            return result;
        }
        
        public async Task<Result<string>> RejectApprovalFlow(ProcessFlowDto request)
        {
            List<ApprovalFlow> flows = (await _ApprovalFlowRepository.WithDetailsAsync())
                              .Where(f => f.rno == request.rno)
                              .ToList();
            if (!flows.Any()){
              return errorResult(new Result<string>(), L["ApprovalFlowNotFound"]);
            }

            foreach( ApprovalFlow flow in flows)
            {
              if( flow.aassignedemplid == request.approverEmplid){
                flow.cuser = request.approverEmplid; //設定簽核人工號
                flow.cdate = DateTime.Now; //設定簽核日期
                flow.comment = request.comment; //設定簽核意見
                flow.status = "R";  //設定為已拒絕
                flow.approvedate = DateTime.Now; //設定簽核日期
              }

              if(flow.status == "P" || flow.status == "N") //如果是處理中或未簽核的流程
                flow.status = "X"; //設定為不處理
            }
            await _ApprovalFlowRepository.UpdateManyAsync(flows, true);

            return null;
        }

        //取得歷史簽核
        public async Task<Result<List<ApprovalFlowDto>>> GetHistoryApprovalFlow(string rno)
        {            
            List<ApprovalFlow> flows = (await _ApprovalFlowRepository.WithDetailsAsync())
                                         .Where(f => f.rno == rno && !new[] { "X", "P", "N" }.Contains(f.status)) //排除處理中或尚未處理的流程
                                         .ToList();
            Result<List<ApprovalFlowDto>> result = new Result<List<ApprovalFlowDto>>();
            if (flows == null || flows.Count == 0)
            {
                result.status = 2;
                result.message = L["ApprovalFlowNotFound"];
                return result;
            }
            result.status = 1;
            result.data = _ObjectMapper.Map<List<ApprovalFlow>, List<ApprovalFlowDto>>(flows);
            return result;
        }

        internal Result<T> errorResult<T>(Result<T> result, string message)
        {
            result.status = 2;
            result.message = message;
            return result;
        }

        //設定收款人簽核流程
        internal ApprovalFlow SetPayeeFlow(bool isBatch, CashHead cashHead, CashDetail cashDetail, decimal step)
        {
            Result<string> result = new Result<string>();
            ApprovalFlow newFlow = new ApprovalFlow();
            string payeeId = isBatch ? cashDetail.payeeid : cashHead.payeeId;
            string payeeName = isBatch ? cashDetail.payeename : cashHead.payeename;

            newFlow.rno = cashHead.rno;  //報銷單號
            newFlow.step = step;         //簽核步驟順序
            newFlow.stepname = "Payee";  //簽核步驟名稱
            newFlow.aassignedemplid = payeeId;   //簽核人工號
            newFlow.assignedapprovername =  payeeName;  //簽核人姓名
            newFlow.assigneddeptid = _EmployeeInfoRepository.QueryByEmplidOrEngName(payeeId).Result.FirstOrDefault().deptid;  //簽核人部門代碼
            newFlow.status = "P";  //處理中
            newFlow.company = cashHead.company;  //簽核公司別
            return newFlow;
        }

        //設定按照簽核情景加簽(主管前和主管後共用)
        internal ApprovalFlow SetAddSignFlow(string afterOrBefore, CashHead cashHead, ExtraStepsDto addSign, decimal step)
        {   
            ApprovalFlow newFlow = new ApprovalFlow();

            newFlow.rno = cashHead.rno;  //報銷單號
            newFlow.step = step;         //簽核步驟順序
            newFlow.stepname = afterOrBefore;  //簽核步驟名稱
            newFlow.aassignedemplid = addSign.ApproverEmplid;   //簽核人工號
            newFlow.assignedapprovername = _EmployeeInfoRepository.QueryByEmplidOrEngName(addSign.ApproverEmplid).Result.FirstOrDefault().name_a;  //簽核人姓名
            newFlow.assigneddeptid = _EmployeeInfoRepository.QueryByEmplidOrEngName(addSign.ApproverEmplid).Result.FirstOrDefault().deptid;  //簽核人部門代碼
            newFlow.status = "P";  //處理中
            newFlow.company = cashHead.company;  //簽核公司別
            return newFlow;
        }

        List<ApprovalFlow>AdjustManagerFlowStep(List<ApprovalFlow> totalFlow, List<BDTreelevel> treeLevel, ref decimal step, decimal maxStep){
            //調整主管簽核的step，補上有受款人或主管簽核人的step
            decimal tempStep = maxStep; //從最大步驟開始計算
            bool firstTime = true;
            foreach(BDTreelevel level in treeLevel.OrderByDescending(a=>a.levelnum)){
              ApprovalFlow tempFlow = totalFlow.Where(a => a.stepname == level.levelname).FirstOrDefault();
              decimal minLevelNum=0;
              if(tempFlow != null){
                List<ApprovalFlow> dupManager = totalFlow.Where(a=>a.aassignedemplid == tempFlow.aassignedemplid).ToList();
                step--;
                if(dupManager != null && dupManager.Count > 1){
                  minLevelNum = dupManager
                                .Select(flow => treeLevel.FirstOrDefault(t => t.levelname == flow.stepname)?.levelnum ?? decimal.MaxValue)
                                .Min();
                   if(level.levelnum >= minLevelNum){
                     ApprovalFlow itemToRemove = totalFlow.FirstOrDefault(a => a.aassignedemplid == tempFlow.aassignedemplid && a.stepname == tempFlow.stepname);
                     if(itemToRemove != null){
                       totalFlow.Remove(itemToRemove); 
                     }
                   }
                }
                if (firstTime) {
                  tempStep = totalFlow.Count(a =>  (a.stepname.Contains("Payee") || a.stepname.Contains("AddSignBefore")) && 
                                                   (a.status == "P" || a.status == "N")) + tempStep +1 ;
                  firstTime = false;
                }
                else tempStep++;
                totalFlow.ForEach(a => { if (a.stepname == tempFlow.stepname) a.step = tempStep; });
              }
            }
            return totalFlow;
        }

        internal List<ApprovalFlow> SetAuditorFlow(List<ApprovalFlow> totalFlow, CashHead cashHead, List<EFormAudit> auditors, ref decimal step)
        {
            List<ApprovalFlow> tempFlow = new List<ApprovalFlow>();
            bool isAdded = false;
            decimal stepCount = 0;//計算增加幾次Auditor
            foreach(ApprovalFlow flow in totalFlow.OrderBy(a=>a.step)){
              EFormAudit audit = auditors.FirstOrDefault(a => (a.emplid == flow.aassignedemplid)&&
                                                             (a.formcode == "ALL" || a.formcode == cashHead.formcode) && 
                                                             (a.deptid == "ALL"|| a.deptid == cashHead.deptid) &&
                                                             (a.company == cashHead.company));
              if(audit != null){
                ApprovalFlow newFlow = new ApprovalFlow();
                newFlow.rno = cashHead.rno;  //報銷單號
                newFlow.step =  flow.step+stepCount;  //簽核步驟順序
                newFlow.stepname = "Auditor";  //簽核步驟名稱
                newFlow.aassignedemplid = audit.auditid;   //簽核人工號
                newFlow.assignedapprovername = _EmployeeInfoRepository.QueryByEmplidOrEngName(audit.auditid).Result.FirstOrDefault().name_a;  //簽核人姓名
                newFlow.assigneddeptid = _EmployeeInfoRepository.QueryByEmplidOrEngName(audit.auditid).Result.FirstOrDefault().deptid;  //簽核人部門代碼
                newFlow.status = "P";  //處理中
                newFlow.company = cashHead.company;  //簽核公司別
                tempFlow.Add(newFlow);
                isAdded = true;
                stepCount++;
              }

              if(isAdded){
                flow.step += stepCount;
                isAdded = false;
              }
              tempFlow.Add(flow);
            }
            step+= decimal.ToInt32(stepCount);
            
            return tempFlow;
        }

        internal ApprovalFlow SetManagerFlow(List<BDTreelevel> treeLevel, CashHead cashHead, SignManager manager, decimal step)
        {
            ApprovalFlow newFlow = new ApprovalFlow();
            newFlow.rno = cashHead.rno;  //報銷單號
            newFlow.step = step;         //簽核步驟順序
            newFlow.stepname = treeLevel.Where(l => l.levelnum == int.Parse(manager.signlevel)).Select(l => l.levelname).FirstOrDefault();  //簽核步驟名稱
            newFlow.aassignedemplid = manager.managerid;   //簽核人工號
            newFlow.assignedapprovername = _EmployeeInfoRepository.QueryByEmplidOrEngName(manager.managerid).Result.FirstOrDefault().name_a;  //簽核人姓名
            newFlow.assigneddeptid = manager.deptid;  //簽核人部門代碼
            newFlow.status = "P";  //處理中
            newFlow.company = cashHead.company;  //簽核公司別
            return newFlow;
        }

        //設定財務簽核流程
        async Task<List<ApprovalFlow>> SetFinFlow(List<ApprovalFlow>totalFlow, CashHead cashHead, SignDept maxAmountDept, List<Finreview> finReview, decimal step)
        {
            string deptPlant = (await _EmpOrgRepository.WithDetailsAsync())
                                     .Where(o => o.deptid == maxAmountDept.deptid)
                                     .Select(o => o.plant_id_a)
                                     .FirstOrDefault();

            var plantFilter = string.IsNullOrWhiteSpace(deptPlant)
                              ? new[] { "ALL", "OTHERS" }
                              : new[] { deptPlant };
            
            var finReviewQuery = finReview.Where(w => w.company == cashHead.company 
                                                   && w.category == 0 
                                                   && plantFilter.Contains(w.plant, StringComparer.OrdinalIgnoreCase));

            List<Finreview> fin1 = finReviewQuery.Where(w => !string.IsNullOrWhiteSpace(w.rv1)).ToList();
            List<Finreview> fin2 = finReviewQuery.Where(w => !string.IsNullOrWhiteSpace(w.rv2)).ToList();
            List<EmployeeInfo> empInfo1 = new List<EmployeeInfo>();
            List<EmployeeInfo> empInfo2 = new List<EmployeeInfo>();

            var rv1List = fin1 != null ? fin1.Select(f => f.rv1).Distinct().ToList() : new List<string>();
            empInfo1 = (await _EmployeeInfoRepository.WithDetailsAsync()).Where(e=>rv1List.Contains(e.emplid)).ToList();
            var validRv1 = fin1 != null 
                                 ? fin1.Select(f => f.rv1)
                                       .Where(id => empInfo1.Any(e => e.emplid == id))
                                       .Distinct()
                                       .ToList()
                                 : new List<string>();
            string allRv1 = string.Join(";", validRv1);

            var rv2List = fin2 != null ? fin2.Select(f => f.rv2).Distinct().ToList() : new List<string>();
            empInfo2 = (await _EmployeeInfoRepository.WithDetailsAsync()).Where(e=>rv2List.Contains(e.emplid)).ToList();
            var validRv2 = fin2 != null 
                                 ? fin2.Select(f => f.rv2)
                                       .Where(id => empInfo2.Any(e => e.emplid == id))
                                       .Distinct()
                                       .ToList()
                                 : new List<string>();
            string allRv2 = string.Join(";", validRv2);
            if(allRv1 == "")
              return null;

            if(allRv1 != ""){
              ApprovalFlow newFlow = new ApprovalFlow();
              var empFilter = allRv1.Split(';', StringSplitOptions.RemoveEmptyEntries)
                      .Select(id => id.Trim())
                      .ToList();
              var rv1Names = validRv1.Select(id=>empInfo1.FirstOrDefault(e => e.emplid == id)).Where(e => e != null).ToList();
              string allRv1Name = rv1Names != null ? string.Join(";", rv1Names.Select(f => string.IsNullOrWhiteSpace(f.name_a) ? f.name : f.name_a)) : "";
              string allRv1Dept = rv1Names != null ? string.Join(";", rv1Names.Select(f => f.deptid)) : "";

              newFlow.rno = cashHead.rno;  //報銷單號
              newFlow.step = step;         //簽核步驟順序
              newFlow.stepname = "Finance1";  //簽核步驟名稱
              newFlow.aassignedemplid = allRv1;   //簽核人工號
              newFlow.assignedapprovername = allRv1Name;  //簽核人姓名
              newFlow.assigneddeptid = allRv1Dept;  //簽核人部門代碼
              newFlow.status = "P";  //處理中
              newFlow.company = cashHead.company;  //簽核公司別
              totalFlow.Add(newFlow);
            }
            if(allRv1 != "" && allRv2 != ""){  //要有會計一才能有會計二
              ApprovalFlow newFlow = new ApprovalFlow();              
              var empFilter = allRv2.Split(';', StringSplitOptions.RemoveEmptyEntries)
                      .Select(id => id.Trim())
                      .ToList();
              var rv2Names = validRv2.Select(id=>empInfo2.FirstOrDefault(e => e.emplid == id)).Where(e => e != null).ToList();
              string allRv2Name = rv2Names != null ? string.Join(";", rv2Names.Select(f => string.IsNullOrWhiteSpace(f.name_a) ? f.name : f.name_a)) : "";
              string allRv2Dept = rv2Names != null ? string.Join(";", rv2Names.Select(f => f.deptid)) : "";
              newFlow.rno = cashHead.rno;  //報銷單號
              newFlow.step = step+1;         //簽核步驟順序
              newFlow.stepname = "Finance2";  //簽核步驟名稱
              newFlow.aassignedemplid = allRv2;   //簽核人工號
              newFlow.assignedapprovername = allRv2Name;  //簽核人姓名
              newFlow.assigneddeptid = allRv2Dept;  //簽核人部門代碼
              newFlow.status = "P";  //處理中
              newFlow.company = cashHead.company;  //簽核公司別
              totalFlow.Add(newFlow);
            }

            return totalFlow;
        }
       
        internal List<SignDept> ProcessSignDept(CashHead head, List<CashDetail> details, List<BDSenarioDto> senario, List<BDSignlevel> signLevels)
        {
            // 1. 根據 deptid 分群並計算總金額以及統計expcode
            List<SignDept> signDept = details
              .GroupBy(d => d.deptid)
              .Select(g => new SignDept{
                   deptid = g.Key,
                   amount = g.Sum(d => d.formcode == "CASH_2" && d.flag != 0 ? d.amount ?? 0 : d.baseamt ?? 0),
                   senarioLevel = g.Select(x => x.senarioid)
                                   .Where(id => id.HasValue) // 過濾掉 null 的 senarioid
                                   .Select(id => id.Value)   // 將 Guid? 轉為 Guid
                                   .Distinct()
                                   .Join(senario, id => id, s => s.Id, (id, s) => s.auditlevelcode) // 假設 auditlevelcode 是 Guid 格式
                                   .ToArray()
              }).ToList();

            // 2. 根據expcode，amount和signLevels計算簽核項目和簽核等級
            foreach (SignDept dept in signDept)
            {
              string bestSignItem = null;
              List<string> collectedSignLevels = new List<string>();
              decimal minSignLevel= -10;

              foreach (var level in dept.senarioLevel){
                // 過濾 SignLevel
                List<BDSignlevel> applicableLevels = signLevels
                    .Where(sl => sl.item == level && sl.money < dept.amount && sl.company == head.company)
                    .OrderByDescending(sl => int.Parse(sl.signlevel))  // 按照 signlevel 升序排列
                    .ToList();

                if (applicableLevels.Any()){
                  if (bestSignItem == null){
                    collectedSignLevels.AddRange(applicableLevels.Select(sl => sl.signlevel));
                    minSignLevel = applicableLevels.Min(sl => int.Parse(sl.signlevel));
                    bestSignItem = level;
                  }
                  else{
                    if (applicableLevels.Min(sl => int.Parse(sl.signlevel)) < minSignLevel){
                      collectedSignLevels.Clear();
                      collectedSignLevels.AddRange(applicableLevels.Select(sl => sl.signlevel));
                      minSignLevel = applicableLevels.Min(sl => int.Parse(sl.signlevel));
                      bestSignItem = level;
                    }
                  }
                }
              }

              dept.signitem = bestSignItem ?? string.Empty;
              dept.signlevel = collectedSignLevels.Min();
            }

            return signDept;
        }

        async internal Task<List<SignManager>> ProcessSignManager(SignDept signDept){
            List<SignManager> signManager = new List<SignManager>();            
            EmpOrg org = (await _EmpOrgRepository.WithDetailsAsync()).FirstOrDefault(e => e.deptid == signDept.deptid);            

            while(org != null){
              int orgTreeLevelNum = org.tree_level_num;
              int signLevel = int.Parse(signDept.signlevel);
              if (orgTreeLevelNum <= signLevel){
                signManager.Add(new SignManager { deptid = org.deptid, signlevel = org.tree_level_num.ToString(), managerid = org.manager_id });
                break;
              }
              else{
                signManager.Add(new SignManager { deptid = org.deptid, signlevel = org.tree_level_num.ToString(), managerid = org.manager_id });
                org = (await _EmpOrgRepository.WithDetailsAsync()).FirstOrDefault(e => e.deptid == org.uporg_code_a);
              }

            }
            return signManager;
        }

        internal async Task<Guid> UpdateNextFlowId(Guid flowId, Guid nextFlowId, int step){
          if(step==5){
            step=5;
          }
          if(step == 0)  {
            flowId = nextFlowId;
          }
          else{
            if(flowId == nextFlowId) nextFlowId = Guid.Empty;
            await _ApprovalFlowRepository.UpdateNextFlowIdById(flowId, nextFlowId);
            flowId = nextFlowId;
          }
          return flowId;
        }

        async Task<bool> CheckIsFlowOnGoing(string rno)
        {
           List<ApprovalFlow> hasFLow = (await _ApprovalFlowRepository.GetApprovalFlowByRno(rno)).Where(x => x.status == "P" || x.status == "N").ToList();
           if (hasFLow.Any()) return true;
           return false;
        }

        // 查詢最大 step
        async Task<decimal> GetMaxStep(string rno)
        {
           List<ApprovalFlow> hasFLow = await _ApprovalFlowRepository.GetApprovalFlowByRno(rno);
           decimal maxStep = hasFLow.Any() ? hasFLow.Max(x => x.step) : 0;
           return maxStep;
        }

        //查詢最大無效step
        async Task<decimal> GetMaxStepWithX(string rno)
        {
           List<ApprovalFlow> hasFLow = (await _ApprovalFlowRepository.GetApprovalFlowByRno(rno)).Where(x => x.status == "X").ToList();
           decimal maxStep = hasFLow.Any() ? hasFLow.Max(x => x.step) : 0;
           return maxStep;
        }

        internal class SignDept{
            public string deptid { get; set; }
            public string signitem { get; set; }
            public string signlevel { get; set; }
            public decimal amount { get; set; }=0;
            //public string[] expcode { get; set; }
            public string[] senarioLevel { get; set; }
        }

        internal class SignManager{
            public string deptid { get; set; }
            public string signlevel { get; set; }
            public string managerid { get; set; }

        }

    }
}