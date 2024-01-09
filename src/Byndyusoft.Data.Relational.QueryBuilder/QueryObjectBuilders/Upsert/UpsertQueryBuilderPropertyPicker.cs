using Byndyusoft.Data.Relational.QueryBuilder.Abstractions.Extensions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Upsert
{
    public class UpsertQueryBuilderPropertyPicker<T> where T : IEntity
    {
        private readonly UpsertQueryBuilder<T> _builder;

        internal UpsertQueryBuilderPropertyPicker(UpsertQueryBuilder<T> builder)
        {
            _builder = builder;
        }

        public UpsertQueryBuilderConflictPropertyPicker<T> UpsertPublicValues()
        {
            _builder.IncludeAllPublicValues(i => i.ExcludeId());
            return new UpsertQueryBuilderConflictPropertyPicker<T>(_builder);
        }
    }
}