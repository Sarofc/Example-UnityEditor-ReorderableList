using System;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

[CustomEditor(typeof(Readme))]
public class ReadmeEditor : Editor
{
    const string k_ShowedReadmesessionStateName = "ReadmeEditor.showedReadme";

    const float k_Space = 16f;
    private static Readme s_Readme;

    static ReadmeEditor()
    {
        EditorApplication.delayCall += SelectReadmeAuto;
    }

    private static void SelectReadmeAuto()
    {
        if (!SessionState.GetBool(k_ShowedReadmesessionStateName, false))
        {
            s_Readme = SelectReadme();
            SessionState.SetBool(k_ShowedReadmesessionStateName, true);
            if (s_Readme && !s_Readme.loadedLayout)
            {
                LoadLayout();
                s_Readme.loadedLayout = true;
            }
        }
    }

    private static void LoadLayout()
    {
        var assembly = typeof(EditorApplication).Assembly;
        var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
        var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
        method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "/Layout.wlt"), false });
    }

    [MenuItem("Readme/Create Readme")]
    private static void CreateReadme()
    {
        var readme = ScriptableObject.CreateInstance<Readme>();
        AssetDatabase.CreateAsset(readme, "Assets/Readme.asset");
        AssetDatabase.Refresh();
    }

    [MenuItem("Readme/Show Readme")]
    private static Readme SelectReadme()
    {
        var ids = AssetDatabase.FindAssets("Readme t:Readme");

        if (ids.Length == 1)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));
            Selection.objects = new UnityEngine.Object[] { obj };

            return (Readme)obj;
        }
        else
        {
            Debug.Log("can't find a readme.");
            return null;
        }
    }

    protected override void OnHeaderGUI()
    {
        Ensure();

        var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);
        GUILayout.BeginHorizontal("In BigTitle");
        {
            GUILayout.Label(s_Readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            GUILayout.Label(s_Readme.title, s_Styles.titleStyle);
        }
        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        Ensure();

        foreach (var section in s_Readme.sections)
        {
            if (!string.IsNullOrEmpty(section.heading))
            {
                GUILayout.Label(section.heading, s_Styles.headingStyle);
            }
            if (!string.IsNullOrEmpty(section.text))
            {
                GUILayout.Label(section.text, s_Styles.bodyStyle);
            }
            if (!string.IsNullOrEmpty(section.linkText))
            {
                if (LinkLabel(new GUIContent(section.linkText)))
                {
                    Application.OpenURL(section.url);
                }
            }
            GUILayout.Space(k_Space);
        }
    }

    bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
    {
        var position = GUILayoutUtility.GetRect(label, s_Styles.linkStyle, options);
        Handles.BeginGUI();
        Handles.color = s_Styles.linkStyle.normal.textColor;
        Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
        Handles.color = Color.white;
        Handles.EndGUI();

        EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

        return GUI.Button(position, label, s_Styles.linkStyle);
    }

    void Ensure()
    {
        if (s_Readme == null) s_Readme = target as Readme;
        if (s_Styles == null) s_Styles = new Styles();
    }

    static Styles s_Styles;

    class Styles
    {
        public GUIStyle linkStyle, titleStyle, headingStyle, bodyStyle;

        public Styles()
        {
            bodyStyle = new GUIStyle(EditorStyles.label);
            bodyStyle.fontSize = 14;
            bodyStyle.wordWrap = false;

            titleStyle = new GUIStyle(bodyStyle);
            titleStyle.fontSize = 26;

            headingStyle = new GUIStyle(bodyStyle);
            headingStyle.fontSize = 18;

            linkStyle = new GUIStyle(bodyStyle);
            linkStyle.wordWrap = false;
            linkStyle.stretchWidth = false;
            linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
        }
    }
}
