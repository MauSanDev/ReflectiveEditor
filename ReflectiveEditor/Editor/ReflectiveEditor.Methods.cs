using UnityEditor;

/// <summary>
/// This partial class contains extension methods that can be used to simplify workflow inside Editor Windows.
/// </summary>
public partial class ReflectiveEditor
{
    protected virtual void OnEnable()
    {
        SearchSnippetDrawerInheritances();
    }

    protected void PingObject(UnityEngine.Object toPing)
    {
        Selection.activeObject = toPing;
        EditorGUIUtility.PingObject(toPing.GetInstanceID());
    }
}
