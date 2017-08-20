# DapperDal - 简单易用的微 ORM 类库，数据访问层基类

[![NuGet Badge](https://buildstats.info/nuget/DapperDal)](https://www.nuget.org/packages/DapperDal/)
[![Build status](https://ci.appveyor.com/api/projects/status/rxogkxvhdthd4rf0?svg=true)](https://ci.appveyor.com/project/arbing/dapperdal)

Introduction
-------------
基于 `Dapper`、`Dapper-Extensions` 构建的微型 ORM 类库，提供一个包含增、删、改、查等常用方法的数据访问层基类，支持用 `Lambda` 表达式书写查询和更新条件。

Features
-------------
* 零配置，开箱即用。
* 数据库表、实体类型自动映射，主键自动映射。实体类上无需定义 Attribute。
* 灵活易用的增、删、改、查、分页查询等常用重载方法，单表操作无需编写任何 SQL 语句。
* 查询和更新条件支持 `Lambda` 表达式组合，自动生成安全参数化的 SQL 语句。
* 提供 SQL 语句、存储过程执行方法，返回结果集自动模型映射，比 DataSet 效率高。
* 支持部分字段更新，无变化字段不更新。
* 数据库表字段变化时重新生成实体类即可，数据访问层无需重新生成。
* 基类定义了 IDbConnection 属性，所有 `Dapper`、`Dapper-Extensions` 方法都能使用。
* 完善的单元测试。
* 命名约定：实体类名和数据库表名应匹配，实体属性名与表字段名应匹配。主键字段名应以 `Id` 命名或结尾
* 限制：多表联合查询还是需要编写 SQL 语句或者存储过程。

Installation
-------------
https://www.nuget.org/packages/DapperDal

```
PM> Install-Package DapperDal
```

API Document
-------------
https://arbing.github.io/DapperDal

Examples
-------------

定义实体类 PersonEntity
```
/// <summary>
/// 人员信息表实体类
/// </summary>
public class PersonEntity
{
    public long PersonId { get; set; }

    public string PersonName { get; set; }

    public int CarId { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }

    public short IsActive { get; set; }
}
```

数据访问层定义DAL类 PersonDal，继承自 DalBase<PersonEntity, long>
```
/// <summary>
/// 人员信息表数据访问类
/// </summary>
public class PersonDal : DalBase<PersonEntity, long>
{
    public PersonDal() : base(ConnectionNames.Default)
    {
        
    }
    ...
}
```

单条插入
```
var personDal = new PersonDal();

PersonEntity p = new PersonEntity { PersonName = "Foo", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };

long id = personDal.Insert(p);
```

多条批量插入
```
var personDal = new PersonDal();

PersonEntity p1 = new PersonEntity { PersonName = "Foo", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
PersonEntity p3 = new PersonEntity { PersonName = "Baz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };

personDal.Insert(new[] { p1, p2, p3 });
```

主键获取
```
var personDal = new PersonDal();

long personId = 1;

PersonEntity person = personDal.Get(id);
```

条件获取
```
var personDal = new PersonDal();

PersonEntity person = personDal.GetFirst(p => p.PersonId = 1);

PersonEntity person = personDal.GetFirst(p => p.IsActive != 1);

PersonEntity person = personDal.GetFirst(p => UpdateTime >= DateTime.Today);

PersonEntity person = personDal.GetFirst(p => p.PersonId = 1 && p.IsActive == 1);

PersonEntity person = personDal.GetFirst(p => p.PersonId = 1 || p.PersonName = "Foo");

PersonEntity person = personDal.GetFirst(p => p.IsActive == 1 && (p.PersonName == "Foo" || p.CarId == 3));

PersonEntity person = personDal.GetFirst(p => p.PersonName.Equals("Foo"));
PersonEntity person = personDal.GetFirst(p => p.PersonName.Contains("a"));
PersonEntity person = personDal.GetFirst(p => p.PersonName.StartsWith("Ba"));
PersonEntity person = personDal.GetFirst(p => p.PersonName.EndsWith("az"));

var carIds = new[] { p2.CarId, 3, 2};
PersonEntity person = personDal.GetFirst(p => carIds.Contains(p.CarId));
```

判断实体是否存在
```
var personDal = new PersonDal();

bool isExsit = personDal.Exsit(id: 1);
bool isExsit = personDal.Exsit(p => p.PersonId = 1);
```

更新实体
```
var p2 = personDal.Get(id);
p2.IsActive = 1;
p2.PersonName = "Baz";

var result = personDal.Update(p2);
```

更新指定实体指定属性
```
var p2 = personDal.Get(id);

p2.PersonName = "Baz";
p2.CarId = 2;
p2.IsActive = 1;
bool result = personDal.Update(p2, new[] { "personName", "CarId", "CarName" });

bool result = personDal.Update(p2, new { personName = "Baz", CarId = 2 });

bool result = personDal.Update(p2.PersonId, new { personName = "Baz", CarId = 2 });

var personName = p2.PersonName;
bool result = personDal.Update(new { p2.PersonId, personName, p2.CarId, CarName = "CarName" });
```

根据指定更新条件更新实体指定属性
```
var personName = p2.PersonName;
bool result = personDal.Update(new { personName, p2.CarId, CarName = "CarName" }, p => p.PersonId == p2.PersonId);
```

删除实体
```
PersonEntity p2 = personDal.Get(id);
bool result = personDal.Delete(p2);

bool result = personDal.Delete(p2.PersonId);

bool result = personDal.Delete(p => p.PersonName == "Bar");
```

列表查询
```
IEnumerable<PersonEntity> persons = personDal.GetList().ToList();

IEnumerable<PersonEntity> persons = personDal.GetList(SortDirection.Descending, p => p.CarId);

IEnumerable<PersonEntity> persons = personDal.GetList(p.IsActive == 1 && p.PersonName == "c");

var predicate = PredicateBuilder.True<PersonEntity>();
predicate = predicate.And(p => p.IsActive == 1);
predicate = predicate.And(p => p.PersonName == "c");
IEnumerable<PersonEntity> persons = personDal.GetList(predicate);

var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };
IEnumerable<PersonEntity> persons = personDal.GetList(p => p.IsActive == 1, sort).ToList();

IEnumerable<PersonEntity> persons = personDal.GetList(p => p.IsActive == 1, SortDirection.Descending, p => p.CarId).ToList();
```

生成的 SQL
```
SELECT [Person].[PersonId], [Person].[PersonName], [Person].[CarId], [Person].[CreateTime], [Person].[UpdateTime], [Person].[IsActive] 
FROM [Person] WITH (NOLOCK) 
WHERE ([Person].[IsActive] = @IsActive_0) 
ORDER BY [Person].[CarId] DESC
```

获取总条数
```
int count = personDal.Count(p => p.IsActive == 1)
```

生成的 SQL
```
SELECT COUNT(*) AS [Total] FROM [Person] WITH (NOLOCK) 
WHERE ([Person].[IsActive] = @IsActive_0)
```

列表分页查询
```
IEnumerable<PersonEntity> persons = personDal.GetListPaged(p => p.IsActive == 1, new { CarId = SortDirection.Descending }, pageNumber: 1, itemsPerPage: 20).ToList()
```

生成的 SQL
```
SELECT TOP(20) [_proj].[PersonId], [_proj].[PersonName], [_proj].[CarId], [_proj].[CreateTime], [_proj].[UpdateTime], [_proj].[IsActive] 
FROM (
  SELECT ROW_NUMBER() OVER(ORDER BY [Person].[CarId] DESC) AS [_row_number], 
  [Person].[PersonId], [Person].[PersonName], [Person].[CarId], [Person].[CreateTime], [Person].[UpdateTime], [Person].[IsActive] 
  FROM [Person] WITH (NOLOCK) 
  WHERE ([Person].[IsActive] = @IsActive_0)) [_proj] 
WHERE [_proj].[_row_number] >= @_pageStartRow 
ORDER BY [_proj].[_row_number]
```

执行查询语句
```
IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
    "select PersonName as Name, CarId from Person where CarId = @CarId", new { CarId = 3 });
```

执行查询存储过程
```
IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
    "dbo.P_GetPersonModelsByCarId",
    new { CarId = 3 }, System.Data.CommandType.StoredProcedure);
```

执行多结果集查询语句
```
Tuple<IEnumerable<PersonEntity>, IEnumerable<PersonModel>> tuple = personDal.QueryMultiple<PersonEntity, PersonModel>(
    "select * from Person where CarId = @CarId;select PersonName AS [Name], CarId from Person where CarId = @CarId",
    new { CarId = 3 });
```

执行多结果集查询存储过程
```
Tuple<IEnumerable<PersonEntity>, IEnumerable<PersonModel>> tuple = personDal.QueryMultiple<PersonEntity, PersonModel>(
    "P_GetPersonMultipleModelsByCarId", new { CarId = 3 }, System.Data.CommandType.StoredProcedure);
```

执行语句
```
int affected = personDal.Execute("update Person set IsActive = 1 where CarId = @CarId", new { CarId = 3 });
```

执行存储过程
```
int affected = personDal.Execute("P_SetPersonsByCarId", new { CarId = 3 }, System.Data.CommandType.StoredProcedure);
```

执行带返回参数的存储过程
```
var parameters = new DynamicParameters(new { CarId = 3 });
parameters.Add("TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

int affected = personDal.Execute("P_SetPersonsByCarId_OutputCount", parameters, System.Data.CommandType.StoredProcedure);

int totalCount = parameters.Get<int>("TotalCount");
```

执行带返回参数的查询存储过程
```
var parameters = new DynamicParameters(new { CarId = 3 });
parameters.Add("TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
    "dbo.P_GetPersonModelsByCarId_OutputCount",
    parameters, System.Data.CommandType.StoredProcedure);

int totalCount = parameters.Get<int>("TotalCount");
```

其他常用方法
```
打开新的 IDbConnection
OpenConnection

执行SQL语句，返回第一行第一列数据
ExecuteScalar

获取前N条
GetTop

逻辑删除
SoftDelete

逻辑删除或激活
SwitchActive
```

常见应用
```
/// <summary>
/// 请求分页数据时的参数基类
/// </summary>
public class PagedParam
{
    /// <summary>
    /// 初始化
    /// </summary>
    public PagedParam()
    {
        PageIndex = 1;
        PageSize = 10;
    }

    /// <summary>
    /// 当前页索引
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页显示的记录数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 要分页的数据总数
    /// </summary>
    public int RecordCount { get; set; }
}

/// <summary>
/// 获取人员分页列表的查询参数
/// </summary>
public class GetPersonPagedListParam : PagedParam
{
    public string PersonName { get; set; }

    public int CarId { get; set; }

    /// <summary>
    /// 创建起始时间
    /// </summary>
    public DateTime? BeginCreateTime { get; set; }

    /// <summary>
    /// 创建结束时间
    /// </summary>
    public DateTime? EndCreateTime { get; set; }
}

/// <summary>
/// 获取人员分页列表的返回实体
/// </summary>
public class GetPersonPagedListOutputDto // : PersonEntity
{
    /// 人员信息
    /// 其他信息
}

/// <summary>
/// 人员信息表数据访问类
/// </summary>
public class PersonDal : DalBase<PersonEntity, long>
{
    public PersonDal() : base(ConnectionNames.Default)
    {
        
    }

    /// <summary>
    /// 获取人员分页列表
    /// </summary>
    /// <param name="param">查询参数</param>
    /// <returns>人员分页列表</returns>
    public PagingList<GetPersonPagedListOutputDto> GetPersonPagedList(GetPersonPagedListParam param)
    {
        var parameters = new DynamicParameters(param).Output(param, p => p.RecordCount);
    
        var list = Query<GetPersonPagedListOutputDto>(
                "p_GetReplacementOrderPagedList",
                parameters,
                commandType: CommandType.StoredProcedure)
            .ToList();
    
        return PagingList.Create(list, param.RecordCount);
    }
}

```

Release Notes
-------------
### 1.5.16
* 重构，合并 `Dapper-Extensions` 到 `DapperDal`，然后移除 `Dapper-Extensions`

### 1.5.15
* 删除方法 `QueryFirstOrDefault`，可以使用 `QueryFirst` 替换
* 添加重载方法 `OpenConnection`、`Execute`、`Query`, 支持传入其他 DB 连接串

### 1.5.14
* 添加逻辑删除或激活方法 `SwitchActive`
* 删除方法 `GetFirstOrDefault`，可以使用 `GetFirst` 替换
* 添加支持主键 ID 的重载方法 `Update`、`Delete`
* 配置项 `SoftDeleteProps` 迁移到 `DapperDal` 中，新添加配置项 `SoftActiveProps`

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

License
-------------
Apache License 2.0