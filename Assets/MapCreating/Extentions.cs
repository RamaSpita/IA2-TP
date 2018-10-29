#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class Extentions
{
    public static GameObject ObjectChange(this GameObject partObject, CellPart spawnPart, Transform anchor, Transform onSelectedSelect)
    {

        if (Application.isPlaying)
            return partObject;

        if (partObject != null) Object.DestroyImmediate(partObject);

        if (spawnPart == null)
            return null;
        var partObjectAux = (GameObject)PrefabUtility.InstantiatePrefab(spawnPart.prop);
        partObjectAux.AddComponent<IfSelectedSelectOther>().targetToSelect = onSelectedSelect;
        partObjectAux.transform.parent = anchor;
        partObjectAux.transform.position = anchor.position;
        partObjectAux.transform.localScale = anchor.localScale;
        partObjectAux.transform.rotation = anchor.rotation;

        return partObjectAux;

    }
    public static GameObject PropChange(this GameObject partObject, Prop spawnProp, Transform anchor, Transform onSelectedSelect)
    {

        if (Application.isPlaying)
            return partObject;

        if (partObject != null) Object.DestroyImmediate(partObject);

        if (spawnProp == null)
            return null;
        var partObjectAux = (GameObject)PrefabUtility.InstantiatePrefab(spawnProp.prop);
        partObjectAux.transform.parent = anchor;
        partObjectAux.AddComponent<IfSelectedSelectOther>().targetToSelect = onSelectedSelect;
        partObjectAux.transform.position = anchor.position;
        partObjectAux.transform.localScale = anchor.localScale;
        partObjectAux.transform.rotation = anchor.rotation;

        return partObjectAux;

    }

}
#endif
