using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;

public class LevelEditorWindow : EditorWindow
{

    public enum Dirs
    {
        Forward, Right, Up
    }

    private const float moveConstant = 2;

    private IntVariable cantCells;
    private GameObject baseCell;
    private GUIStyle _labelStyle;
    private Cell cell;
    private bool _showCellEditor;
    private bool _showFloorFoldoutF;
    private bool _showFloorFoldoutB;
    private bool _showWallBackFoldoutF;
    private bool _showWallBackFoldoutB;
    private bool _showWallLeftFoldoutF;
    private bool _showWallLeftFoldoutB;
    private bool _showBeamBackFoldoutF;
    private bool _showBeamBackFoldoutB;
    private bool _showBeamLeftFoldoutF;
    private bool _showBeamLeftFoldoutB;
    private bool _showColumnFoldout;
    private bool _showVertexFoldout;


    public GUIStyle myStyle;
    public GUIStyle _style;
    public GUIStyle style;
    private Sprite image;

    [MenuItem("Tools/Level Creator")]
    public static void OpenWindow()
    {
        var w = GetWindow<LevelEditorWindow>();
        w.minSize = new Vector2(600,225);

        var sa = GameObject.Find("Lvl");
        if (sa == null)
        {
           var parent = Instantiate(new GameObject());
            parent.name = "Lvl";
        }
        w._labelStyle = new GUIStyle();
        w._labelStyle.fontStyle = FontStyle.Bold;
        w._labelStyle.alignment = TextAnchor.MiddleCenter;
        w.baseCell = Resources.Load<GameObject>("Prefabs/Cell");
        w.cantCells = Resources.Load<IntVariable>("Scriptables/CantCells");

        w.StartFunctions(w);

    }
    public void StartFunctions(LevelEditorWindow w)
    {

        w._style = new GUIStyle();
        w._style.fontStyle = FontStyle.Italic;
        w._style.alignment = TextAnchor.MiddleCenter;
        w._style.wordWrap = true;

       

        w.myStyle = new GUIStyle();
        w.myStyle.fontStyle = FontStyle.BoldAndItalic;
        w.myStyle.fontSize = 15;
        w.myStyle.alignment = TextAnchor.MiddleCenter;
        w.myStyle.wordWrap = true;
        w.Show();

    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("NO SE PUEDE EDITAR PARAMETROS DURANTE PLAY MODE", MessageType.Warning);
            return;
        }
        style = new GUIStyle(GUI.skin.button);
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;

        Repaint();
        Space(2);
        EditorGUILayout.LabelField("Level Editor", _style);
        Space(3);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("FINISH ALL CELLS EDITING"))
        {
            FinishedEditing();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        Space(2);
        if (GUILayout.Button("OPEN ALL CELLS EDITING"))
        {
            ReOpenEditing();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("All Trancitable"))
            SetAllAsTrancitable();

        GUILayout.EndHorizontal();

        Space(3);

        Cell cellAux = null;

        var sa = Selection.gameObjects;
        if (sa.Length != 0)
            cellAux = sa[0].GetComponent<Cell>();

        this.cell = cellAux;

        if (this.cell == null)
        {
            return;
        }
        _showCellEditor = EditorGUILayout.Foldout(_showCellEditor, "Show Cell Editor");
        if (_showCellEditor)
        {
            #region Cell Editor
            if (this.cell.finished)
            {
                EditorGUILayout.HelpBox("SE FINALIZO LA EDICION", MessageType.Warning);
                GUILayout.Space(5);
                if (GUILayout.Button("Edit"))
                {
                    this.cell.EditCell();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                return;
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("NO SE PUEDE EDITAR PARAMETROS DURANTE PLAY MODE", MessageType.Warning);
                return;
            }

            GUILayout.Space(5);
            if (cell.floorPart != null)
            {
                GUILayout.BeginHorizontal();
                cell.isTrancitable = EditorGUILayout.Toggle("Is Trancitable", cell.isTrancitable);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Floor", _labelStyle);
            cell.floorPart = (FloorCellPart)EditorGUILayout.ObjectField(cell.floorPart, typeof(FloorCellPart), false);
            GUILayout.EndHorizontal();


            if (this.cell.floorPart != null)
            {
                _showFloorFoldoutF = EditorGUILayout.Foldout(_showFloorFoldoutF, "Anchors Front");
                if (_showFloorFoldoutF)
                {
                    var floorpartAnchorsF = this.cell.floorPart.FAnchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < floorpartAnchorsF.Length; i++)
                    {
                        if (floorpartAnchorsF[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                            this.cell.floorPropF[i] = (FloorProp)EditorGUILayout.ObjectField(this.cell.floorPropF[i], typeof(FloorProp), false);
                            GUILayout.EndHorizontal();
                            if (i == 4 && cell.floorPropF[i] != null)
                            {
                                GUILayout.BeginHorizontal();
                                cell.centerRotationY = EditorGUILayout.FloatField("CenterRotation", cell.centerRotationY);
                                GUILayout.EndHorizontal();
                            }

                        }
                    }
                }
                _showFloorFoldoutB = EditorGUILayout.Foldout(_showFloorFoldoutB, "Anchors Back");
                if (_showFloorFoldoutB)
                {
                    var floorpartAnchorsB = this.cell.floorPart.BAnchors;
                    GUILayout.Space(1);
                    for (int i = 0; i < floorpartAnchorsB.Length; i++)
                    {
                        if (floorpartAnchorsB[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                            this.cell.floorPropB[i] = (FloorProp)EditorGUILayout.ObjectField(this.cell.floorPropB[i], typeof(FloorProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);


            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wall Back", _labelStyle);
            this.cell.wallBackPart = (WallCellPart)EditorGUILayout.ObjectField(this.cell.wallBackPart, typeof(WallCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.wallBackPart != null)
            {
                _showWallBackFoldoutF = EditorGUILayout.Foldout(_showWallBackFoldoutF, "Anchors Front");
                if (_showWallBackFoldoutF)
                {
                    var wallBackpartAnchorsF = this.cell.wallBackPart.FAnchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < wallBackpartAnchorsF.Length; i++)
                    {
                        if (wallBackpartAnchorsF[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                            this.cell.wallBackPropF[i] = (WallProp)EditorGUILayout.ObjectField(this.cell.wallBackPropF[i], typeof(WallProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                _showWallBackFoldoutB = EditorGUILayout.Foldout(_showWallBackFoldoutB, "Anchors Back");
                if (_showWallBackFoldoutB)
                {
                    var wallBackpartAnchorsB = this.cell.wallBackPart.BAnchors;
                    GUILayout.Space(1);
                    for (int i = 0; i < wallBackpartAnchorsB.Length; i++)
                    {
                        if (wallBackpartAnchorsB[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                            this.cell.wallBackPropB[i] = (WallProp)EditorGUILayout.ObjectField(this.cell.wallBackPropB[i], typeof(WallProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);


            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("Wall Left", _labelStyle);
            this.cell.wallLeftPart = (WallCellPart)EditorGUILayout.ObjectField(this.cell.wallLeftPart, typeof(WallCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.wallLeftPart != null)
            {
                _showWallLeftFoldoutF = EditorGUILayout.Foldout(_showWallLeftFoldoutF, "Anchors Front");
                if (_showWallLeftFoldoutF)
                {
                    var wallLeftpartAnchorsF = this.cell.wallLeftPart.FAnchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < wallLeftpartAnchorsF.Length; i++)
                    {
                        if (wallLeftpartAnchorsF[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                            this.cell.wallLeftPropF[i] = (WallProp)EditorGUILayout.ObjectField(this.cell.wallLeftPropF[i], typeof(WallProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                _showWallLeftFoldoutB = EditorGUILayout.Foldout(_showWallLeftFoldoutB, "Anchors Back");
                if (_showWallLeftFoldoutB)
                {
                    var wallLeftpartAnchorsB = this.cell.wallLeftPart.BAnchors;
                    GUILayout.Space(1);
                    for (int i = 0; i < wallLeftpartAnchorsB.Length; i++)
                    {
                        if (wallLeftpartAnchorsB[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                            this.cell.wallLeftPropB[i] = (WallProp)EditorGUILayout.ObjectField(this.cell.wallLeftPropB[i], typeof(WallProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);


            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("Beam Back", _labelStyle);
            this.cell.beamBackPart = (BeamCellPart)EditorGUILayout.ObjectField(this.cell.beamBackPart, typeof(BeamCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.beamBackPart != null)
            {
                _showBeamBackFoldoutF = EditorGUILayout.Foldout(_showBeamBackFoldoutF, "Anchors Front");
                if (_showBeamBackFoldoutF)
                {
                    var beamBackpartAnchorsF = this.cell.beamBackPart.FAnchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < beamBackpartAnchorsF.Length; i++)
                    {
                        if (beamBackpartAnchorsF[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                            this.cell.beamBackPropF[i] = (BeamProp)EditorGUILayout.ObjectField(this.cell.beamBackPropF[i], typeof(BeamProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                _showBeamBackFoldoutB = EditorGUILayout.Foldout(_showBeamBackFoldoutB, "Anchors Back");
                if (_showBeamBackFoldoutB)
                {
                    var beamBackpartAnchorsB = this.cell.beamBackPart.BAnchors;
                    GUILayout.Space(1);
                    for (int i = 0; i < beamBackpartAnchorsB.Length; i++)
                    {
                        if (beamBackpartAnchorsB[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                            this.cell.beamBackPropB[i] = (BeamProp)EditorGUILayout.ObjectField(this.cell.beamBackPropB[i], typeof(BeamProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("Beam Left", _labelStyle);
            this.cell.beamLeftPart = (BeamCellPart)EditorGUILayout.ObjectField(this.cell.beamLeftPart, typeof(BeamCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.beamLeftPart != null)
            {
                _showBeamLeftFoldoutF = EditorGUILayout.Foldout(_showBeamLeftFoldoutF, "Anchors Front");
                if (_showBeamLeftFoldoutF)
                {
                    var beamLeftpartAnchorsF = this.cell.beamLeftPart.FAnchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < beamLeftpartAnchorsF.Length; i++)
                    {
                        if (beamLeftpartAnchorsF[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                            this.cell.beamLeftPropF[i] = (BeamProp)EditorGUILayout.ObjectField(this.cell.beamLeftPropF[i], typeof(BeamProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                _showBeamLeftFoldoutB = EditorGUILayout.Foldout(_showBeamLeftFoldoutB, "Anchors Back");
                if (_showBeamLeftFoldoutB)
                {
                    var beamLeftpartAnchorsB = this.cell.beamLeftPart.BAnchors;
                    GUILayout.Space(1);
                    for (int i = 0; i < beamLeftpartAnchorsB.Length; i++)
                    {
                        if (beamLeftpartAnchorsB[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                            this.cell.beamLeftPropB[i] = (BeamProp)EditorGUILayout.ObjectField(this.cell.beamLeftPropB[i], typeof(BeamProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);
            }



            GUILayout.BeginHorizontal();
            GUILayout.Label("Column", _labelStyle);
            this.cell.columnPart = (ColumnCellPart)EditorGUILayout.ObjectField(this.cell.columnPart, typeof(ColumnCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.columnPart != null)
            {
                _showColumnFoldout = EditorGUILayout.Foldout(_showColumnFoldout, "Anchors");
                if (_showColumnFoldout)
                {
                    var columnpartAnchors = this.cell.columnPart.Anchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < columnpartAnchors.Length; i++)
                    {
                        if (columnpartAnchors[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Anchors" + (i + 1), _labelStyle);
                            this.cell.columnProp[i] = (ColumnProp)EditorGUILayout.ObjectField(this.cell.columnProp[i], typeof(ColumnProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);
            }



            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertex", _labelStyle);
            this.cell.vertexPart = (VertexCellPart)EditorGUILayout.ObjectField(this.cell.vertexPart, typeof(VertexCellPart), false);
            GUILayout.EndHorizontal();

            if (this.cell.vertexPart != null)
            {
                _showVertexFoldout = EditorGUILayout.Foldout(_showVertexFoldout, "Anchors");
                if (_showVertexFoldout)
                {
                    var vertexpartAnchors = this.cell.vertexPart.Anchors;
                    GUILayout.Space(1);

                    for (int i = 0; i < vertexpartAnchors.Length; i++)
                    {
                        if (vertexpartAnchors[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Anchors" + (i + 1), _labelStyle);
                            this.cell.vertexProp[i] = (VertexProp)EditorGUILayout.ObjectField(this.cell.vertexProp[i], typeof(VertexProp), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                GUILayout.Space(5);
            }

            if (GUILayout.Button("Finish"))
            {
                this.cell.FinishCell();
                this.cell.finished = true;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUI.changed == true)
                this.cell.Confirm();


#endregion
        }

        GUI.color = new Color32(200, 200, 200, 255);

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

        if (GUILayout.Button("↻", style))
            cell.gameObject.transform.Rotate(Vector3.up, 45);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("↺", style))
            cell.gameObject.transform.Rotate(Vector3.up, -45);

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

        if (GUILayout.Button("⇧", style))
        {
            //_stayOpen = false;
            Copy(GetPosFromCamera(Dirs.Forward) * moveConstant,cell.transform);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇧", style))//move
            cell.gameObject.transform.position += GetPosFromCamera(Dirs.Forward) * moveConstant;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇦", style))
        {
            //_stayOpen = false;
            Copy(-GetPosFromCamera(Dirs.Right) * moveConstant,cell.transform);

        }

        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("⇨", style))
        {
            //_stayOpen = false;
            Copy(GetPosFromCamera(Dirs.Right) * moveConstant, cell.transform);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇦", style))//move
            cell.gameObject.transform.position += -GetPosFromCamera(Dirs.Right) * moveConstant;


        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("⇨", style))//move
            cell.gameObject.transform.position += GetPosFromCamera(Dirs.Right) * moveConstant;
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇩", style))
        {
            // _stayOpen = false;
            Copy(-GetPosFromCamera(Dirs.Forward) * moveConstant, cell.transform);
        }
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("⇩", style))//move
            cell.gameObject.transform.position += -GetPosFromCamera(Dirs.Forward) * moveConstant;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Move up"))
            cell.gameObject.transform.position += cell.transform.up * moveConstant;

        if (GUILayout.Button("Move Down"))
            cell.gameObject.transform.position -= cell.transform.up * moveConstant;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.color = Color.white;


        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();


    }

    public Vector3 GetPosFromCamera(Dirs dir)
    {
        var sceneCamPos = SceneView.lastActiveSceneView;

        if (sceneCamPos != null)
        {
            if (sceneCamPos == null) return Vector3.zero;

            var auxForward = GetCloserVector3(sceneCamPos.camera.transform.forward);
            var auxUp = GetCloserVector3(sceneCamPos.camera.transform.up);
            var auxRight = GetCloserVector3(sceneCamPos.camera.transform.right);
            switch (dir)
            {
                case Dirs.Forward:
                    return auxForward;
                case Dirs.Right:
                    return auxRight;
                case Dirs.Up:
                    return auxUp;
            }
        }
        switch (dir)
        {
            case Dirs.Forward:
                return Vector3.forward;
            case Dirs.Right:
                return Vector3.right;
            case Dirs.Up:
                return Vector3.up;
        }

        return Vector3.zero;
    }

    private Vector3 GetCloserVector3(Vector3 dir)
    {
        var auxDir = Vector3.right;
        var aux = Mathf.Abs(Vector3.Angle(dir, Vector3.right));

        if (aux > Mathf.Abs(Vector3.Angle(dir, -Vector3.right)))
        {
            auxDir = -Vector3.right;
            aux = Mathf.Abs(Vector3.Angle(dir, -Vector3.right));
        }

        if (aux > Mathf.Abs(Vector3.Angle(dir, Vector3.forward)))
        {
            auxDir = Vector3.forward;
            aux = Mathf.Abs(Vector3.Angle(dir, Vector3.forward));
        }
        if (aux > Mathf.Abs(Vector3.Angle(dir, -Vector3.forward)))
        {
            auxDir = -Vector3.forward;
            aux = Mathf.Abs(Vector3.Angle(dir, -Vector3.forward));
        }
        return auxDir;
    }

    public void Copy(Vector3 dir , Transform target)
    {
        GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(baseCell);

        #region Copy CellParts
        var cell = target.GetComponent<Cell>();
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

        t.transform.position = target.transform.position + dir;
        t.transform.localScale = target.transform.localScale;
        t.transform.rotation = target.transform.rotation;
        t.transform.parent = target.transform.parent;

        t.gameObject.name = "Cell " + cantCells.value;
        cantCells.value++;
        Selection.activeObject = t;
    }

    public void FinishedEditing()
    {
        var allCellsList = GameObject.FindObjectsOfType<Cell>();
        foreach (var cell in allCellsList)
        {
            cell.FinishCell();
        }
    }

    public void UpdateAllCells()
    {
        var allCellsList = GameObject.FindObjectsOfType<Cell>();
        foreach (var cell in allCellsList)
        {
            cell.UpdateCell();
        }
    }

    public void SetAllAsTrancitable()
    {
        var allCellsList = GameObject.FindObjectsOfType<Cell>();
        foreach (var cell in allCellsList)
        {
            cell.isTrancitable = true;
            cell.UpdateNodeTransitable();
        }
    }

    public void ReOpenEditing()
    {
        var allCellsList = GameObject.FindObjectsOfType<Cell>();
        foreach (var cell in allCellsList)
        {
            cell.EditCell();
        }
    }

    public void Space()
    {
        Space(1);
    }
    public void Space(int cantSpace)
    {
        for (int i = 0; i < cantSpace; i++)
        {
            EditorGUILayout.Space();
        }
    }

}
