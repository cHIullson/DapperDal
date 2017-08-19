USE master;

IF EXISTS (   SELECT *
              FROM   sysdatabases
              WHERE  name = 'DapperDalTest'
          )
    DROP DATABASE DapperDalTest;

EXEC('
CREATE DATABASE DapperDalTest
')