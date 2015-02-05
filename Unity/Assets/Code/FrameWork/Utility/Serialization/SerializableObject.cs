using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SerializableObject : ISerializationCallbackReceiver
{
    [NonSerialized]
    private object unserializedObject;

    [SerializeField]
    private byte[] byteArray;

    // To prevent spam of checks for non serializable objects
    private bool unserializable = false;

    public object Object
    {
        get { return unserializedObject; }
        set { unserializedObject = value; unserializable = false; }
    }

    // Deserialize object
    public void OnAfterDeserialize()
    {
        // If nothing was serialized into the byte array, exit out
        if(byteArray.Length == 0)
            return;

        // Deserialize
        var serializer = new BinaryFormatter();
        using (var stream = new MemoryStream(byteArray))
            unserializedObject = serializer.Deserialize(stream);

        // Check if surrogate and replace
        if(unserializedObject.GetType().GetInterfaces().Contains(typeof(ISerializationSurrogate)))
            SurrogateHandler.GetOriginal(ref unserializedObject);

        //Debug.Log("Deserialized Type: " + unserializedObject.GetType() + " | Value: " + unserializedObject.ToString());
    }

    // Serialize object
    public void OnBeforeSerialize()
    {
        // No need to serialize if the object is null
        // or declared nonserializable
        if (unserializable || unserializedObject == null)
            return;

        // Possible to loop over fields for serialization of (unity) non serializables
        // This will prevent this method from blowing up when the serializer hits non serializable fields
        // Possibly store all the fields in a dictionary of some kind? (increased memory usage, but more stable)
        // For now just check one type
        Type objType = unserializedObject.GetType();

        // Check surrogates for non serializable types
        if (!objType.IsSerializable)
        {
            if (!SurrogateHandler.GetSurrogate(ref unserializedObject))
            {
                Debug.Log("SerializableObject.Serialization: " + objType.ToString() + " is not a serializable type and has no surrogate");
                unserializable = true;
                return;
            }
        }

        // Serialize
        using(var stream = new MemoryStream())
        {
            var serializer = new BinaryFormatter();

            serializer.Serialize(stream, unserializedObject);
            byteArray = stream.ToArray();
        }

        //Debug.Log("Serialized Type: " + unserializedObject.GetType() + " | Value: " + unserializedObject.ToString());
    }
}
