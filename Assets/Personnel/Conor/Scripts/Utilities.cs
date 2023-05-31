using System;
using System.Collections.Generic;
using UnityEngine;

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

    public static List<GameObject> FindRoomFloors(GameObject room)
    {
        List<GameObject> childrenMatched = new List<GameObject>();
        Transform t = room.transform;

        for(int i = 0; i < t.childCount; i++)
        {
            if(t.GetChild(i).gameObject.tag == "FloorParent") 
            { 
                Transform cT = t.GetChild(i);

                for(int j = 0; j < cT.childCount; j++)
                {
                    if(cT.GetChild(j).gameObject.tag == "Floor")
                    {
                        childrenMatched.Add(cT.GetChild(j).gameObject);
                    }
                }
            }
        }

        return childrenMatched;
    }

    public static string ConvertFloatToTime(float value)
    {
        int minutes = 0;
        int seconds = 0;
        int milliseconds = (int)((value % 1) * 100);

        while(value >= 1)
        {
            if(seconds + 1 == 60) { seconds = 0; minutes++; }
            else { seconds++; }

            value--;
        }

        string minutesText = minutes >= 10 ? $"{minutes}" : $"0{minutes}";
        string secondsText =  seconds >= 10 ? $"{seconds}" : $"0{seconds}";
        string millisecondsText = milliseconds >= 10 ? $"{milliseconds}" : $"0{milliseconds}";

        return $"{minutesText}:{secondsText}:{millisecondsText}";
    }
}
