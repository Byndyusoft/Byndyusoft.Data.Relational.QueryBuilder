using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities
{
    public class Company : IEntity
    {
        public long Id { get; set; }

        public string Name { get; set; } = default!;

        public string Inn { get; set; } = default!;
    }
}