using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;

[ExecuteInEditMode][SelectionBase]
public class Cell : MonoBehaviour
{

#if (UNITY_EDITOR)


    private bool check = false;
    public bool finished = false;

    public float centerRotationY;

    public FloorCellPart floorPart = null;
    private FloorCellPart _lastFloorPart = null;

    public WallCellPart wallBackPart = null;
    private WallCellPart _lastWallBackPart = null;

    public WallCellPart wallLeftPart = null;
    private WallCellPart _lastWallLeftPart = null;

    public BeamCellPart beamBackPart = null;
    private BeamCellPart _lastBeamBackPart = null;

    public BeamCellPart beamLeftPart = null;
    private BeamCellPart _lastBeamLeftPart = null;

    public ColumnCellPart columnPart = null;
    private ColumnCellPart _lastColumnPart = null;

    public VertexCellPart vertexPart = null;
    private VertexCellPart _lastVertexPart = null;

    public FloorProp[] floorPropF = new FloorProp[9];
    private FloorProp[] _lastFloorPropF = new FloorProp[9];
    private GameObject[] _floorPropsObjectsF = new GameObject[9];

    public FloorProp[] floorPropB = new FloorProp[9];
    private FloorProp[] _lastFloorPropB = new FloorProp[9];
    private GameObject[] _floorPropsObjectsB = new GameObject[9];

    public WallProp[] wallBackPropF = new WallProp[9];
    private WallProp[] _lastwallBackPropF = new WallProp[9];
    private GameObject[] _wallBackPropsObjectsF = new GameObject[9];

    public WallProp[] wallBackPropB = new WallProp[9];
    private WallProp[] _lastwallBackPropB = new WallProp[9];
    private GameObject[] _wallBackPropsObjectsB = new GameObject[9];

    public WallProp[] wallLeftPropF = new WallProp[9];
    private WallProp[] _lastwallLeftPropF = new WallProp[9];
    private GameObject[] _wallLeftPropsObjectsF = new GameObject[9];

    public WallProp[] wallLeftPropB = new WallProp[9];
    private WallProp[] _lastwallLeftPropB = new WallProp[9];
    private GameObject[] _wallLeftPropsObjectsB = new GameObject[9];

    public BeamProp[] beamBackPropF = new BeamProp[3];
    private BeamProp[] _lastBeamBackPropF = new BeamProp[3];
    private GameObject[] _beamBackPropsObjectsF = new GameObject[3];

    public BeamProp[] beamBackPropB = new BeamProp[3];
    private BeamProp[] _lastBeamBackPropB = new BeamProp[3];
    private GameObject[] _beamBackPropsObjectsB = new GameObject[3];

    public BeamProp[] beamLeftPropF = new BeamProp[3];
    private BeamProp[] _lastBeamLeftPropF = new BeamProp[3];
    private GameObject[] _beamLeftPropsObjectsF = new GameObject[3];

    public BeamProp[] beamLeftPropB = new BeamProp[3];
    private BeamProp[] _lastBeamLeftPropB = new BeamProp[3];
    private GameObject[] _beamLeftPropsObjectsB = new GameObject[3];

    public ColumnProp[] columnProp = new ColumnProp[4];
    private ColumnProp[] _lastcolumnProp = new ColumnProp[4];
    private GameObject[] _columnPropsObjects = new GameObject[4];

    public VertexProp[] vertexProp = new VertexProp[1];
    private VertexProp[] _lastvertexProp = new VertexProp[1];
    private GameObject[] _vertexPropsObjects = new GameObject[1];

    private GameObject _floorObject;
    private GameObject _wallBackObject;
    private GameObject _wallLeftObject;
    private GameObject _beamBackObject;
    private GameObject _beamLeftObject;
    private GameObject _columnObject;
    private GameObject _vertexObject;

    #region Anchor Variables
    private Transform _floorAnchor;
	private List<Transform> _floorAnchors = new List<Transform>();
	
	private Transform _wallBackAnchor;
	private List<Transform> _wallBackAnchors = new List<Transform>();
	
	private Transform _wallLeftAnchor;
	private List<Transform> _wallLeftAnchors = new List<Transform>();
	
	private Transform _beamBackAnchor;
	private List<Transform> _beamBackAnchors = new List<Transform>();
	
	private Transform _beamLeftAnchor;
	private List<Transform> _beamLeftAnchors = new List<Transform>();
	
	private Transform _columnAnchor;
	private List<Transform> _columnAnchors = new List<Transform>();
	
	private Transform _vertexAnchor;
	private List<Transform> _vertexAnchors = new List<Transform>();

    #endregion

   
    public void UpdateCell()
    {
        //FinishCell();

        //OldEditCell();

    }

    public void FinishCell()
    {
        if (finished) return;

        FillAllAnchors();

        DestroyAllObjectsInListWithNoChilds(_floorAnchors);
        DestroyAllObjectsInListWithNoChilds(_wallBackAnchors);
        DestroyAllObjectsInListWithNoChilds(_wallLeftAnchors);
        DestroyAllObjectsInListWithNoChilds(_beamBackAnchors);
        DestroyAllObjectsInListWithNoChilds(_beamLeftAnchors);
        DestroyAllObjectsInListWithNoChilds(_columnAnchors);
        DestroyAllObjectsInListWithNoChilds(_vertexAnchors);
        finished = true;

    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            FinishCell();
            return;
        }
    }
    public void OldEditCell()
    {
        if (!finished) return;

        if (Application.isPlaying)
        {
            Debug.Log("Esta en Play");
            return;
        }

        System.Type type = this.GetType();
        var baseCell = Resources.Load<GameObject>("Prefabs/Cell");
        var newCellObject = Instantiate(baseCell);
        newCellObject.transform.position = transform.position;
        newCellObject.transform.rotation = transform.rotation;
        newCellObject.transform.localScale = transform.localScale;
		newCellObject.transform.parent = transform.parent;
        newCellObject.name = transform.name;
        var cell = newCellObject.GetComponent<Cell>();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(cell, field.GetValue(this));
        }
        cell.Confirm();
        cell.finished = false;
        Selection.activeGameObject =  newCellObject;
        DestroyImmediate(gameObject);

    }

    public void EditCell()
    {
        if (!finished) return;

        if (Application.isPlaying)
        {
            Debug.Log("Esta en Play");
            return;
        }

        FillBaseAnchors();

        var baseCell = Resources.Load<GameObject>("Prefabs/Cell").GetComponent<Cell>();
        var baseCellFloorAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _floorAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _floorAnchors = ReCreateAnchors(baseCellFloorAnchors,_floorAnchors,_floorAnchor,
            (i, y) =>
            {
                if (i < 9)
                {
                    if (_floorPropsObjectsF[i] != null)
                        _floorPropsObjectsF[i].transform.parent = y;
                }
                else
                {
                    if (_floorPropsObjectsB[i - 9] != null)
                        _floorPropsObjectsB[i - 9].transform.parent = y;
                }
            });


        var baseCellWallBackAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _wallBackAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _wallBackAnchors = ReCreateAnchors(baseCellWallBackAnchors,_wallBackAnchors,_wallBackAnchor, (i, y) =>
        {
            if (i < 9)
            {
                if (_wallBackPropsObjectsF[i] != null)
                    _wallBackPropsObjectsF[i].transform.parent = y;
            }
            else
            {
                if (_wallBackPropsObjectsB[i - 9] != null)
                    _wallBackPropsObjectsB[i - 9].transform.parent = y;
            }
        });

        var baseCellWallLeftAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _wallLeftAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _wallLeftAnchors = ReCreateAnchors(baseCellWallLeftAnchors, _wallLeftAnchors, _wallLeftAnchor, (i, y) =>
        {
            if (i < 9)
            {
                if (_wallLeftPropsObjectsF[i] != null)
                    _wallLeftPropsObjectsF[i].transform.parent = y;
            }
            else
            {
                if (_wallLeftPropsObjectsB[i - 9] != null)
                    _wallLeftPropsObjectsB[i - 9].transform.parent = y;
            }
        });

        var baseCellBeamBackAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _beamBackAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _beamBackAnchors = ReCreateAnchors(baseCellBeamBackAnchors, _beamBackAnchors, _beamBackAnchor, (i, y) =>
        {
            if (i < 3)
            {
                if (_beamBackPropsObjectsF[i] != null)
                    _beamBackPropsObjectsF[i].transform.parent = y;
            }
            else
            {
                if (_beamBackPropsObjectsB[i - 3] != null)
                    _beamBackPropsObjectsB[i - 3].transform.parent = y;
            }
        });

        var baseCellBeamLeftAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _beamLeftAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _beamLeftAnchors = ReCreateAnchors(baseCellBeamLeftAnchors, _beamLeftAnchors, _beamLeftAnchor, (i, y) =>
        {
            if (i < 3)
            {
                if (_beamLeftPropsObjectsF[i] != null)
                    _beamLeftPropsObjectsF[i].transform.parent = y;
            }
            else
            {
                if (_beamLeftPropsObjectsB[i - 3] != null)
                    _beamLeftPropsObjectsB[i - 3].transform.parent = y;
            }
        });

        var baseCellColumnAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _columnAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _columnAnchors = ReCreateAnchors(baseCellColumnAnchors, _columnAnchors, _columnAnchor, (i, y) =>
        {
            if (_columnPropsObjects[i] != null)
                _columnPropsObjects[i].transform.parent = y;
        });

        var baseCellVertexAnchors = baseCell.GetComponentsInChildren<Transform>().Where(x => x.name == _vertexAnchor.name).First().GetComponentsInChildren<Transform>().Skip(1);
        _vertexAnchors = ReCreateAnchors(baseCellVertexAnchors, _vertexAnchors, _vertexAnchor, (i, y) =>
        {
            if (_vertexPropsObjects[i] != null)
                _vertexPropsObjects[i].transform.parent = y;
        });

        FillAllAnchors();
        CheckIfObjects();

        finished = false;
    }

    private List<Transform> ReCreateAnchors(IEnumerable<Transform> baseAnchorsList, List<Transform> myAnchorsList, Transform newParent, Action<int, Transform> SetAnchorAsParent)
    {
        var anchorChilds = newParent.GetComponentsInChildren<Transform>().ToList();
        for (int i = 0; i < baseAnchorsList.Count(); i++)
        {
            var auxAnchor = anchorChilds.Where(x => (x != null && x.name == baseAnchorsList.Skip(i).First().name));
            GameObject anchor;
            if (auxAnchor.Any())
            {
                anchor = auxAnchor.First().gameObject;
            }
            else
            {
                anchor = new GameObject();
                anchor.transform.parent = newParent;
                anchor.name = baseAnchorsList.Skip(i).First().name;
                anchor.layer = LayerMask.NameToLayer("Anchor");
                anchor.transform.localEulerAngles = baseAnchorsList.Skip(i).First().localEulerAngles;
                anchor.transform.localPosition = baseAnchorsList.Skip(i).First().localPosition;
            }
           
            SetAnchorAsParent(i, anchor.transform);
            if (myAnchorsList.Count <= i)
                myAnchorsList.Add(anchor.transform);
            else
                myAnchorsList[i] = anchor.transform;
        }

        var auxAnchors = myAnchorsList.Where(x=>x.name.Contains("F")).OrderBy(x => x.name);
        var auxAnchors2 = myAnchorsList.Where(x=>x.name.Contains("B")).OrderBy(x => x.name);
        myAnchorsList = auxAnchors.Concat(auxAnchors2).ToList();

        for (int i = 0; i < myAnchorsList.Count; i++)
        {
            myAnchorsList[i].SetAsLastSibling();
        }

        return myAnchorsList;
    }

    void OnApplicationQuit()
    {
       
    }
    public void OnEnable()
    {
        FillBaseAnchors();
        BaseObjectsCheck();

        if (finished) return;

        FillAllAnchors();
        CheckIfObjects();

    }

    private void CheckIfObjects()
    {
        BaseObjectsCheck();

        for (int i = 0; i < _floorPropsObjectsF.Length; i++)
        {
            _floorPropsObjectsF[i] = CheckIfObjectOnPropAnchor(_floorAnchors[i]);
        }

        for (int i = 0; i < _floorPropsObjectsB.Length; i++)
        {
                 _floorPropsObjectsB[i] = CheckIfObjectOnPropAnchor(_floorAnchors[i + 9]);
        }

        for (int i = 0; i < _wallBackPropsObjectsF.Length; i++)
        {
            _wallBackPropsObjectsF[i] = CheckIfObjectOnPropAnchor(_wallBackAnchors[i]);
        }

        for (int i = 0; i < _wallBackPropsObjectsB.Length; i++)
        {
            _wallBackPropsObjectsB[i] = CheckIfObjectOnPropAnchor(_wallBackAnchors[i + 9]);
        }

        for (int i = 0; i < _wallLeftPropsObjectsF.Length; i++)
        {
            _wallLeftPropsObjectsF[i] = CheckIfObjectOnPropAnchor(_wallLeftAnchors[i]);
        }

        for (int i = 0; i < _wallLeftPropsObjectsB.Length; i++)
        {
            _wallLeftPropsObjectsB[i] = CheckIfObjectOnPropAnchor(_wallLeftAnchors[i + 9]);
        }

        for (int i = 0; i < _beamBackPropsObjectsF.Length; i++)
        {
            _beamBackPropsObjectsF[i] = CheckIfObjectOnPropAnchor(_beamBackAnchors[i]);
        }

        for (int i = 0; i < _beamBackPropsObjectsB.Length; i++)
        {
            _beamBackPropsObjectsB[i] = CheckIfObjectOnPropAnchor(_beamBackAnchors[i + 3]);
        }

        for (int i = 0; i < _beamLeftPropsObjectsF.Length; i++)
        {
            _beamLeftPropsObjectsF[i] = CheckIfObjectOnPropAnchor(_beamLeftAnchors[i]);
        }

        for (int i = 0; i < _beamLeftPropsObjectsB.Length; i++)
        {
            _beamLeftPropsObjectsB[i] = CheckIfObjectOnPropAnchor(_beamLeftAnchors[i + 3]);
        }

        for (int i = 0; i < _columnPropsObjects.Length; i++)
        {
            _columnPropsObjects[i] = CheckIfObjectOnPropAnchor(_columnAnchors[i]);
        }

        for (int i = 0; i < _vertexPropsObjects.Length; i++)
        {
            _vertexPropsObjects[i] = CheckIfObjectOnPropAnchor(_vertexAnchors[i]);
        }
    }

    private void BaseObjectsCheck()
    {
        _floorObject = CheckIfObject(_floorAnchor);
        _wallBackObject = CheckIfObject(_wallBackAnchor);
        _wallLeftObject = CheckIfObject(_wallLeftAnchor);
        _beamBackObject = CheckIfObject(_beamBackAnchor);
        _beamLeftObject = CheckIfObject( _beamLeftAnchor);
        _columnObject = CheckIfObject(_columnAnchor);
        _vertexObject = CheckIfObject(_vertexAnchor);
    }

    private void FillAllAnchors()
    {
        if (_floorAnchor == null)
            _floorAnchor = transform.Find("Floor");

        _floorAnchors = CreateChildList(_floorAnchor);

        if (_wallBackAnchor == null)
            _wallBackAnchor = transform.Find("WallBack");

        _wallBackAnchors = CreateChildList(_wallBackAnchor);

        if (_wallLeftAnchor == null)
            _wallLeftAnchor = transform.Find("WallLeft");

        _wallLeftAnchors = CreateChildList(_wallLeftAnchor);

        if (_beamBackAnchor == null)
            _beamBackAnchor = transform.Find("BeamBack");

        _beamBackAnchors = CreateChildList(_beamBackAnchor);

        if (_beamLeftAnchor == null)
            _beamLeftAnchor = transform.Find("BeamLeft");

        _beamLeftAnchors = CreateChildList(_beamLeftAnchor);

        if (_columnAnchor == null)
            _columnAnchor = transform.Find("Column");

        _columnAnchors = CreateChildList(_columnAnchor);

        if (_vertexAnchor == null)
            _vertexAnchor = transform.Find("Vertex");

        _vertexAnchors = CreateChildList(_vertexAnchor);
    }
    private void FillBaseAnchors()
    {
        if (_floorAnchor == null)
            _floorAnchor = transform.Find("Floor");

        if (_wallBackAnchor == null)
            _wallBackAnchor = transform.Find("WallBack");

        if (_wallLeftAnchor == null)
            _wallLeftAnchor = transform.Find("WallLeft");

        if (_beamBackAnchor == null)
            _beamBackAnchor = transform.Find("BeamBack");

        if (_beamLeftAnchor == null)
            _beamLeftAnchor = transform.Find("BeamLeft");

        if (_columnAnchor == null)
            _columnAnchor = transform.Find("Column");

        if (_vertexAnchor == null)
            _vertexAnchor = transform.Find("Vertex");

    }

    private GameObject CheckIfObject(Transform anchor)
    {
        if (anchor.childCount == 0)
            return null;

        foreach (Transform item in anchor)
        {
            if (item.gameObject.layer != LayerMask.NameToLayer("Anchor"))
            {
                return item.gameObject;
            }
        }
        return null;
    }
    private GameObject CheckIfObjectOnPropAnchor(Transform anchor)
    {
        if (anchor == null || anchor.childCount == 0)
            return null;

        return anchor.GetChild(0).gameObject;
    }

    void OnValidate()
    {
        if (finished) return;
        check = true;
    }

    void Update()
    {
        if (finished) return;
        if (check)
        {
            Confirm();
            check = false;
        }

    }

    private List<Transform> CreateChildList(Transform parent)
    {
		if(parent == null)
			return null;
		List<Transform> auxChilds = new List<Transform>();

		foreach(Transform item in parent)
		{
            if (item.gameObject.layer == LayerMask.NameToLayer("Anchor"))
			    auxChilds.Add(item);
		}
		return auxChilds;
    }

    private void DestroyAllObjectsInListWithNoChilds(List<Transform> list)
    {
        foreach (Transform item in list)
        {
            if (item != null && item.childCount == 0)
               DestroyImmediate(item.gameObject);
        }
    }

    public void Confirm()
   {
        if (finished) return;

        if (floorPart != _lastFloorPart)
        {
            _lastFloorPart = floorPart;
            _floorObject = _floorObject.ObjectChange(floorPart, _floorAnchor, transform);
        }

        if (wallBackPart != _lastWallBackPart)
        {
            _lastWallBackPart = wallBackPart;
            _wallBackObject = _wallBackObject.ObjectChange(wallBackPart, _wallBackAnchor, transform);
        }

        if (wallLeftPart != _lastWallLeftPart)
        {
            _lastWallLeftPart = wallLeftPart;
            _wallLeftObject = _wallLeftObject.ObjectChange(wallLeftPart, _wallLeftAnchor, transform);
        }

        if (beamBackPart != _lastBeamBackPart)
        {
            _lastBeamBackPart = beamBackPart;
            _beamBackObject = _beamBackObject.ObjectChange(beamBackPart, _beamBackAnchor, transform);
        }

        if (beamLeftPart != _lastBeamLeftPart)
        {
            _lastBeamLeftPart = beamLeftPart;
            _beamLeftObject = _beamLeftObject.ObjectChange(beamLeftPart, _beamLeftAnchor, transform);
        }

        if (columnPart != _lastColumnPart)
        {
            _lastColumnPart = columnPart;
            _columnObject = _columnObject.ObjectChange(columnPart, _columnAnchor, transform);
        }

        if (vertexPart != _lastVertexPart)
        {
            _lastVertexPart = vertexPart;
            _vertexObject = _vertexObject.ObjectChange(vertexPart, _vertexAnchor, transform);
        }

        for (int i = 0; i < floorPropF.Length; i++)
        {
            if (floorPropF[i] != _lastFloorPropF[i])
            {
                _lastFloorPropF[i] = floorPropF[i];
                _floorPropsObjectsF[i] = _floorPropsObjectsF[i].PropChange(floorPropF[i], _floorAnchors[i], transform);
            }
        }
        for (int i = 0; i < floorPropB.Length; i++)
        {
            if (floorPropB[i] != _lastFloorPropB[i])
            {
                _lastFloorPropB[i] = floorPropB[i];
                _floorPropsObjectsB[i] = _floorPropsObjectsB[i].PropChange(floorPropB[i], _floorAnchors[i+9], transform);
            }
        }
        for (int i = 0; i < wallBackPropF.Length; i++)
        {
            if (wallBackPropF[i] != _lastwallBackPropF[i])
            {
                _lastwallBackPropF[i] = wallBackPropF[i];
                _wallBackPropsObjectsF[i] = _wallBackPropsObjectsF[i].PropChange(wallBackPropF[i], _wallBackAnchors[i], transform);
            }
        }

        for (int i = 0; i < wallBackPropB.Length; i++)
        {
            if (wallBackPropB[i] != _lastwallBackPropB[i])
            {
                _lastwallBackPropB[i] = wallBackPropB[i];
                _wallBackPropsObjectsB[i] = _wallBackPropsObjectsB[i].PropChange(wallBackPropB[i], _wallBackAnchors[i + 9], transform);
            }
        }

        for (int i = 0; i < wallLeftPropF.Length; i++)
        {
            if (wallLeftPropF[i] != _lastwallLeftPropF[i])
            {
                _lastwallLeftPropF[i] = wallLeftPropF[i];
                _wallLeftPropsObjectsF[i] = _wallLeftPropsObjectsF[i].PropChange(wallLeftPropF[i], _wallLeftAnchors[i], transform);
            }
        }

        for (int i = 0; i < wallLeftPropB.Length; i++)
        {
            if (wallLeftPropB[i] != _lastwallLeftPropB[i])
            {
                _lastwallLeftPropB[i] = wallLeftPropB[i];
                _wallLeftPropsObjectsB[i] = _wallLeftPropsObjectsB[i].PropChange(wallLeftPropB[i], _wallLeftAnchors[i + 9], transform);
            }
        }

        for (int i = 0; i < beamBackPropF.Length; i++)
        {
            if (beamBackPropF[i] != _lastBeamBackPropF[i])
            {
                _lastBeamBackPropF[i] = beamBackPropF[i];
                _beamBackPropsObjectsF[i] = _beamBackPropsObjectsF[i].PropChange(beamBackPropF[i], _beamBackAnchors[i], transform);
            }
        }

        for (int i = 0; i < beamBackPropB.Length; i++)
        {
            if (beamBackPropB[i] != _lastBeamBackPropB[i])
            {
                _lastBeamBackPropB[i] = beamBackPropB[i];
                _beamBackPropsObjectsB[i] = _beamBackPropsObjectsB[i].PropChange(beamBackPropB[i], _beamBackAnchors[i + 3], transform);
            }
        }

        for (int i = 0; i < beamLeftPropF.Length; i++)
        {
            if (beamLeftPropF[i] != _lastBeamLeftPropF[i])
            {
                _lastBeamLeftPropF[i] = beamLeftPropF[i];
                _beamLeftPropsObjectsF[i] = _beamLeftPropsObjectsF[i].PropChange(beamLeftPropF[i], _beamLeftAnchors[i], transform);
            }
        }

        for (int i = 0; i < beamLeftPropB.Length; i++)
        {
            if (beamLeftPropB[i] != _lastBeamLeftPropB[i])
            {
                _lastBeamLeftPropB[i] = beamLeftPropB[i];
                _beamLeftPropsObjectsB[i] = _beamLeftPropsObjectsB[i].PropChange(beamLeftPropB[i], _beamLeftAnchors[i + 3], transform);
            }
        }

        for (int i = 0; i < columnProp.Length; i++)
        {
            if (columnProp[i] != _lastcolumnProp[i])
            {
                _lastcolumnProp[i] = columnProp[i];
                _columnPropsObjects[i] = _columnPropsObjects[i].PropChange(columnProp[i], _columnAnchors[i], transform);
            }
        }

        for (int i = 0; i < vertexProp.Length; i++)
        {
            if (vertexProp[i] != _lastvertexProp[i])
            {
                _lastvertexProp[i] = vertexProp[i];
                _vertexPropsObjects[i] = _vertexPropsObjects[i].PropChange(vertexProp[i], _vertexAnchors[i],transform);
            }
        }

        var centerFloorObject = _floorPropsObjectsF[4];
        if (centerFloorObject != null)
        {
            var centerObjectRotation = centerFloorObject.transform.eulerAngles;
            centerObjectRotation.y = centerRotationY;
            centerFloorObject.transform.eulerAngles = centerObjectRotation;
        }

    }

#endif
}
