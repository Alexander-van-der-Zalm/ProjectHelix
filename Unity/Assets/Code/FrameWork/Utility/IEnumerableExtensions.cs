using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class IEnumerableExtensions 
{

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        if (source == null)
            return true;
        return !source.Any();
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new UnassignedReferenceException();
        return !source.Any();
    }
    
}
