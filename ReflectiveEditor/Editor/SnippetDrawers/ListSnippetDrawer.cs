using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[ReflectiveEditorDrawer(typeof(List<>))]
public class ListSnippetDrawer : ReflectiveEditorSnippet
{
    public override void Draw(FieldInfo field, object obj, BindingFlags flags, string objectPath, ReflectiveEditor editor)
    {
        IList list = field.GetValue(obj) as IList;
        Type generic = field.GetValue(obj).GetType().GenericTypeArguments[0];

        editor.RegisterCollapsable(objectPath, false);

        EditorGUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();

        editor.DrawCollapsableButton(objectPath);

        EditorGUILayout.LabelField(field.Name, GUILayout.Width(80));
        EditorGUILayout.LabelField($"(List of {generic})",
            new GUIStyle() {fontStyle = FontStyle.Italic, normal = {textColor = Color.gray}});
        if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            object created = Activator.CreateInstance(generic);
            list.Add(created);
            editor.RegisterCollapsable(objectPath + (list.Count - 1), false);
        }

        EditorGUILayout.EndHorizontal();

        if (editor.IsCollapsed(objectPath))
        {
            EditorGUILayout.EndVertical();
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            string elementPath = objectPath + i;

            editor.RegisterCollapsable(elementPath);

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            editor.DrawCollapsableButton(elementPath);

            EditorGUILayout.LabelField($"Element {i + 1}", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Italic});
            if (GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                if (i - 1 >= 0)
                {
                    list.Insert(i - 1, list[i]);
                    list.RemoveAt(i + 1);
                    if (list.Count <= i)
                    {
                        continue;
                    }
                }
            }

            if (GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20)))
            {
                if (i + 2 <= list.Count)
                {
                    list.Insert(i + 2, list[i]);
                    list.RemoveAt(i);
                    if (list.Count <= i)
                    {
                        continue;
                    }
                }
            }

            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                list.RemoveAt(i);
                if (list.Count <= i)
                {
                    continue;
                }
            }


            EditorGUILayout.EndHorizontal();

            if (editor.IsCollapsed(elementPath))
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUILayout.BeginVertical("HelpBox");
            editor.DrawByReflection(list[i], parentPath: objectPath, flags: flags);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }
}
