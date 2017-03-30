# DapperDal

用 `Dapper`、`Dapper-Extensions`、`Abp.Dapper` 封装的数据访问层

NuGet: [![NuGet Badge](https://buildstats.info/nuget/DapperDal)](https://www.nuget.org/packages/DapperDal/)

Release Notes
-------------
### 1.5.4
* 表达式转换用 `QueryBuilder` 替换 `ExpressionVisitor` 实现，以支持多个查询条件（来自[ryanwatson/Dapper.Extensions.Linq](https://github.com/ryanwatson/Dapper.Extensions.Linq/blob/master/Dapper.Extensions.Linq/Builder/QueryBuilder.cs)）

### 1.5.3
* 添加支持多个实体查询的 `Query` 方法

### 1.5.2
* 添加更新部分属性的 `Update` 方法（来自[vilix13/Dapper-Extensions](https://github.com/vilix13/Dapper-Extensions)）
* 添加使用谓词、匿名对象或 `lambda` 表达式作条件的 `Update` 方法

### 1.5.1
* 整合 `Abp.Dapper`，为 `Dapper-Extensions` 添加 `lambda` 表达式功能（来自[Abp.Dapper](https://github.com/arbing/aspnetboilerplate/tree/master/src/Abp.Dapper)）

