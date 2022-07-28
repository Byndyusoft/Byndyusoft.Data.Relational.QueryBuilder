using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Consts;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Dtos;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories
{
    public class CompanyReportDtoRepository : DbSessionConsumer
    {
        public CompanyReportDtoRepository(IDbSessionAccessor sessionAccessor) : base(sessionAccessor)
        {
        }

        public async Task<CompanyReportDto[]> GetAsync(CancellationToken cancellationToken)
        {
            var queryObject = new SelectQuery().Build();
            var companyReportDtos = await DbSession.QueryAsync<CompanyReportDto>(queryObject, cancellationToken: cancellationToken);
            return companyReportDtos.ToArray();
        }

        public class SelectQuery : SelectQueryBuilderBase<SelectQuery>
        {
            public SelectQuery()
            {
                GroupBy.For<Company>(Aliases.Company).Add(i => i.Name);
                OrderBy.Add<Company, string>(i => i.Name, isDescending: false, Aliases.Company);
            }

            protected override void PrepareFrom()
            {
                FromCollector
                    .From<Company>(TableNames.Company, Aliases.Company)
                    .LeftJoin<User>(TableNames.Users, Aliases.Users, (c, u) => $"{c.Id} = {u.CompanyId}");
            }

            protected override void PrepareSelect()
            {
                SelectCollector.To<CompanyReportDto>()
                    .Get<User, long>(
                        dto => dto.UserCount,
                        u => $"SUM(CASE WHEN {u.Id} IS NULL THEN 0 ELSE 1 END)",
                        Aliases.Users)
                    .From<Company>(Aliases.Company)
                        .Get(c => c.Name, dto => dto.CompanyName);
            }
        }
    }
}
