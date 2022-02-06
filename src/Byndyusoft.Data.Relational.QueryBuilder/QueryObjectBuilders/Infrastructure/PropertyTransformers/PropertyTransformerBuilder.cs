using Byndyusoft.Data.Relational.QueryBuilder.Extensions;
using System;
using System.Linq.Expressions;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure.PropertyTransformers
{
    public class PropertyTransformerBuilder<TFrom>
    {
        private readonly string? _tableAlias;
        private readonly PropertyTransformer _transformer;

        private PropertyTransformerBuilder(PropertyTransformer transformer, string? tableAlias)
        {
            _transformer = transformer;
            _tableAlias = tableAlias;
        }

        internal static PropertyTransformerBuilder<TFrom> Create(PropertyTransformer transformer,
            string? tableAlias)
        {
            return new PropertyTransformerBuilder<TFrom>(transformer, tableAlias);
        }

        public PropertyTransformerBuilder<TFrom> Add<TProp>(Expression<Func<TFrom, TProp>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var propertyName = property.GetPropertyName();

            _transformer.AddItem(propertyName, propertyName, _tableAlias);
            return this;
        }

        public PropertyTransformerBuilder<TFrom> Add<TProp>(Expression<Func<TFrom, TProp>> property, string toProperty,
            string? toTableAlias)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            _transformer.AddItem(property.GetPropertyName(), toProperty, toTableAlias);
            return this;
        }

        public PropertyTransformerBuilder<TFrom, TTo> To<TTo>(string? tableAlias = null)
        {
            return PropertyTransformerBuilder<TFrom, TTo>.Create(this, tableAlias);
        }

        public PropertyTransformer Build()
        {
            return _transformer;
        }
    }

    public class PropertyTransformerBuilder<TFrom, TTo>
    {
        private readonly string? _tableAlias;
        private readonly PropertyTransformerBuilder<TFrom> _transformerBuilder;

        private PropertyTransformerBuilder(PropertyTransformerBuilder<TFrom> transformerBuilder, string? tableAlias)
        {
            _transformerBuilder = transformerBuilder;
            _tableAlias = tableAlias;
        }

        internal static PropertyTransformerBuilder<TFrom, TTo> Create(
            PropertyTransformerBuilder<TFrom> transformerBuilder, string? tableAlias)
        {
            return new PropertyTransformerBuilder<TFrom, TTo>(transformerBuilder, tableAlias);
        }

        public PropertyTransformerBuilder<TFrom, TTo> Add<TProp>(Expression<Func<TFrom, TProp>> fromProperty,
            Expression<Func<TTo, TProp>> toProperty)
        {
            if (toProperty == null)
                throw new ArgumentNullException(nameof(toProperty));

            _transformerBuilder.Add(fromProperty, toProperty.GetPropertyName(), _tableAlias);
            return this;
        }

        public PropertyTransformerBuilder<TFrom, TOther> To<TOther>(string? tableAlias = null)
        {
            return PropertyTransformerBuilder<TFrom, TOther>.Create(_transformerBuilder, tableAlias);
        }

        public PropertyTransformer Build()
        {
            return _transformerBuilder.Build();
        }
    }
}