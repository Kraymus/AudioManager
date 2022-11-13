using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kraymus.AudioManager
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        private bool enableDeleting = false;
        private VisualElement mainPage;
        private VisualElement subPage;
        private int audioGroupIndex;

        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();

            mainPage = CreateMainPage();
            subPage = CreateSubPage();
            myInspector.Add(mainPage);
            myInspector.Add(subPage);
            GoToMainPage();

            return myInspector;
        }

        public VisualElement CreateMainPage()
        {
            var mainPageElement = new VisualElement();

            SerializedProperty audioGroupsProperty = serializedObject.FindProperty("audioGroups");

            // General Section

            SerializedProperty playerTransformProperty = serializedObject.FindProperty("playerTransform");
            PropertyField playerTransformField = new PropertyField();
            playerTransformField.BindProperty(playerTransformProperty);
            mainPageElement.Add(playerTransformField);

            // Audio Groups Section

            Toggle toggle = new Toggle("Enable Deleting");
            toggle.RegisterValueChangedCallback(OnToggleChanged);
            mainPageElement.Add(toggle);

            Button addAudioGroupButton = new Button(() =>
            {
                audioGroupsProperty.InsertArrayElementAtIndex(audioGroupsProperty.arraySize);
                // Set default values
                audioGroupsProperty.GetArrayElementAtIndex(audioGroupsProperty.arraySize - 1).FindPropertyRelative("name").stringValue = CreateDefaultName(audioGroupsProperty, "Audio Group ");
                audioGroupsProperty.GetArrayElementAtIndex(audioGroupsProperty.arraySize - 1).FindPropertyRelative("volume").floatValue = 1f;
                serializedObject.ApplyModifiedProperties();
            });
            addAudioGroupButton.text = "Add";
            mainPageElement.Add(addAudioGroupButton);

            ListView audioGroupsListView = MakeAudioGroupListView(audioGroupsProperty);
            mainPageElement.Add(audioGroupsListView);

            // Audio Segments Section
            SerializedProperty audioSegmentsProperty = serializedObject.FindProperty("audioSegments");
            Button addAudioSegmentButton = new Button(() =>
            {
                audioSegmentsProperty.InsertArrayElementAtIndex(audioSegmentsProperty.arraySize);
                // Set default values
                audioSegmentsProperty.GetArrayElementAtIndex(audioSegmentsProperty.arraySize - 1).FindPropertyRelative("name").stringValue = CreateDefaultAudioSegmentName(audioSegmentsProperty, "Segment ");
                audioSegmentsProperty.GetArrayElementAtIndex(audioSegmentsProperty.arraySize - 1).FindPropertyRelative("audioSegment").FindPropertyRelative("volume").floatValue = 1f;
                audioSegmentsProperty.GetArrayElementAtIndex(audioSegmentsProperty.arraySize - 1).FindPropertyRelative("audioSegment").FindPropertyRelative("pitch").floatValue = 1f;
                serializedObject.ApplyModifiedProperties();
            });
            addAudioSegmentButton.text = "Add";
            mainPageElement.Add(addAudioSegmentButton);

            ListView audioSegmentsListView = MakeAudioSegmentListView(audioSegmentsProperty);
            mainPageElement.Add(audioSegmentsListView);

            // Music Section
            SerializedProperty musicProperty = serializedObject.FindProperty("music");

            Button addMusicButton = new Button(() =>
            {
                musicProperty.InsertArrayElementAtIndex(musicProperty.arraySize);
                // TODO: Set default values
                musicProperty.GetArrayElementAtIndex(musicProperty.arraySize - 1).FindPropertyRelative("name").stringValue = CreateDefaultName(musicProperty, "Music ");
                musicProperty.GetArrayElementAtIndex(musicProperty.arraySize - 1).FindPropertyRelative("volume").floatValue = 1f;
                serializedObject.ApplyModifiedProperties();
            });
            addMusicButton.text = "Add";
            mainPageElement.Add(addMusicButton);

            Button stopMusicButton = new Button(() =>
            {
                AudioManager.Instance.StopMusicInEditor();
            });
            stopMusicButton.text = "Stop";
            mainPageElement.Add(stopMusicButton);

            ListView musicListView = MakeMusicListView(musicProperty);
            mainPageElement.Add(musicListView);

            return mainPageElement;
        }

        private ListView MakeAudioGroupListView(SerializedProperty audioGroupsProperty)
        {
            Func<VisualElement> makeItem = () =>
            {
                VisualElement element = new VisualElement();

                VisualElement headerRow = new VisualElement();
                headerRow.style.flexDirection = FlexDirection.Row;

                VisualElement buttonRow = new VisualElement();
                buttonRow.style.flexDirection = FlexDirection.Row;

                Label label = new Label();
                label.style.width = 100;
                label.style.overflow = Overflow.Hidden;
                headerRow.Add(label);
                PropertyField propertyField = new PropertyField();
                propertyField.style.flexGrow = 1;
                propertyField.label = "";
                headerRow.Add(propertyField);

                Button playButton = new Button(() =>
                {
                    AudioManager.Instance.PlayInEditor(AudioCategory.Group, audioGroupsProperty.GetArrayElementAtIndex((int)element.userData).FindPropertyRelative("name").stringValue);
                });
                playButton.text = "Play";
                playButton.style.flexGrow = 1;
                buttonRow.Add(playButton);

                Button editButton = new Button(() =>
                {
                    GoToSubPage((int)element.userData);
                });
                editButton.text = "Edit";
                editButton.style.flexGrow = 1;
                buttonRow.Add(editButton);

                Button deleteButton = new Button(() =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning("Can't delete in play mode");
                    }
                    else
                    {
                        if (enableDeleting)
                        {
                            audioGroupsProperty.DeleteArrayElementAtIndex((int)element.userData);
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                });
                deleteButton.text = "Delete";
                deleteButton.style.flexGrow = 1;
                buttonRow.Add(deleteButton);

                element.Add(headerRow);
                element.Add(buttonRow);

                return element;
            };

            Action<VisualElement, int> bindItem = (element, index) =>
            {
                element.userData = index;
                (element.ElementAt(0).ElementAt(0) as Label).text = audioGroupsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
                (element.ElementAt(0).ElementAt(1) as PropertyField).BindProperty(audioGroupsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("volume"));
            };

            return SetListViewSettings(makeItem, bindItem, "Audio Groups", 50, bindingPath: audioGroupsProperty.propertyPath);
        }

        private ListView MakeAudioSegmentListView(SerializedProperty audioSegmentsProperty)
        {
            Func<VisualElement> makeItem = () =>
            {
                VisualElement element = new VisualElement();

                element.Add(new PropertyField());

                Button playButton = new Button(() =>
                {
                    AudioManager.Instance.PlayInEditor(AudioCategory.Segment, audioSegmentsProperty.GetArrayElementAtIndex((int)element.userData).FindPropertyRelative("name").stringValue);
                });
                playButton.text = "Play";
                element.Add(playButton);

                Button deleteButton = new Button(() =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning("Can't delete in play mode");
                    }
                    else
                    {
                        audioSegmentsProperty.DeleteArrayElementAtIndex((int)element.userData);
                        serializedObject.ApplyModifiedProperties();
                    }
                });
                deleteButton.text = "Delete";
                element.Add(deleteButton);

                return element;
            };

            Action<VisualElement, int> bindItem = (element, index) =>
            {
                element.userData = index;
                (element.ElementAt(0) as PropertyField).BindProperty(audioSegmentsProperty.GetArrayElementAtIndex(index));
            };

            return SetListViewSettings(makeItem, bindItem, "Segments", 170, 400, audioSegmentsProperty.propertyPath);
        }

        private ListView MakeMusicListView(SerializedProperty musicProperty)
        {
            Func<VisualElement> makeItem = () =>
            {
                VisualElement element = new VisualElement();

                element.Add(new PropertyField());

                Button playButton = new Button(() =>
                {
                    AudioManager.Instance.PlayInEditor(AudioCategory.Music, musicProperty.GetArrayElementAtIndex((int)element.userData).FindPropertyRelative("name").stringValue);
                });
                playButton.text = "Play";
                element.Add(playButton);

                Button deleteButton = new Button(() =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning("Can't delete in play mode");
                    }
                    else
                    {
                        musicProperty.DeleteArrayElementAtIndex((int)element.userData);
                        serializedObject.ApplyModifiedProperties();
                    }
                });
                deleteButton.text = "Delete";
                element.Add(deleteButton);

                return element;
            };

            Action<VisualElement, int> bindItem = (element, index) =>
            {
                element.userData = index;
                (element.ElementAt(0) as PropertyField).BindProperty(musicProperty.GetArrayElementAtIndex(index));
            };

            return SetListViewSettings(makeItem, bindItem, "Music", 110, 400, musicProperty.propertyPath);
        }


        private static ListView SetListViewSettings(Func<VisualElement> makeItem, Action<VisualElement, int> bindItem, string title, int fixedItemHeight, int width = 0, string bindingPath = "")
        {
            var listView = new ListView();
            if (bindingPath != "")
                listView.bindingPath = bindingPath;
            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.showFoldoutHeader = true;
            listView.showBoundCollectionSize = false;
            listView.headerTitle = title; //listView.headerTitle = "Segments";
            listView.fixedItemHeight = fixedItemHeight; //listView.fixedItemHeight = 170;
            if (width != 0)
                listView.style.width = width; //listView.style.width = 400; // Not on all

            listView.style.flexGrow = 1.0f;
            listView.reorderable = true;
            listView.selectionType = SelectionType.None;
            listView.showBorder = true;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

            return listView;
        }

        private VisualElement CreateSubPage()
        {
            VisualElement subPageElement = new VisualElement();

            Button backButton = new Button(() =>
            {
                GoToMainPage();
            });
            backButton.text = "Back";
            subPageElement.Add(backButton);

            TextField nameTextField = new TextField();
            subPageElement.Add(nameTextField);

            HelpBox helpBox = new HelpBox("The Audio Group name is used as strings in scripts to play sounds. \nBe careful when you change the name!", HelpBoxMessageType.Warning);
            subPageElement.Add(helpBox);

            PropertyField volumeField = new PropertyField();
            subPageElement.Add(volumeField);

            PropertyField typeField = new PropertyField();
            subPageElement.Add(typeField);

            PropertyField resetTimeField = new PropertyField();
            subPageElement.Add(resetTimeField);

            Button playButton = new Button(() =>
            {
                SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                AudioManager.Instance.PlayInEditor(AudioCategory.Group, audioGroup.FindPropertyRelative("name").stringValue);
            });
            playButton.text = "Play";
            subPageElement.Add(playButton);

            Button addButton = new Button(() =>
            {
                SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
                segmentsProperty.InsertArrayElementAtIndex(segmentsProperty.arraySize);
                SerializedProperty groupSegmentProperty = segmentsProperty.GetArrayElementAtIndex(segmentsProperty.arraySize - 1);
                SerializedProperty segmentProperty = groupSegmentProperty.FindPropertyRelative("audioSegment");
                segmentProperty.FindPropertyRelative("volume").floatValue = 1f;
                segmentProperty.FindPropertyRelative("pitch").floatValue = 1f;
                groupSegmentProperty.FindPropertyRelative("weight").intValue = 1;
                serializedObject.ApplyModifiedProperties();
            });
            addButton.text = "Add";
            subPageElement.Add(addButton);

            ListView listView = MakeAudioGroupSegmentListView();
            subPageElement.Add(listView);

            return subPageElement;
        }

        private ListView MakeAudioGroupSegmentListView()
        {
            Func<VisualElement> makeItem = () =>
            {
                VisualElement element = new VisualElement();

                PropertyField propertyField = new PropertyField();
                element.Add(propertyField);

                Button playButton = new Button(() =>
                {
                    SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                    SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
                    SerializedProperty segmentProperty = segmentsProperty.GetArrayElementAtIndex((int)element.userData);
                    float audioGroupVolume = audioGroup.FindPropertyRelative("volume").floatValue;
                    AudioManager.Instance.PlayInEditor(AudioCategory.Group, audioGroup.FindPropertyRelative("name").stringValue, (int)element.userData);
                });
                playButton.text = "Play";
                element.Add(playButton);

                Button deleteButton = new Button(() =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning("Can't delete in play mode");
                    }
                    else
                    {
                        SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                        SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
                        segmentsProperty.DeleteArrayElementAtIndex((int)element.userData);
                        serializedObject.ApplyModifiedProperties();
                    }
                });
                deleteButton.text = "Delete";
                element.Add(deleteButton);

                return element;
            };

            Action<VisualElement, int> bindItem = (element, index) =>
            {
                SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
                element.userData = index;
                (element.ElementAt(0) as PropertyField).BindProperty(segmentsProperty.GetArrayElementAtIndex(index));
            };


            return SetListViewSettings(makeItem, bindItem, "Segments", 170, 400);
        }

        private void GoToSubPage(int index)
        {
            audioGroupIndex = index;
            mainPage.style.display = DisplayStyle.None;
            subPage.style.display = DisplayStyle.Flex;

            SerializedProperty audioGroupProperty = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(index);
            (subPage.ElementAt(1) as TextField).BindProperty(audioGroupProperty.FindPropertyRelative("name"));
            (subPage.ElementAt(3) as PropertyField).BindProperty(audioGroupProperty.FindPropertyRelative("volume"));
            (subPage.ElementAt(4) as PropertyField).BindProperty(audioGroupProperty.FindPropertyRelative("type"));
            (subPage.ElementAt(5) as PropertyField).BindProperty(audioGroupProperty.FindPropertyRelative("resetTime"));
            (subPage.ElementAt(8) as ListView).BindProperty(audioGroupProperty.FindPropertyRelative("segments"));
        }

        private void GoToMainPage()
        {
            subPage.style.display = DisplayStyle.None;
            mainPage.style.display = DisplayStyle.Flex;
        }

        private void OnToggleChanged(ChangeEvent<bool> evt)
        {
            enableDeleting = evt.newValue;
        }

        private string CreateDefaultName(SerializedProperty audioGroupsProperty, string title)
        {
            int index = 1;
            while (true)
            {
                string testTitle = title + index;
                bool found = false;
                for (int i = 0; i < audioGroupsProperty.arraySize; i++)
                {
                    if (audioGroupsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == testTitle)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return testTitle;
                index++;
            }
        }

        private string CreateDefaultAudioSegmentName(SerializedProperty audioProperty, string title)
        {
            int index = 1;
            while (true)
            {
                string testTitle = title + index;
                bool found = false;
                for (int i = 0; i < audioProperty.arraySize; i++)
                {
                    if (audioProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == testTitle)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return testTitle;
                index++;
            }
        }
    }
}
