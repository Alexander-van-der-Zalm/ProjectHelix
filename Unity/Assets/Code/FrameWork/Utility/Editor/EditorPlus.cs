using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class EditorPlus:Editor
{
    private static List<Type> ValidTypes;
    private static string[] ValidTypeStrings;

    #region SavedFoldout

    /// <summary>
    /// Uses EditorPrefs to save and load fold boolean data
    /// </summary>
    internal static bool SavedFoldoutInstance(GUIContent name, UnityEngine.Object target, int index = 0, string uniqueID = "")
    {
        string uniqueString = GetUniqueString(name.text, target, index, uniqueID);

        bool fold = EditorPrefs.GetBool(uniqueString, false);
        fold = EditorGUILayout.Foldout(fold, name);
        
        if (GUI.changed)
            EditorPrefs.SetBool(uniqueString, fold);
        
        return fold;
    }

    /// <summary>
    /// Uses EditorPrefs to save and load fold boolean data
    /// </summary>
    internal static bool SavedFoldoutShared(string name, int index = -1, string uniqueID = "")
    {
        return SavedFoldoutInstance(new GUIContent(name, ""), null, index, uniqueID);
    }



    private static string GetUniqueString(string name, UnityEngine.Object target, int index, string uniqueID = "")
    {
        // NEEDS REVAMP
        return "Fold: " + (target != null ? target.GetInstanceID().ToString() : "") + " - " + uniqueID + index;
    }

    /// <summary>
    /// Uses EditorPrefs to save and load fold boolean data
    /// </summary>
    internal static bool SavedFoldoutInstance(GUIContent name, Rect rect, UnityEngine.Object target, int index = 0, string uniqueID = "")
    {
        string uniqueString = GetUniqueString(name.text, target, index, uniqueID);
        
        bool fold = EditorPrefs.GetBool(uniqueString, false);
        fold = EditorGUI.Foldout(rect, fold, name);

        if (GUI.changed)
            EditorPrefs.SetBool(uniqueString, fold);

        return fold;
    }

    /// <summary>
    /// Uses EditorPrefs to save and load fold boolean data
    /// </summary>
    internal static bool SavedFoldoutShared(string name, Rect rect, int index = 0, string uniqueID = "")
    {
        return SavedFoldoutInstance(new GUIContent(name, ""), rect, null, index, uniqueID);
    }

    internal static void RemoveSavedFoldout(string uniqueID, UnityEngine.Object target, int index = 0)
    {
        string baseString = "Fold: " + target.GetInstanceID().ToString() + " - " + uniqueID;
        string curString = baseString + index;
        string nextString = baseString + ++index;
        
        while (EditorPrefs.HasKey(nextString))
        {
            bool nextBool = EditorPrefs.GetBool(nextString);
            EditorPrefs.SetBool(curString, nextBool);
            nextString = baseString + ++index;
        }
    }

    #endregion

    #region Draw Every/Alot of Types

    protected static object EditorField(object value, string label = "", bool labelField = false, bool isVariableObject = false, params GUILayoutOption[] options)//, GUIContent glabel)
    {
        // Possibly make nice with http://docs.unity3d.com/ScriptReference/GUILayout.FlexibleSpace.html
        string type;
        object returnvalue = value;
        FixedWidthLabel fixedWidth = null;

        if (ValidTypeStrings==null)
            PopulateTypeArrays();

        // Early case if empty
        if (value == null && !isVariableObject)
        {
            EditorGUILayout.LabelField(label, "null", options);
            return value;
        }

        #region Handle variable object

        if (isVariableObject)
        {
            // Get index from type
            int index = value == null ? 0 : ValidTypes.FindIndex(t => t == value.GetType());

            fixedWidth = new FixedWidthLabel(label);
            float popUpWidht = GUI.skin.label.CalcSize(new GUIContent(ValidTypeStrings[index])).x + 10;
            index = EditorGUILayout.Popup("", index, ValidTypeStrings, GUILayout.Width(popUpWidht));
            
            label = "";

            type = ValidTypeStrings[index];
            Type selectedType = ValidTypes[index];

            #region set Default type
            if (value == null || value.GetType() != selectedType)
            {
                if (selectedType.IsValueType)
                {
                    value = Activator.CreateInstance(selectedType);
                }
                else if(selectedType == typeof(string))
                {
                    value = "";
                }
                else
                {
                    EditorGUILayout.LabelField(label, "null", options);
                    return value;
                }
            }
            #endregion
        }
        else
            type = value.GetType().ToString();

        #endregion

        if (labelField)
        {
            EditorGUILayout.LabelField(label, value.ToString(), options);
            return value;
        }

        // AutoHandle by type
        switch (type)
        {
            case "UnityEngine.Vector4":
                returnvalue = EditorGUILayout.Vector4Field(label, (Vector4)value, options);
                break;
            case "UnityEngine.Vector3":
                returnvalue = EditorGUILayout.Vector3Field(label, (Vector3)value, options);
                break;
            case "UnityEngine.Vector2":
                returnvalue = EditorGUILayout.Vector2Field(label, (Vector2)value, options);
                break;
            case "System.Single":
                returnvalue = EditorGUILayout.FloatField(label, (float)value, options);
                break;
            case "System.Int32":
                returnvalue = EditorGUILayout.IntField(label, (int)value, options);
                break;
            case "System.Boolean":
                returnvalue = EditorGUILayout.Toggle(label, (bool)value, options);
                break;
            case "System.String":
                returnvalue = EditorGUILayout.TextField(label, (string)value, options);
                break;
            //case "AI_AgentParameter":
            //    returnvalue = AI_AgentBlackBoardAccessParameterDrawer.CustomDraw(label,(AI_AgentParameter)value);
            //    break;
            default:
                EditorGUILayout.LabelField(label, value.ToString() + " - undefined", options);
                Debug.Log("EditorPlus.EditorField does not contain definition for " + value.GetType().ToString());
                break;

        }

        if (fixedWidth != null)
        {
            fixedWidth.Dispose();
        }
            //EditorGUILayout.EndHorizontal();

        return returnvalue;
    }

    private static void PopulateTypeArrays()
    {
        ValidTypes = new List<Type>() { typeof(int), typeof(float), typeof(bool), typeof(string), typeof(UnityEngine.Vector2), typeof(UnityEngine.Vector3), typeof(UnityEngine.Vector4) };
        var strings = from t in ValidTypes
                      select t.ToString();
        ValidTypeStrings = strings.ToArray();
    }

    #endregion
}
