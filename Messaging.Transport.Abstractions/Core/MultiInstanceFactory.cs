using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Transport.Abstractions.Core
{
    public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);
}
