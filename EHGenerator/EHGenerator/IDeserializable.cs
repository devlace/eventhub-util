using System;
using System.Collections.Generic;
using System.Text;

namespace EHGenerator
{
    public interface IDeserializable<T>
    {
        T deserialize();
    }
}
