using System;
using System.Collections.Generic;

namespace Byndyusoft.Data.Relational.QueryBuilder.QueryObjectBuilders.Infrastructure.PropertyTransformers
{
    public class PropertyTransformer
    {
        private readonly Dictionary<string, PropertyTransformerItem> _items =
            new Dictionary<string, PropertyTransformerItem>();

        private PropertyTransformer()
        {
        }

        public static PropertyTransformerBuilder<TFrom> From<TFrom>(string? tableAlias = null)
        {
            var transformer = new PropertyTransformer();
            return PropertyTransformerBuilder<TFrom>.Create(transformer, tableAlias);
        }

        public void AddItem(string propertyFrom, string propertyTo, string? tableAlias)
        {
            if (string.IsNullOrEmpty(propertyFrom))
                throw new ArgumentNullException(nameof(propertyFrom));

            if (string.IsNullOrEmpty(propertyTo))
                throw new ArgumentNullException(nameof(propertyTo));

            if (_items.ContainsKey(propertyFrom))
                throw new ArgumentException($"Property {propertyFrom} is already added", nameof(propertyFrom));

            var item = new PropertyTransformerItem(propertyFrom, propertyTo, tableAlias);
            _items.Add(propertyFrom, item);
        }

        public PropertyTransformerItem? GetItem(string propertyFrom)
        {
            if (_items.TryGetValue(propertyFrom, out var item) == false)
                return null;

            return item;
        }
    }
}