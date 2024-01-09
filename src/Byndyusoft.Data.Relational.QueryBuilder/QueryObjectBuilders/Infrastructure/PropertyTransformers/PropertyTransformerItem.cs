using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Sort;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure.PropertyTransformers
{
    public class PropertyTransformerItem
    {
        public PropertyTransformerItem(string from, string to, string? tableAlias = null)
        {
            From = from;
            To = to;
            TableAlias = tableAlias;
        }

        public string From { get; }

        public string To { get; }

        public string? TableAlias { get; }
    }

    public class DefaultPropertyTransformerItem : PropertyTransformerItem
    {
        public DefaultPropertyTransformerItem(string from, string to, SortDirection sortDirection,
            string? tableAlias = null) : base(from, to, tableAlias)
        {
            SortDirection = sortDirection;
        }

        public SortDirection SortDirection { get; }
    }
}