using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElementsDropdown<T>
{
    private T[] dropdownElements = null;
    private string[] elementStrings = null;
    private int selectedIndex = 0;

    private string SelectedElementString => elementStrings[selectedIndex];
    public T SelectedElement => dropdownElements[selectedIndex];
    
    public ElementsDropdown(List<T> elements)
    {
        dropdownElements = elements.ToArray();
        elementStrings = new string[dropdownElements.Length];
        for (int i = 0; i < elements.Count; i++)
        {
            elementStrings[i] = elements[i].ToString();
        }
    }
    
    public ElementsDropdown(T[] elements)
    {
        dropdownElements = elements;
        elementStrings = new string[dropdownElements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            elementStrings[i] = elements[i].ToString();
        }
        
    }

    public string DrawGUI(Rect rect, string label, T currentSelection)
    {
        if (currentSelection.Equals(SelectedElement))
        {
            selectedIndex = GetElementIndex(currentSelection);
        }

        selectedIndex = EditorGUI.Popup(rect, label, selectedIndex, elementStrings);

        return SelectedElementString;
    }

    public string DrawGUILayout(string label, T currentSelection)
    {
        if (currentSelection.Equals(SelectedElement))
        {
            selectedIndex = GetElementIndex(currentSelection);
        }

        selectedIndex = EditorGUILayout.Popup(label, selectedIndex, elementStrings);

        return SelectedElementString;
    }

    private int GetElementIndex(T element)
    {
        for (int i = 0; i < dropdownElements.Length; i++)
        {
            if (dropdownElements[i].Equals(element))
            {
                return i;
            }
        }

        return 0;
    }
}