using System.Data;
using DapperDal.Sql;

namespace DapperDal.Test.Helpers
{
    public class DatabaseInfo
    {
        public IDbConnection Connection { get; set; }
        public ISqlDialect Dialect { get; set; }
    }
}