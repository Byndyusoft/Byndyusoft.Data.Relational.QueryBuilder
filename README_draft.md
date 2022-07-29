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

Для запросов можно использовать любой тип для первичного ключа. Если использовать интерфейс IEntity для модели, в которых первичный ключ определен как *long*, то часть запросов будет проще, например, вставка всех полей, обновление всех полей, фильтр по ИД.

Библиотека будет воспринимать все публичные свойства с присутствующими методами *get* и *set* как колонки таблицы. Их наименования будут соответствовать названиям колонок (для PostgreSQL используется форматирование SnakeCase).

## Select

Создаем *SelectQuery* объект.

```csharp
TBDL
```

Вызываем *SelectQuery* с необходимыми параметрами и настройками.

```csharp
TBDL
```

Пример простого джойна с Select'ом в DTO.

```csharp
TBDL
```

Пример с вычисляемыми колонками с группировкой и сортировкой.

```csharp
TBDL
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
