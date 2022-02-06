using Byndyusoft.Data.Relational.QueryBuilder.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Byndyusoft.Data.Relational.QueryBuilder.Extensions
{
    public static class DbSessionInsertQueryExtensions
    {
        public static async Task InsertAsync<T>(this IDbSession dbSession, T entity, QueryObject queryObject,
            CancellationToken cancellationToken)
            where T : IEntity
        {
            var id = await dbSession.ExecuteScalarAsync<long>(queryObject, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            entity.Id = id;
        }
    }
}