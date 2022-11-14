using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kraymus.AudioManager
{
    [CustomPropertyDrawer(typeof(Music))]
    public class MusicDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            TextField nameTextField = new TextField();
            nameTextField.BindProperty(property.FindPropertyRelative("name"));
            nameTextField.RegisterValueChangedCallback(NameChanged);
            container.Add(nameTextField);

            container.Add(new PropertyField(property.FindPropertyRelative("audioClip")));
            container.Add(new PropertyField(property.FindPropertyRelative("volume")));

            return container;
        }

        private void NameChanged(ChangeEvent<string> evt)
        {
            int count = AudioManager.Instance.GetMusicNames().Where(s => s == evt.newValue).Count();
            if (count > 1)
                Debug.LogWarning(evt.newValue + " is a duplicate Music name");
        }
    }
}
