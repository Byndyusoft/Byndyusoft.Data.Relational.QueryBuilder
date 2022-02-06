using System;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjects
{
    public class MultipleQueryObject<TFirst, TSecond, TResult>
    {
        public MultipleQueryObject(string sql, Func<TFirst, TSecond, TResult> map, string splitOn = "Id")
            : this(sql, null, map, splitOn)
        {
        }

        public MultipleQueryObject(string sql, object? queryParams, Func<TFirst, TSecond, TResult> map,
            string splitOn = "Id")
        {
            Map = map;
            SplitOn = splitOn;
            QueryObject = new QueryObject(sql, queryParams);
        }

        public Func<TFirst, TSecond, TResult> Map { get; }
        public string SplitOn { get; }
        public QueryObject QueryObject { get; }
    }
}