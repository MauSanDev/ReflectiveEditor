using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

public partial class ReflectiveEditor
{
    private Dictionary<Type, Type> drawerReferences = new Dictionary<Type, Type>();
    private Dictionary<Type, Type> matchedTypes = new Dictionary<Type, Type>();
    private Dictionary<Type, ReflectiveEditorSnippet> drawerInstances = new Dictionary<Type, ReflectiveEditorSnippet>();
    private List<Type> unmatchedTypes = new List<Type>();
    private void SearchSnippetDrawerInheritances()
    {
        List<Type> snippetDrawerInheritances = ReflectionUtilities.GetInheritancesOfType<ReflectiveEditorSnippet>();

        foreach (Type snippetType in snippetDrawerInheritances)
        {
            ReflectiveEditorDrawer attribute = snippetType.GetAttributeOfType<ReflectiveEditorDrawer>();
            if (attribute != null)
            {
                drawerReferences[attribute.Owner] = snippetType;
            }
        }
    }

    private bool TryDrawCustomElement<T>(FieldInfo field, T obj, string fieldName, string parentPath, BindingFlags flags)
    {
        Type objType = field.FieldType;
        Type drawerType = field.FieldType;
        
        if (!drawerReferences.ContainsKey(objType) && !TryGetAssignableType(objType, out drawerType))
        {
            return false;
        }

        if (!drawerInstances.ContainsKey(drawerType))
        {
            ReflectiveEditorSnippet snippetDrawer = (ReflectiveEditorSnippet)Activator.CreateInstance(drawerReferences[drawerType]);
            drawerInstances[drawerType] = snippetDrawer;
        }

        drawerInstances[drawerType].Draw(field, obj, flags, parentPath, this);
        return true;
    }

    private bool TryGetAssignableType(Type objType, out Type assignableType)
    {
        if (unmatchedTypes.Contains(objType))
        {
            assignableType = null;
            return false;
        }
        
        if (matchedTypes.ContainsKey(objType))
        {
            assignableType = matchedTypes[objType];
            return true;
        }
        
        foreach (Type key in drawerReferences.Keys)
        {
            bool isAssignable = typeof(ISerializableDictionary).IsAssignableFrom(objType);
            bool isGenericDefinition = objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>);
            
            if (isAssignable || isGenericDefinition)
            {
                matchedTypes[objType] = key;
                assignableType = key;
                return true;
            }
        }

        assignableType = null;
        return false;
    }

    public void DrawByReflection<T>(T obj, List<Type> attributeTypes = null, Func<FieldInfo, string> fieldNameParser = null, string parentPath = null, BindingFlags flags = BindingFlags.Default | BindingFlags.Instance)
    {
        FieldInfo[] allFields = obj.GetType().GetFields(flags);
        foreach (FieldInfo field in allFields)
        {
            if (attributeTypes != null && attributeTypes.Count > 0)
            {
                foreach (Type attribute in attributeTypes)
                {
                    if(Attribute.IsDefined(field, attribute))
                    {
                        DrawFieldByReflection(field, obj, fieldNameParser, parentPath, flags);
                    }
                }
            }
            else
            {
                DrawFieldByReflection(field, obj, fieldNameParser, parentPath, flags);
            }
        }
    }

    
    public void DrawFieldByReflection<T>(FieldInfo field, T obj, Func<FieldInfo, string> fieldNameParser = null, string parentPath = null, BindingFlags flags = BindingFlags.Default)
    {
        string objectPath = parentPath + field.Name;
        
        string fieldName = fieldNameParser != null ? fieldNameParser(field) : field.Name;

        if (TryDrawPrimitiveField(obj, field, fieldName))
        {
            return;
        }

        if (TryDrawUnityElement(field, obj, fieldName))
        {
            return;
        }
        
        if (TryDrawCustomElement(field, obj, fieldName, objectPath, flags))
        {
            return;
        }

        DrawObjectByReflection(field, obj, flags, objectPath);
    }

    private bool TryDrawUnityElement<T>(FieldInfo field, T obj, string fieldName)
    {
        //Color Field
        if(field.FieldType == typeof(Color))
        {
            field.SetValue(obj, EditorGUILayout.ColorField(fieldName, (Color)field.GetValue(obj)));
            return true;
        }
        
        //Vector2 Field
        if(field.FieldType == typeof(Vector2))
        {
            field.SetValue(obj, EditorGUILayout.Vector2Field(fieldName, (Vector2)field.GetValue(obj)));
            return true;
        }
        
        //Vector2Int Field
        if(field.FieldType == typeof(Vector2Int))
        {
            field.SetValue(obj, EditorGUILayout.Vector2IntField(fieldName, (Vector2Int)field.GetValue(obj)));
            return true;
        }
        
        //Vector3 Field
        if(field.FieldType == typeof(Vector3))
        {
            field.SetValue(obj, EditorGUILayout.Vector3Field(fieldName, (Vector3)field.GetValue(obj)));
            return true;
        }
        
        //Vector3Int Field
        if(field.FieldType == typeof(Vector3Int))
        {
            field.SetValue(obj, EditorGUILayout.Vector3IntField(fieldName, (Vector3Int)field.GetValue(obj)));
            return true;
        }
        
        //Vector3 Field
        if(field.FieldType == typeof(Vector4))
        {
            field.SetValue(obj, EditorGUILayout.Vector4Field(fieldName, (Vector4)field.GetValue(obj)));
            return true;
        }
        
        //Object Field
        if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
        {
            field.SetValue(obj, EditorGUILayout.ObjectField((UnityEngine.Object)field.GetValue(obj), field.FieldType, true));
            return true;
        }

        return false;
    }
    

    protected bool TryDrawPrimitiveField<T>(T obj, FieldInfo field, string fieldName)
    {
        TypeCode typeCode = Type.GetTypeCode(field.FieldType);
        
        switch (typeCode)
        {
            case TypeCode.Int32:
            case TypeCode.Int64:
                if(field.FieldType.BaseType == typeof(Enum))
                {
                    field.SetValue(obj, EditorGUILayout.EnumPopup(fieldName, field.GetValue(obj) as Enum));
                }
                else
                {
                    field.SetValue(obj, EditorGUILayout.IntField(fieldName, Convert.ToInt32(field.GetValue(obj))));
                }
                return true;
            case TypeCode.String:
                field.SetValue(obj, EditorGUILayout.TextField(fieldName, Convert.ToString(field.GetValue(obj))));
                return true;
            case TypeCode.Single:
            case TypeCode.Double:
                field.SetValue(obj, EditorGUILayout.FloatField(fieldName, Convert.ToSingle(field.GetValue(obj))));
                return true;
            case TypeCode.Boolean:
                field.SetValue(obj, EditorGUILayout.Toggle(fieldName, Convert.ToBoolean(field.GetValue(obj))));
                return true;
        }

        return false;
    }
    

    protected void DrawObjectByReflection<T>(FieldInfo field, T obj, BindingFlags flags, string objectPath)
    {
        if (field.GetValue(obj) == null)
        {
            field.SetValue(obj, Activator.CreateInstance(field.FieldType));
        }

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(field.Name, GUILayout.Width(150));
        EditorGUILayout.LabelField($"(Object of type {field.FieldType})",
            new GUIStyle() {fontStyle = FontStyle.Italic, normal = {textColor = Color.gray}});

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("HelpBox");
        DrawByReflection(field.GetValue(obj), parentPath: objectPath, flags: flags);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void DrawSerializableDictionaryByReflection<T>(FieldInfo field, T obj, BindingFlags flags, string objectPath)
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

        RegisterCollapsable(objectPath, false);

        EditorGUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();

        DrawCollapsableButton(objectPath);

        EditorGUILayout.LabelField(field.Name, GUILayout.Width(80));
        EditorGUILayout.LabelField($"(Dictionary of {dictionary.KeyType} - {dictionary.ValueType})",
            new GUIStyle() {fontStyle = FontStyle.Italic, normal = {textColor = Color.gray}});

        if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            dictionary.Add("Element" + (dictionary.Count + 1), Activator.CreateInstance(dictionary.ValueType));
            RegisterCollapsable(objectPath + (dictionary.Count - 1), false);
        }

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        if (IsCollapsed(objectPath))
        {
            return;
        }

        for (int i = 0; i < dictionary.Count; i++)
        {
            string elementPath = objectPath + i;

            RegisterCollapsable(elementPath);

            EditorGUILayout.BeginVertical();

            ISerializableDictionaryPair pair = dictionary.Content[i];

            EditorGUILayout.BeginHorizontal();

            DrawCollapsableButton(elementPath);

            EditorGUILayout.LabelField("Key:", GUILayout.Width(40));
            pair.Key = EditorGUILayout.TextField((string) pair.Key, EditorStyles.label);

            if (GUILayout.Button("Remove", EditorStyles.label, GUILayout.Width(60)))
            {
                dictionary.RemoveAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();

            if (IsCollapsed(elementPath))
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
            DrawByReflection(pair.Value, parentPath: elementPath, flags: flags);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawListByReflection<T>(FieldInfo field, T obj, BindingFlags flags, string objectPath)
    {
        IList list = field.GetValue(obj) as IList;
        Type generic = field.GetValue(obj).GetType().GenericTypeArguments[0];

        RegisterCollapsable(objectPath, false);

        EditorGUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();

        DrawCollapsableButton(objectPath);

        EditorGUILayout.LabelField(field.Name, GUILayout.Width(80));
        EditorGUILayout.LabelField($"(List of {generic})",
            new GUIStyle() {fontStyle = FontStyle.Italic, normal = {textColor = Color.gray}});
        if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            object created = Activator.CreateInstance(generic);
            list.Add(created);
            RegisterCollapsable(objectPath + (list.Count - 1), false);
        }

        EditorGUILayout.EndHorizontal();

        if (IsCollapsed(objectPath))
        {
            EditorGUILayout.EndVertical();
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            string elementPath = objectPath + i;

            RegisterCollapsable(elementPath);

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            DrawCollapsableButton(elementPath);

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

            if (IsCollapsed(elementPath))
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUILayout.BeginVertical("HelpBox");
            DrawByReflection(list[i], parentPath: objectPath, flags: flags);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }
}
