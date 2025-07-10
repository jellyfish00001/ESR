using Microsoft.Extensions.Configuration;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Threading.Tasks;

namespace ERS.Dapper
{
    public class DapperHelper
    {
        private string _conn = "";
        private DatabaseType _database;
        public DapperHelper(DatabaseType database, string conn)
        {
            _conn = conn ?? throw new Exception("数据库连接不能为空！");
            _database = database;
        }
        public DapperHelper(IConfiguration Configuration, DatabaseType database, string conn)
        {
            _conn = conn ?? throw new Exception("数据库连接不能为空！");
            _database = database;
        }
        private IDbConnection OpenConnection()
        {
            IDbConnection DbConnection = null;
            switch (_database)
            {
                case DatabaseType.Oracle:
                    DbConnection = new OracleConnection(_conn);
                    break;
                case DatabaseType.SQLServer:
                    DbConnection = new SqlConnection(_conn);
                    break;
                case DatabaseType.PostgreSQL:
                    DbConnection = new NpgsqlConnection(_conn);
                    break;
                default:
                    throw new Exception("数据库不存在！");
            }
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            return DbConnection;
        }
        public async Task<IList<T>> QueryAsync<T>(string sql, object condition = null)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var res = await conn.QueryAsync<T>(sql, condition);
                conn.Close();
                return res.ToList();
            }
        }

    }
}
