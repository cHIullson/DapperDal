using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using NUnit.Framework;

namespace DapperDal.Test.IntegrationTests.SqlServer
{
    [SetUpFixture]
    public class MySetUpClass
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            CreateDatabase();

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

        public void CreateDatabase()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated security=True;Application Name=DapperDal;";
            var connection = new SqlConnection(connectionString);
            var files = new List<string>
            {
                ReadScriptFile("CreateDatabase"),
            };

            foreach (var setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }


        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"];
            var connection = new SqlConnection(connectionString.ConnectionString);
            var files = new List<string>
            {
                ReadScriptFile("TruncateTable"),
                //ReadScriptFile("DropDatabase"),
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

    public class SqlServerBaseFixture
    {
        [SetUp]
        public virtual void Setup()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"];
            var connection = new SqlConnection(connectionString.ConnectionString);
            var files = new List<string>
            {
                ReadScriptFile("TruncateTable"),
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