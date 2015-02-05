using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(ControlScheme)),System.Serializable,ExecuteInEditMode]
public class ControlSchemeEditor : EditorPlus 
{
    [SerializeField]
    private List<Type> AllEnums;
    private string[] AllEnumsNames;

    [SerializeField]
    private int selectedIndex;

    private bool xboxSupport;

    string prefID { get { return "ControlSchemeEditor - SelectedEnumIndex[int]:" + target.GetInstanceID().ToString(); } }

    void OnEnable()
    {
        if (AllEnums == null)//Check for assembly reload
        {
            AllEnums = new List<Type>();
            Assembly ass = AssemblyHelper.GetCSharpAssembly();
            
            Type[] types = ass.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsEnum && types[i].Name.ToLower().Contains("action"))
                    AllEnums.Add(types[i]);
            }

            // set name list
            AllEnumsNames = AllEnums.Select(e => e.Name).ToArray();
        }

        selectedIndex = EditorPrefs.GetInt(prefID);

        if (selectedIndex >= AllEnums.Count)
            selectedIndex = 0;

        xboxSupport = ((ControlScheme)(target)).XboxSupport;
    }

    void OnDisable()
    {
        EditorPrefs.SetInt(prefID, selectedIndex);
    }

    public override void OnInspectorGUI()
    {
        #region Original

        if (SavedFoldoutShared("OriginalGUI"))
        {
            EditorGUI.indentLevel++;
            base.OnInspectorGUI();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        #endregion

        //Prep
        ControlScheme ct = (ControlScheme)target;

        EditorGUILayout.Space();

        #region Action selection and setting

        // Set & select a new action list
        EditorGUILayout.BeginHorizontal();
        selectedIndex = EditorGUILayout.Popup(selectedIndex, AllEnumsNames);
        if (GUILayout.Button("ResetActions"))
            ct.SetActionsFromEnum(AllEnums[selectedIndex]);
        EditorGUILayout.EndHorizontal();

        #endregion

        #region PlayerID & Xbox ID
        // PlayerID
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("PlayerID", GUILayout.Width(60.0f));
            ct.playerID = EditorGUILayout.IntField(ct.playerID, GUILayout.Width(20.0f));
            EditorGUILayout.LabelField("XboxSupport: " + ct.XboxSupport.ToString());
        }
        EditorGUILayout.EndHorizontal();

        if (xboxSupport)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("ControllerID", GUILayout.Width(80.0f));
                ct.controllerID = EditorGUILayout.IntField(ct.controllerID, GUILayout.Width(20.0f));
                EditorGUILayout.LabelField("LastInput: " + ct.InputType.ToString());
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Horizontal

        if (SavedFoldoutShared("Horizontal Axis"))
        {
            int delete = -1;
            bool add = false;
            for (int i = 0; i < ct.Horizontal.AxisKeys.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (i == 0 && GUILayout.Button("+", GUILayout.Width(20.0f)))
                {
                    add = true;
                }
                else if (i != 0 && GUILayout.Button("x", GUILayout.Width(20.0f)))
                {
                    delete = i;
                }
                ct.Horizontal.AxisKeys[i].OnGui();
                GUILayout.EndHorizontal();
            }
            if (delete >= 0)
                ct.Horizontal.AxisKeys.RemoveAt(delete);
            if (add)
                ct.Horizontal.AxisKeys.Add(AxisKey.PC(KeyCode.LeftArrow, KeyCode.RightArrow));
        }

        #endregion

        #region Vertical

        if (SavedFoldoutShared("Vertical Axis"))
        {
            int delete = -1;
            bool add = false;
            for (int i = 0; i < ct.Vertical.AxisKeys.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (i == 0 && GUILayout.Button("+", GUILayout.Width(20.0f)))
                {
                    add = true;
                }
                else if (i != 0 && GUILayout.Button("x", GUILayout.Width(20.0f)))
                {
                    delete = i;
                }
                ct.Vertical.AxisKeys[i].OnGui();
                GUILayout.EndHorizontal();
            }
            if (delete >= 0)
                ct.Vertical.AxisKeys.RemoveAt(delete);
            if (add)
                ct.Vertical.AxisKeys.Add(AxisKey.PC(KeyCode.DownArrow, KeyCode.UpArrow));
        }

        #endregion

        #region Actions

        if (SavedFoldoutShared("Actions"))
        {
            for (int i = 0; i < ct.Actions.Count; i++)
            {
                EditorGUI.indentLevel++;
                // For each action - Show a foldout
                if (SavedFoldoutShared(ct.Actions[i].Name))
                {
                    int delete = -1;
                    bool add = false;
                    EditorGUI.indentLevel++;
                    if (ct.Actions[i].Keys.Count == 0 && GUILayout.Button("Add a key"))
                        add = true;

                    for (int j = 0; j < ct.Actions[i].Keys.Count; j++)
                    {
                        GUILayout.BeginHorizontal();

                        if (j == 0 && GUILayout.Button("+", GUILayout.Width(20.0f)))
                        {
                            add = true;
                        }
                        else if (j != 0 && GUILayout.Button("x", GUILayout.Width(20.0f)))
                        {
                            delete = j;
                        }
                        ct.Actions[i].Keys[j].OnGui();

                        GUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                    if (delete >= 0)
                        ct.Actions[i].Keys.RemoveAt(delete);
                    if (add)
                        ct.Actions[i].Keys.Add(ControlKey.PCKey(KeyCode.KeypadEnter));
                }
                EditorGUI.indentLevel--;
            }

        }

        #endregion

        if (GUI.changed)
        {
            xboxSupport = ((ControlScheme)(target)).XboxSupport;
            ScriptableObjectHelper.RefreshAsset(ct);
        }
            
    }
}
