using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Hadoop.Avro;

namespace Messaging.Transport.Kafka.Avro.ContractResolver
{
    /// <summary>
    /// Allows using standard <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> and 
    /// <see cref="T:System.Runtime.Serialization.DataMemberAttribute"/> attributes for defining what types/properties/fields
    /// should be serialized.
    /// </summary>
    public class AvroDataContractResolver : AvroContractResolver
    {
        private readonly bool _allowNullable;
        private readonly bool _useAlphabeticalOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvroDataContractResolver"/> class.
        /// </summary>
        public AvroDataContractResolver() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvroDataContractResolver"/> class.
        /// </summary>
        /// <param name="allowNullable">If set to <c>true</c>, null values are allowed.</param>
        public AvroDataContractResolver(bool allowNullable)
            : this(allowNullable, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvroDataContractResolver"/> class.
        /// </summary>
        /// <param name="allowNullable">If set to <c>true</c>, null values are allowed.</param>
        /// <param name="useAlphabeticalOrder">If set to <c>true</c> use alphabetical data member order during serialization/deserialization.</param>
        public AvroDataContractResolver(bool allowNullable, bool useAlphabeticalOrder)
        {
            this._allowNullable = allowNullable;
            this._useAlphabeticalOrder = useAlphabeticalOrder;
        }

        /// <summary>
        /// Gets the known types out of an abstract type or interface that could be present in the tree of
        /// objects serialized with this contract resolver.
        /// </summary>
        /// <param name="type">The abstract type.</param>
        /// <returns>
        /// An enumerable of known types.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The type argument is null.</exception>
        public override IEnumerable<Type> GetKnownTypes(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return new HashSet<Type>(type.GetAllKnownTypes());
        }

        /// <summary>
        /// Gets the serialization information about the type.
        /// This information is used for creation of the corresponding schema node.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>
        /// Serialization information about the type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The type argument is null.</exception>
        public override TypeSerializationInfo ResolveType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsUnsupported())
            {
                throw new SerializationException(
                    string.Format(CultureInfo.InvariantCulture, "Type '{0}' is not supported by the resolver.", type));
            }

            bool canContainNull = this._allowNullable && type.CanContainNull();

            if (type.IsInterface() ||
                type.IsNativelySupported() ||
                (type.IsEnum() && !type.GetTypeInfo().GetCustomAttributes(false).OfType<DataContractAttribute>().Any()))
            {
                return new TypeSerializationInfo
                {
                    Name = StripAvroNonCompatibleCharacters(type.Name),
                    Namespace = StripAvroNonCompatibleCharacters(type.Namespace),
                    Nullable = canContainNull
                };
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            var attributes = type.GetTypeInfo().GetCustomAttributes(false);
            var dataContract = attributes.OfType<DataContractAttribute>().SingleOrDefault();
            if (dataContract == null)
            {
                throw new SerializationException(
                    string.Format(CultureInfo.InvariantCulture, "Type '{0}' is not supported by the resolver.", type));
            }

            var name = StripAvroNonCompatibleCharacters(dataContract.Name ?? type.Name);
            var nspace = StripAvroNonCompatibleCharacters(dataContract.Namespace ?? type.Namespace);
            return new TypeSerializationInfo
            {
                Name = name,
                Namespace = nspace,
                Nullable = canContainNull
            };
        }

        /// <summary>
        /// Gets the serialization information about the type members.
        /// This information is used for creation of the corresponding schema nodes.
        /// </summary>
        /// <param name="type">The type, members of which should be serialized.</param>
        /// <returns>
        /// Serialization information about the fields/properties.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The type argument is null.</exception>
        public override MemberSerializationInfo[] ResolveMembers(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsKeyValuePair())
            {
                var keyValueProperties = type.GetAllProperties();
                return keyValueProperties
                    .Select(p => new MemberSerializationInfo
                    {
                        Name = p.Name,
                        MemberInfo = p,
                        Nullable = false
                    })
                    .ToArray();
            }

            var fields = type.GetAllFields();
            var properties = type.GetAllProperties();

            var dataMemberProperties = properties
                .Where(p => p.GetDataMemberAttribute() != null);

            var serializedProperties = Microsoft.Hadoop.Avro.TypeExtensions.RemoveDuplicates(dataMemberProperties);
            Microsoft.Hadoop.Avro.TypeExtensions.CheckPropertyGetters(serializedProperties);

            var members = fields
                .Concat<MemberInfo>(serializedProperties)
                .Select(m => new
                {
                    Member = m,
                    Attribute = m.GetCustomAttributes(false).OfType<DataMemberAttribute>().SingleOrDefault(),
                    Nullable = m.GetCustomAttributes(false).OfType<NullableSchemaAttribute>().Any()
                });

            var result = members.Where(m => m.Attribute != null)
                .Select(m => new MemberSerializationInfo
                {
                    Name = m.Attribute.Name ?? m.Member.Name,
                    MemberInfo = m.Member,
                    Nullable = m.Nullable
                });

            if (this._useAlphabeticalOrder)
            {
                result = result.OrderBy(p => p.Name);
            }

            return result.ToArray();
        }
    }
}