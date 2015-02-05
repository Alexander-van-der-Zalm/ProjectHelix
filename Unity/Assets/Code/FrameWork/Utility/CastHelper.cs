using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class CastHelper
{
    public static T Cast<T>(object o)
    {
        return (T)o;
    }


    public static object Cast(object obj, Type type)
    {
        try
        {
            MethodInfo castMethod = typeof(CastHelper).GetMethod("Cast").MakeGenericMethod(type);
            return castMethod.Invoke(null, new object[] { obj });
        }
        catch
        {
            return obj;
        }
    }
}
