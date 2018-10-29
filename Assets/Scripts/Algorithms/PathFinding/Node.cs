using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Node : MonoBehaviour
{
    [Header("To see Neighbours")]
    public List<Node> neighbours = new List<Node>();
    [Header("Neighbousrs Door")]
    public List<Node> neighboursDoor = new List<Node>();
    [Header("Cost to pass")]
    public float cost;
    [Header("Layer To see")]
    public LayerMask mask;
    [Header("Can NPC Walk?")]
    public bool transitable;
    [Header("Stair?")]
    public bool stair;

    public Vector3 center { get { return _center; } }
    private Vector3 _center;
    private List<Vector3> _directions = new List<Vector3>();


    private void Awake()
    {
        _center = !stair ? transform.position : transform.position + new Vector3(0, 0.3f, 0);
        if (!stair)
        {
            _directions.Add((center + Vector3.right * 2) - (center + Vector3.up));
            _directions.Add((center - Vector3.right * 2) - (center + Vector3.up));
            _directions.Add((center - Vector3.forward * 2) - (center + Vector3.up));
            _directions.Add((center + Vector3.forward * 2) - (center + Vector3.up));
        }
    }

    private void Start()
    {
        GetNeighbours();
    }

    private void GetNeighbours()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray();
        ray.origin = center + Vector3.up;

        for (int i = 0; i < _directions.Count; i++)
        {
            ray.direction = _directions[i];
            if (Physics.Raycast(ray, out hitInfo, 2.5f, mask))
            {
                var node = hitInfo.transform.GetComponent<Node>();
                if (node != null)
                { if (node.transitable) neighbours.Add(node); }
                else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Stairs"))
                {
                    node = hitInfo.transform.GetComponentInChildren<Transform>().Find("Node").GetComponent<Node>();
                    if (node.transitable)
                    {
                        neighbours.Add(node);
                        node.neighbours.Add(this);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {       
        for (int i = 0; i < neighbours.Count; i++)
        {
            Gizmos.color = transitable ? Color.yellow : Color.red;
            Gizmos.DrawLine(center + new Vector3(0, 0.1f, 0), neighbours[i].center + new Vector3(0, 0.1f, 0));
        }
        for (int i = 0; i < neighboursDoor.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center + new Vector3(0, 0.1f, 0), neighboursDoor[i].center + new Vector3(0, 0.1f, 0));
        }
    }
}
