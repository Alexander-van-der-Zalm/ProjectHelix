using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Abandoned, untested and not finished!!!
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class ScriptableObjectCollection<T> : EasyScriptableObject<ScriptableObjectCollection<T>>, IList<T> where T : ScriptableObject, IInitSO, IEasyScriptableObject
{
    [SerializeField]
    private List<T> soCollection;

    public bool AddObjectToAsset = false;
    public bool DestroyAsset = false;

    public List<T> Collection
    {
        get { return soCollection; }// == null ? soCollection = new List<T>() : soCollection; }
        private set { soCollection = value; }
    }

    public override void Init(HideFlags newHideFlag = HideFlags.None)
    {
        hideFlags = newHideFlag;
        soCollection = new List<T>();
    }

    #region List Implementation

    public void Add(T newSO)
    {
        // Add to list
        Collection.Add(newSO);
        if (AddObjectToAsset)
        {
            //ScriptableObjectHelper.AddObjectToAsset(this, newSO);
            newSO.AddObjectToAsset(this);
        }
            
    }

    public int IndexOf(T item)
    {
        return Collection.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Collection.Insert(index, item);
        if (AddObjectToAsset)
            //ScriptableObjectHelper.AddObjectToAsset(this, item);
            item.AddObjectToAsset(this);
    }

    public void RemoveAt(int index)
    {
        if (DestroyAsset)
            Collection[index].Destroy();
        
        Collection.RemoveAt(index);
    }

   

    public T this[int index]
    {
        get
        {
            return Collection[index];
        }
        set
        {
            // Remove old?
            if (DestroyAsset)
                Collection[index].Destroy();
            Collection[index] = value;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return Collection.GetEnumerator();
    }


    public void Clear()
    {
        Collection.Clear();
    }

    public bool Contains(T item)
    {
        return Collection.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Collection.CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return Collection.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(T item)
    {
        if (DestroyAsset)
            UnityEngine.Object.DestroyImmediate(this, true);

        return Collection.Remove(item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Collection.GetEnumerator();
    }

    #endregion
}
