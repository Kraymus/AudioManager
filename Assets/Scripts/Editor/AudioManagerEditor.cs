using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private bool enableDeleting = false;
    private VisualElement mainPage;
    private VisualElement subPage;
    private VisualElement resetTimeElement;
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
                AudioManager.Instance.PlayInEditor(audioGroupsProperty.GetArrayElementAtIndex((int)element.userData).FindPropertyRelative("name").stringValue);
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
                if (enableDeleting)
                {
                    audioGroupsProperty.DeleteArrayElementAtIndex((int)element.userData);
                    serializedObject.ApplyModifiedProperties();
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

        var listView = new ListView();
        listView.bindingPath = audioGroupsProperty.propertyPath;
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.showFoldoutHeader = true;
        listView.showBoundCollectionSize = false;
        listView.headerTitle = "Audio Groups";
        listView.fixedItemHeight = 50;

        listView.style.flexGrow = 1.0f;
        listView.reorderable = true;
        listView.selectionType = SelectionType.None;
        listView.showBorder = true;
        listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

        SerializedProperty playerTransformProperty = serializedObject.FindProperty("playerTransform");
        PropertyField playerTransformField = new PropertyField();
        playerTransformField.BindProperty(playerTransformProperty);
        mainPageElement.Add(playerTransformField);

        Toggle toggle = new Toggle("Enable Deleting");
        toggle.RegisterValueChangedCallback(OnToggleChanged);
        mainPageElement.Add(toggle);

        Button addButton = new Button(() =>
        {
            audioGroupsProperty.InsertArrayElementAtIndex(audioGroupsProperty.arraySize);
            audioGroupsProperty.GetArrayElementAtIndex(audioGroupsProperty.arraySize - 1).FindPropertyRelative("name").stringValue = CreateDefaultName(audioGroupsProperty);
            audioGroupsProperty.GetArrayElementAtIndex(audioGroupsProperty.arraySize - 1).FindPropertyRelative("volume").floatValue = 1f;
            serializedObject.ApplyModifiedProperties();
        });
        addButton.text = "Add";
        mainPageElement.Add(addButton);

        mainPageElement.Add(listView);

        return mainPageElement;
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
        //typeField.RegisterValueChangeCallback(TypeChanged);
        subPageElement.Add(typeField);

        PropertyField resetTimeField = new PropertyField();
        resetTimeElement = resetTimeField;
        subPageElement.Add(resetTimeField);

        Button playButton = new Button(() =>
        {
            SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
            AudioManager.Instance.PlayInEditor(audioGroup.FindPropertyRelative("name").stringValue);
        });
        playButton.text = "Play";
        subPageElement.Add(playButton);

        Button addButton = new Button(() =>
        {
            SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
            SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
            segmentsProperty.InsertArrayElementAtIndex(segmentsProperty.arraySize);
            SerializedProperty segmentProperty = segmentsProperty.GetArrayElementAtIndex(segmentsProperty.arraySize - 1);
            segmentProperty.FindPropertyRelative("volume").floatValue = 1f;
            segmentProperty.FindPropertyRelative("pitch").floatValue = 1f;
            segmentProperty.FindPropertyRelative("weight").intValue = 1;
            serializedObject.ApplyModifiedProperties();
        });
        addButton.text = "Add";
        subPageElement.Add(addButton);

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
                AudioManager.Instance.PlayAudioGroupSegmentInEditor(audioGroup.FindPropertyRelative("name").stringValue, (int)element.userData);
            });
            playButton.text = "Play";
            element.Add(playButton);

            Button deleteButton = new Button(() =>
            {
                SerializedProperty audioGroup = serializedObject.FindProperty("audioGroups").GetArrayElementAtIndex(audioGroupIndex);
                SerializedProperty segmentsProperty = audioGroup.FindPropertyRelative("segments");
                segmentsProperty.DeleteArrayElementAtIndex((int)element.userData);
                serializedObject.ApplyModifiedProperties();
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

        var listView = new ListView();
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.showFoldoutHeader = true;
        listView.showBoundCollectionSize = false;
        listView.headerTitle = "Segments";
        listView.fixedItemHeight = 170;
        listView.style.width = 400;
        //listView.style.maxWidth = 600;

        listView.style.flexGrow = 1.0f;
        listView.reorderable = true;
        listView.selectionType = SelectionType.None;
        listView.showBorder = true;
        listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

        subPageElement.Add(listView);

        return subPageElement;
    }

    //private void TypeChanged(SerializedPropertyChangeEvent evt)
    //{
    //    if (evt.changedProperty.intValue == 0)
    //        resetTimeElement.style.display = DisplayStyle.None;
    //    else
    //        resetTimeElement.style.display = DisplayStyle.Flex;
    //}

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

    private string CreateDefaultName(SerializedProperty audioGroupsProperty)
    {
        string title = "Audio Group ";

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
}
