using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Transport.Kafka.Avro.ContractResolver
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class NotNullableSchemaAttribute : Attribute
    {
    }
}