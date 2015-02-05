using UnityEngine;
using System.Collections;

public interface ISerializationSurrogate
{
    //object GetSurrogateObject(object original);
    object GetOriginalObject();
}
