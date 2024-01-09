using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Delete;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Consts;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories
{
    public class CompanyRepository : DbSessionConsumer
    {
        public CompanyRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
        {
        }

        public async Task<Company?> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery().ById(id).Build();
            return await DbSession.QuerySingleOrDefaultAsync<Company?>(queryObject,
                cancellationToken: cancellationToken);
        }

        public async Task<Company[]> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery().ByName(name).Build();
            var companies = await DbSession.QueryAsync<Company>(queryObject, cancellationToken: cancellationToken);
            return companies.ToArray();
        }

        public async Task InsertAsync(Company company, CancellationToken cancellationToken)
        {
            var queryObject = InsertQueryBuilder<Company>
                .For(company, TableNames.Company)
                .InsertAllPublicValues()
                .Build();
            var id = await DbSession.ExecuteScalarAsync<long>(queryObject, cancellationToken: cancellationToken);
            company.Id = id;
        }

        public async Task UpdateAsync(Company company, CancellationToken cancellationToken)
        {
            var queryObject = UpdateItemQueryBuilder<Company>
                .For(company, TableNames.Company)
                .UpdateAllPublicValues()
                .ById()
                .Build();
            await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
        }

        public async Task UpdateInnAsync(long id, string inn, CancellationToken cancellationToken)
        {
            var queryObject = UpdateQueryBuilder<Company>
                .For(TableNames.Company)
                .Set(i => i.Inn, inn)
                .ById(id)
                .Build();
            await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
        }

        public async Task DeleteByIdAsync(long id, CancellationToken cancellationToken)
        {
            var queryObject = DeleteQueryBuilder<Company>
                .For(TableNames.Company)
                .ById(id)
                .Build();
            await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
        }

        public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
        {
            protected override void PrepareFrom()
            {
                FromCollector.From<Company>(TableNames.Company, Aliases.Company);
            }

            protected override void PrepareSelect()
            {
                SelectCollector.To<Company>(Aliases.Company).GetAllPublicValues();
            }

            public SelectQuery ById(long id)
            {
                Conditionals.For<Company>(Aliases.Company).ById(id);
                return this;
            }

            public SelectQuery ByName(string name)
            {
                Conditionals.For<Company>(Aliases.Company).AddEquals(i => i.Name, name);
                return this;
            }
        }
    }
}