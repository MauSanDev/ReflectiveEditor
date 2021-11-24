using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public partial class ReflectiveEditor : EditorWindow
{
    private Dictionary<string, bool> collapsedElements = new Dictionary<string, bool>();

    private void CollapseElement(string path)
    {
        if (collapsedElements.ContainsKey(path))
        {
            collapsedElements[path] = !collapsedElements[path];
        }
        else
        {
            collapsedElements[path] = true;
        }
    }

    public void RegisterCollapsable(string path, bool collapsed = true)
    {
        if (!collapsedElements.ContainsKey(path))
        {
            collapsedElements[path] = collapsed;
        }
    }
    
    public bool IsCollapsed(string path) => collapsedElements.ContainsKey(path) && collapsedElements[path];

    public void DrawCollapsableButton(string path)
    {
        if (GUILayout.Button(GetFoldoutCharacter(path), EditorStyles.label, GUILayout.Width(15)))
        {
            CollapseElement(path);
        }
    }

    private string GetFoldoutCharacter(string path) => IsCollapsed(path) ? "►" : "▼";
    
    protected void DrawLabelButton(string label, Action action = null, int fontSize = 12, FontStyle fontStyle = FontStyle.Normal)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.white },
            fontStyle = fontStyle,
            fontSize = fontSize
        };
        
        if (GUILayout.Button(label, style))
        {
            action?.Invoke();
        }
    }
    
    protected void DrawSizeableButton(string label, Action action, int width, GUIStyle style = null)
    {
        if(GUILayout.Button(label, style ?? EditorStyles.miniButton, GUILayout.Width(width)))
        {
            action.Invoke();
        }
    }
    
    protected void DrawSimpleButton(string label, Action action, GUIStyle style = null)
    {
        if (GUILayout.Button(label, style ?? EditorStyles.miniButton))
        {
            action.Invoke();
        }
    }
    
}
