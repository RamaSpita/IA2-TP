using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellPart))]
public class CellPartCustom : Editor
 {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		var cellpart = (CellPart)target;
		
	}
}
