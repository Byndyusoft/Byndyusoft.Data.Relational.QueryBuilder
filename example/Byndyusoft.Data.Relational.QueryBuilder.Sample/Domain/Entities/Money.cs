using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities
{
    public class Money : IEntity
    {
        public long Id { get; set; }

        public Currency Currency { get; set; }

        public decimal Value { get; set; }
    }
}