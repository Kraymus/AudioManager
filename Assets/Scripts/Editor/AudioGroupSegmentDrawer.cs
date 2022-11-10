using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AudioGroupSegment))]
public class AudioGroupSegmentDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        container.Add(new PropertyField(property.FindPropertyRelative("audioClip")));
        container.Add(new PropertyField(property.FindPropertyRelative("volume")));
        container.Add(new PropertyField(property.FindPropertyRelative("pitch")));
        container.Add(new PropertyField(property.FindPropertyRelative("randomPitch")));
        container.Add(new PropertyField(property.FindPropertyRelative("randomVolume")));
        container.Add(new PropertyField(property.FindPropertyRelative("weight")));

        return container;
    }
}
