using System.Collections;
using System.Collections.Generic;
using System;

public interface ISerializableDictionary : IEnumerable
{
    List<object> Keys { get; }
    List<object> Values { get; }

    int Count { get; }

    void Add(object key, object value);

    Type KeyType { get; }
    Type ValueType { get; }

    void RemoveAtIndex(int index);
    
    List<ISerializableDictionaryPair> Content { get; }
}