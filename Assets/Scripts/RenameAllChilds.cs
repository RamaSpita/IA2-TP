using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameAllChilds : MonoBehaviour
{

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
    public void Sa()
    {
        var sa = GameObject.FindObjectsOfType<IfSelectedSelectOther>();

        foreach (var item in sa)
        {
            var sa2 = item.transform.GetComponents<IfSelectedSelectOther>();

            for (int i = 0; i < sa2.Length; i++)
            {
                if (sa2[i]!= item)
                {
                    DestroyImmediate(sa2[i]);
                }
            }
        }

    }


    public void Chau2()
    {
        var sa = GameObject.FindObjectsOfType<IfSelectedSelectOther>();

        foreach (var item in sa)
        {
            if (item.transform.parent.GetComponent<Cell>() != null)
            {
                item.targetToSelect = item.transform.parent;
            }
            else if(item.transform.parent.parent.GetComponent<Cell>() != null)
            {
                item.targetToSelect = item.transform.parent.parent;
            }
            else
            {
                item.targetToSelect = item.transform.parent.parent.parent;

            }
        }

    }
}
