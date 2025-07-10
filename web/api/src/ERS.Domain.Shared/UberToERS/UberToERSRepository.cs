using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using ERS.OracleToPostgreSQL;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Functions;

namespace ERS.UberToERS
{
    public class UberToERSRepository : IUberToERSRepository, IScopedDependency
    {
        private IConfiguration _Configuration;

        public UberToERSRepository(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }

        //获取昨天数据生成签核
        public void TransactionToSign()
        {
            string rno = "";
            string tripId = "";
            try
            {
                Console.WriteLine("Uber_TransactionToSign----------------start----------------" + System.DateTime.Now);
                DataTable dataTable = new DataTable();
                string sql = "";
                string erswwdb = _Configuration.GetSection("PS:PSDB").Value.Trim();
                NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
                //获取未生成申请单的行程单(取消的行程单？)
                sql = "SELECT * FROM uber_transactional_day WHERE rno IS NULL AND sign_status='N' AND  isdeleted = false";
                dataTable = ersww.GetDataTable(sql);
                Console.WriteLine("Uber_TransactionToSign----------------待生成申请单行数： " + dataTable.Rows.Count);
                if (dataTable.Rows.Count > 0)
                {
                    string erswihdb = _Configuration.GetSection("PS:ERSWIHDB").Value.Trim();
                    OracleHelp erswih = new OracleHelp(erswihdb);
                    foreach (DataRow item in dataTable.Rows)
                    {
                        tripId = item["trip_id"].ToString();
                        sql = "select * from EMPLOYEE where EMPLID=:emplid";
                        var conditionEmpl = new { emplid = item["employee_id"] };
                        DataTable employee = erswih.GetDataTable(sql, conditionEmpl);
                        if (employee.Rows.Count == 0)
                        {
                            continue;
                        }
                        rno = "U" + System.DateTime.Now.ToString("yyyyMMdd") + Getseq();
                        decimal amount = Convert.ToDecimal(item["transaction_amount_in_home_currency"]);
                        //放的预约日期
                        // 先格式化日期和时间部分
                        string formattedDate = DateTimeHelper.ConvertToDateFormat(item["request_date"].ToString()); // 应该返回 yyyy-MM-dd
                        string formattedTime = DateTimeHelper.ConvertTo24HourFormat(item["request_time"].ToString()); // 应该返回 HH:mm:ss

                        // 转换为 DateTime 类型
                        DateTime startdate = DateTimeHelper.ConvertToDateTime(formattedDate, formattedTime);

                        Console.WriteLine("Converted DateTime: " + startdate); // 输出: 2023-10-10 14:30:00
                        string startdatestr = formattedDate + " " + formattedTime;
                        List<string> insertSqls = new List<string>();
                        List<object> insertParams = new List<object>();
                        List<string> procedures = new List<string>();
                        List<object> proceduresCondition = new List<object>();
                        string program = item["program"].ToString();
                        string company = GetCompanyByUserId(item["employee_id"].ToString().Trim());
                        sql = "INSERT INTO CASH_FORM_UBER_HEAD(EMPLID,RNO,FORM_CODE,COMAPNY,NAME,PROJECTCODE,CUSER,CDATE,MUSER,MDATE,PROGRAM) " +
                          "VALUES(:emplid,:rno,:form_code,:company,:name,:projectcode,:cuser,:cdate,:muser,:mdate,:program)";
                        string projCode = string.Empty;
                        var condition_insert = new
                        {
                            emplid = item["employee_id"],
                            rno = rno,
                            form_code = "CASH_UBER",
                            company = company,
                            name = employee.Rows[0]["CNAME"],
                            projectcode = projCode,
                            cuser = item["employee_id"],
                            cdate = System.DateTime.Now,
                            muser = item["employee_id"],
                            mdate = System.DateTime.Now,
                            program= program
                        };
                        insertSqls.Add(sql);
                        insertParams.Add(condition_insert);
                        sql = @"INSERT INTO CASH.CASH_FORM_UBER_DETAIL
(FORM_CODE, RNO, ITEM, STARTDATE, DESTINATION, ORIGIN, STATUS, AMOUNT, CUSER, CDATE, MUSER, MDATE, REASON, EXPCODE, EMPLID, NAME, DEPTID)
VALUES(:form_code,:rno,:item,:startdate,:destination,:origin, :status,:amount,:cuser, :cdate,:muser, :mdate,:reason,'',:emplid,:name,:deptid)";
                        var condition = new
                        {
                            form_code = "CASH_UBER",
                            rno = rno,
                            item = tripId,
                            startdate = startdate,
                            destination = item["city"].ToString() + item["drop_off_address"],
                            origin = item["city"].ToString() + item["pickup_address"],
                            status = "P",
                            amount = amount,
                            cuser = item["employee_id"],
                            cdate = System.DateTime.Now,
                            muser = item["employee_id"],
                            mdate = System.DateTime.Now,
                            reason = item["expense_memo"],
                            emplid = item["employee_id"],
                            name = employee.Rows[0]["CNAME"],
                            deptid = item["expense_code"].ToString()
                        };
                        insertSqls.Add(sql);
                        insertParams.Add(condition);

                        sql = "INSERT INTO CASH.E_FORM_HEAD(FORM_CODE, RNO, CUSER, CNAME, CDATE, CEMPLID, STEP, APID, STATUS, NEMPLID, ADDUSER)" +
                           "VALUES(:form_code, :rno, :cuser, :cname,:cdate, :cemplid, :step, :apid, :status, :nemplid, :adduser)";
                        var condition_insert1 = new
                        {
                            form_code = "CASH_UBER",
                            rno = rno,
                            cuser = item["employee_id"],
                            cname = "",
                            cdate = System.DateTime.Now,
                            cemplid = "",
                            step = 1,
                            apid = "RQ905",
                            status = 'P',
                            nemplid = "",
                            adduser = ""
                        };
                        insertSqls.Add(sql);
                        insertParams.Add(condition_insert1);
                        //签核
                        string signUserId = "";
                        string stepname = "";
                        int count = 1;
                        if (program== "交通車滿載")//签总务 10704054 Ricky LS Chen
                        {
                            stepname = "總務";
                            signUserId = "10704054";
                            count = 10; //总务签核
                        }
                        else
                        {
                            stepname = "申請人";
                            signUserId = item["employee_id"].ToString().Trim();
                        }
                        sql = "select * from EMPLOYEE WHERE EMPLID=:emplid";
                        var condition5 = new { emplid = signUserId };
                        DataTable employeeTable = erswih.GetDataTable(sql, condition5);
                        sql = "select * from  emp_org WHERE DEPTID=:deptid";
                        var conditiondept = new { deptid = employeeTable.Rows[0]["DEPTID"] };
                        DataTable deptTable = erswih.GetDataTable(sql, conditiondept);
                        sql = "INSERT INTO CASH.E_FORM_ALIST(FORM_CODE, RNO, STEP, CEMPLID, STATUS, DEPTID, DEPTN, STEPNAME, UUSER, UDATE, SEQ)" +
                               "VALUES(:form_code, :rno, :step, :cemplid, :status, :deptid, :deptn, :stepname, :uuser, :udate, :seq)";
                        var condition_insert2 = new
                        {
                            form_code = "CASH_UBER",
                            rno = rno,
                            step = count,
                            cemplid = signUserId,
                            status = "C",
                            deptid = employeeTable.Rows[0]["DEPTID"],
                            deptn = deptTable.Rows[0]["DESCR"],
                            stepname = stepname,
                            uuser = item["employee_id"],
                            udate = System.DateTime.Now,
                            seq = count
                        };
                        insertSqls.Add(sql);
                        insertParams.Add(condition_insert2);
                        sql = "SELECT * FROM MAILTEMPLATE m where m.MAILTYPE=:Mailtype";
                        var conditionMail = new { Mailtype = "7" };
                        DataTable resMail = erswih.GetDataTable(sql, conditionMail);
                        string mailMsg = resMail.Rows[0]["MAILMSG"].ToString();

                        sql = "select * from EMPLOYEE where EMPLID=:emplid";
                        var conditionEmp = new { emplid = signUserId };
                        var resEmp = erswih.GetDataTable(sql, conditionEmp);
                        mailMsg = mailMsg.Replace("{0}", resEmp.Rows[0]["ENAME"].ToString());
                        mailMsg = mailMsg.Replace("{1}", rno);
                        mailMsg = mailMsg.Replace("{2}", employee.Rows[0]["CNAME"].ToString());
                        sql = "SELECT * FROM APPCONFIG a where a.KEY=:key";
                        var conditionApp = new { key = "Mail_WebURL" };
                        var resApp = erswih.GetDataTable(sql, conditionApp);
                        mailMsg = mailMsg.Replace("{3}", resApp.Rows[0]["VALUE"].ToString());
                        var conditionAppWisign = new { key = "WiSign_WebURL" };
                        var wisign = erswih.GetDataTable(sql, conditionAppWisign);
                        mailMsg = mailMsg.Replace("{4}", wisign.Rows[0]["VALUE"].ToString());
                        mailMsg = mailMsg.Replace("{5}", System.DateTime.Now.ToString("yyyy-MM-dd"));
                        string mailAddress = string.Empty;
                        sql = "SELECT VALUE FROM APPCONFIG a WHERE KEY='REPLACE_MAIL'";
                        var resMailAddres = erswih.GetDataTable(sql);
                        if (!string.IsNullOrEmpty(resMailAddres.Rows[0]["VALUE"].ToString()))
                            mailAddress = resMailAddres.Rows[0]["VALUE"].ToString();
                        else
                            mailAddress = resEmp.Rows[0]["MAIL"].ToString();
                        procedures.Add("P_SEND_EMAIL");
                        string subject = resMail.Rows[0]["SUBJECT"].ToString().Replace("{0}", rno);
                        var procedure_pram = new
                        {
                            MAIL_TO = mailAddress,
                            MAIL_CC = "",
                            MAIL_SUB = subject,
                            MAIL_TXT = mailMsg
                        };
                        proceduresCondition.Add(procedure_pram);
                        if (program != "交通車滿載")
                        {
                            sql = "SELECT TREE_LEVEL_NUM FROM DOA d WHERE DTYPE ='A1' AND COMPANY=:company AND :maxAmount<=eamt AND :maxAmount>=samt and Effdate=" +
                         "( SELECT max(effdate) FROM DOA d WHERE DTYPE ='A1' AND COMPANY=:company AND :maxAmount<=eamt AND :maxAmount>=samt)";
                            var condition_doa = new { company = company, maxAmount = amount };
                            DataTable res = erswih.GetDataTable(sql, condition_doa);

                            sql = "SELECT o.DESCR_A ,x.DEPTID,x.DESCR,x.MANAGER_ID,o.TREE_LEVEL_NUM FROM EMP_ORG_LV o,  (SELECT * FROM EMP_ORG START WITH deptid=:dept CONNECT BY PRIOR  UPORG_CODE_A =deptid) x WHERE o.TREE_LEVEL_NUM = x.TREE_LEVEL_NUM";
                            var condition1 = new { dept = item["expense_code"] };
                            DataTable signTable = erswih.GetDataTable(sql, condition1);
                            foreach (DataRow n in signTable.Rows)
                            {
                                count++;
                                string status = "C";
                                if (count > 1)
                                {
                                    status = "U";
                                }
                                sql = "INSERT INTO CASH.E_FORM_ALIST(FORM_CODE, RNO, STEP, CEMPLID, STATUS, DEPTID, DEPTN, STEPNAME, UUSER, UDATE, SEQ)" +
                                    "VALUES(:form_code, :rno, :step, :cemplid, :status, :deptid, :deptn, :stepname, :uuser, :udate, :seq)";
                                var condition_insert3 = new
                                {
                                    form_code = "CASH_UBER",
                                    rno = rno,
                                    step = count,
                                    cemplid = n["MANAGER_ID"],
                                    status = status,
                                    deptid = n["DEPTID"],
                                    deptn = n["DESCR"],
                                    stepname = n["DESCR_A"],
                                    uuser = item["employee_id"],
                                    udate = System.DateTime.Now,
                                    seq = count
                                };
                                insertSqls.Add(sql);
                                insertParams.Add(condition_insert3);
                                if (Convert.ToInt32(n["TREE_LEVEL_NUM"]) <= Convert.ToInt32(res.Rows[0]["TREE_LEVEL_NUM"]))
                                {
                                    break;
                                }
                            }
                        }
                        sql = "INSERT INTO CASH.E_FORM_AUSER(FORM_CODE,RNO,CEMPLID,SEQ,USED)VALUES(:form_code, :rno, :cemplid, :seq, :used)";
                        var condition_insert4 = new
                        {
                            form_code = "CASH_UBER",
                            rno = rno,
                            cemplid = signUserId,//不能赋值为"",数据库此字段非空
                            seq = 1,
                            used = 'N'
                        };
                        insertSqls.Add(sql);
                        insertParams.Add(condition_insert4);
                        //插入表单数据，签核数据，发送待签核邮件
                        erswih.InsertTable(insertSqls, insertParams, procedures, proceduresCondition);
                        //更新单号和状态到行程单
                        UpdateUberTransactionStatus(rno, "P", item["Id"]);
                    }
                }
                Console.WriteLine("Uber_TransactionToSign----------------end----------------" + System.DateTime.Now);
                SyncSignStatus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Uber_TransactionToSign 异常: {ex}，单号: {rno}，Id: {tripId}" + System.DateTime.Now);
            }
        }
        //更新行程单状态
        private void UpdateUberTransactionStatus(string rno,string SignStatus,object Id)
        {
            string erswwdb = _Configuration.GetSection("PS:PSDB").Value.Trim();
            NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
            string sql = "UPDATE uber_transactional_day SET  rno=:rno, sign_status=:SignStatus, mdate=:mdate  WHERE \"Id\"=:Id";
            var condition= new { rno = rno, SignStatus=SignStatus,mdate= System.DateTime.Now, Id= Id };
            ersww.ExecuteNonQuery(sql, condition);
        }

        /// <summary>
        /// 根据用户ID获取公司信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetCompanyByUserId(string userId)
        {
            string company = string.Empty;
            string erswwdb = _Configuration.GetSection("PS:PSDB").Value.Trim();
            NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
            string sql = "select *  from employeeinfo where emplid=:emplid";
            var condition = new { emplid = userId };
            DataTable dataTable = ersww.GetDataTable(sql, condition);
            if (dataTable.Rows.Count > 0)
            {
                if (dataTable.Rows[0]["company"].ToString() == "M00")
                {
                    if (dataTable.Rows[0]["site"].ToString() == "WNH" || dataTable.Rows[0]["site"].ToString() == "WHC")
                    {
                        company = "M00-HQ"; // M00 公司总部
                    }
                    else if (dataTable.Rows[0]["site"].ToString() == "WIH")
                    {
                        company = "WIH"; // M00 分公司
                    }
                }
                else if (dataTable.Rows[0]["company"].ToString() == "696" && dataTable.Rows[0]["site"].ToString() == "WMT")
                {
                    company = "WMT-HQ";
                }
            }
            return company;
        }
        private  string Getseq()
        {
            string erswihdb = _Configuration.GetSection("PS:ERSWIHDB").Value.Trim();
            OracleHelp erswih = new OracleHelp(erswihdb);
            string datesNow = System.DateTime.Now.ToString("yyyyMMdd");
            string sql = "select NO from E_AUTONO t where t.DATES =:dates and t.FORM_CODE =:form_code ";
            var condition = new { dates = datesNow, form_code = "CASH_UBER" };
            DataTable dataTable =  erswih.GetDataTable(sql, condition);

            if (dataTable.Rows.Count >0)
            {
                sql = "UPDATE E_AUTONO SET NO=:no WHERE FORM_CODE=:form_code AND DATES=:dates";
                var condition_update = new { no = (Convert.ToInt32(dataTable.Rows[0]["NO"]) + 1).ToString().PadLeft(4, '0'), form_code = "CASH_UBER", dates = datesNow };
                erswih.ExecuteNonQuery(sql, condition_update);
            }
            else
            {
                sql = "INSERT INTO E_AUTONO(DATES,NO,UDATE,FORM_CODE)VALUES(:dates,:seq,sysdate, :form_code)";
                var condition_insert = new { dates = datesNow, seq = "0001", form_code = "CASH_UBER" };
                erswih.ExecuteNonQuery(sql, condition_insert);
            }
            sql = "select NO from E_AUTONO t where t.DATES =:dates and t.FORM_CODE =:form_code";
            condition = new { dates = datesNow, form_code = "CASH_UBER" };
            dataTable =  erswih.GetDataTable(sql, condition);
            return dataTable.Rows[0]["NO"].ToString();
        }
        public void test()
        {
            string seq= Getseq();
            Console.WriteLine("seq数: " + seq);
        }
        ///同步签核状态
        public void SyncSignStatus()
        {
            Console.WriteLine("SyncSignStatus----------------start----------------" + System.DateTime.Now);
            DataTable dataTable = new DataTable();
            string sql = "";
            string erswwdb = _Configuration.GetSection("PS:PSDB").Value.Trim();
            NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
            //获取签核中行程单
            sql = "SELECT * FROM uber_transactional_day WHERE rno is not NULL AND isdeleted = false and sign_status ='P'";
            dataTable = ersww.GetDataTable(sql);
            if (dataTable.Rows.Count > 0)
            {
                string erswihdb = _Configuration.GetSection("PS:ERSWIHDB").Value.Trim();
                OracleHelp erswih = new OracleHelp(erswihdb);
                foreach (DataRow item in dataTable.Rows)
                {
                    //获取行程单签核状态
                    sql = "SELECT * FROM CASH.E_FORM_HEAD WHERE FORM_CODE='CASH_UBER' AND RNO=:rno";
                    var condition = new { rno = item["rno"] };
                    DataTable res = erswih.GetDataTable(sql, condition);
                    if (res.Rows.Count > 0)
                    {
                        string signStatus = res.Rows[0]["STATUS"].ToString();
                        if (signStatus == "A")
                        {
                            UpdateUberTransactionStatus(item["rno"].ToString(), signStatus, item["Id"]);
                        }
                        else if (signStatus == "R")
                        {
                            UpdateUberTransactionStatus(item["rno"].ToString(), signStatus, item["Id"]);
                        }
                    }
                }
            }
            Console.WriteLine("SyncSignStatus----------------end----------------" + System.DateTime.Now);
        }

        //取得所有員工資料
        public IList<Model.uber_employees> GetAllEmployees()
        {
            string erswwdb = _Configuration.GetSection("PS:ERSPGDB").Value.Trim();
            string[] employeeSiteList = _Configuration.GetSection("hangfire:SFTP:employeeSiteList").Get<string[]>();
            string wheresql = "";
            if (employeeSiteList != null && employeeSiteList.Length > 0)
            {
                wheresql = "AND site = ANY(ARRAY[" + string.Join(",", employeeSiteList.Select(x => $"'{x}'")) + @"])";
            }

            NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
            string strSQL = string.Format($@"SELECT 	emplid
            , LEFT(name_a, length(name_a) - POSITION(' ' IN reverse(name_a))) AS first_name
            , RIGHT(name_a, POSITION(' ' IN reverse(name_a)) - 1) AS last_name
            , email_address_a
            , site
            , termination_dt
            FROM sub_employeeinfo
            WHERE email_address_a IS NOT NULL
            {wheresql}
            ORDER BY email_address_a ASC");
            DataTable dt = ersww.GetDataTable(strSQL ,false);
            Console.WriteLine("GetAllEmployees DTCount: " + dt.Rows.Count.ToString());

            //轉換成 List<Model.uber_employees>
            IList<Model.uber_employees> employees = dt.AsEnumerable()
                .Select(row => new Model.uber_employees
                {
                    employee_id = row.Field<string>("emplid"),
                    first_name = row.Field<string>("first_name"),
                    last_name = row.Field<string>("last_name"),
                    email = row.Field<string>("email_address_a"),
                    group = row.Field<string>("site"),
                    termination_dt = row.Field<DateTime?>("termination_dt")
                })
                .ToList();
            //
            string InsertSql = @"INSERT INTO uber_employees (
                log_datetime
                , first_name
                , last_name
                , email
                , employee_id
                , ""group""
            ) VALUES(
            current_timestamp
            , @first_name
            , @last_name
            , @email_address_a
            , @emplid
            , @site
            );";
            int ScuessCount = 0;
            foreach (var emp in employees)
            {
                var parameters = new List<Npgsql.NpgsqlParameter>
                {
                    new Npgsql.NpgsqlParameter("@emplid", emp.employee_id ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@first_name", emp.first_name ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@last_name", emp.last_name ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@email_address_a", emp.email ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@site", emp.group ?? (object)DBNull.Value)
                    //new Npgsql.NpgsqlParameter("@termination_dt", emp.termination_dt ?? (object)DBNull.Value)
                };
                ersww.InsertUpdateSql(InsertSql, parameters, true);
                ScuessCount += 1;
            }
            Console.WriteLine("Insert uber_employees Scuess Count: " + ScuessCount.ToString());
            ersww.connClose();
            return employees;
        }
        //將在職或離職員工轉換成 CSV 格式
        public string GetEmployeesToCsv(IList<Model.uber_employees> uber_Employees, bool isReMove = false)
        {
            var sb = new System.Text.StringBuilder();
            List<Model.uber_employees> filters = isReMove
                ? uber_Employees.Where(x => x.termination_dt != null).ToList()      // 離職員工
                : uber_Employees.Where(x => x.termination_dt == null).ToList();     // 在職員工
            //判斷是否有資料
            if (filters.Count == 0) { return string.Empty; }
            else
            {
                foreach (Model.uber_employees item in filters)
                {
                    sb.AppendLine($"{item.first_name},{item.last_name},{item.email},{item.employee_id},{item.group}");
                }
                return sb.ToString();
            }

        }
        //取得部門資料轉換成 CSV 格式
        public string GetDeptIdToCsv()
        {
            var sb = new System.Text.StringBuilder();
            string erswwdb = _Configuration.GetSection("PS:ERSPGDB").Value.Trim();
            NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);
            string strSQL = "SELECT DISTINCT deptid , descr FROM sub_organization ORDER BY deptid ASC";
            DataTable dt = ersww.GetDataTable(strSQL);
            Console.WriteLine("GetDeptIdToCsv DTCount: " + dt.Rows.Count.ToString());

            if (dt.Rows.Count == 0) { return string.Empty; }
            else
            {
                int ScuessCount = 0;
                string InsertSql = @"INSERT INTO uber_expenseinfo (
                    log_datetime
                    , expense_code
                    , description
                    , employee_emails
                ) VALUES(
                current_timestamp
                , @expense_code
                , @description
                ,''
                );";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string deptid = dt.Rows[i]["deptid"].ToString();
                    //string descr = dt.Rows[i]["descr"].ToString();
                    string descr = $"\"{dt.Rows[i]["descr"]}\"";

                    var parameters = new List<Npgsql.NpgsqlParameter> {
                    new Npgsql.NpgsqlParameter("@expense_code", deptid ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@description", descr ?? (object)DBNull.Value)};
                    ersww.InsertUpdateSql(InsertSql, parameters, true);
                    sb.AppendLine($"{deptid},{descr}");
                    ScuessCount += 1;
                }
                Console.WriteLine("Insert uber_expenseinfo Scuess Count: " + ScuessCount.ToString());

                ersww.connClose();
                return sb.ToString();
            }

        }

        public void CsvInsDB(string csvFiledName, string csvContent, bool isDailly = true)
        {
            var list = new List<Model.uber_transactional>();
            //取得Job名稱 用來代替寫入者
            string jobName = isDailly ? "DaillyJob" : "MonthyJob";
            string tableName = isDailly ? "uber_transactional_day" : "uber_transactional_month";
            //每行資料(會排除空白行)
            string[] lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //第六行開始讀取
            int startIndex = 5;
            string csvInsFiledNames = string.Empty;
            string csvInsFiledParams = string.Empty;

            var listFieldDic = new List<Dictionary<Model.transactional_Field, string>>();
            // 取得 enum 的所有名稱（順序要和 CSV 欄位一致）
            var enumNames = Enum.GetNames(typeof(Model.transactional_Field));

            //
            for (int idx = startIndex; idx < lines.Length; idx++)
            {
                string line = lines[idx];
                string[] fields = line.Split(',');

                // 建立一個 Dictionary<enum, value>
                var fieldDict = new Dictionary<Model.transactional_Field, string>();
                for (int i = 0; i < fields.Length && i < enumNames.Length; i++)
                {
                    var enumValue = (Model.transactional_Field)Enum.Parse(typeof(Model.transactional_Field), enumNames[i]);
                    // fieldDict[enumValue] 為內容值
                    fieldDict[enumValue] = fields[i];
                    //將保留字前前後加上雙引號
                    csvInsFiledNames += "," + (enumNames[i] == "group" ? "\"group\"" : enumNames[i]);
                    csvInsFiledParams += "," + "@" + enumNames[i];
                }
                listFieldDic.Add(fieldDict);

            }
            int ScuessCount = 0;
            /*lines[0]公司：
              lines[1]管理員：
              lines[2]報表日期：
              lines[3]交易
              lines[4]標題欄
            */
            if (lines.Length > startIndex) //
            {
                string erswwdb = _Configuration.GetSection("PS:ERSPGDB").Value.Trim();
                NpgsqlHelp ersww = new NpgsqlHelp(erswwdb);

                //SQL 範本
                string InsertSql = string.Format($@"INSERT INTO {tableName}( ""Id""
                {csvInsFiledNames}
                ,rno
                ,sign_status
                ,mdate
                ,cdate
                ,cuser
                ,muser
                ,isdeleted
                ,file_name
                ) VALUES( gen_random_uuid()
                {csvInsFiledParams}
                ,null --rno
                ,'N' --sign_status
                ,CURRENT_TIMESTAMP
                ,CURRENT_TIMESTAMP
                , '{jobName}' --cuser
                , '' --muser
                , false --isdeleted
                , '{csvFiledName}' --file_name
                );");
                Console.WriteLine(InsertSql);

                // Build parameters list from the last processed line
                var parameters = new List<Npgsql.NpgsqlParameter>();
                for (int i = 0; i < listFieldDic.Count; i++)
                {
                    var fieldDict = listFieldDic[i];
                    //取得每個欄位名稱及內容值
                    foreach (var kvp in fieldDict)
                    {
                        switch (kvp.Key)
                        {
                            case Model.transactional_Field.trip_id:
                                parameters.Add(new Npgsql.NpgsqlParameter("@" + kvp.Key.ToString(), Guid.Parse(kvp.Value)));
                                break;
                            case Model.transactional_Field.transaction_timestamp:
                            case Model.transactional_Field.drop_off_date_utc:
                                parameters.Add(new Npgsql.NpgsqlParameter("@" + kvp.Key.ToString(), DateTime.Parse(kvp.Value)));
                                break;
                            case Model.transactional_Field.request_date_utc:
                            case Model.transactional_Field.request_date:
                            case Model.transactional_Field.drop_off_date:
                                parameters.Add(new Npgsql.NpgsqlParameter("@" + kvp.Key.ToString(), DateOnly.Parse(kvp.Value)));
                                break;
                            case Model.transactional_Field.distance:
                            case Model.transactional_Field.duration:
                            case Model.transactional_Field.fare_in_local_currency:
                            case Model.transactional_Field.taxes_in_local_currency:
                            case Model.transactional_Field.tip_in_local_currency:
                            case Model.transactional_Field.transaction_amount_in_local_currency:
                            case Model.transactional_Field.fare_in_home_currency:
                            case Model.transactional_Field.taxes_in_home_currency:
                            case Model.transactional_Field.tip_in_home_currency:
                            case Model.transactional_Field.transaction_amount_in_home_currency:
                            case Model.transactional_Field.estimated_service_and_technology_fee:
                                parameters.Add(new Npgsql.NpgsqlParameter("@" + kvp.Key.ToString(), decimal.Parse(kvp.Value)));
                                break;
                            default:
                                parameters.Add(new Npgsql.NpgsqlParameter("@" + kvp.Key.ToString(), kvp.Value ?? (object)DBNull.Value));
                                break;
                        }

                    }
                    //寫入資料庫
                    ersww.InsertUpdateSql(InsertSql, parameters, true);
                    ScuessCount += 1;
                    parameters.Clear();
                }


                Console.WriteLine("Insert uber_employees Scuess Count: " + ScuessCount.ToString());
                ersww.connClose();
            }

        }
    }
}