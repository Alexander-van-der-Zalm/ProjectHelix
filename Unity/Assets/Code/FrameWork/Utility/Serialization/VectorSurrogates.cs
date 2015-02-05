using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector3Surrogate : ISerializationSurrogate 
{
    [SerializeField]
    private float x, y, z;

    public static object GetSurrogateObject(Vector3 v3)
    {
        return new Vector3Surrogate() { x = v3.x, y = v3.y, z = v3.z };
    }

    public object GetOriginalObject()
    {
        return new Vector3() { x = this.x, y = this.y, z = this.z };
    }

    public override string ToString()
    {
        return "Vector3Surrogate[" + x + "," + y + "," + z + "]";
    }
}

[System.Serializable]
public struct Vector2Surrogate : ISerializationSurrogate
{
    [SerializeField]
    private float x, y;

    public static object GetSurrogateObject(Vector2 original)
    {
        return new Vector2Surrogate() { x = original.x, y = original.y };
    }

    public object GetOriginalObject()
    {
        return new Vector2() { x = this.x, y = this.y};
    }

    public override string ToString()
    {
        return "Vector3Surrogate[" + x + "," + y  + "]";
    }
}

[System.Serializable]
public struct Vector4Surrogate : ISerializationSurrogate
{
    [SerializeField]
    private float x, y, z, w;

    public static object GetSurrogateObject(Vector4 v4)
    {
        return new Vector4Surrogate() { x = v4.x, y = v4.y, z = v4.z, w = v4.w };
    }

    public object GetOriginalObject()
    {
        return new Vector4() { x = this.x, y = this.y, z = this.z, w = this.w };
    }

    public override string ToString()
    {
        return "Vector3Surrogate[" + x + "," + y + "," + z + "," + w + "]";
    }
}