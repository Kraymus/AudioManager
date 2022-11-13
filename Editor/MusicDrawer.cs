using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Kraymus.AudioManager
{
    [CustomPropertyDrawer(typeof(Music))]
    public class MusicDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            container.Add(new PropertyField(property.FindPropertyRelative("name")));
            container.Add(new PropertyField(property.FindPropertyRelative("audioClip")));
            container.Add(new PropertyField(property.FindPropertyRelative("volume")));

            return container;
        }
    }
}
