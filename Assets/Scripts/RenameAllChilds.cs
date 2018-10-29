using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameAllChilds : MonoBehaviour {

    public string baseName = "Cell ";
    public bool OnlyRenameCellChilds = true;
    public void Rename()
    {
        int count = 0;

        if (OnlyRenameCellChilds)
        {
            var cellChilds = transform.GetComponentsInChildren<Cell>();
            foreach (var item in cellChilds)
            {
                item.transform.name = baseName + count;
                count++;
            }
            return;
        }
        var childs = transform.GetComponentsInChildren<Transform>();
        foreach (var item in childs)
        {
            item.name = baseName + count;
            count++;
        }

    }
}
