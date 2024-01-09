namespace Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Sort
{
    public class SortDescription : ISortDescription
    {
        public string? PropertyName { get; set; }

        public SortDirection Direction { get; set; }
    }
}