# Byndyusoft.Data.Relational.QueryBuilder [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/)

Библиотека для постороения запросов.

Основная концепция библиотеки заключается в том, чтобы в запросах не использовать строковые константы для названий колонок. Так можно бьть увереннее и спокойнее рефакторить код. К тому же не нужно будет проверять постоянно, правильно ли написаны типовые (и не очень) запросы.

Для стандартных запросов, в которых участвую все колонки (SELECT, UPDATE, INSERT) не придется беспокоиться о том, что добавится или удалится новая колонка.

# Сценарии использования

## Модель данных для примеров
Для примеро будем использовать следующую модель данных.

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

Для запросов можно использовать любой тип для первичного ключа. Если использовать интерфейс *IEntity* для модели, в которых первичный ключ определен как *long*, то часть запросов будет проще, например, вставка всех полей, обновление всех полей, фильтр по ИД.

Библиотека будет воспринимать все публичные свойства с присутствующими методами *get* и *set* как колонки таблицы. Их наименования будут соответствовать названиям колонок (для PostgreSQL используется форматирование SnakeCase).

## Select

### Запрос сущности из таблицы

Для запросов SELECT нужно создать **SelectQuery** объект. Вот пример для сущности *Company*:

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

Пример запросов на получении компании по ИД и по имени:

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

### Запрос из нескольких таблиц с проекцией данных в DTO

Предположим, нам нужно получить информацию о пользователе в виде следующего объекта:

```csharp
public class UserDto
{
  public string Login { get; set; } = default!;
  public string Password { get; set; } = default!;
  public string CompanyName { get; set; } = default!;
}
```

Пример репозитория и объекта **SelectQuery**:

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

### Запрос из нескольких таблиц с агрегацией и сортировкой

Предположим, нам нужно получить информацию о списке компаний с количеством пользователей:

```csharp
public class CompanyReportDto
{
  public string CompanyName { get; set; } = default!;
  public long UserCount { get; set; }
}
```

Пример репозитория и объекта **SelectQuery**:

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

Пример.

```csharp
TBDL
```

## Update

Пример с обновлениями всех колонок.

```csharp
TBDL
```

Пример с обновлениями части колонок.

```csharp
TBDL
```

## Delete

Пример.

```csharp
TBDL
```

## Запрос с использованием *StringExpressionMapper*

```csharp
TBDL
```
