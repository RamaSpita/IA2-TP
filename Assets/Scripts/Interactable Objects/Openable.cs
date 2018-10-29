using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Openable : MonoBehaviour, Iinteractable
{
    private List<Vector3> _directions = new List<Vector3>();
    public List<Node> mynodes = new List<Node>();
    public PlayerReference playerVariables;

    public virtual void Awake()
    {
        GetNeighbours();
    }

    private void GetNeighbours()
    {
        //cambia con la pared nueva es para probar
        RaycastHit hitInfo;
        Ray ray = new Ray()
        {
            origin = transform.position + transform.up
        };
        _directions.Add((-Vector3.up + transform.forward * 2).normalized);
        _directions.Add((-Vector3.up - transform.forward * 2).normalized);
        var mask = 1 << LayerMask.NameToLayer("Node");
        for (int i = 0; i < _directions.Count; i++)
        {
            ray.direction = _directions[i];
            if (Physics.Raycast(ray, out hitInfo, 3, mask))
            {
                var node = hitInfo.transform.GetComponent<Node>();
                if (node != null)
                {
                    mynodes.Add(node);
                }
            }
        }
    }
    public virtual void Interact(params object[] obj)
    {
        for (int i = 0; i < mynodes.Count; i++)
        {
            mynodes[i].neighbours.AddRange(mynodes.Where(x => x != mynodes[i]));
        }
    }

}
