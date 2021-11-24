using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Dictionary that allows to modify the Key and Value. Useful for Inspector and Configs where the key can be dynamic.
/// </summary>
[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<SerializableDictionaryPair<TKey, TValue>>, ISerializableDictionary, IDictionaryReflector
{
    //This two lists works as Serializers
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();
    
    //This list contains the dictionary itself at runtime
    protected List<ISerializableDictionaryPair> dictionary = new List<ISerializableDictionaryPair>(); //Contains the list of pairs
    protected Dictionary<TKey, int> positions = new Dictionary<TKey,int>(); //Save the index of the keys to avoid recursion.

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new DuplicateNameException();
        }
        
        positions.Add(key, positions.Count);
        dictionary.Add(new SerializableDictionaryPair<TKey, TValue>(key, value));
    }

    public bool Remove(TKey key)
    {
        if (!ContainsKey(key))
        {
            return false;
        }

        dictionary.RemoveAt(IndexOf(key));
        positions.Remove(key);
        return true;
    }

    public void Clear()
    {
        dictionary.Clear();
        positions.Clear();
    }

    public int IndexOf(TKey key) => positions[key];

    public SerializableDictionaryPair<TKey, TValue> ElementAt(int index)
    {
        if (dictionary.Count <= index)
        {
            throw new IndexOutOfRangeException();
        }

        return dictionary[index] as SerializableDictionaryPair<TKey, TValue>;
    }

    public bool ContainsKey(TKey key) => positions.ContainsKey(key);

    public TValue this[TKey key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException();
            }
            
            return (TValue)dictionary[IndexOf(key)].Value;
        }
        set
        {
            if (ContainsKey(key))
            {
                dictionary[IndexOf(key)].Value = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < this.keys.Count && i < this.values.Count; i++)
        {
            dictionary.Add(new SerializableDictionaryPair<TKey, TValue>(keys[i], values[i]));
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (SerializableDictionaryPair<TKey, TValue> item in dictionary)
        {
            keys.Add(item.Key);
            values.Add(item.Value);
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> toReturn = new Dictionary<TKey, TValue>();
        
        foreach (SerializableDictionaryPair<TKey, TValue> item in dictionary)
        {
            toReturn.Add(item.Key, item.Value);
        }

        return toReturn;
    }

    public IEnumerator<SerializableDictionaryPair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < dictionary.Count; i++)
        {
            yield return dictionary[i] as SerializableDictionaryPair<TKey, TValue>;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<object> Keys 
    {
        get
        {
            List<object> toReturn = new List<object>();

            foreach (SerializableDictionaryPair<TKey, TValue> pair in dictionary)
            {
                toReturn.Add(pair.Key);
            }

            return toReturn;
        }
    }

    public int Count => dictionary.Count;

    public List<object> Values
    {
        get
        {
            List<object> toReturn = new List<object>();

            foreach (SerializableDictionaryPair<TKey, TValue> pair in dictionary)
            {
                toReturn.Add(pair.Value);
            }

            return toReturn;
        }
    }
    
    
    public void Add(object key, object value)
    {
        if (key is TKey parsedKey && value is TValue parsedValue)
        {
            Add(parsedKey, parsedValue);
        }
    }

    public Type KeyType => typeof(TKey);
    public Type ValueType => typeof(TValue);

    public void RemoveAtIndex(int index)
    {
        if (index < dictionary.Count)
        {
            TKey key = (TKey)dictionary[index].Key;
            positions.Remove(key);
            dictionary.RemoveAt(index);
        }
    }

    public List<ISerializableDictionaryPair> Content => dictionary;


    public virtual Dictionary<string, object> Serialize()
    {
        // Dictionary<string, object> toReturn = new Dictionary<string, object>();
        //
        // foreach (SerializableDictionaryPair<TKey, TValue> obj in dictionary)
        // {
        //     toReturn.Add(obj.Key.ToString(), ScriptableConfigBase.ReflectToDictionary(obj.Value, false, BindingFlags.Public | BindingFlags.Instance));
        // }
        //
        // return toReturn;

        return new Dictionary<string, object>();
    }
}