using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Kraymus.AudioManager
{
    [CustomPropertyDrawer(typeof(AudioGroupSegment))]
    public class AudioGroupSegmentDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            container.Add(new PropertyField(property.FindPropertyRelative("audioSegment")));
            container.Add(new PropertyField(property.FindPropertyRelative("weight")));

            return container;
        }
    }
}
