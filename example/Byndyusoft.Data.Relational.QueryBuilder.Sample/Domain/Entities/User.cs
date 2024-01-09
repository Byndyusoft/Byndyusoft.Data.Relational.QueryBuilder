using Byndyusoft.Data.Relational.QueryBuilder.Abstractions;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities
{
    public class User : IEntity
    {
        public string Login { get; set; } = default!;

        public string Password { get; set; } = default!;

        public long CompanyId { get; set; }
        public long Id { get; set; }
    }
}