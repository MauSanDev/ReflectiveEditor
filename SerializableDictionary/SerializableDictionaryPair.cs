using System;

/// <summary>
/// This class works as a modifiable KeyValuePair for the SerializableDictionary.
/// </summary>
/// <typeparam name="TK"></typeparam>
/// <typeparam name="TV"></typeparam>
public class SerializableDictionaryPair<TK, TV> : ISerializableDictionaryPair
{
    public TK Key;
    public TV Value;

    public SerializableDictionaryPair(TK key, TV value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString() => $"Key: {Key.ToString()} - Value: {Value.ToString()}";

    object ISerializableDictionaryPair.Key
    {
        get => Key;
        set
        {
            if (value is TK parsedKey)
            {
                Key = parsedKey;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }

    object ISerializableDictionaryPair.Value
    {
        get => Value;
        set
        {
            if (value is TV parsedValue)
            {
                Value = parsedValue;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}