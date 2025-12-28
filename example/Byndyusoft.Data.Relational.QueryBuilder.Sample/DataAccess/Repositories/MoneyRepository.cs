using System.Linq;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Consts;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Update;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories
{
    public class MoneyRepository : DbSessionConsumer
    {
        public MoneyRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
        {
        }

        public async Task InsertAsync(Money money, CancellationToken cancellationToken)
        {
            var queryObject = InsertQueryBuilder<Money>
                .For(money, TableNames.Money)
                .InsertAllPublicValues()
                .Build();
            var id = await DbSession.ExecuteScalarAsync<long>(queryObject, cancellationToken: cancellationToken);
            money.Id = id;
        }

        public async Task UpdateAsync(Money money, CancellationToken cancellationToken)
        {
            var queryObject = UpdateItemQueryBuilder<Money>
                .For(money, TableNames.Money)
                .UpdateAllPublicValues()
                .ById()
                .Build();
            await DbSession.ExecuteAsync(queryObject, cancellationToken: cancellationToken);
        }

        public async Task<Money?> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery()
                .ById(id)
                .Build();
            return await DbSession.QuerySingleOrDefaultAsync<Money>(queryObject, cancellationToken: cancellationToken);
        }

        public async Task<Money[]> GetByCurrencyAsync(Currency currency, CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery()
                .ByCurrency(currency)
                .Build();
            var money = await DbSession.QueryAsync<Money>(queryObject, cancellationToken: cancellationToken);
            return money.ToArray();
        }

        public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
        {
            protected override void PrepareSelect()
            {
                SelectCollector.To<Money>(Aliases.Money).GetAllPublicValues();
            }

            protected override void PrepareFrom()
            {
                FromCollector.From<Money>(TableNames.Money, Aliases.Money);
            }

            public SelectQuery ById(long id)
            {
                Conditionals.For<Money>(Aliases.Money).ById(id);
                return this;
            }

            public SelectQuery ByCurrency(Currency currency)
            {
                Conditionals.For<Money>(Aliases.Money).AddEquals(x => x.Currency, currency);
                return this;
            }
        }
    }
}