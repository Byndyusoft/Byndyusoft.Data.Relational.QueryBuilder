namespace Byndyusoft.Data.Relational.QueryBuilder.Reflection
{
    public class TypePropertyInfo
    {
        public TypePropertyInfo(string name, object? value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object? Value { get; }
    }
}