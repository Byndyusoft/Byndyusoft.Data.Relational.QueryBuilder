using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities
{
    public class Company : IEntity
    {
        public string Name { get; set; } = default!;

        public string Inn { get; set; } = default!;
        public long Id { get; set; }
    }
}