using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Consts;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Dtos;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories
{
    public class UserDtoRepository : DbSessionConsumer
    {
        public UserDtoRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
        {
        }

        public async Task<UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery().ById(id).Build();
            return await DbSession.QuerySingleOrDefaultAsync<UserDto>(queryObject,
                cancellationToken: cancellationToken);
        }

        public async Task<UserDto?> GetByIdAlternativelyAsync(long id, CancellationToken cancellationToken)
        {
            var columnConverter = new ColumnConverter(true);
            var sql = columnConverter.Map<User, Company>((user, company) => $@"
SELECT
    {user.Login} AS {nameof(UserDto.Login)},
    {user.Password} AS {nameof(UserDto.Password)},
    {company.Name} AS {nameof(UserDto.CompanyName)}
FROM
    {TableNames.Users} u
    INNER JOIN {TableNames.Company} c on {company.Id} = {user.CompanyId}
WHERE
    {user.Id} = @UserId", "u", "c");

            var queryObject = new QueryObject(sql, new { UserId = id });
            return await DbSession.QuerySingleOrDefaultAsync<UserDto>(queryObject,
                cancellationToken: cancellationToken);
        }

        public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
        {
            protected override void PrepareFrom()
            {
                FromCollector
                    .From<User>(TableNames.Users, Aliases.Users)
                    .InnerJoin(TableNames.Company, Aliases.Company, i => i.CompanyId);
            }

            protected override void PrepareSelect()
            {
                SelectCollector.To<UserDto>()
                    .From<User>(Aliases.Users)
                    .Get(u => u.Login, dto => dto.Login)
                    .Get(u => u.Password, dto => dto.Password)
                    .Other<Company>(Aliases.Company)
                    .Get(c => c.Name, dto => dto.CompanyName);
            }

            public SelectQuery ById(long id)
            {
                Conditionals.For<User>(Aliases.Users).ById(id);
                return this;
            }
        }
    }
}