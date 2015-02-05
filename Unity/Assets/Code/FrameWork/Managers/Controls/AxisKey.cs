using UnityEngine;
using UnityEditor;
using XboxCtrlrInput;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class AxisKey
{
    #region Fields

    public ControlKeyType Type;
    public List<string> keys = new List<string>();
    public XboxAxisType xboxAxisType;
    //private HorVert horVert;

    [SerializeField,HideInInspector]
    private int selectedIndex1;
    [SerializeField, HideInInspector]
    private int selectedIndex2;

    #endregion

    #region Enums

    public enum HorVert
    {
        Horizontal,
        Vertical
    }

    public enum XboxAxisType
    {
        dpad,
        axis
    }

    #endregion

    #region Creates

    public static AxisKey XboxAxis(XboxAxis axis)
    {
        AxisKey ak = new AxisKey();
        ak.Type = ControlKeyType.Xbox;
        ak.xboxAxisType = XboxAxisType.axis;
        ak.keys.Add(axis.ToString());

        ak.changed();

        return ak;
    }

    public static AxisKey XboxDpad(HorVert horintalOrVertical)
    {
        AxisKey ak = new AxisKey();
        ak.Type = ControlKeyType.Xbox;
        ak.xboxAxisType = XboxAxisType.dpad;
        
        if(horintalOrVertical == HorVert.Horizontal)
        {
            ak.keys.Add(XboxDPad.Left.ToString());
            ak.keys.Add(XboxDPad.Right.ToString());
        }
        else
        {
            ak.keys.Add(XboxDPad.Down.ToString());
            ak.keys.Add(XboxDPad.Up.ToString());
        }

        ak.changed();

        return ak;
    }

    public static AxisKey PC(KeyCode neg, KeyCode pos)
    {
        AxisKey ak = new AxisKey();
        ak.Type = ControlKeyType.PC;

        ak.keys.Add(neg.ToString());
        ak.keys.Add(pos.ToString());

        ak.changed();

        return ak;
    }

    #endregion

    #region Value

    public float Value(int xboxController)
    {
        float v = 0;
        switch (Type)
        {
            case ControlKeyType.PC:
                if(Input.GetKey(ControlHelper.ReturnKeyCode(keys[0])))
                    v--;
                if(Input.GetKey(ControlHelper.ReturnKeyCode(keys[1])))
                    v++;
                break;

            case ControlKeyType.Xbox:
                if (xboxAxisType == XboxAxisType.axis)
                    v = XCI.GetAxis(ControlHelper.ReturnXboxAxis(keys[0]),xboxController);
                else
                {
                    if (XCI.GetDPad(ControlHelper.ReturnXboxDPad(keys[0]), xboxController))
                        v--;
                    if (XCI.GetDPad(ControlHelper.ReturnXboxDPad(keys[1]), xboxController))
                        v++;
                }

                break;
            default:
                return 0;
        }
        return v;
    }

    #endregion

    #region GUI

    public void OnGui()
    {
        //EditorGUILayout.BeginHorizontal();

        Type = (ControlKeyType)EditorGUILayout.EnumPopup(Type, GUILayout.Width(50.0f));
        if (GUI.changed)
            changed();

        switch(Type)
        {
            case ControlKeyType.PC:
                pcGui();
                break;

            case ControlKeyType.Xbox:
                xboxGui();
                break;
        }
       
        //EditorGUILayout.EndHorizontal();
    }

    private void xboxGui()
    {
        xboxAxisType = (XboxAxisType)EditorGUILayout.EnumPopup(xboxAxisType, GUILayout.Width(40.0f));
        if (GUI.changed)
            changed();

        switch(xboxAxisType)
        {
            case XboxAxisType.axis:
                xboxAxisGUI();
                break;
            case XboxAxisType.dpad:
                xboxDpadGUI();
                break;
        }
    }

    private void changed()
    {
        if(keys.Count ==0)
            return;

        switch (Type)
        {
            case ControlKeyType.PC:
                selectedIndex1 = Enum.GetNames(typeof(KeyCode)).ToList().FindIndex(e => e == keys[0]);
                selectedIndex2 = Enum.GetNames(typeof(KeyCode)).ToList().FindIndex(e => e == keys[1]);
                break;

            case ControlKeyType.Xbox:
                switch (xboxAxisType)
                {
                    case XboxAxisType.axis:
                        selectedIndex1 = (int)ControlHelper.ReturnXboxAxis(keys[0]);
                        break;
                    case XboxAxisType.dpad:
                        selectedIndex1 = (int)ControlHelper.ReturnXboxDPad(keys[0]);
                        selectedIndex2 = (int)ControlHelper.ReturnXboxDPad(keys[1]);
                        break;
                }
                break;
        }
        
    }

    private void xboxDpadGUI()
    {
        while(keys.Count < 2)
            keys.Add(ControlHelper.DPadOptions[0]);

        EditorGUILayout.LabelField("-", GUILayout.Width(15.0f));
        selectedIndex1 = EditorGUILayout.Popup(selectedIndex1, ControlHelper.DPadOptions, GUILayout.Width(60.0f));
        keys[0] = ControlHelper.DPadOptions[selectedIndex1];

        GUILayout.Space(20.0f);

        EditorGUILayout.LabelField("+", GUILayout.Width(15.0f));
        selectedIndex2 = EditorGUILayout.Popup(selectedIndex2, ControlHelper.DPadOptions, GUILayout.Width(60.0f));
        keys[1] = ControlHelper.DPadOptions[selectedIndex2];
    }

    private void xboxAxisGUI()
    {
        while (keys.Count < 1)
            keys.Add(ControlHelper.XboxAxixOptions[0]);
        GUILayout.Space(20.0f);
        //EditorGUILayout.LabelField("Axis", GUILayout.Width(20.0f));
        selectedIndex1 = EditorGUILayout.Popup(selectedIndex1, ControlHelper.XboxAxixOptions, GUILayout.Width(80.0f));
        keys[0] = ControlHelper.XboxAxixOptions[selectedIndex1];
    }

    private void pcGui()
    {
        while (keys.Count < 1)
            keys.Add(ControlHelper.KeyCodeOptions[0]);

        GUILayout.Space(45.0f);

        EditorGUILayout.LabelField("-", GUILayout.Width(15.0f));
        selectedIndex1 = EditorGUILayout.Popup(selectedIndex1, ControlHelper.KeyCodeOptions, GUILayout.Width(80.0f));
        keys[0] = ControlHelper.KeyCodeOptions[selectedIndex1];
        EditorGUILayout.LabelField("+", GUILayout.Width(15.0f));
        selectedIndex2 = EditorGUILayout.Popup(selectedIndex2, ControlHelper.KeyCodeOptions, GUILayout.Width(80.0f));
        keys[1] = ControlHelper.KeyCodeOptions[selectedIndex2];
    }

    #endregion
}
