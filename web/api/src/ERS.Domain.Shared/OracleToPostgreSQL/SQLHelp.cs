using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System.Collections;

namespace ERS.OracleToPostgreSQL
{
    public class SQLHelp
    {

    }
    public class OracleHelp
    {
        private OracleConnection conn;

        public OracleHelp(string strConn)
        {
            conn = new OracleConnection(strConn);
        }

        public DataTable GetDataTable(string SQL)
        {
            using (var cmd = new OracleCommand(SQL, conn))
            using (var da = new OracleDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    da.Fill(dt);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                return dt;
            }
        }

        public DataTable GetDataTable(string SQL, object parameters = null)
        {
            using (var cmd = new OracleCommand(SQL, conn))
            {
                // 添加参数
                if (parameters != null)
                {
                    foreach (var prop in parameters.GetType().GetProperties())
                    {
                        cmd.Parameters.Add(new OracleParameter(prop.Name, prop.GetValue(parameters) ?? DBNull.Value));
                    }
                }

                using (var da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                    return dt;
                }
            }
        }

        public int ExecuteNonQuery(string sql, object parameters = null)
        {
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null)
                {
                    foreach (var prop in parameters.GetType().GetProperties())
                    {
                        cmd.Parameters.Add(new OracleParameter(prop.Name, prop.GetValue(parameters) ?? DBNull.Value));
                    }
                }
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }
        public void close()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Dispose();
        }
        public void InsertTable(List<string> insertSqls, List<object> insertParams, List<string> procedures, List<object> proceduresCondition)
        {
            if (insertSqls.Count != insertParams.Count || procedures.Count != proceduresCondition.Count)
                throw new ArgumentException("SQL语句和参数列表数量不一致");

            OracleTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // 执行插入操作
                for (int i = 0; i < insertSqls.Count; i++)
                {
                    using (var cmd = new OracleCommand(insertSqls[i], conn))
                    {
                        cmd.Transaction = transaction; // 设置事务

                        // 添加参数
                        if (insertParams[i] != null)
                        {
                            foreach (var prop in insertParams[i].GetType().GetProperties())
                            {
                                string paramName = prop.Name;
                                object paramValue = prop.GetValue(insertParams[i]) ?? DBNull.Value;

                                // 调试输出当前参数信息
                                //Console.WriteLine($"Parameter Name: {paramName}, Value: {paramValue}, Type: {paramValue.GetType()}");

                                // 添加参数
                                cmd.Parameters.Add(new OracleParameter(paramName, paramValue));
                            }
                        }
                        cmd.ExecuteNonQuery();
                    }
                }

                // 执行存储过程
                for (int i = 0; i < procedures.Count; i++)
                {
                    using (var cmd = new OracleCommand(procedures[i], conn))
                    {
                        cmd.Transaction = transaction; // 设置事务
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 添加参数
                        if (proceduresCondition[i] != null)
                        {
                            foreach (var prop in proceduresCondition[i].GetType().GetProperties())
                            {
                                cmd.Parameters.Add(new OracleParameter(prop.Name, prop.GetValue(proceduresCondition[i]) ?? DBNull.Value));
                            }
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                // 提交事务
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // 发生异常时回滚事务
                if (transaction != null)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
    public class NpgsqlHelp
    {
        NpgsqlConnection conn;
        NpgsqlCommand cmd;

        public NpgsqlHelp(string strConn)
        {
            conn = new NpgsqlConnection(strConn);
        }
        public DataTable GetDataTable(string SQL, object parameters = null, bool isDispose = true)
        {
            cmd = new NpgsqlCommand(SQL, conn);
            // 添加参数
            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var paramName = prop.Name.StartsWith("@") ? prop.Name : $"@{prop.Name}";
                    cmd.Parameters.AddWithValue(paramName, prop.GetValue(parameters) ?? DBNull.Value);
                }
            }
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                da.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
              if(isDispose)
                    conn.Dispose();
            }
            return dt;
        }
        
        public int ExecuteNonQuery(string sql, object parameters = null)
        {
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    foreach (var prop in parameters.GetType().GetProperties())
                    {
                        cmd.Parameters.Add(new NpgsqlParameter(prop.Name, prop.GetValue(parameters) ?? DBNull.Value));
                    }
                }
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }
        public void InsertTable(string chkSQL, string chkSQLParm, string iSQL, string uSQL, string dSQL, DataTable dt, string strConn, string updateStatus, string procedureName)
        {
            cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            cmd.Transaction = trans;

            bool isPass = true;

            try
            {
                if (iSQL.Length > 0 && uSQL.Length > 0 && chkSQL.Length > 0 && chkSQLParm.Length > 0)
                {
                    #region Delete
                    if (dSQL.Length > 0)
                    {
                        string strDeleteTableName = dSQL.ToUpper().Replace("FROM", "^").Replace("WHERE", "^").Split('^')[1].Trim();
                        string strSelectTableName = chkSQL.ToUpper().Replace("FROM", "^").Replace("WHERE", "^").Split('^')[1].Trim();
                        if (dSQL.ToUpper().IndexOf("INSERT") != -1 || dSQL.ToUpper().IndexOf("UPDATE") != -1 ||
                            dSQL.ToUpper().IndexOf("DELETE") == -1 || dSQL.ToUpper().Substring(0, 6) != "DELETE" ||
                            strDeleteTableName != strSelectTableName ||
                            iSQL.IndexOf(strDeleteTableName) == -1 || uSQL.IndexOf(strDeleteTableName) == -1 || chkSQL.IndexOf(strDeleteTableName) == -1)
                        {
                            isPass = false;
                            string sMsg = "The SQL contain illegal characters." + " <br /> INSERT:" + iSQL + "; <br /> UPDATE:" + uSQL + "; <br /> CHECK:" + chkSQL + "; <br /> DELETE:" + dSQL;
                            Console.WriteLine(sMsg);
                        }
                        else
                        {
                            cmd.CommandText = dSQL;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region UpdateStatus SQL
                    if (updateStatus.Length > 0 && isPass)
                    {
                        if (updateStatus.ToUpper().IndexOf("UPDATE") == -1 || updateStatus.ToUpper().Substring(0, 6) != "UPDATE")
                        {
                            isPass = false;
                            string sMsg = "The SQL contain illegal characters." + " <br /> UpdateStatusSQL:" + updateStatus;
                            Console.WriteLine(sMsg);
                        }
                        else
                        {
                            cmd.CommandText = updateStatus;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region Insert & Update
                    if (isPass)
                    {
                        if (iSQL.ToUpper().Substring(0, 6) == "DELETE" || iSQL.ToUpper().Substring(0,6) == "UPDATE" ||
                            iSQL.ToUpper().IndexOf("INSERT") == -1 || iSQL.ToUpper().Substring(0, 6) != "INSERT" ||
                            uSQL.ToUpper().Substring(0, 6) == "DELETE" || uSQL.ToUpper().IndexOf("INSERT") != -1 ||
                            uSQL.ToUpper().IndexOf("UPDATE") == -1 || uSQL.ToUpper().Substring(0, 6) != "UPDATE" ||
                            chkSQL.ToUpper().IndexOf("DELETE") != -1 || chkSQL.ToUpper().IndexOf("INSERT") != -1 || chkSQL.ToUpper().IndexOf("UPDATE") != -1 ||
                            chkSQL.ToUpper().IndexOf("SELECT") == -1 || chkSQL.ToUpper().Substring(0, 6) != "SELECT")
                        {
                            isPass = false;
                            string sMsg = "The SQL contain illegal characters." + " <br /> INSERT:" + iSQL + "; <br /> UPDATE:" + uSQL + "; <br /> CHECK:" + chkSQL + "; <br /> DELETE:" + dSQL;
                            Console.WriteLine(sMsg);
                        }
                        else
                        {
                            Hashtable ht = new Hashtable();
                            chkSQLParm = chkSQLParm.Replace(";", ",");
                            string[] s = chkSQLParm.Split(',');

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                ht.Clear();
                                for (int t = 0; t < s.Length; t++)
                                {
                                    string sParm = s[t].ToString().Trim();
                                    ht.Add(sParm, dt.Rows[i][sParm].ToString().Trim());
                                }

                                if (dSQL.Length > 0)
                                    cmd.CommandText = iSQL;
                                else
                                {
                                    if (isExists(chkSQL, chkSQLParm, ht, strConn))
                                        cmd.CommandText = uSQL;
                                    else
                                        cmd.CommandText = iSQL;
                                }

                                cmd.Parameters.Clear();
                                //string rowData = "";
                                for (int x = 0; x < dt.Columns.Count; x++)
                                {
                                    //取列名
                                    string sParm = dt.Columns[x].ColumnName.ToString().Trim();
                                    string sValue = dt.Rows[i][sParm].ToString().Trim();
                                    if (string.IsNullOrEmpty(sValue))
                                    {
                                        cmd.Parameters.AddWithValue(sParm, DBNull.Value);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue(sParm, sValue);
                                    }
                                    Console.WriteLine();
                                    // rowData += sParm + "：" + sValue + " , ";
                                }

                                //Console.WriteLine(rowData);

                                if(iSQL.Contains(":Id"))
                                {
                                    cmd.Parameters.AddWithValue("Id", Guid.NewGuid());
                                }

                                cmd.ExecuteNonQuery();
                                if (iSQL.Contains("CASH_ACCOUNT_PS") ||uSQL.Contains("CASH_ACCOUNT_PS"))
                                {
                                    trans.Commit();
                                }
                                Console.WriteLine("Inserting or Updating: " + (i + 1).ToString() + " (total:" + dt.Rows.Count.ToString() + ")");
                            }
                        }
                    }
                    #endregion

                    #region Procedure
                    if (procedureName.Length > 0 && isPass)
                    {
                        // 檢查procedure中是否含有空格
                        if (procedureName.Split(' ').Length > 1)
                        {
                            isPass = false;
                            string sMsg = "The procedure contain illegal characters." + " <br /> Procedure:" + procedureName;
                            Console.WriteLine(sMsg);
                        }
                        else
                        {
                            cmd.CommandText = procedureName;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region Commit & Rollback
                    if (isPass)
                    {
                        if (!(iSQL.Contains("CASH_ACCOUNT_PS") || uSQL.Contains("CASH_ACCOUNT_PS")))
                        {
                            trans.Commit();
                        }
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                trans.Dispose();
                conn.Dispose();
            }
        }


        public void InsertTableForBankAccount(string chkSQL, string chkSQLParm, string iSQL, string uSQL, string dSQL, DataTable dt, string strConn, string updateStatus, string procedureName)
        {
            cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            conn.Open();
            bool isPass = true;
            try
            {
                if (iSQL.Length > 0 && uSQL.Length > 0 && chkSQL.Length > 0 && chkSQLParm.Length > 0)
                {

                    #region Insert & Update
                    if (isPass)
                    {
                        if (iSQL.ToUpper().Substring(0, 6) == "DELETE" || iSQL.ToUpper().Substring(0, 6) == "UPDATE" ||
                            iSQL.ToUpper().IndexOf("INSERT") == -1 || iSQL.ToUpper().Substring(0, 6) != "INSERT" ||
                            uSQL.ToUpper().Substring(0, 6) == "DELETE" || uSQL.ToUpper().IndexOf("INSERT") != -1 ||
                            uSQL.ToUpper().IndexOf("UPDATE") == -1 || uSQL.ToUpper().Substring(0, 6) != "UPDATE" ||
                            chkSQL.ToUpper().IndexOf("DELETE") != -1 || chkSQL.ToUpper().IndexOf("INSERT") != -1 || chkSQL.ToUpper().IndexOf("UPDATE") != -1 ||
                            chkSQL.ToUpper().IndexOf("SELECT") == -1 || chkSQL.ToUpper().Substring(0, 6) != "SELECT")
                        {
                            isPass = false;
                            string sMsg = "The SQL contain illegal characters." + " <br /> INSERT:" + iSQL + "; <br /> UPDATE:" + uSQL + "; <br /> CHECK:" + chkSQL + "; <br /> DELETE:" + dSQL;
                            Console.WriteLine(sMsg);
                        }
                        else
                        {
                            Hashtable ht = new Hashtable();
                            chkSQLParm = chkSQLParm.Replace(";", ",");
                            string[] s = chkSQLParm.Split(',');
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                NpgsqlTransaction trans = conn.BeginTransaction();
                                cmd.Transaction = trans;
                                ht.Clear();
                                for (int t = 0; t < s.Length; t++)
                                {
                                    string sParm = s[t].ToString().Trim();
                                    ht.Add(sParm, dt.Rows[i][sParm].ToString().Trim());
                                }

                                if (dSQL.Length > 0)
                                    cmd.CommandText = iSQL;
                                else
                                {
                                    if (isExists(chkSQL, chkSQLParm, ht, strConn))
                                        cmd.CommandText = uSQL;
                                    else
                                        cmd.CommandText = iSQL;
                                }

                                cmd.Parameters.Clear();
                                //string rowData = "";
                                for (int x = 0; x < dt.Columns.Count; x++)
                                {
                                    //取列名
                                    string sParm = dt.Columns[x].ColumnName.ToString().Trim();
                                    string sValue = dt.Rows[i][sParm].ToString().Trim();
                                    if (string.IsNullOrEmpty(sValue))
                                    {
                                        cmd.Parameters.AddWithValue(sParm, DBNull.Value);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue(sParm, sValue);
                                    }
                                    Console.WriteLine();
                                    // rowData += sParm + "：" + sValue + " , ";
                                }

                                //Console.WriteLine(rowData);

                                if (iSQL.Contains(":Id"))
                                {
                                    cmd.Parameters.AddWithValue("Id", Guid.NewGuid());
                                }

                                cmd.ExecuteNonQuery();
                                trans.Commit();
                                Console.WriteLine("Inserting or Updating: " + (i + 1).ToString() + " (total:" + dt.Rows.Count.ToString() + ")");
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                //trans.Dispose();
                conn.Dispose();
            }
        }

        private bool isExists(string chkSQL, string chkSQLParm, Hashtable ht, string strConn)
        {
            bool bResult = false;
            NpgsqlConnection _conn = new NpgsqlConnection(strConn);
            NpgsqlCommand _cmd = new NpgsqlCommand(chkSQL, _conn);
            //_cmd.CommandTimeout =7200;
            _cmd.CommandType = CommandType.Text;
            _cmd.Parameters.Clear();
            string[] s = chkSQLParm.Split(',');
            for (int i = 0; i < s.Length; i++)
            {
                string sParm = s[i].ToString().Trim();
                string sValue = ht[sParm].ToString().Trim();
                _cmd.Parameters.AddWithValue(sParm, sValue);
            }

            NpgsqlDataReader dr;
            try
            {
                _conn.Open();
                dr = _cmd.ExecuteReader();
                bResult = dr.Read();

                dr.Close();
                dr.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                _conn.Dispose();
            }

            return bResult;
        }

        /// <summary>
        public void InsertUpdateSql(string sqlCommandText, List<NpgsqlParameter> parameters, bool isContinue = false, bool isTransaction = false)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (var trans = isTransaction ? conn.BeginTransaction() : null)
                using (var cmd = new NpgsqlCommand(sqlCommandText, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Count > 0)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    if (isTransaction)
                        cmd.Transaction = trans;

                    cmd.ExecuteNonQuery();

                    if (isTransaction)
                        trans.Commit();
                }
            }
            catch (Exception e)
            {
                // 若有 transaction，rollback
                if (isTransaction && conn.State == ConnectionState.Open)
                {
                    try { conn.BeginTransaction().Rollback(); } catch { }
                }
                throw;
            }
            finally
            {
                //
                if ((conn.State == ConnectionState.Open) && !isContinue)
                    conn.Close();
            }
        }
        public void connClose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
