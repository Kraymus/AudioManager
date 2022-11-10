using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioGroupTester))]
public class AudioGroupTesterEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        // If you're running a recent version of the package, or 2021.2, you can use
        InspectorElement.FillDefaultInspector(container, serializedObject, this);

        return container;
    }
}
