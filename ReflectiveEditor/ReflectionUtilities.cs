using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReflectionUtilities
{
    public static List<Type> GetInheritancesOfType<T>()
    {
        List<Type> toReturn = new List<Type>();
        Type[] allTypes = Assembly.GetAssembly(typeof(T)).GetTypes();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsSubclassOf(typeof(T)))
            {
                toReturn.Add(allTypes[i]);
            }
        }

        return toReturn;
    }

    public static T GetAttributeOfType<T>(this MemberInfo obj) where T : Attribute
    {
        if(Attribute.IsDefined(obj, typeof(T)))
        {
            return (T)Attribute.GetCustomAttribute(obj, typeof(T));
        }

        return null;
    }
}
