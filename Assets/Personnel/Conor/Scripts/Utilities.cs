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

    public static T[] Remove<T>(this T[] source, T element)
    {
        int index = 0;

        for(int i = 0; i < source.Length; i++)
        {
            if(source[i].Equals(element)) { index = i; break; }
        }

        return source.RemoveAt(index);
    }

    public static T[] Add<T>(this T[] source, T element)
    {
        T[] newArray = new T[source.Length + 1];

        for(int i = 0; i < newArray.Length; i++)
        {
            if(i == source.Length) { newArray[i] = element; }
            else { newArray[i] = source[i]; }
        }

        return newArray;
    }
}
