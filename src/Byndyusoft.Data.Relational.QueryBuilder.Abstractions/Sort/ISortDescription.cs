namespace Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Sort
{
    public interface ISortDescription
    {
        string? PropertyName { get; set; }

        SortDirection Direction { get; set; }
    }
}