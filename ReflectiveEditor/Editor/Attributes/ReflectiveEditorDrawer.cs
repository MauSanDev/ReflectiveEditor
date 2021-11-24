using System;

[AttributeUsage(AttributeTargets.Class)]
public class ReflectiveEditorDrawer : Attribute
{
    public Type Owner { get; private set; }

    public ReflectiveEditorDrawer(Type owner)
    {
        Owner = owner;
    }
}
