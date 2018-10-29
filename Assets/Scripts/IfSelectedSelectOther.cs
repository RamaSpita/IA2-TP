#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class IfSelectedSelectOther : MonoBehaviour {

    public Transform targetToSelect; 

    private void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject == gameObject && SceneView.focusedWindow.titleContent.text == "Scene")
        {
            Selection.activeGameObject = targetToSelect.gameObject;
        }
    }
}
#endif

