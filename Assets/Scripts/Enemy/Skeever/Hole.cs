using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Transform anchor;
    public LayerMask mask;
    public Node myNode;

    private void Awake()
    {
        if (EnemyVariables.Skeever_Holes == null) EnemyVariables.Skeever_Holes = new List<Hole>();
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = anchor.transform.position,
            direction = -Vector3.up
        };
        if (Physics.Raycast(ray, out hitInfo, 5, mask))
        {
            var node = hitInfo.transform.GetComponent<Node>();
            if (node != null)
            {
                myNode = node;
            }
        }
        if (!EnemyVariables.Skeever_Holes.Contains(this))
        {
            EnemyVariables.Skeever_Holes.Add(this);
        }
    }

}
