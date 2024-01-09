using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure
{
    public static class ConditionalCollectorExtensions
    {
        public static ConditionalCollectorWrapper<T> ById<T>(this ConditionalCollectorWrapper<T> wrapper, long id)
            where T : IEntity
        {
            return wrapper.AddEquals(i => i.Id, id);
        }
    }
}