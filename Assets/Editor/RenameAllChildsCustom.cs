﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenameAllChilds))]
public class RenameAllChildsCustom : Editor
{
    RenameAllChilds parent;
    private void OnEnable()
    {
        parent = (RenameAllChilds)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Rename"))
        {
            parent.Rename();
        }
    }
}