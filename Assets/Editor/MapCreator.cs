using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseObject))]
public class MapCreator : Editor
{
    public static int cellCount = 0;
    private BaseObject _target;
    GUIStyle _style;
    //private bool _stayOpen = false;
    //private string _lockButton;
    private const float moveConstant = 2;
    private GameObject baseCell;

    public void OnEnable()
    {
        _target = (BaseObject)target;
        //_lockButton = "Unlock";
        //_stayOpen = false;

        baseCell = Resources.Load<GameObject>("Prefabs/Cell");
    }

    private void OnSceneGUI()
    {
        _style = new GUIStyle(GUI.skin.button);
        _style.fontSize = 20;
        _style.alignment = TextAnchor.MiddleCenter;
        Handles.BeginGUI();
        var v = EditorWindow.GetWindow<SceneView>().camera.pixelRect;

        if (GUI.Button(new Rect(v.width - v.width / 2 - 50, 20, 100, 50), "Destroy"))
        {
            DestroyImmediate(_target.gameObject);
        }
        UpperLeftControlPanel();
        Handles.EndGUI();
        //if (_stayOpen)
        //{
        //    Selection.activeObject = _target;
        //}
    }

    private void UpperLeftControlPanel()
    {
        GUILayout.BeginArea(new Rect(20, 20, 250, 500));
        var rec = EditorGUILayout.BeginVertical();
        GUI.color = new Color32(200, 200, 200, 255);

        GUI.Box(rec, GUIContent.none);

        GUILayout.BeginHorizontal();


        GUILayout.FlexibleSpace();

        GUILayout.Label("Rotation Controls");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.color = Color.white;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("↻",_style))
            _target.gameObject.transform.Rotate(Vector3.up, 45);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("↺", _style))
            _target.gameObject.transform.Rotate(Vector3.up, -45);

        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.Label("Copy Controls");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Move Controls");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.color = Color.white;

        #region Move And Copy
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("⇧", _style))
        {
            //_stayOpen = false;
            Copy(_target.transform.forward * moveConstant);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇧", _style))//move
            _target.gameObject.transform.position += _target.transform.forward * moveConstant;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇦", _style))
        {
            //_stayOpen = false;
            Copy(-_target.transform.right * moveConstant);

        }

        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("⇨", _style))
        {
            //_stayOpen = false;
            Copy(_target.transform.right * moveConstant);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇦", _style))//move
        _target.gameObject.transform.position += -_target.transform.right * moveConstant;


        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("⇨", _style))//move
            _target.gameObject.transform.position += _target.transform.right * moveConstant;
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇩", _style))
        {
           // _stayOpen = false;
            Copy(-_target.transform.forward * moveConstant);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇩", _style))//move
            _target.gameObject.transform.position += -_target.transform.forward * moveConstant;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Move up"))
            _target.gameObject.transform.position += _target.transform.up * moveConstant;

        if (GUILayout.Button("Move Down"))
            _target.gameObject.transform.position -= _target.transform.up * moveConstant;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.color = Color.white;


        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();

        //if (GUILayout.Button(_lockButton))
        //{
        //    _stayOpen = !_stayOpen;
        //    if (_stayOpen)
        //    {
        //        _lockButton = "Unlock";
        //    }
        //    else
        //    {
        //        _lockButton = "Lock";
        //    }
        //}
        //if (_stayOpen)
        //{
        //    Event e = Event.current;
        //    switch (e.type)
        //    {
        //        case EventType.KeyDown:
        //            if (e.keyCode == KeyCode.Escape)
        //            {
        //                _stayOpen = false;
        //                _lockButton = "Lock";
        //                Selection.activeObject = null;
        //            }
        //                break;
        //    }
        //}
       

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public void Copy(Vector3 dir)
    {
        GameObject t = Instantiate(baseCell);

        #region Copy CellParts
        var cell = _target.GetComponent<Cell>();
        var newCell = t.GetComponent<Cell>();
        newCell.floorPart = cell.floorPart;
        newCell.wallBackPart = cell.wallBackPart;
        newCell.wallLeftPart = cell.wallLeftPart;
        newCell.beamBackPart = cell.beamBackPart;
        newCell.beamLeftPart = cell.beamLeftPart;
        newCell.columnPart = cell.columnPart;
        newCell.vertexPart = cell.vertexPart;
        newCell.Confirm();
        #endregion

        t.transform.position = _target.transform.position + dir;
        t.transform.localScale = _target.transform.localScale;
        t.transform.rotation = _target.transform.rotation;
        t.transform.parent = _target.transform.parent;

        t.gameObject.name = "Cell " + cellCount;
        cellCount++;
        Selection.activeObject = t;
    }
}
