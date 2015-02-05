using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using System;

public class ControlHelper 
{
    public static XboxButton ReturnXboxButton(string str)
    {
        return ParseEnum<XboxButton>(str);
    }

    public static XboxAxis ReturnXboxAxis(string str)
    {
        return ParseEnum<XboxAxis>(str);
    }

    public static XboxDPad ReturnXboxDPad(string str)
    {
        return ParseEnum<XboxDPad>(str);
    }

    public static KeyCode ReturnKeyCode(string str)
    {
        return ParseEnum<KeyCode>(str);
    }

    private static T ParseEnum<T>(string value)
    {
        T en = (T)Enum.Parse(typeof(T), value, true);
        if (!Enum.IsDefined(typeof(T), en))
            Debug.LogError("String "+ value+ " is not contained in enumtype:"+  typeof(T).ToString());
        return en;
    }
    [SerializeField]
    private static string[] keyCodeOptions;
    public static string[] KeyCodeOptions { get { return keyCodeOptions != null ? keyCodeOptions : keyCodeOptions = Enum.GetNames(typeof(KeyCode)); } }

    [SerializeField]
    private static string[] dpadOptions;
    public static string[] DPadOptions { get { return dpadOptions != null ? dpadOptions : dpadOptions = Enum.GetNames(typeof(XboxDPad)); } }//new string[] { "left", "right", "up", "down" }; } }

    [SerializeField]
    private static string[] xboxAxixOptions;
    public static string[] XboxAxixOptions { get { return xboxAxixOptions != null ? xboxAxixOptions : xboxAxixOptions = Enum.GetNames(typeof(XboxAxis)); } }

    [SerializeField]
    private static string[] xboxButtonOptions;
    public static string[] XboxButtonOptions { get { return xboxButtonOptions != null ? xboxButtonOptions : xboxButtonOptions = Enum.GetNames(typeof(XboxButton)); } }
}
