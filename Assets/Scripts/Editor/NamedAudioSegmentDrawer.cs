using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(NamedAudioSegment))]
public class NamedAudioSegmentDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        container.Add(new PropertyField(property.FindPropertyRelative("audioName")));
        container.Add(new PropertyField(property.FindPropertyRelative("audioSegment")));

        return container;
    }
}
