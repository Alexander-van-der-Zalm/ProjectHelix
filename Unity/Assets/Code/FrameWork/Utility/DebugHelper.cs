using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DebugHelper 
{

    public static void LogList<T>(List<T> list)
    {
        int i = 0;
        foreach (T t in list)
        {
            Debug.Log(i + "st in list: " + t.ToString());
            i++;
        }
    }

    internal static void LogArray<T>(T[, ,] Group)
    {
        string space = " ";
        for (int i = 0; i < Group.GetLength(0); i++)
        {
            for (int j = 0; j < Group.GetLength(1); j++)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int k = 0; k < Group.GetLength(2); k++)
                {
                    sb.Append(Group[i, j, k].ToString() + space);
                }
                Debug.Log(sb.ToString());
            }
        }
    }
}
