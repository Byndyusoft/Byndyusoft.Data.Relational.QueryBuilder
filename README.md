# Byndyusoft.Data.Relational.QueryBuilder [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/)

Library for query building.

The main concept of the library is to avoid using string constants for column names in queries. This allows for more confident and peaceful code refactoring. Additionally, there is no need to constantly check if the standard (or non-standard) queries are written correctly.

For standard queries that involve all columns (SELECT, UPDATE, INSERT), there is no need to worry about adding or deleting a new column.

Below are the descriptions of standard use cases.

# Installing
```
dotnet add package Byndyusoft.Data.Relational.QueryBuilder 
```

# Usage

## Data model for examples
For the examples, we will use the following data model.

```csharp
public class Company : IEntity
{
  public long Id { get; set; }
  public string Name { get; set; } = default!;
  public string Inn { get; set; } = default!;
}

public class User : IEntity
{
  public long Id { get; set; }
  public string Login { get; set; } = default!;
  public string Password { get; set; } = default!;
  public long CompanyId { get; set; }
}
```

For queries, any type can be used for the primary key. If the *IEntity* interface is used for models, where the primary key is defined as *long*, some queries will be easier, for example, inserting all fields, updating all fields, filtering by ID.

The library will treat all public properties with corresponding *get* and *set* methods as table columns. Their names will correspond to the column names (SnakeCase formatting is used for PostgreSQL).
## Select

### Querying an entity from a table.

To query an entity from a table, you need to create a **SelectQuery** object. Here is an example for the *Company* entity:

```csharp
public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
{
  protected override void PrepareFrom()
  {
    FromCollector.From<Company>(TableNames.Company, Aliases.Company);
  }

  protected override void PrepareSelect()
  {
    SelectCollector.To<Company>(Aliases.Company).GetAllPublicValues();
  }

  public SelectQuery ById(long id)
  {
    Conditionals.For<Company>(Aliases.Company).ById(id);
    return this;
  }

  public SelectQuery ByName(string name)
  {
    Conditionals.For<Company>(Aliases.Company).AddEquals(i => i.Name, name);
    return this;
  }
}
```

Example queries to retrieve a company by ID and by name:

```csharp
public class CompanyRepository : DbSessionConsumer
{
  public CompanyRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
  {
  }

  public async Task<Company?> GetById(long id, CancellationToken cancellationToken)
  {
    var queryObject = new SelectQuery().ById(id).Build();
    return await DbSession.QuerySingleOrDefaultAsync(queryObject, cancellationToken: cancellationToken);
  }

  public async Task<Company[]> GetByName(string name, CancellationToken cancellationToken)
  {
    var queryObject = new SelectQuery().ByName(name).Build();
    var companies = await DbSession.QueryAsync<Company>(queryObject, cancellationToken: cancellationToken);
    return companies.ToArray();
  }
}
```

### Query from multiple tables with data projection into DTO

Let's assume we need to obtain information about a user in the form of the following object:

```csharp
public class UserDto
{
  public string Login { get; set; } = default!;
  public string Password { get; set; } = default!;
  public string CompanyName { get; set; } = default!;
}
```

Example of a repository and the **SelectQuery** object:

```csharp
public class UserDtoRepository : DbSessionConsumer
{
  public UserDtoRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
  {
  }

  public async Task<UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
  {
    var queryObject = new SelectQuery().ById(id).Build();
    return await DbSession.QuerySingleOrDefaultAsync<UserDto>(queryObject,
      cancellationToken: cancellationToken);
  }

  public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
  {
    protected override void PrepareFrom()
    {
      FromCollector
        .From<User>(TableNames.Users, Aliases.Users)
        .InnerJoin(TableNames.Company, Aliases.Company, i => i.CompanyId);
    }

    protected override void PrepareSelect()
    {
      SelectCollector.To<UserDto>()
        .From<User>(Aliases.Users)
          .Get(u => u.Login, dto => dto.Login)
          .Get(u => u.Password, dto => dto.Password)
        .Other<Company>(Aliases.Company)
          .Get(c => c.Name, dto => dto.CompanyName);
    }

    public SelectQuery ById(long id)
    {
      Conditionals.For<User>(Aliases.Users).ById(id);
      return this;
    }
  }
}
```

### Query from multiple tables with aggregation and sorting

Let's suppose we need to obtain information about a list of companies with the number of users:

```csharp
public class CompanyReportDto
{
  public string CompanyName { get; set; } = default!;
  public long UserCount { get; set; }
}
```

Example of a repository and the **SelectQuery** object:

```csharp
public class CompanyReportDtoRepository : DbSessionConsumer
{
  public CompanyReportDtoRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
  {
  }

  public async Task<CompanyReportDto[]> GetAsync(CancellationToken cancellationToken)
  {
    var queryObject = new SelectQuery().Build();
    var companyReportDtos = await DbSession.QueryAsync<CompanyReportDto>(queryObject, cancellationToken: cancellationToken);
    return companyReportDtos.ToArray();
  }

  public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
  {
    public SelectQuery()
    {
      GroupBy.For<Company>(Aliases.Company).Add(i => i.Name);
      OrderBy.Add<Company, string>(i => i.Name, isDescending: false, Aliases.Company);
    }

    protected override void PrepareFrom()
    {
      FromCollector
        .From<Company>(TableNames.Company, Aliases.Company)
        .LeftJoin<User>(TableNames.Users, Aliases.Users, (c, u) => $"{c.Id} = {u.CompanyId}");
    }

    protected override void PrepareSelect()
    {
      SelectCollector.To<CompanyReportDto>()
        .Get<User, long>(
          dto => dto.UserCount,
          u => $"SUM(CASE WHEN {u.Id} IS NULL THEN 0 ELSE 1 END)",
          Aliases.Users)
        .From<Company>(Aliases.Company)
          .Get(c => c.Name, dto => dto.CompanyName);
    }
  }
}
```

## Insert

Example of a company insertion:

```csharp
public async Task InsertAsync(Company company, CancellationToken cancellationToken)
{
  var queryObject = InsertQueryBuilder<Company>
    .For(company, TableNames.Company)
    .InsertAllPublicValues()
    .Build();
  var id = await DbSession.ExecuteScalarAsync<long>(queryObject, cancellationToken: cancellationToken);
  company.Id = id;
}
```

## Update

### Updating all fields

Example of updating all fields of a company:

```csharp
public async Task UpdateAsync(Company company, CancellationToken cancellationToken)
{
  var queryObject = UpdateItemQueryBuilder<Company>
    .For(company, TableNames.Company)
    .UpdateAllPublicValues()
    .ById()
    .Build();
  await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
}
```

### Updating a single field

Example of updating the company's tax identification number (INN):

```csharp
public async Task UpdateInnAsync(long id, string inn, CancellationToken cancellationToken)
{
  var queryObject = UpdateQueryBuilder<Company>
    .For(TableNames.Company)
    .Set(i => i.Inn, inn)
    .ById(id)
    .Build();
  await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
}
```

## Delete

Example of deleting a company:

```csharp
public async Task DeleteByIdAsync(long id, CancellationToken cancellationToken)
{
  var queryObject = DeleteQueryBuilder<Company>
    .For(TableNames.Company)
    .ById(id)
    .Build();
  await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
}
```

## Custom queries using *ColumnConverter*

An alternative example of retrieving a *UserDto*, as described above:

```csharp
public async Task<UserDto?> GetByIdAlternativelyAsync(long id, CancellationToken cancellationToken)
{
  var columnConverter = new ColumnConverter(true);
  var sql = columnConverter.Map<User, Company>((user, company) => $@"
SELECT
  {user.Login} AS {nameof(UserDto.Login)},
  {user.Password} AS {nameof(UserDto.Password)},
  {company.Name} AS {nameof(UserDto.CompanyName)}
FROM
  {TableNames.Users} u
  INNER JOIN {TableNames.Company} c on {company.Id} = {user.CompanyId}
WHERE
  {user.Id} = @UserId", "u", "c");

  var queryObject = new QueryObject(sql, new { UserId = id });
  return await DbSession.QuerySingleOrDefaultAsync<UserDto>(queryObject,
    cancellationToken: cancellationToken);
}
```
