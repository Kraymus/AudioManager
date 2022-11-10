using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FirstClass))]
public class FirstClassEditor : Editor
{
    private FirstClass castedTarget => (FirstClass)target;

    private VisualElement root { get; set; }

    private TextField nameTextField { get; set; }
    private IntegerField healthIntegerField { get; set; }

    private Button printFromClassButton { get; set; }
    private Button printFromEditorButton { get; set; }

    private SerializedProperty propertyName { get; set; }
    private SerializedProperty propertyHealth { get; set; }

    public override VisualElement CreateInspectorGUI()
    {
        FindProperties();
        InitializeEditor();
        Compose();
        return root;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void FindProperties()
    {
        propertyName = serializedObject.FindProperty(nameof(FirstClass.Name));
        propertyHealth = serializedObject.FindProperty("Health");
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
        root.style.flexDirection = FlexDirection.Row;

        nameTextField = new TextField();
        nameTextField.BindProperty(propertyName);
        nameTextField.style.flexGrow = 1;
        nameTextField.tooltip = "Name of the character";

        healthIntegerField = new IntegerField();
        healthIntegerField.BindProperty(propertyHealth);
        healthIntegerField.style.flexGrow = 1;
        healthIntegerField.tooltip = "Name of the character";

        printFromClassButton = new Button(castedTarget.PrintInfoFromClass);
        printFromClassButton.text = "Print from class";

        printFromEditorButton = new Button(PrintInfoFromEditor);
        printFromEditorButton.text = "Print from editor";
    }

    public void PrintInfoFromEditor()
    {
        Debug.Log($"[Class] Name: {nameTextField.value} Health: {healthIntegerField.value}");
    }

    private void Compose()
    {
        root.Add(nameTextField);
        root.Add(healthIntegerField);
        root.Add(printFromClassButton);
        root.Add(printFromEditorButton);
    }
}
