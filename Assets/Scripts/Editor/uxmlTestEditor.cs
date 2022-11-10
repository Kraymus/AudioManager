using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(uxmlTest))]
public class uxmlTestEditor : Editor
{
    public VisualTreeAsset visualTreeAsset;
    private VisualElement uxmlElement;
    private bool showUxml = false;

    private Button toggleUxmlButton { get; set; }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();

        myInspector.Add(new Label("This is a custom inspector"));

        uxmlElement = new VisualElement();
        visualTreeAsset.CloneTree(uxmlElement);
        UpdateUxmlDisplay();
        myInspector.Add(uxmlElement);

        toggleUxmlButton = new Button(ToggleUxml);
        toggleUxmlButton.text = "Toggle Uxml";
        myInspector.Add(toggleUxmlButton);

        return myInspector;
    }

    private void ToggleUxml()
    {
        showUxml = !showUxml;
        Debug.Log("Toggled");
        UpdateUxmlDisplay();
    }

    private void UpdateUxmlDisplay()
    {
        uxmlElement.style.display = showUxml ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
