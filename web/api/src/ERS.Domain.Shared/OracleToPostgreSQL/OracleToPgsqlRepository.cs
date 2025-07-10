using System.IO;
using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace ERS.OracleToPostgreSQL
{
    public class OracleToPgsqlRepository : IOracleToPgsqlRepository, IScopedDependency
    {
        private IConfiguration _Configuration;

        public OracleToPgsqlRepository(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }

        // public DataSet GetXMLSet(string xmlStr)
        // {
        //     if(!string.IsNullOrEmpty(xmlStr))
        //     {
        //         StringReader StrStream = null;
        //         XmlTextReader Xmlrdr = null;
        //         try
        //         {
        //             DataSet ds = new DataSet();
        //             //读取字符串中的信息
        //             StrStream = new StringReader(xmlStr);
        //             //获取StrStream中的数据
        //             Xmlrdr = new XmlTextReader(StrStream);
        //             //ds获取Xmlrdr中的数据              
        //             ds.ReadXml(Xmlrdr);
        //             return ds;
        //         }
        //         finally
        //         {
        //             //释放资源
        //             if (Xmlrdr != null)
        //             {
        //                 Xmlrdr.Close();
        //                 StrStream.Close();
        //                 StrStream.Dispose();
        //             }
        //         }
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }

        public void OracleToPgsqlDataSync(string xmlFileName, string db = "PSDB")
        {
            string sMsg = "Start...";
            Console.WriteLine(sMsg);

            string oradb = _Configuration.GetSection("PS:" + db).Value.Trim();
            string npgdb = _Configuration.GetSection("PS:ERSPGDB").Value.Trim();

            DataSet myDS = new DataSet();
            myDS.ReadXml(Path.Combine(Path.Combine("Files", "XML", xmlFileName)));
            Console.WriteLine(Path.Combine(Path.Combine("Files", "XML", xmlFileName)));
            sMsg = "Read xml DB connection:" + myDS.Tables.Count.ToString();
            Console.WriteLine(sMsg);

            DataTable dt = new DataTable();
            dt = myDS.Tables["source"];
            string sConn = oradb;
            // sConn = sConn.Replace(":HOST", dt.Rows[0]["HOST"].ToString().Trim());
            // sConn = sConn.Replace(":PORT", dt.Rows[0]["PORT"].ToString().Trim());
            // sConn = sConn.Replace(":SERVICE_NAME", dt.Rows[0]["SERVICE_NAME"].ToString().Trim());
            // sConn = sConn.Replace(":UserId", dt.Rows[0]["UserId"].ToString().Trim());
            // sConn = sConn.Replace(":Password", dt.Rows[0]["Password"].ToString().Trim());

            string strSQL = dt.Rows[0]["Select"].ToString().Trim();
            string iSQL = dt.Rows[0]["Insert"].ToString().Trim();
            string uSQL = dt.Rows[0]["Update"].ToString().Trim();
            string dSQL = dt.Rows[0]["Delete"].ToString().Trim();
            string chkSQL = dt.Rows[0]["Check"].ToString().Trim();
            string chkSQLParm = dt.Rows[0]["CheckParm"].ToString().Trim();
            string updateStatus = string.Empty;
            if (dt.Columns.Contains("UpdateStatus"))
            {
                updateStatus = dt.Rows[0]["UpdateStatus"].ToString().Trim();
            }

            string procedureName = string.Empty;
            if (dt.Columns.Contains("Procedure"))
            {
                procedureName = dt.Rows[0]["Procedure"].ToString().Trim();
            }

            sMsg = "Read source:" + dt.Rows[0]["System"].ToString().Trim() + " connectionString";
            Console.WriteLine(sMsg);

            OracleHelp myGet = new OracleHelp(sConn);



            // Oracle数据库获取的DataTable
            DataTable dtIn = new DataTable();
            dtIn = myGet.GetDataTable(strSQL);



            sMsg = "Read source:" + dt.Rows[0]["System"].ToString().Trim() + " rows count:" + dtIn.Rows.Count.ToString();
            Console.WriteLine(sMsg);

            dt = null;
            myDS.Tables.Remove("source");

            for (int i = 0; i < myDS.Tables.Count; i++)
            {
                for (int j = 0; j < myDS.Tables[i].Rows.Count; j++)
                {
                    try
                    {
                        sConn = npgdb;
                        // sConn = sConn.Replace(":HOST", myDS.Tables[i].Rows[j]["HOST"].ToString().Trim());
                        // sConn = sConn.Replace(":PORT", myDS.Tables[i].Rows[j]["PORT"].ToString().Trim());
                        // sConn = sConn.Replace(":SERVICE_NAME", myDS.Tables[i].Rows[j]["SERVICE_NAME"].ToString().Trim());
                        // sConn = sConn.Replace(":UserId", myDS.Tables[i].Rows[j]["UserId"].ToString().Trim());
                        // sConn = sConn.Replace(":Password", myDS.Tables[i].Rows[j]["Password"].ToString().Trim());
                        sMsg = "Read " + myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + " connectionString";
                        Console.WriteLine(sMsg);

                        NpgsqlHelp myInsert = new NpgsqlHelp(sConn);
                        sMsg = myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + ",Update data....";
                        Console.WriteLine(sMsg);
                        myInsert.InsertTable(chkSQL, chkSQLParm, iSQL, uSQL, dSQL, dtIn, sConn, updateStatus, procedureName);

                        sMsg = myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + ",Data saving succeed.";
                        Console.WriteLine(sMsg);
                    }
                    catch (Exception e)
                    {
                        sMsg = "Error:" + myDS.Tables[i].TableName + ", Data saving failed,reason ";
                        sMsg += e.Message;
                        Console.WriteLine(sMsg);
                    }
                }
            }
            myDS = null;

        }

        public void OracleToPgsqlDataSyncForBankAccount(string xmlFileName, string db = "PSDB")
        {
            string sMsg = "Start...";
            Console.WriteLine(sMsg);

            string oradb = _Configuration.GetSection("PS:" + db).Value.Trim();
            string npgdb = _Configuration.GetSection("PS:ERSPGDB").Value.Trim();

            DataSet myDS = new DataSet();
            myDS.ReadXml(Path.Combine(Path.Combine("Files", "XML", xmlFileName)));
            Console.WriteLine(Path.Combine(Path.Combine("Files", "XML", xmlFileName)));
            sMsg = "Read xml DB connection:" + myDS.Tables.Count.ToString();
            Console.WriteLine(sMsg);

            DataTable dt = new DataTable();
            dt = myDS.Tables["source"];
            string sConn = oradb;
            // sConn = sConn.Replace(":HOST", dt.Rows[0]["HOST"].ToString().Trim());
            // sConn = sConn.Replace(":PORT", dt.Rows[0]["PORT"].ToString().Trim());
            // sConn = sConn.Replace(":SERVICE_NAME", dt.Rows[0]["SERVICE_NAME"].ToString().Trim());
            // sConn = sConn.Replace(":UserId", dt.Rows[0]["UserId"].ToString().Trim());
            // sConn = sConn.Replace(":Password", dt.Rows[0]["Password"].ToString().Trim());

            string strSQL = dt.Rows[0]["Select"].ToString().Trim();
            string iSQL = dt.Rows[0]["Insert"].ToString().Trim();
            string uSQL = dt.Rows[0]["Update"].ToString().Trim();
            string dSQL = dt.Rows[0]["Delete"].ToString().Trim();
            string chkSQL = dt.Rows[0]["Check"].ToString().Trim();
            string chkSQLParm = dt.Rows[0]["CheckParm"].ToString().Trim();
            string updateStatus = string.Empty;
            if (dt.Columns.Contains("UpdateStatus"))
            {
                updateStatus = dt.Rows[0]["UpdateStatus"].ToString().Trim();
            }

            string procedureName = string.Empty;
            if (dt.Columns.Contains("Procedure"))
            {
                procedureName = dt.Rows[0]["Procedure"].ToString().Trim();
            }

            sMsg = "Read source:" + dt.Rows[0]["System"].ToString().Trim() + " connectionString";
            Console.WriteLine(sMsg);

            OracleHelp myGet = new OracleHelp(sConn);



            // Oracle数据库获取的DataTable
            DataTable dtIn = new DataTable();
            dtIn = myGet.GetDataTable(strSQL);



            sMsg = "Read source:" + dt.Rows[0]["System"].ToString().Trim() + " rows count:" + dtIn.Rows.Count.ToString();
            Console.WriteLine(sMsg);

            dt = null;
            myDS.Tables.Remove("source");

            for (int i = 0; i < myDS.Tables.Count; i++)
            {
                for (int j = 0; j < myDS.Tables[i].Rows.Count; j++)
                {
                    try
                    {
                        sConn = npgdb;
                        // sConn = sConn.Replace(":HOST", myDS.Tables[i].Rows[j]["HOST"].ToString().Trim());
                        // sConn = sConn.Replace(":PORT", myDS.Tables[i].Rows[j]["PORT"].ToString().Trim());
                        // sConn = sConn.Replace(":SERVICE_NAME", myDS.Tables[i].Rows[j]["SERVICE_NAME"].ToString().Trim());
                        // sConn = sConn.Replace(":UserId", myDS.Tables[i].Rows[j]["UserId"].ToString().Trim());
                        // sConn = sConn.Replace(":Password", myDS.Tables[i].Rows[j]["Password"].ToString().Trim());
                        sMsg = "Read " + myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + " connectionString";
                        Console.WriteLine(sMsg);

                        NpgsqlHelp myInsert = new NpgsqlHelp(sConn);
                        sMsg = myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + ",Update data....";
                        Console.WriteLine(sMsg);
                        myInsert.InsertTableForBankAccount(chkSQL, chkSQLParm, iSQL, uSQL, dSQL, dtIn, sConn, updateStatus, procedureName);

                        sMsg = myDS.Tables[i].TableName + ":" + myDS.Tables[i].Rows[j]["System"].ToString().Trim() + ",Data saving succeed.";
                        Console.WriteLine(sMsg);
                    }
                    catch (Exception e)
                    {
                        sMsg = "Error:" + myDS.Tables[i].TableName + ", Data saving failed,reason ";
                        sMsg += e.Message;
                        Console.WriteLine(sMsg);
                    }
                }
            }
            myDS = null;

        }
    }
}