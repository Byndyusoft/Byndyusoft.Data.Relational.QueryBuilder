using System;
using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Page;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Dapper;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders
{
    public abstract class SelectQueryBuilderBase<TBuilder> where TBuilder : SelectQueryBuilderBase<TBuilder>
    {
        protected readonly ColumnConverter ColumnConverter;
        protected readonly ConditionalCollector Conditionals;
        protected readonly FromCollector FromCollector;
        protected readonly GroupByCollector GroupBy;
        protected readonly OrderByCollector OrderBy;
        protected readonly SelectCollector SelectCollector;

        protected bool CountOnly;

        protected long? Limit;
        protected long? Offset;

        protected SelectQueryBuilderBase(bool isPostgres = true, string? paramPrefix = null)
        {
            ColumnConverter = new ColumnConverter(isPostgres);
            SelectCollector = new SelectCollector(ColumnConverter);
            FromCollector = new FromCollector(ColumnConverter);
            Conditionals = new ConditionalCollector(ColumnConverter, paramPrefix);
            OrderBy = new OrderByCollector(ColumnConverter);
            GroupBy = new GroupByCollector(ColumnConverter);
        }

        protected TBuilder Add(string conditional, object template)
        {
            Conditionals.Add(conditional, template);
            return (TBuilder)this;
        }

        protected TBuilder Add(FormattableString conditional)
        {
            Conditionals.AddFormattableString(conditional);
            return (TBuilder)this;
        }

        protected void SetLimitOffsetFromPage(long itemCountInPage, long pageNo)
        {
            Offset = (pageNo - 1) * itemCountInPage;
            Limit = itemCountInPage;
        }

        protected abstract void PrepareSelect();

        [Obsolete]
        protected void SelectAll()
        {
            SelectCollector.AddExpression("*");
        }

        protected abstract void PrepareFrom();

        protected string GetSql()
        {
            if (CountOnly == false)
                PrepareSelect();
            PrepareFrom();
            var sql = GetSelectClause();
            sql = AddFrom(sql);
            sql = AddWhereIfNeeded(sql);
            if (CountOnly == false)
            {
                sql = AddGroupByIfNeeded(sql);
                sql = AddOrderByIfNeeded(sql);
                sql = AddLimitOffsetIfNeeded(sql);
            }

            return sql;
        }

        protected DynamicParameters GetParameters()
        {
            return Conditionals.GetParameters();
        }

        protected QueryObject GetQueryObject()
        {
            return new QueryObject(GetSql(), GetParameters());
        }

        protected string GetSelectClause()
        {
            if (CountOnly)
                SelectCollector.AddCount(nameof(CountModel.TotalRows));
            return SelectCollector.GetSelectClause();
        }

        protected string AddFrom(string sql)
        {
            return FromCollector.AddFromClause(sql);
        }

        protected string AddWhereIfNeeded(string sql)
        {
            return Conditionals.AddWhereIfNeeded(sql);
        }

        protected string AddOrderByIfNeeded(string sql)
        {
            return OrderBy.AddOrderByIfNeeded(sql);
        }

        protected string AddLimitOffsetIfNeeded(string sql)
        {
            if (Limit.HasValue && Offset.HasValue)
                sql += $@"
LIMIT {Limit} OFFSET {Offset}";

            return sql;
        }

        protected string AddGroupByIfNeeded(string sql)
        {
            return GroupBy.AddGroupByIfNeeded(sql);
        }

        public TBuilder SelectPage(long itemCountInPage, long pageNo)
        {
            SetLimitOffsetFromPage(itemCountInPage, pageNo);
            return (TBuilder)this;
        }

        public TBuilder LimitOffset(long limit, long offset = 0)
        {
            Limit = limit;
            Offset = offset;
            return (TBuilder)this;
        }

        public TBuilder SelectCountOnly()
        {
            CountOnly = true;
            return (TBuilder)this;
        }

        public virtual QueryObject Build()
        {
            return GetQueryObject();
        }

        public TBuilder If(bool check, Action<TBuilder> callback, Action<TBuilder>? callbackElse = null)
        {
            if (check)
                callback((TBuilder)this);
            else
                callbackElse?.Invoke((TBuilder)this);

            return (TBuilder)this;
        }
    }
}