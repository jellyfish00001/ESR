using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ERS.Common;
using ERS.DTO.DataSync;
using ERS.Entities;
using ERS.IDomainServices;
using ERS.IRepositories;
using ERS.OracleToPostgreSQL;
using Microsoft.Extensions.Configuration;
namespace ERS.DomainServices
{
    /// <summary>
    /// 旧数据同步
    /// </summary>
    public class CashDataSyncDomainService : CommonDomainService, ICashDataSyncDomainService
    {
        private IEFormHeadRepository _EFormHeadRepository;
        private ICashHeadRepository _CashHeadRepository;
        private ICashAmountRepository _CashAmountRepository;
        private ICashDetailRepository _CashDetailRepository;
        private IConfiguration _Configuration;
        public CashDataSyncDomainService(
            IEFormHeadRepository EFormHeadRepository,
            ICashHeadRepository CashHeadRepository,
            ICashAmountRepository CashAmountRepository,
            ICashDetailRepository CashDetailRepository,
            IConfiguration Configuration
        )
        {
            _EFormHeadRepository = EFormHeadRepository;
            _CashHeadRepository = CashHeadRepository;
            _CashAmountRepository = CashAmountRepository;
            _CashDetailRepository = CashDetailRepository;
            _Configuration = Configuration;
        }
        public async Task testSync(List<string> rnos)
        {
            rnos = rnos.Distinct().ToList();
            List<EFormHead> eFormHeads = await GetEFormheadData(rnos);
            List<CashHead> cashHeads = await GetCashHeadData(rnos);
            List<CashAmount> cashAmounts = await GetCashAmountData(rnos);
            List<CashDetail> cashDetails = await GetCashDetailData(rnos);
            foreach (var efhead in eFormHeads)
            {
                foreach (var cashhead in cashHeads)
                {
                    foreach (var cashamount in cashAmounts)
                    {
                        if (efhead.rno == cashhead.rno && cashamount.rno == cashhead.rno)
                        {
                            efhead.company = cashhead.company;
                            cashamount.company = cashhead.company;
                        }
                    }
                }
            }
            if (eFormHeads.Count > 0 && cashHeads.Count > 0 && cashAmounts.Count > 0 && cashDetails.Count > 0)
            {
                await _EFormHeadRepository.InsertManyAsync(eFormHeads);
                await _CashHeadRepository.InsertManyAsync(cashHeads);
                await _CashAmountRepository.InsertManyAsync(cashAmounts);
                await _CashDetailRepository.InsertManyAsync(cashDetails);
            }
        }
        /// <summary>
        /// 获取eformhead数据
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<List<EFormHead>> GetEFormheadData(List<string> rno)
        {
            List<OraEFormHeadDto> oraList = new();
            List<EFormHead> pgList = new();
            DataTable oraDt = new();
            string oraConn = _Configuration.GetSection("CashDataSync:ERSOracleConn").Value.Trim();
            string oraSql = _Configuration.GetSection("CashDataSync:ERSOracleSql:EFormHead").Value.Trim();
            string rnos = "";
            List<string> existList = (await _EFormHeadRepository.WithDetailsAsync()).Where(w => rno.Contains(w.rno)).Select(s => s.rno).ToList();
            rno = rno.Except(existList).ToList();
            if (rno.Count > 0)
            {
                foreach (var item in rno)
                {
                    rnos += ",'" + item.ToString() + "'";
                }
                rnos = rnos.Substring(1);
                oraSql = String.Format(oraSql, rnos);
                OracleHelp myGet = new OracleHelp(oraConn);
                oraDt = myGet.GetDataTable(oraSql);
                oraList = DTConvertHelper<OraEFormHeadDto>.ConvertToList(oraDt);
                foreach (var item in oraList)
                {
                    EFormHead eFormHead = new();
                    eFormHead.formcode = item.FORM_CODE;
                    eFormHead.rno = item.RNO;
                    eFormHead.cuser = item.CUSER;
                    eFormHead.cname = item.CNAME;
                    eFormHead.cdate = item.CDATE;
                    eFormHead.cemplid = item.CEMPLID;
                    eFormHead.step = item.STEP;
                    eFormHead.apid = item.APID;
                    eFormHead.status = item.STATUS;
                    eFormHead.nemplid = item.NEMPLID;
                    eFormHead.adduser = item.ADDUSER;
                    pgList.Add(eFormHead);
                }
            }
            return pgList;
        }
        /// <summary>
        /// 获取cashhead数据
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<List<CashHead>> GetCashHeadData(List<string> rno)
        {
            List<OraCashHeadDto> oraList = new();
            List<CashHead> pgList = new();
            DataTable oraDt = new();
            string oraConn = _Configuration.GetSection("CashDataSync:ERSOracleConn").Value.Trim();
            string oraSql = _Configuration.GetSection("CashDataSync:ERSOracleSql:CashHead").Value.Trim();
            string rnos = "";
            List<string> existList = (await _CashHeadRepository.WithDetailsAsync()).Where(w => rno.Contains(w.rno)).Select(s => s.rno).ToList();
            rno = rno.Except(existList).ToList();
            if (rno.Count > 0)
            {
                foreach (var item in rno)
                {
                    rnos += ",'" + item.ToString() + "'";
                }
                rnos = rnos.Substring(1);
                oraSql = String.Format(oraSql, rnos);
                OracleHelp myGet = new OracleHelp(oraConn);
                oraDt = myGet.GetDataTable(oraSql);
                oraList = DTConvertHelper<OraCashHeadDto>.ConvertToList(oraDt);
                foreach (var item in oraList)
                {
                    CashHead cashHead = new();
                    cashHead.formcode = item.FORM_CODE;
                    cashHead.rno = item.RNO;
                    cashHead.cuser = item.CUSER;
                    cashHead.cname = item.CNAME;
                    cashHead.deptid = item.DEPTID;
                    cashHead.ext = item.EXT;
                    cashHead.company = item.COMPANY;
                    cashHead.projectcode = item.PROJECT_CODE;
                    cashHead.currency = item.CURRENCY;
                    cashHead.amount = item.AMOUNT;
                    cashHead.payeeId = item.PAYEE_ID;
                    cashHead.payeename = item.PAYEE_NAME;
                    cashHead.payeeaccount = item.PAYEE_ACCOUNT;
                    cashHead.cdate = item.CDATE;
                    cashHead.muser = item.MUSER;
                    cashHead.mdate = item.MDATE;
                    cashHead.welfare = item.WELFARE;
                    cashHead.bank = item.BANK;
                    cashHead.payment = item.PAYMENT;
                    cashHead.dtype = item.DTYPE;
                    cashHead.l2l3seq = item.L2_L3_SEQ;
                    cashHead.actualamount = Convert.ToDecimal(item.ACTUALAMOUNT);
                    cashHead.companysap = item.COMPANY_SAP;
                    cashHead.stat = item.STAT;
                    cashHead.originalcurrency = item.ORIGINALCURRENCY;
                    cashHead.whetherapprove = item.WHETHERAPPROVE;
                    cashHead.approvereason = item.APPROVEREASON;
                    cashHead.expensetype = item.EXPENSETYPE;
                    cashHead.vendor = item.VENDOR;
                    cashHead.paymentdate = item.PAYMENTDATE;
                    cashHead.expensetypedesc = item.EXPENSETYPEDESC;
                    cashHead.vendordesc = item.VENDORDESC;
                    cashHead.iscloudinvoice = item.ISCLOUDINVOICE;
                    pgList.Add(cashHead);
                }
            }
            return pgList;
        }
        /// <summary>
        /// 获取cashamount数据
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<List<CashAmount>> GetCashAmountData(List<string> rno)
        {
            List<OraCashAmountDto> oraList = new();
            List<CashAmount> pgList = new();
            DataTable oraDt = new();
            string oraConn = _Configuration.GetSection("CashDataSync:ERSOracleConn").Value.Trim();
            string oraSql = _Configuration.GetSection("CashDataSync:ERSOracleSql:CashAmount").Value.Trim();
            string rnos = "";
            List<string> existList = (await _CashAmountRepository.WithDetailsAsync()).Where(w => rno.Contains(w.rno)).Select(s => s.rno).ToList();
            rno = rno.Except(existList).ToList();
            if (rno.Count > 0)
            {
                foreach (var item in rno)
                {
                    rnos += ",'" + item.ToString() + "'";
                }
                rnos = rnos.Substring(1);
                oraSql = String.Format(oraSql, rnos);
                OracleHelp myGet = new OracleHelp(oraConn);
                oraDt = myGet.GetDataTable(oraSql);
                oraList = DTConvertHelper<OraCashAmountDto>.ConvertToList(oraDt);
                foreach (var item in oraList)
                {
                    CashAmount cashAmount = new();
                    cashAmount.formcode = item.FORM_CODE;
                    cashAmount.rno = item.RNO;
                    cashAmount.currency = item.CURRENCY;
                    cashAmount.amount = item.AMOUNT;
                    cashAmount.actpay = item.ACTPAY;
                    cashAmount.actamt = item.ACTAMT;
                    cashAmount.cuser = item.CUSER;
                    cashAmount.cdate = item.CDATE;
                    cashAmount.muser = item.MUSER;
                    cashAmount.mdate = item.MDATE;
                    cashAmount.company = "WZS";
                    pgList.Add(cashAmount);
                }
            }
            return pgList;
        }
        /// <summary>
        /// 获取cashdetail数据
        /// </summary>
        /// <param name="rno"></param>
        /// <returns></returns>
        public async Task<List<CashDetail>> GetCashDetailData(List<string> rno)
        {
            List<OraCashDetailDto> oraList = new();
            List<CashDetail> pgList = new();
            DataTable oraDt = new();
            string oraConn = _Configuration.GetSection("CashDataSync:ERSOracleConn").Value.Trim();
            string oraSql = _Configuration.GetSection("CashDataSync:ERSOracleSql:CashDetail").Value.Trim();
            string rnos = "";
            List<string> existList = (await _CashDetailRepository.WithDetailsAsync()).Where(w => rno.Contains(w.rno)).Select(s => s.rno).ToList();
            rno = rno.Except(existList).ToList();
            if (rno.Count > 0)
            {
                foreach (var item in rno)
                {
                    rnos += ",'" + item.ToString() + "'";
                }
                rnos = rnos.Substring(1);
                oraSql = String.Format(oraSql, rnos);
                OracleHelp myGet = new OracleHelp(oraConn);
                oraDt = myGet.GetDataTable(oraSql);
                oraList = DTConvertHelper<OraCashDetailDto>.ConvertToList(oraDt);
                foreach (var item in oraList)
                {
                    CashDetail cashDetail = new();
                    cashDetail.formcode = item.FORM_CODE;
                    cashDetail.rno = item.RNO;
                    cashDetail.seq = Convert.ToInt32(item.ITEM);
                    cashDetail.rdate = item.RDATE;
                    cashDetail.currency = item.CURRENCY;
                    cashDetail.amount1 = item.AMOUNT1;
                    cashDetail.deptid = item.DEPTID;
                    cashDetail.summary = item.SUMMARY;
                    cashDetail.@object = item.OBJECT;
                    cashDetail.keep = item.KEEP;
                    cashDetail.remarks = item.REMARKS;
                    cashDetail.amount2 = item.AMOUNT2;
                    cashDetail.basecurr = item.BASE_CURR;
                    cashDetail.baseamt = item.BASE_AMT;
                    cashDetail.rate = item.RATE;
                    cashDetail.expcode = item.EXPCODE;
                    cashDetail.expname = item.EXPNAME;
                    cashDetail.acctcode = item.ACCTCODE;
                    cashDetail.acctname = item.ACCTNAME;
                    cashDetail.revsdate = item.REVSDATE;
                    cashDetail.paytyp = item.PAYTYP;
                    cashDetail.payname = item.PAYNAME;
                    cashDetail.payeeid = item.PAYEE_ID;
                    cashDetail.payeename = item.PAYEE_NAME;
                    cashDetail.payeeaccount = item.PAYEE_ACCOUNT;
                    cashDetail.bank = item.BANK;
                    cashDetail.stat = item.STAT;
                    cashDetail.payeedeptid = item.PAYEE_DEPTID;
                    cashDetail.advanceamount = item.ADVANCEAMOUNT;
                    cashDetail.advancecurrency = item.ADVANCECURRENCY;
                    cashDetail.advancebaseamount = item.ADVANCEBASEAMOUNT;
                    cashDetail.invoice = item.INVOICE;
                    cashDetail.pretaxamount = item.PRETAXAMOUNT;
                    cashDetail.taxexpense = item.TAXEXPENSE;
                    cashDetail.treataddress = item.TREATADDRESS;
                    cashDetail.otherobject = item.OTHEROBJECT;
                    cashDetail.objectsum = item.OBJECTSUM;
                    cashDetail.keepcategory = item.KEEPCATEGORY;
                    cashDetail.otherkeep = item.OTHERKEEP;
                    cashDetail.keepsum = item.KEEPSUM;
                    cashDetail.isaccordnumber = item.ISACCORDNUMBER;
                    cashDetail.notaccordreason = item.NOTACCORDREASON;
                    cashDetail.actualexpense = item.ACTUALEXPENSE;
                    cashDetail.isaccordcost = item.ISACCORDCOST;
                    cashDetail.overbudget = item.OVERBUDGET;
                    cashDetail.processmethod = item.PROCESSMETHOD;
                    cashDetail.custsuperme = item.CUSTSUPERME;
                    cashDetail.flag = Convert.ToInt32(item.FLAG);
                    cashDetail.unifycode = item.UNIFYCODE;
                    cashDetail.treattime = item.TREATTIME;
                    cashDetail.taxbaseamount = item.TAXBASEAMOUNT;
                    cashDetail.assignment = item.ASSIGNMENT;
                    cashDetail.hospdate = item.HOSPDATE;
                    cashDetail.company = "WZS";
                    cashDetail.cuser = item.CUSER;
                    cashDetail.cdate = item.CDATE;
                    cashDetail.mdate = item.MDATE;
                    cashDetail.muser = item.MUSER;
                    pgList.Add(cashDetail);
                }
            }
            return pgList;
        }
    }
}