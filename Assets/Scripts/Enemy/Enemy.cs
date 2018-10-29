using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{

    [Header("Enemy parameters")]
    public int life;
    public float speed;
    [Header("Can Open Door?")]
    public bool canOpenDoor;
    [Header("To set if its constant or variable")]
    public PlayerReference playerVariables;
    [Header("Patrol Type")]
    public _patrolType patroltype;
    public enum _patrolType
    {
        loop,
        goAndRetrurn,
        random
    }
    public float heuristicMultiplier = 1f;

    //Path finding
    protected List<Vector3> _path = new List<Vector3>();
    private List<Vector3> _randomPath = new List<Vector3>();

    //Waypoint
    [Header("Waypoint")]
    public GameObject waypointsContainer;

    public List<Node> _waypoints;
    private int _next;
    private int _direction;

    protected Rigidbody _rb;

    protected bool _receiveDamage;

    public virtual void Start ()
    {
        #region waypoints setup
        _waypoints = new List<Node>();
        if (waypointsContainer != null)
        {
            for (int i = 0; i < waypointsContainer.transform.childCount; i++)
            {
                _waypoints.Add(waypointsContainer.transform.GetChild(i).GetComponent<Waypoint>().NodePos);
            }
        }
        Destroy(waypointsContainer);
        _next = 0;
        _direction = 1;
        #endregion

        _rb = GetComponent<Rigidbody>();
        _receiveDamage = true;
    }
    protected void Move(Vector3 positionToArrive)
    {
        var directionToFollow = (positionToArrive - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, directionToFollow != Vector3.zero ? directionToFollow : transform.forward, Time.deltaTime * 10);
        float y = _rb.velocity.y;
        directionToFollow = directionToFollow * speed;
        _rb.velocity = new Vector3(directionToFollow.x, y, directionToFollow.z);
    }
    //WayPoints
    #region WayPoints Movement
    public void EnterWaypointMovement()
    {
        if (_waypoints.Count > 0) _path = Search(GetCurrentNode(), _waypoints[_next]);
    }
    public void MoveWayPoints()
    {
        if (_waypoints.Count > 0)
        {
            #region Patrol types
            var nexWaypoint = new Vector3(_waypoints[_next].center.x, transform.position.y, _waypoints[_next].center.z);
            if (patroltype == _patrolType.loop)
            {
                if (Vector3.Distance(transform.position, nexWaypoint) >= Time.deltaTime * speed)
                {
                    MovePathWayPoints();
                }
                else
                {
                    _next += _direction;
                    if (_next >= _waypoints.Count) _next = 0;
                    //Get Path
                    _path = Search(GetCurrentNode(), _waypoints[_next]);
                    nexWaypoint = new Vector3(_waypoints[_next].center.x, transform.position.y, _waypoints[_next].center.z);
                }
            }
            else if (patroltype == _patrolType.goAndRetrurn)
            {
                if (Vector3.Distance(transform.position, nexWaypoint) >= Time.deltaTime * speed)
                {
                    MovePathWayPoints();
                }
                else
                {
                    _next += _direction;
                    if (_next >= _waypoints.Count)
                    {
                        _direction = _direction * -1;
                        _next = _waypoints.Count - 1;
                    }
                    if (_next < 0)
                    {
                        _direction = _direction * -1;
                        _next = 0;
                    }
                    //Get Path
                    _path = Search(GetCurrentNode(), _waypoints[_next]);
                    nexWaypoint = new Vector3(_waypoints[_next].center.x, transform.position.y, _waypoints[_next].center.z);
                }
            }
            else if (patroltype == _patrolType.random)
            {
                if (Vector3.Distance(transform.position, nexWaypoint) >= Time.deltaTime * speed)
                {
                    MovePathWayPoints();
                }
                else
                {
                    int random = UnityEngine.Random.Range(0, _waypoints.Count);
                    while (random == _next && random == _waypoints.Count)
                    {
                        random = UnityEngine.Random.Range(0, _waypoints.Count - 1);
                    }
                    _next = random;
                    //Get Path
                    _path = Search(GetCurrentNode(), _waypoints[_next]);
                    nexWaypoint = new Vector3(_waypoints[_next].center.x, transform.position.y, _waypoints[_next].center.z);
                }
            }
            #endregion
        }
        else MoveRandom();
    }
    public void MovePathWayPoints()
    {
        if (_path != null && _waypoints.Count > 0) 
        {
            if (_path.Count > 0)
            {
                var nextNode = new Vector3(_path[0].x, transform.position.y, _path[0].z);
                if (Vector3.Distance(transform.position, nextNode) < Time.deltaTime * speed)
                {
                    _path.RemoveAt(0);
                }
                if (_path.Count > 0) Move(nextNode);
            }
        }
        else MoveRandom();
    }
    #endregion
    //Chase
    #region Chase Movement
    public void EnterPathMovement()
    {
        _path = Search(GetCurrentNode(), playerVariables.PlayerNode);
    }
    public void MovePathChase()
    {
        if (_path != null)
        {
            if (_path.Count > 0)
            {
                var nextNode = new Vector3(_path[0].x, transform.position.y, _path[0].z);
                Move(nextNode);
                if (Vector3.Distance(transform.position, nextNode) < Time.deltaTime * speed)
                {
                    _path = Search(GetCurrentNode(), playerVariables.PlayerNode);
                }
            }
            else
            {
                var n = GetCurrentNode();
                if (playerVariables.PlayerNode != n) _path = Search(GetCurrentNode(), playerVariables.PlayerNode);
                Move(new Vector3(playerVariables.Position.x, transform.position.y, playerVariables.Position.z));
            }
        }
    }
    #endregion

    //Move Random
    #region Random Movement
    public void MoveRandom()
    {
        if (_randomPath != null && _randomPath.Count > 0)
        {
            var nextNode = new Vector3(_randomPath[0].x, transform.position.y, _randomPath[0].z);
            if (Vector3.Distance(transform.position, nextNode) < Time.deltaTime * speed)
            {
                _randomPath.RemoveAt(0);
            }
            if (_randomPath.Count > 0) Move(nextNode);
        }
        else _randomPath = GetRandompath();
    }

    public List<Vector3> GetRandompath()
    {
        Node currentNode = GetCurrentNode();
        if (currentNode == null) return null;
        currentNode = currentNode.transitable ? currentNode : getClosetsNode(currentNode);
        Node finalnode = currentNode;
        for (int i = 0; i < 4; i++)
        {
            var neighbour = canOpenDoor ? finalnode.neighbours.Concat(finalnode.neighboursDoor.Where(x => !finalnode.neighbours.Contains(x))).ToList() : finalnode.neighbours;
            int aux = UnityEngine.Random.Range(0, neighbour.Count);
            finalnode = neighbour.Skip(aux).Where(x => !x.Equals(finalnode)).DefaultIfEmpty(finalnode).First();
        }
        return Search(currentNode, finalnode);
    }
    #endregion

    //Path
    #region GetPath
    public List<Vector3> Search(Node actualNode, Node targetNode)
    {
        if (actualNode == null || targetNode == null) { return null; }
        var path = AStar.Run(actualNode, targetNode, Satisfies, GetWightedNeighbours, Heuristic);
        return (path == null) ? path : CheckClosetsNode(path[0]) ? path.Skip(1).ToList() : path;
    }
    //GetNodes
    public Node GetCurrentNode()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = transform.position + new Vector3(0, 0.5f, 0),
            direction = -Vector3.up
        };
        var _mask = 1 << LayerMask.NameToLayer("Node");
        if (Physics.Raycast(ray, out hitInfo, 15, _mask))
        {
            var node = hitInfo.transform.GetComponent<Node>();
            if (node != null)
            {
                return node.transitable ? node : getClosetsNode(node);
            }
        }
        return null;
    }
    //Find a node to go when current node is not transitable
    private Node getClosetsNode(Node node)
    {
        return node.neighbours.OrderBy(x => Vector3.Distance(transform.position, x.center)).Where(x => x.transitable).First();
    }
    //Check if the first node can be skipped
    private bool CheckClosetsNode(Vector3 firstNode)
    {
        var mypos = new Vector3(transform.position.x, 0, transform.position.z);
        var nodepos = new Vector3(firstNode.x, 0, firstNode.z);
        return Vector3.Distance(mypos, nodepos) <= Time.deltaTime * speed * 2;
    }
    //Path Logic
    private List<Tuple<Node, float>> GetWightedNeighbours(Node current)
    {
        var weightedNeighbours = new List<Tuple<Node, float>>();

        foreach (var item in current.neighbours)
        {
             weightedNeighbours.Add(Tuple.Create(item, Vector3.Distance(current.transform.position,item.transform.position) * item.cost));
        }
        if (canOpenDoor)
        {
            weightedNeighbours.Concat(current.neighboursDoor.Where(x =>
            {
                foreach (var item in weightedNeighbours)
                {
                    if (item.Item1 == x) return false;
                }
                return true;
            }).Select(x => Tuple.Create(x, Vector3.Distance(current.transform.position, x.transform.position) * x.cost)).ToList());
        }
        return weightedNeighbours;
    }
    private bool Satisfies(Node node,Node targetNode)
    {
        return node.Equals(targetNode);
    }
    private float Heuristic(Node node, Node targetNode)
    {
        return EuclideanDist(targetNode, node) * heuristicMultiplier;
    }
    private float EuclideanDist(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
    #endregion

    //para probar recibir ataque
    #region Receive Damage
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Attack"))
        {
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            if (playerAttack != null) ReceiveDamage(playerAttack.damage);
        }
    }
    public virtual void ReceiveDamage(int damage)
    {
    }
    #endregion
}
