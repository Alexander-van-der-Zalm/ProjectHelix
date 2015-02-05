using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Control 
{
    [SerializeField]
    public string Name;

    [SerializeField]
    protected ControlScheme scheme;

    public Control(ControlScheme scheme, string name = "defaultControlName")
    {
        this.scheme = scheme;
        this.Name = name;
    }
}
