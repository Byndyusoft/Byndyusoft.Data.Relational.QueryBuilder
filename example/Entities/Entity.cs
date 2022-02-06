using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;

namespace Byndyusoft.Data.Relational.QueryBuilder.Example.Entities
{
    public class Entity : IEntity
    {
        public string Name { get; set; } = default!;

        public string Birthday { get; set; } = default!;

        public string City { get; set; } = default!;

        public long Id { get; set; }

    }
}