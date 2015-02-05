using UnityEngine;
using System.Collections;

[System.Serializable]
public struct QuaternionSurrogate : ISerializationSurrogate
{
    [SerializeField]
    private float x, y, z, w;

    public static object GetSurrogateObject(Quaternion q)
    {
        return new QuaternionSurrogate() { x = q.x, y = q.y, z = q.z, w = q.w };
    }

    public object GetOriginalObject()
    {
        return new QuaternionSurrogate() { x = this.x, y = this.y, z = this.z, w = this.w };
    }

    public override string ToString()
    {
        return "Vector3Surrogate[" + x + "," + y + "," + z + "," + w + "]";
    }
}
