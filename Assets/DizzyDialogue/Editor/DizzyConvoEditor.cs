using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

/// <summary>
/// Adapted from troien's code on ReorderableLists.
/// </summary>
[CustomEditor(typeof(DizzyConvo))]
public class DizzyConvoEditor : Editor
{
    // The list displayed in the editor UI
    private ReorderableList list;

    // This list contains information on which item in the list is active
    private List<bool> activeItems = new List<bool>();

    private DizzyConvo convoList
    {
        get
        {
            return target as DizzyConvo;
        }
    }

    private void OnEnable()
    {
        while (activeItems.Count < convoList.lines.Count)
            activeItems.Add(false);

        list = new ReorderableList(convoList.lines, typeof(DizzyLine), true, true, true, true)
        {
            drawElementCallback = (rect, index, active, focused) =>
            {
                if (activeItems.Count < index + 1)
                    activeItems.Add(false);

                try
                {
                    activeItems[index] = active;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
            },

            elementHeightCallback = (index) =>
            {
                float height = 80;
                try
                {
                    if (activeItems[index])
                        height += 40;
                    else if (convoList.lines[index].speakerIdentity == "")
                        height = 42;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
                return height;
            }
        };

        // Add listeners to draw events
        list.drawHeaderCallback += DrawHeader;
        list.drawElementCallback += DrawElement;

        list.onAddCallback += AddItem;
        list.onRemoveCallback += RemoveItem;
    }

    private void OnDisable()
    {
        // Make sure we don't get memory leaks etc.
        list.drawHeaderCallback -= DrawHeader;
        list.drawElementCallback -= DrawElement;

        list.onAddCallback -= AddItem;
        list.onRemoveCallback -= RemoveItem;
    }

    /// <summary>
    /// List header
    /// </summary>
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Change convo here, select item to change conditions");
    }

    /// <summary>
    /// Draws one element of the list
    /// </summary>
    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        float width = rect.width;
        float smallWidth = width / 6;
        DizzyLine item = convoList.lines[index];
        EditorStyles.textField.wordWrap = true;

        Undo.RecordObject(item, "Edited convo");
        EditorGUI.BeginChangeCheck();

        // If speaker is not assigned, only show speaker, length, & condition (because this line is a pause)
        if (item.speakerIdentity == "" && !active)
        {
            item.speakerIdentity = EditorGUI.TextField(new Rect(rect.x, rect.y, smallWidth, 18), item.speakerIdentity);
            EditorGUI.LabelField(new Rect(rect.x + smallWidth, rect.y + 1, 3 * smallWidth, 16), "No speaker; pausing.");
            EditorGUI.LabelField(new Rect(rect.x + 4 * smallWidth, rect.y + 1, smallWidth, 16), "Length:");
            item.length = EditorGUI.FloatField(new Rect(rect.x + 5 * smallWidth, rect.y + 1, smallWidth, 16), item.length);

            item.condition = (DizzyCondition)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 19, 3 * smallWidth, 16), item.condition, typeof(DizzyCondition), false);
            if(item.condition)
            {
                EditorGUI.LabelField(new Rect(rect.x + 3 * smallWidth, rect.y + 19, smallWidth - 18, 16), "Branch:");
                item.conditionIsBranch = EditorGUI.Toggle(new Rect(rect.x + 4 * smallWidth - 18, rect.y + 19, 18, 16), item.conditionIsBranch);
                EditorGUI.BeginDisabledGroup(!item.conditionIsBranch);
                item.condJump0 = EditorGUI.IntField(new Rect(rect.x + 4 * smallWidth, rect.y + 19, smallWidth, 16), item.condJump0);
                item.condJump1 = EditorGUI.IntField(new Rect(rect.x + 5 * smallWidth, rect.y + 19, smallWidth, 16), item.condJump1);
                EditorGUI.EndDisabledGroup();
            }
        }
        else
        {
            item.speakerIdentity = EditorGUI.TextField(new Rect(rect.x, rect.y, 100, 18), item.speakerIdentity);
            item.line = EditorGUI.TextArea(new Rect(rect.x + 100, rect.y, width - 100, 36), item.line);

            EditorGUI.LabelField(new Rect(rect.x, rect.y + 18, 80, 18), "Show name:");
            item.subtitleShowsSpeaker = EditorGUI.Toggle(new Rect(rect.x + 80, rect.y + 18, 18, 18), item.subtitleShowsSpeaker);

            item.subtitleStyle = (SubtitlePreset)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 37, 100, 16), item.subtitleStyle, typeof(SubtitlePreset), false);
            item.clip = (AudioClip)EditorGUI.ObjectField(new Rect(rect.x + 100, rect.y + 37, width - 100, 16), item.clip, typeof(AudioClip), false);

            EditorGUI.LabelField(new Rect(rect.x, rect.y + 55, smallWidth, 16), "Length:");
            item.length = EditorGUI.FloatField(new Rect(rect.x + smallWidth, rect.y + 55, smallWidth, 16), item.length);
            EditorGUI.LabelField(new Rect(rect.x + 2 * smallWidth, rect.y + 55, smallWidth, 16), "Dist.:");
            item.distance = EditorGUI.FloatField(new Rect(rect.x + 3 * smallWidth, rect.y + 55, smallWidth, 16), item.distance);
            EditorGUI.LabelField(new Rect(rect.x + 4 * smallWidth, rect.y + 55, smallWidth, 16), "Power:");
            item.power = EditorGUI.FloatField(new Rect(rect.x + 5 * smallWidth, rect.y + 55, smallWidth, 16), item.power);

            if (active)
            {
                item.condition = (DizzyCondition)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 78, width - 75, 16), item.condition, typeof(DizzyCondition), false);
                EditorGUI.LabelField(new Rect(rect.x + width - 75, rect.y + 78, 57, 16), "Branch:");
                item.conditionIsBranch = EditorGUI.Toggle(new Rect(rect.x + width - 18, rect.y + 78, 18, 16), item.conditionIsBranch);

                EditorGUI.BeginDisabledGroup(!item.conditionIsBranch);
                item.condJump0 = EditorGUI.IntField(new Rect(rect.x, rect.y + 96, width / 2, 16), "Branch Jump True", item.condJump0);
                item.condJump1 = EditorGUI.IntField(new Rect(rect.x + width / 2, rect.y + 96, width / 2, 16), "Branch Jump False", item.condJump1);
                EditorGUI.EndDisabledGroup();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }

    /// <summary>
    /// Adds an item to the list
    /// </summary>
    private void AddItem(ReorderableList list)
    {
        if (activeItems.Count == list.count)
            activeItems.Add(false);

        convoList.lines.Add(CreateInstance<DizzyLine>());

        EditorUtility.SetDirty(target);
    }

    /// <summary>
    /// Removes an item from the list
    /// </summary>
    private void RemoveItem(ReorderableList list)
    {
        convoList.lines.RemoveAt(list.index);
        
        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        convoList.startOnAwake = EditorGUILayout.Toggle("Start on Awake", convoList.startOnAwake);
        convoList.loop = EditorGUILayout.Toggle("Loop", convoList.loop);
        list.DoLayoutList();
    }
}