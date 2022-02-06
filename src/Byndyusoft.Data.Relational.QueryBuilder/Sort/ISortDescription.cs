namespace Byndyusoft.Data.Relational.QueryBuilder.Sort
{
    public interface ISortDescription
    {
        string? PropertyName { get; set; }

        SortDirection Direction { get; set; }
    }
}