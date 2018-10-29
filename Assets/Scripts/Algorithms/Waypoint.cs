using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Node NodePos
    {
        get
        {
            return GetNodeOnPos(transform.position);
        }
    }
    
    private Node GetNodeOnPos(Vector3 pos)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray();
        ray.origin = pos + new Vector3(0, 0.5f, 0);
        ray.direction = -Vector3.up;
        var mask = 1 << LayerMask.NameToLayer("Node");
        if (Physics.Raycast(ray, out hitInfo, 10, mask)) 
        {
            var node = hitInfo.transform.GetComponent<Node>();
            if (node != null && node.transitable)
                return node; 
        }
        return null;
    }
}
