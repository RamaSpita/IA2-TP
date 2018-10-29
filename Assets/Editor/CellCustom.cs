using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Cell))]
public class CellCustom : Editor
{
    private GUIStyle _labelStyle;
    private Cell cell;
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


    private void OnEnable()
    {
        cell = (Cell)target;
        _labelStyle = new GUIStyle();
        _labelStyle.fontStyle = FontStyle.Bold;
        _labelStyle.alignment = TextAnchor.MiddleCenter;

    }
    void Start()
    {
        if (Application.isPlaying)
        {
            cell.finished = true;
        }
    }

    

    public override void OnInspectorGUI()
    {
        if (cell.finished)
        {
            EditorGUILayout.HelpBox("SE FINALIZO LA EDICION", MessageType.Warning);
            GUILayout.Space(5);
            if (GUILayout.Button("Edit"))
            {
                cell.EditCell();
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
        GUILayout.BeginHorizontal();
        GUILayout.Label("Floor", _labelStyle);
        cell.floorPart = (FloorCellPart)EditorGUILayout.ObjectField(cell.floorPart, typeof(FloorCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.floorPart != null)
        {
            _showFloorFoldoutF = EditorGUILayout.Foldout(_showFloorFoldoutF, "Anchors Front");
            if (_showFloorFoldoutF)
            {
                var floorpartAnchorsF = cell.floorPart.FAnchors;
                GUILayout.Space(1);

                for (int i = 0; i < floorpartAnchorsF.Length; i++)
                {
                    if (floorpartAnchorsF[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                        cell.floorPropF[i] = (FloorProp)EditorGUILayout.ObjectField(cell.floorPropF[i], typeof(FloorProp), false);
                        GUILayout.EndHorizontal();

                        if (i==4 && cell.floorPropF[i] != null)
                        {
                            GUILayout.BeginHorizontal();
                            cell.centerRotationY = EditorGUILayout.FloatField("CenterRotation",cell.centerRotationY);
                            GUILayout.EndHorizontal();
                        }

                    }
                }
            }
            _showFloorFoldoutB = EditorGUILayout.Foldout(_showFloorFoldoutB, "Anchors Back");
            if (_showFloorFoldoutB)
            {
                var floorpartAnchorsB = cell.floorPart.BAnchors;
                GUILayout.Space(1);
                for (int i = 0; i < floorpartAnchorsB.Length; i++)
                {
                    if (floorpartAnchorsB[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                        cell.floorPropB[i] = (FloorProp)EditorGUILayout.ObjectField(cell.floorPropB[i], typeof(FloorProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);


        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Wall Back", _labelStyle);
        cell.wallBackPart = (WallCellPart)EditorGUILayout.ObjectField(cell.wallBackPart, typeof(WallCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.wallBackPart != null)
        {
            _showWallBackFoldoutF = EditorGUILayout.Foldout(_showWallBackFoldoutF, "Anchors Front");
            if (_showWallBackFoldoutF)
            {
                var wallBackpartAnchorsF = cell.wallBackPart.FAnchors;
                GUILayout.Space(1);

                for (int i = 0; i < wallBackpartAnchorsF.Length; i++)
                {
                    if (wallBackpartAnchorsF[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                        cell.wallBackPropF[i] = (WallProp)EditorGUILayout.ObjectField(cell.wallBackPropF[i], typeof(WallProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            _showWallBackFoldoutB = EditorGUILayout.Foldout(_showWallBackFoldoutB, "Anchors Back");
            if (_showWallBackFoldoutB)
            {
                var wallBackpartAnchorsB = cell.wallBackPart.BAnchors;
                GUILayout.Space(1);
                for (int i = 0; i < wallBackpartAnchorsB.Length; i++)
                {
                    if (wallBackpartAnchorsB[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                        cell.wallBackPropB[i] = (WallProp)EditorGUILayout.ObjectField(cell.wallBackPropB[i], typeof(WallProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);


        }


        GUILayout.BeginHorizontal();
        GUILayout.Label("Wall Left", _labelStyle);
        cell.wallLeftPart = (WallCellPart)EditorGUILayout.ObjectField(cell.wallLeftPart, typeof(WallCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.wallLeftPart != null)
        {
            _showWallLeftFoldoutF = EditorGUILayout.Foldout(_showWallLeftFoldoutF, "Anchors Front");
            if (_showWallLeftFoldoutF)
            {
                var wallLeftpartAnchorsF = cell.wallLeftPart.FAnchors;
                GUILayout.Space(1);

                for (int i = 0; i < wallLeftpartAnchorsF.Length; i++)
                {
                    if (wallLeftpartAnchorsF[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                        cell.wallLeftPropF[i] = (WallProp)EditorGUILayout.ObjectField(cell.wallLeftPropF[i], typeof(WallProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            _showWallLeftFoldoutB = EditorGUILayout.Foldout(_showWallLeftFoldoutB, "Anchors Back");
            if (_showWallLeftFoldoutB)
            {
                var wallLeftpartAnchorsB = cell.wallLeftPart.BAnchors;
                GUILayout.Space(1);
                for (int i = 0; i < wallLeftpartAnchorsB.Length; i++)
                {
                    if (wallLeftpartAnchorsB[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                        cell.wallLeftPropB[i] = (WallProp)EditorGUILayout.ObjectField(cell.wallLeftPropB[i], typeof(WallProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);


        }


        GUILayout.BeginHorizontal();
        GUILayout.Label("Beam Back", _labelStyle);
        cell.beamBackPart = (BeamCellPart)EditorGUILayout.ObjectField(cell.beamBackPart, typeof(BeamCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.beamBackPart != null)
        {
            _showBeamBackFoldoutF = EditorGUILayout.Foldout(_showBeamBackFoldoutF, "Anchors Front");
            if (_showBeamBackFoldoutF)
            {
                var beamBackpartAnchorsF = cell.beamBackPart.FAnchors;
                GUILayout.Space(1);

                for (int i = 0; i < beamBackpartAnchorsF.Length; i++)
                {
                    if (beamBackpartAnchorsF[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                        cell.beamBackPropF[i] = (BeamProp)EditorGUILayout.ObjectField(cell.beamBackPropF[i], typeof(BeamProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            _showBeamBackFoldoutB = EditorGUILayout.Foldout(_showBeamBackFoldoutB, "Anchors Back");
            if (_showBeamBackFoldoutB)
            {
                var beamBackpartAnchorsB = cell.beamBackPart.BAnchors;
                GUILayout.Space(1);
                for (int i = 0; i < beamBackpartAnchorsB.Length; i++)
                {
                    if (beamBackpartAnchorsB[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                        cell.beamBackPropB[i] = (BeamProp)EditorGUILayout.ObjectField(cell.beamBackPropB[i], typeof(BeamProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);
        }


        GUILayout.BeginHorizontal();
        GUILayout.Label("Beam Left", _labelStyle);
        cell.beamLeftPart = (BeamCellPart)EditorGUILayout.ObjectField(cell.beamLeftPart, typeof(BeamCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.beamLeftPart != null)
        {
            _showBeamLeftFoldoutF = EditorGUILayout.Foldout(_showBeamLeftFoldoutF, "Anchors Front");
            if (_showBeamLeftFoldoutF)
            {
                var beamLeftpartAnchorsF = cell.beamLeftPart.FAnchors;
                GUILayout.Space(1);

                for (int i = 0; i < beamLeftpartAnchorsF.Length; i++)
                {
                    if (beamLeftpartAnchorsF[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorF" + (i + 1), _labelStyle);
                        cell.beamLeftPropF[i] = (BeamProp)EditorGUILayout.ObjectField(cell.beamLeftPropF[i], typeof(BeamProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            _showBeamLeftFoldoutB = EditorGUILayout.Foldout(_showBeamLeftFoldoutB, "Anchors Back");
            if (_showBeamLeftFoldoutB)
            {
                var beamLeftpartAnchorsB = cell.beamLeftPart.BAnchors;
                GUILayout.Space(1);
                for (int i = 0; i < beamLeftpartAnchorsB.Length; i++)
                {
                    if (beamLeftpartAnchorsB[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("AnchorB" + (i + 1), _labelStyle);
                        cell.beamLeftPropB[i] = (BeamProp)EditorGUILayout.ObjectField(cell.beamLeftPropB[i], typeof(BeamProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);
        }



        GUILayout.BeginHorizontal();
        GUILayout.Label("Column", _labelStyle);
        cell.columnPart = (ColumnCellPart)EditorGUILayout.ObjectField(cell.columnPart, typeof(ColumnCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.columnPart != null)
        {
            _showColumnFoldout = EditorGUILayout.Foldout(_showColumnFoldout, "Anchors");
            if (_showColumnFoldout)
            {
                var columnpartAnchors = cell.columnPart.Anchors;
                GUILayout.Space(1);

                for (int i = 0; i < columnpartAnchors.Length; i++)
                {
                    if (columnpartAnchors[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Anchors" + (i + 1), _labelStyle);
                        cell.columnProp[i] = (ColumnProp)EditorGUILayout.ObjectField(cell.columnProp[i], typeof(ColumnProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);
        }



        GUILayout.BeginHorizontal();
        GUILayout.Label("Vertex", _labelStyle);
        cell.vertexPart = (VertexCellPart)EditorGUILayout.ObjectField(cell.vertexPart, typeof(VertexCellPart), false);
        GUILayout.EndHorizontal();

        if (cell.vertexPart != null)
        {
            _showVertexFoldout = EditorGUILayout.Foldout(_showVertexFoldout, "Anchors");
            if (_showVertexFoldout)
            {
                var vertexpartAnchors = cell.vertexPart.Anchors;
                GUILayout.Space(1);

                for (int i = 0; i < vertexpartAnchors.Length; i++)
                {
                    if (vertexpartAnchors[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Anchors" + (i + 1), _labelStyle);
                        cell.vertexProp[i] = (VertexProp)EditorGUILayout.ObjectField(cell.vertexProp[i], typeof(VertexProp), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(5);
        }

        if (GUILayout.Button("Finish"))
        {
            cell.FinishCell();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        }

        if (GUI.changed == true)
        {
            EditorUtility.SetDirty(cell);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            cell.Confirm();
        }
	}

}
