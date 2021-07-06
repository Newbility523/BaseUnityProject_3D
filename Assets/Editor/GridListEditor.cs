using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using Engine;

[CustomEditor(typeof(GridList), true)]
[CanEditMultipleObjects]
public class GridListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridList comp = target as GridList;

        GUILayout.BeginVertical();
        comp.horizontal = EditorGUILayout.Toggle("Horizontal", comp.horizontal);
        comp.vertical = EditorGUILayout.Toggle("Vertical", comp.vertical);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Start Corner");
        comp.startCorner = (GridLayoutGroup.Corner)EditorGUILayout.EnumPopup(comp.startCorner);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
