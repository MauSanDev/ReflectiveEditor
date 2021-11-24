using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[ReflectiveEditorDrawer(typeof(ISerializableDictionary))]
public class SerializableDictionarySnippetDrawer : ReflectiveEditorSnippet
{
    public override void Draw(FieldInfo field, object obj, BindingFlags flags, string objectPath, ReflectiveEditor editor)
    {
        
        ISerializableDictionary dictionary = field.GetValue(obj) as ISerializableDictionary;

        if (dictionary.KeyType != typeof(string))
        {
            GUI.backgroundColor = Color.red;
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField(
                $"{field.Name} : Serializable Dictionary Key Type is not supported by Extended Editor. Please use string as Key type parameter.");
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            return;
        }

        editor.RegisterCollapsable(objectPath, false);

        EditorGUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();

        editor.DrawCollapsableButton(objectPath);

        EditorGUILayout.LabelField(field.Name, GUILayout.Width(80));
        EditorGUILayout.LabelField($"(Dictionary of {dictionary.KeyType} - {dictionary.ValueType})",
            new GUIStyle() {fontStyle = FontStyle.Italic, normal = {textColor = Color.gray}});

        if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            dictionary.Add("Element" + (dictionary.Count + 1), Activator.CreateInstance(dictionary.ValueType));
            editor.RegisterCollapsable(objectPath + (dictionary.Count - 1), false);
        }

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        if (editor.IsCollapsed(objectPath))
        {
            return;
        }

        for (int i = 0; i < dictionary.Count; i++)
        {
            string elementPath = objectPath + i;

            editor.RegisterCollapsable(elementPath);

            EditorGUILayout.BeginVertical();

            ISerializableDictionaryPair pair = dictionary.Content[i];

            EditorGUILayout.BeginHorizontal();

            editor.DrawCollapsableButton(elementPath);

            EditorGUILayout.LabelField("Key:", GUILayout.Width(40));
            pair.Key = EditorGUILayout.TextField((string) pair.Key, EditorStyles.label);

            if (GUILayout.Button("Remove", EditorStyles.label, GUILayout.Width(60)))
            {
                dictionary.RemoveAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();

            if (editor.IsCollapsed(elementPath))
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUILayout.BeginVertical("HelpBox");

            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField("Value:");
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            EditorGUI.indentLevel++;
            editor.DrawByReflection(pair.Value, parentPath: elementPath, flags: flags);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }
}
