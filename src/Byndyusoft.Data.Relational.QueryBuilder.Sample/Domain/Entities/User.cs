using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities
{
    public class User : IEntity
    {
        public long Id { get; set; }

        public string Login { get; set; } = default!;

        public string Password { get; set; } = default!;

        public long CompanyId { get; set; }
    }
}