using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using System.IO;

[InitializeOnLoad]
public class SceneContext
{
    static readonly string[] s_SpecialFolders = new string[]
    {
        "Assets/Src/SceneContext/Example"
    };

    static Stopwatch s_ClickClock;
    static Vector3 s_MousePosition;

    static SceneContext()
    {
        s_ClickClock = new Stopwatch();
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView obj)
    {
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                s_ClickClock.Start();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                s_ClickClock.Stop();
                var period = s_ClickClock.ElapsedMilliseconds;
                s_ClickClock.Reset();
                if (period < 300)
                {
                    // TODO calc mouse position

                    // 2d mode
                    var pos = Event.current.mousePosition;
                    float p = EditorGUIUtility.pixelsPerPoint;
                    pos.y = SceneView.lastActiveSceneView.camera.pixelHeight - pos.y * p;
                    pos.x *= p;

                    var ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(pos);

                    s_MousePosition = ray.origin;
                    s_MousePosition.z = 0;
                    OnCentextClick();
                    Event.current.Use();
                }
            }
        }
    }

    private static void OnCentextClick()
    {
        var menu = new GenericMenu();
        foreach (var i in AssetDatabase.FindAssets("t:GameObject", s_SpecialFolders))
        {
            var path = AssetDatabase.GUIDToAssetPath(i);
            var g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var menuPath = Directory.GetParent(path);
            menu.AddItem(new GUIContent($"{menuPath.Name}/Add {g.name} Here."), false, CreateItem, g);
        }
        menu.ShowAsContext();
    }

    private static void CreateItem(object obj)
    {
        var g = obj as GameObject;
        var i = PrefabUtility.InstantiatePrefab(g) as GameObject;
        Undo.RegisterCreatedObjectUndo(i, $"Add {i.name}");
        i.transform.position = s_MousePosition;

        if(Selection.activeTransform != null)
        {
            i.transform.parent = Selection.activeTransform;
        }
        Selection.activeGameObject = i;
    }
}
