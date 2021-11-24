  using System;
  using System.Reflection;

/// <summary>
/// Defines a snippet for a custom object to be drawn on Editor GUI.
/// </summary>
public abstract class ReflectiveEditorSnippet
{
    /// <summary>
    /// Draw the custom object snippet using EditorGUILayout methods.
    /// </summary>
    public abstract void Draw(FieldInfo field, object obj, BindingFlags flags, string objectPath, ReflectiveEditor editor);

}
