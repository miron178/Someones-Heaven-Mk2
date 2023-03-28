using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;

public static class Utilities
{
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] newArray = new T[source.Length - 1];
        if(index > 0)
        {
            Array.Copy(source, 0, newArray, 0, index);
        }

        if (index < source.Length - 1)
        {
            Array.Copy(source, index + 1, newArray, index, source.Length - index - 1);
        }

        return newArray;
    }
}
