using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using XboxCtrlrInput;

// Make generic
[Serializable]
public class ControlScheme:EasyScriptableObject<ControlScheme>// : MonoBehaviour 
{
    public enum UpdateTypeE
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    #region Fields

    public string Name;
    public int controllerID = 1;
    public int playerID = 1;

    public UpdateTypeE UpdateType = UpdateTypeE.FixedUpdate;

    public ControlKeyType InputType = ControlKeyType.PC;

    public Axis Horizontal;
    public Axis Vertical;

    [SerializeField]
    public List<Action> Actions = new List<Action>();

    public bool XboxSupport { get { return Horizontal.AxisKeys.Any(k => k.Type == ControlKeyType.Xbox) || Vertical.AxisKeys.Any(k => k.Type == ControlKeyType.Xbox) || Actions.Any(a => a.Keys.Any(k => k.Type == ControlKeyType.Xbox)); } }

    #endregion

    public static ControlScheme CreateScheme<T>(UpdateTypeE updateType = UpdateTypeE.FixedUpdate, bool xboxLeftStick = true, bool xboxDPad = true, bool arrows = true, bool wasd = true) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        ControlScheme controlScheme = ControlScheme.Create();
        controlScheme.Name = typeof(T).ToString();
        controlScheme.UpdateType = updateType;
        controlScheme.SetActionsFromEnum<T>();

        controlScheme.Horizontal = new Axis(controlScheme, "Horizontal");
        controlScheme.Vertical = new Axis(controlScheme, "Vertical");

        if (xboxLeftStick)
        {
            controlScheme.Horizontal.AxisKeys.Add(AxisKey.XboxAxis(XboxCtrlrInput.XboxAxis.LeftStickX));
            controlScheme.Vertical.AxisKeys.Add(AxisKey.XboxAxis(XboxCtrlrInput.XboxAxis.LeftStickY));
        }
        if (xboxDPad)
        {
            controlScheme.Horizontal.AxisKeys.Add(AxisKey.XboxDpad(AxisKey.HorVert.Horizontal));
            controlScheme.Vertical.AxisKeys.Add(AxisKey.XboxDpad(AxisKey.HorVert.Vertical));
        }
        if (wasd)
        {
            controlScheme.Horizontal.AxisKeys.Add(AxisKey.PC(KeyCode.A, KeyCode.D));
            controlScheme.Vertical.AxisKeys.Add(AxisKey.PC(KeyCode.S, KeyCode.W));
        }
        if (arrows)
        {
            controlScheme.Horizontal.AxisKeys.Add(AxisKey.PC(KeyCode.LeftArrow, KeyCode.RightArrow));
            controlScheme.Vertical.AxisKeys.Add(AxisKey.PC(KeyCode.DownArrow, KeyCode.UpArrow));
        }

        return controlScheme;
    }

    public void SetActionsFromEnum<T>() where T : struct, IConvertible
    {
        SetActionsFromEnum(typeof(T));
    }

    public void SetActionsFromEnum(Type type)
    {
        if (!type.IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        Actions = new List<Action>();

        string[] names = Enum.GetNames(type);

        for (int i = 0; i < names.Length; i++)
        {
            Actions.Add(new Action(this, names[i]));
        }
    }

    public void Update()
    {
        foreach (Action action in Actions)
        {
            action.Update(this);
        }
    }

    
}
