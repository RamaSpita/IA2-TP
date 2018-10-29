using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Queries3d))]
public class BoxQ3dEditor : Editor
{
    Queries3d _target;
    GUIStyle style;

    private void OnEnable()
    {
        _target = (Queries3d)target;
        style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
    }

    private void OnSceneGUI()
    {
        var originalCol = GUI.color;
        Handles.BeginGUI();
        GUI.color = Color.red;
        foreach(var elem in _target.selected)
        {
            var p = Camera.current.WorldToScreenPoint(elem.transform.position);
            GUI.Label(new Rect(p.x - 5, Screen.height - p.y - 50, 50, 100), ":)", style);
        }
        Handles.EndGUI();
        GUI.color = originalCol;
    }
}
