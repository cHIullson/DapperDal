using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    [SetUpFixture]
    public class MySetUpClass
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            CreateDatabase();

            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dapperTest;Integrated security=True;Application Name=DapperDal;";
            var connection = new SqlConnection(connectionString);
            var files = new List<string>
            {
                ReadScriptFile("CreateAnimalTable"),
                ReadScriptFile("CreateFooTable"),
                ReadScriptFile("CreateMultikeyTable"),
                ReadScriptFile("CreatePersonTable"),
                ReadScriptFile("CreateCarTable")
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
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dapperTest;Integrated security=True;Application Name=DapperDal;";
            var connection = new SqlConnection(connectionString);
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
        protected IDatabase Db;

        [SetUp]
        public virtual void Setup()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dapperTest;Integrated security=True;Application Name=DapperDal;";
            var connection = new SqlConnection(connectionString);
            var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
            config.OutputSql = Console.WriteLine;
            var sqlGenerator = new SqlGeneratorImpl(config);
            Db = new Database(connection, sqlGenerator);
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