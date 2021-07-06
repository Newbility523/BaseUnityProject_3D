using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine;
using System;
using Engine;

[CustomEditor(typeof(NewText), true)]
[CanEditMultipleObjects]
public class NewTextEditor : UnityEditor.UI.TextEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NewText text = target as NewText;

        GUILayout.BeginHorizontal();

        GUILayout.Label("colorTag");
        ColorTag origin = text.colorTag;
        text.colorTag = (ColorTag)EditorGUILayout.EnumPopup(text.colorTag);
        if (text.colorTag != origin)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString(ColorConfig.GetColor(text.colorTag), out c))
            {
                text.color = c;
                EditorUtility.SetDirty(target);
            }
        }

        GUILayout.EndHorizontal();
    }
}
