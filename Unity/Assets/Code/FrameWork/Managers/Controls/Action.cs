using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;

[System.Serializable]
public class Action// : Control 
{
    [SerializeField]
    public string Name;

    [SerializeField]
    protected ControlScheme scheme;

    [SerializeField]
    public List<ControlKey> Keys;// = new List<ControlKey>();

    public Action(ControlScheme scheme, string name = "defaultAction")//:base(scheme, name)
    {
        Keys = new List<ControlKey>();

        this.scheme = scheme;
        this.Name = name;
    }

    public bool IsDown()
    {
        foreach (ControlKey key in Keys)
        {
            if (key.CurState)
                return true;
        }
        return false;
    }

    private bool IsCKDown(ControlKey key)
    {
        bool down = false;
        
        switch (key.Type)
        {
            case ControlKeyType.PC:
                if (Input.GetKey(ControlHelper.ReturnKeyCode(key.KeyValue)))
                    return true;
                break;
            case ControlKeyType.Xbox:
                if (scheme == null)
                {
                    if (XCI.GetButton(ControlHelper.ReturnXboxButton(key.KeyValue)))
                        return true;
                }
                else if (XCI.GetButton(ControlHelper.ReturnXboxButton(key.KeyValue), scheme.controllerID))
                    return true;
                break;
            default:
                Debug.LogError("Action: KeyType not recognized. Please implement the inputType");
                break;
        }

        if (down)
            scheme.InputType = key.Type;
            
        return down;
    }

    public bool IsPressed()
    {
        foreach (ControlKey key in Keys)
        {
            if (key.CurState && !key.LastState)
                return true;
        }
        return false;
    }

    public bool IsReleased()
    {
        foreach (ControlKey key in Keys)
        {
            if (!key.CurState && key.LastState)
                return true;
        }
        return false;
    }

    public void Update(ControlScheme inputScheme = null)
    {
        if (scheme == null && inputScheme != null)
            scheme = inputScheme;

        foreach (ControlKey key in Keys)
        {
            key.LastState   = key.CurState;
            key.CurState    = IsCKDown(key);
            
            if(key.LastState && scheme != null)
                scheme.InputType = key.Type;
        }
    }

}
