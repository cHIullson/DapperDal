USE master;

IF EXISTS (   SELECT *
              FROM   sysdatabases
              WHERE  name = 'dapperTest'
          )
    DROP DATABASE dapperTest;

EXEC('
CREATE DATABASE dapperTest
    ON PRIMARY (   NAME = ''dapperTest''
                 , FILENAME = ''C:\Users\xiebt\dapperTest_data.mdf''
                 , SIZE = 5MB
                 , MAXSIZE = 100MB
                 , FILEGROWTH = 15%
               )
    LOG ON (   NAME = ''dapperTest_log''
             , FILENAME = ''C:\Users\xiebt\dapperTest_log.ldf''
             , SIZE = 2MB
             , FILEGROWTH = 1MB
           );
')