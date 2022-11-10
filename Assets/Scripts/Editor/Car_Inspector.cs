using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Car))]
public class Car_Inspector : Editor
{
    public VisualTreeAsset m_InspectorXML;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();

        myInspector.Add(new Label("This is a custom inspector"));

        m_InspectorXML.CloneTree(myInspector);

        return myInspector;
    }
}
