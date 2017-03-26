using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using NUnit.Framework;

namespace DapperDal.Test.IntegrationTests.SqlServer
{
    public class SqlServerBaseFixture
    {
        [SetUp]
        public virtual void Setup()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"];
            var connection = new SqlConnection(connectionString.ConnectionString);
            var files = new List<string>
                                {
                                    ReadScriptFile("CreateCarTable"),
                                    ReadScriptFile("CreatePersonTable"),
                                    ReadScriptFile("CreatePersonProcedure"),
                                };

            foreach (var setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        public string ReadScriptFile(string name)
        {
            string fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using (Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}