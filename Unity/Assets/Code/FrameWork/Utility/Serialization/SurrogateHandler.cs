using UnityEngine;
using System.Collections;
using System;

public class SurrogateHandler 
{
    public static bool GetSurrogate(ref object surrogate)
    {
        Type objType = surrogate.GetType();

        switch (objType.ToString())
        {
            case "UnityEngine.Vector3":
                surrogate = Vector3Surrogate.GetSurrogateObject((Vector3)surrogate);
                break;
            case "UnityEngine.Vector4":
                surrogate = Vector4Surrogate.GetSurrogateObject((Vector4)surrogate);
                break;
            case "UnityEngine.Vector2":
                surrogate = Vector2Surrogate.GetSurrogateObject((Vector2)surrogate);
                break;
            case "UnityEngine.Quaternion":
                surrogate = QuaternionSurrogate.GetSurrogateObject((Quaternion)surrogate);
                break;
            default:
                return false;
        }

        return true;
    }

    public static bool GetOriginal(ref object original)
    {
        Type objType = original.GetType();

        switch (objType.ToString())
        {
            case "Vector3Surrogate":
                original = GetOriginalObject<Vector3Surrogate>((Vector3Surrogate)original);
                break;
            case "Vector2Surrogate":
                original = GetOriginalObject<Vector2Surrogate>((Vector2Surrogate)original);
                break;
            case "Vector4Surrogate":
                original = GetOriginalObject<Vector4Surrogate>((Vector4Surrogate)original);
                break;
            case "QuaternionSurrogate":
                original = GetOriginalObject<QuaternionSurrogate>((QuaternionSurrogate)original);
                break;
            default:
                return false;
        }

        return true;
    }

    public static object GetOriginalObject<T>(T surrogate) where T : ISerializationSurrogate
    {
        return surrogate.GetOriginalObject();
    }
}
