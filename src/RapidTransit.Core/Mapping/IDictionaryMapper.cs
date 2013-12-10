﻿namespace RapidTransit.Core.Mapping
{
    using System.Collections.Generic;


    public interface IDictionaryMapper<in T>
    {
        void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj);
    }
}