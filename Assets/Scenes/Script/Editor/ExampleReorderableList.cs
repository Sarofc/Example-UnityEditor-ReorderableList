using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(Example.Data))]
public class ExampleDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        position.x += 8;
        position.width -= 8;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("id"));

            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("name"));

            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("gameobject"));

            position.y += EditorGUIUtility.singleLineHeight + 2;
            position.x += 8;
            position.width -= 8;

            var list = property.FindPropertyRelative("nested");

            // draw default
            EditorGUI.PropertyField(position, list, true);

            // customizable
            // list.isExpanded = EditorGUI.Foldout(position, list.isExpanded, list.displayName);
            // if (list.isExpanded)
            // {
            //     position.y += EditorGUIUtility.singleLineHeight + 2;
            //     list.arraySize = EditorGUI.IntField(position, list.arraySize);
            //     for (int i = 0; i < list.arraySize; i++)
            //     {
            //         position.y += EditorGUIUtility.singleLineHeight + 2;
            //         var item = list.GetArrayElementAtIndex(i);
            //         EditorGUI.PropertyField(position, item);
            //     }
            // }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.isExpanded ? (EditorGUIUtility.singleLineHeight + 2) * property.CountInProperty() : EditorGUIUtility.singleLineHeight + 2;
    }
}

[CustomEditor(typeof(Example))]
public class ExampleReorderableList : Editor
{
    private SerializedProperty list;
    ReorderableList m_dateList;
    ReorderableList m_vectorArray;
    private void OnEnable()
    {
        list = serializedObject.FindProperty("m_datas");
        m_dateList = new ReorderableList(serializedObject, list, true, true, true, true);

        //m_orderList.elementHeight = (EditorGUIUtility.singleLineHeight + 2) * 3;

        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Collapse"), false, () =>
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                list.GetArrayElementAtIndex(i).isExpanded = false;
            }
        });

        menu.AddItem(new GUIContent("Expand"), false, () =>
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                list.GetArrayElementAtIndex(i).isExpanded = true;
            }
        });

        var optionsIcon = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");

        m_dateList.drawHeaderCallback = (rect) =>
        {
            EditorGUI.PrefixLabel(rect, new GUIContent(list.displayName));

            rect.x = rect.xMax - optionsIcon.width;
            rect.y += 2;
            if (GUI.Button(rect, new GUIContent(optionsIcon), GUIStyle.none))
            {
                menu.DropDown(rect);
            }
        };

        m_dateList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.GetArrayElementAtIndex(index);
            //rect.height -= 4;
            rect.y += 1;
            EditorGUI.PropertyField(rect, element);
        };

        m_dateList.elementHeightCallback = index =>
        {
            return EditorGUI.GetPropertyHeight(list.GetArrayElementAtIndex(index));
        };


        // m_vectorArray = new ReorderableList(serializedObject, serializedObject.FindProperty("vectorArray"), true, true, true, true);
        m_vectorArray = new ReorderableList(serializedObject, serializedObject.FindProperty("vectorArray"))
        {
            draggable = true,
            displayRemove = true,
            displayAdd = true,

            drawHeaderCallback = rect =>
            {
                rect.y += 2;
                EditorGUI.LabelField(rect, m_vectorArray.serializedProperty.displayName);
            },

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = m_vectorArray.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 1;
                EditorGUI.PropertyField(rect, element);
            },

            elementHeightCallback = index =>
            {
                return EditorGUI.GetPropertyHeight(m_vectorArray.serializedProperty.GetArrayElementAtIndex(index)) + 2;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", target, typeof(MonoScript), false);
        GUI.enabled = true;

        serializedObject.Update();

        m_dateList.DoLayoutList();
        m_vectorArray.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}