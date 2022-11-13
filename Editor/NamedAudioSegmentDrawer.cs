using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Kraymus.AudioManager
{
    [CustomPropertyDrawer(typeof(NamedAudioSegment))]
    public class NamedAudioSegmentDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            container.Add(new PropertyField(property.FindPropertyRelative("name")));
            container.Add(new PropertyField(property.FindPropertyRelative("audioSegment")));

            return container;
        }
    }
}
