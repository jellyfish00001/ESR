using Microsoft.Extensions.Configuration;
using System;

namespace ERS.Dapper
{
    public class DapperFactory : IDapperFactory
    {
        private readonly IConfiguration _Configuration;

        public DapperFactory(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }
        public DapperHelper CreateConnection(string conn = "Default", DatabaseType databaseType = DatabaseType.PostgreSQL)
        {
            IConfiguration ConnectionList = null;

            switch (databaseType)
            {
                case DatabaseType.Oracle:
                    ConnectionList = _Configuration.GetSection("ConnectionStrings");
                    break;
                case DatabaseType.SQLServer:
                    ConnectionList = _Configuration.GetSection("ConnectionStrings");
                    break;
                case DatabaseType.PostgreSQL:
                    ConnectionList = _Configuration.GetSection("ConnectionStrings");
                    break;
                default:
                    throw new Exception("数据库不存在！");
            }
            return new DapperHelper(_Configuration, databaseType, ConnectionList[conn]);
        }

    }
}
