using Byndyusoft.Data.Relational.QueryBuilder.Example.Entities;
using Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders;

namespace Byndyusoft.Data.Relational.QueryBuilder.Example.QueryBuilders
{
    public class EntityInsertQuery : InsertQueryBuilder<Entity>
    {
        public EntityInsertQuery(Entity entity)
        : base(entity, "entities", false)
        {

        }
    }

    public class EntitySelectQuery : SelectQueryBuilderBase<EntitySelectQuery>
    {

        private const string Alias = "a";

        protected override void PrepareSelect()
        {
            SelectCollector.To<Entity>(Alias).GetAllPublicValues();
        }

        protected override void PrepareFrom()
        {
            FromCollector.From<Entity>("entities", Alias);
        }

        public EntitySelectQuery ById(long id)
        {
            Conditionals.For<Entity>(Alias).AddEquals(x => x.Id, id);
            return this;
        }

        public EntitySelectQuery ByCity(string city)
        {
            Conditionals.For<Entity>(Alias).AddEquals(x => x.City, city);
            return this;
        }

        public EntitySelectQuery ByBirthday(string birthday)
        {
            Conditionals.For<Entity>(Alias).AddEquals(x => x.Birthday, birthday);
            return this;
        }
    }
}