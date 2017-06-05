USE master;

IF EXISTS (   SELECT *
              FROM   sysdatabases
              WHERE  name = 'dapperTest'
          )
    DROP DATABASE dapperTest;

EXEC('
CREATE DATABASE dapperTest
')