# DapperDal

用 `Dapper`、`Dapper-Extensions`、`Abp.Dapper` 封装的数据访问层

NuGet: [![NuGet Badge](https://buildstats.info/nuget/DapperDal)](https://www.nuget.org/packages/DapperDal/)

Release Notes
-------------
### 1.5.13
* 添加谓词表达式组合扩展方法（来自[alexfoxgill/ExpressionTools](https://github.com/alexfoxgill/ExpressionTools)）

### 1.5.12
* 优化 `Exsit` 、`Count` 方法

### 1.5.11
* 优化更新方法，支持传递小写字段名
* 添加判断实体是否存在方法：`Exsit` 

### 1.5.10
* 添加逻辑删除方法：`SoftDeleteById` 

### 1.5.9
* 添加实体查询方法：`GetFirstOrDefault` 、`QueryFirstOrDefault` 、`QueryFirst`

### 1.5.8
* 添加SQL执行方法：`Execute` 、`ExecuteScalar`

### 1.5.7
* 添加实体查询方法：`GetFirst` 、`GetTop`
* 优化实体查询方法，添加实体集合返回前是否要缓冲的设置点
* 优化逻辑删除，添加更新属性及值的设置点

### 1.5.6
* 生成查询 SQL 时，添加 `WITH (NOLOCK)`
* 添加设置项：SQL 输出方法

### 1.5.5
* 添加逻辑删除方法 `SoftDelete` 

### 1.5.4
* 表达式转换用 `QueryBuilder` 替换 `ExpressionVisitor` 实现，以支持多个查询条件（来自[ryanwatson/Dapper.Extensions.Linq](https://github.com/ryanwatson/Dapper.Extensions.Linq/blob/master/Dapper.Extensions.Linq/Builder/QueryBuilder.cs)）

### 1.5.3
* 添加支持多个实体查询的 `Query` 方法

### 1.5.2
* 添加更新部分属性的 `Update` 方法（来自[vilix13/Dapper-Extensions](https://github.com/vilix13/Dapper-Extensions)）
* 添加使用谓词、匿名对象或 `lambda` 表达式作条件的 `Update` 方法

### 1.5.1
* 整合 `Abp.Dapper`，为 `Dapper-Extensions` 添加 `lambda` 表达式功能（来自[Abp.Dapper](https://github.com/arbing/aspnetboilerplate/tree/master/src/Abp.Dapper)）

