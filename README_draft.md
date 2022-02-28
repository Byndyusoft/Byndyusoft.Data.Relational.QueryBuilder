# Byndyusoft.Data.Relational.QueryBuilder [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Data.Relational.QueryBuilder.svg)](https://www.nuget.org/packages/Byndyusoft.Data.Relational.QueryBuilder/)

Библиотека для постороения запросов без использования строковых констант для названий колонок. Ускоряет создание типовых запросов. Названия колонок должны соответствовать наименованиям свойств.

TBDL Описать остальные концепции (анемичная модель, ИД - Int64, свойства публичные с публичным геттером/возможно сеттером, для Npgsql используется SnakeCase).

# Сценарии использования

## Модель данных для примеров

```csharp
TBDL
```

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
