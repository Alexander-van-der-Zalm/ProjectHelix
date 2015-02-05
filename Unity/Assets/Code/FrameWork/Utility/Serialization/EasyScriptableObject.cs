using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;


public class ScriptableObjectHelper
{
    public static V Create<V>() where V : ScriptableObject, IInitSO
    {
        V so = ScriptableObject.CreateInstance<V>();
        so.Init();
        return so;
    }
    
    public static string SaveAssetAutoNaming(ScriptableObject asset, string path = "Assets", bool unique = true)
    {
        if (!asset.name.Equals(string.Empty))
            path += "/" + asset.name + ".asset";
        else
            path += "/" + asset.GetType().Name + ".asset";

        if (unique)
            path = AssetDatabase.GenerateUniqueAssetPath(path);

        SaveAsset(asset, path);

        Debug.Log("Saved at " + path);

        return path;
    }

    public static void SaveAsset(ScriptableObject asset, string path)
    {
        correctHideFlagsForSaving(asset);

        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
    }

    private static void correctHideFlagsForSaving(ScriptableObject asset)
    {
        //D//ebug.Log(asset.hideFlags.ToString());
        if (asset.hideFlags == HideFlags.DontSave)
            asset.hideFlags = HideFlags.None;

        if (asset.hideFlags == HideFlags.HideAndDontSave)
            asset.hideFlags = HideFlags.HideInHierarchy;
    }

    public static void RefreshAsset(ScriptableObject asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        if (AssetDatabase.IsSubAsset(asset))
            path = AssetDatabase.GetAssetPath(AssetDatabase.LoadMainAssetAtPath(path));

        EditorUtility.SetDirty(asset);
        //AssetDatabase.StartAssetEditing();
        //AssetDatabase.StopAssetEditing();

        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

public class EasyScriptableObject<T> : ScriptableObject, IEasyScriptableObject, IInitSO where T : ScriptableObject, IEasyScriptableObject, IInitSO
{
    public static T Create()
    {
        T obj = ScriptableObject.CreateInstance<T>();
        obj.Init(HideFlags.DontSave);
        return obj;
    }

    //public static T CreateObjAndAsset(string path, HideFlags newHideFlag = HideFlags.DontSave)
    //{
    //    T obj = Create();

    //    obj.CreateAsset(path);

    //    return obj;
    //}

    //public static T CreateObjAddToAsset(string path, HideFlags newHideFlag = HideFlags.DontSave)
    //{
    //    T obj = Create();

    //    obj.AddObjectToAsset(path);
        
    //    return obj;
    //}


    public void AddObjectToAsset(UnityEngine.Object obj)
    {
        // Fail if the object is not an asset
        if (!AssetDatabase.Contains(obj))
        {
            Debug.LogError("EasyScriptableObject.AddObjectToAsset(Object) object " + obj.name + " " + obj.GetType() + " is not an asset");
        }

        string path = AssetDatabase.GetAssetPath(obj);

        //correctHideFlagsForSaving();
        if (hideFlags == HideFlags.DontSave)
            hideFlags = HideFlags.None;

        if (hideFlags == HideFlags.HideAndDontSave)
            hideFlags = HideFlags.HideInHierarchy;

        // Add the object
        AssetDatabase.AddObjectToAsset(this, obj);

        // Refresh/Reimport
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
    }

    public void AddObjectToAsset(string path)
    {
        AssetDatabase.AddObjectToAsset(this, path);
        AssetDatabase.ImportAsset(path);
    }

    //public void Save()
    //{

    //}

    public void Destroy()
    {
        UnityEngine.Object.Destroy(this);
    }

    public void DestroyImmediate()
    {
        UnityEngine.Object.DestroyImmediate(this, true);
    }

    public virtual void Init(HideFlags newHideFlag = HideFlags.None)
    {
        hideFlags = newHideFlag;
    }
}

public interface IEasyScriptableObject
{
    //void CreateAsset(string path);
    void AddObjectToAsset(string path);
    void AddObjectToAsset(UnityEngine.Object obj);
    void Destroy();
    void DestroyImmediate();
    //void Save();
}

public interface IInitSO
{
    void Init(HideFlags newHideFlag = HideFlags.None);
}
#endif