using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MsdnSpy.Core.Common
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T>
        where T : ValueType<T>
    {
        public override bool Equals(object obj) => Equals(obj as T);
        public bool Equals(T other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(other, null))
                return false;
            if (GetType() != other.GetType())
                return false;

            return Properties.All(property => Equals(property.Get(this), property.Get(other)));
        }

        public override int GetHashCode()
        {
            if (Hash == null)
            {
                const int startValue = 17;
                const int multiplier = 59;
                Hash = Properties.Aggregate(startValue,
                    (hash, property) => unchecked(hash * multiplier + property.Get(this).GetHashCode()));
            }
            return Hash.Value;
        }

        public override string ToString()
        {
            if (StringRepresentation == null)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(typeof(T).Name)
                    .Append('(');

                var firstProperty = true;
                foreach (var property in Properties)
                {
                    if (!firstProperty)
                        stringBuilder.Append("; ");
                    stringBuilder.Append(property.Info.Name)
                        .Append(": ")
                        .Append(property.Get(this));

                    firstProperty = false;
                }
                stringBuilder.Append(')');

                StringRepresentation = stringBuilder.ToString();
            }
            return StringRepresentation;
        }

        protected int? Hash;
        protected string StringRepresentation;

        protected static readonly IEnumerable<Property> Properties
            = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property =>
                    property.GetMethod != null &&
                    property.SetMethod == null)
                .OrderBy(property => property.Name)
                .Select(property => new Property(property))
                .ToArray();
    }

    public struct Property
    {
        public readonly PropertyInfo Info;
        public readonly Func<object, object> Get;

        public Property(PropertyInfo property)
        {
            Info = property ?? throw new ArgumentNullException(nameof(property));
            Get = InitializeGet(property);
        }

        public override string ToString() => Info.Name;

        private static Func<object, object> InitializeGet(PropertyInfo property)
        {
            var instanceParam = Expression.Parameter(typeof(object));
            return Expression.Lambda<Func<object, object>>
            (
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(instanceParam, property.DeclaringType),
                        property.GetMethod),
                    typeof(object)),
                instanceParam
            ).Compile();
        }
    }
}