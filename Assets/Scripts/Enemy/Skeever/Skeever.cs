using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Animator))]
public class Skeever : Enemy
{
    public bool showRadius = true;
    [Header("Set smell detection")]
    public float smellDetectionRad;
    public float smellingDetectionTime = 2;
    [Header("Set view detection")]
    public float viewdetectionRad;
    [Header("Set immediate detection")]
    public float detectionRad;
    [Header("Set Attack radious")]
    public float attackRad;
    [Header("Jump Attack Force")]
    public float jumpForce;
    [Header("Querie")]
    public bool tellOthers = false;
    public Queries3d querie;

    [Header("Extras")]
    public Animator anim;
    public GameObject attack;
    public MeshRenderer material;

    private List<Vector3> _holePath;
    private List<Node> _holeNode;

    private float _smellingTimer;
    private Skeever_States _stateMachine;
    private int _lifeToScape;
    private bool _canScape;

    public override void Start ()
    {
        querie.targetGrid = GameObject.FindObjectOfType<SpatialGrid3d>();

        anim = GetComponent<Animator>();
        base.Start();

        _stateMachine = new Skeever_States(this);

        _smellingTimer = 0;

        _lifeToScape = life / 2;
        _canScape = true;
    }

    void Update()
    {
        if (Vector3.Distance(playerVariables.Position, transform.position) < attackRad + UnityEngine.Random.value * 0.5f)
            _stateMachine.SendFsmInput(EnemyInput.PrepareAttack);
        if (life < _lifeToScape && _canScape)
            _stateMachine.SendFsmInput(EnemyInput.Scape);


        _stateMachine.Update();
    }

    //Smell
    public void SmellEnter()
    {
        _smellingTimer = 0;
    }
    public void SmellingForPlayer()
    {
        if (Vector3.Distance(playerVariables.Position, transform.position) > smellDetectionRad + 2)
            _stateMachine.SendFsmInput(EnemyInput.Patrol);
        if (Vector3.Distance(playerVariables.Position, transform.position) < viewdetectionRad && OnSight())
            _stateMachine.SendFsmInput(EnemyInput.Chase);
        if (Vector3.Distance(playerVariables.Position, transform.position) < detectionRad)
            _stateMachine.SendFsmInput(EnemyInput.Chase);
        if (_smellingTimer > smellingDetectionTime)
            _stateMachine.SendFsmInput(EnemyInput.Chase);
        else       
            _smellingTimer += Time.deltaTime;
        //anim.Play("Smell");
    }
    private bool OnSight()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = transform.position + new Vector3(0, 0.5f, 0),
            direction = transform.forward
        };
        LayerMask mask = 1 << LayerMask.NameToLayer("Enemy");
        mask += LayerMask.NameToLayer("Enemy Attack");
        if (Physics.Raycast(ray, out hitInfo, mask))
        {
            if (hitInfo.transform.GetComponent<Player>() != null)
                return true;
        }
        return false;
    }
    //Patrol
    public void Patrol()
    {
        MoveWayPoints();
        if (Vector3.Distance(playerVariables.Position, transform.position) < smellDetectionRad)
        {
            var aux = _path;
            Search(GetCurrentNode(), playerVariables.PlayerNode);
            if (_path != null) _stateMachine.SendFsmInput(EnemyInput.Smell);
            else
            {
                _path = aux;
            }
        }
           
    }

    public void PlayerSpottet()
    {
        if (querie.targetGrid!= null)
        {
            var closeSkievers = querie.Query().Select(x => x.GetComponent<Skeever>()).Where(y => y!= null && y!= this);

            foreach (var enemy in closeSkievers)
            {
                enemy.CheckIfCanGoToPlayer();
            }
        }
    }

    public void CheckIfCanGoToPlayer()
    {
        if (Vector3.Distance(playerVariables.Position, transform.position) > attackRad)
        {
            var aux = _path;
            Search(GetCurrentNode(), playerVariables.PlayerNode);
            if (_path != null)
            {
                Debug.Log(transform.name);
                _stateMachine.SendFsmInput(EnemyInput.Chase);
            }
            else
            {
                _path = aux;
            }
        }
    }


    //Return To Hole
    public void EnterScapeMovement()
    {
        if (EnemyVariables.Skeever_Holes == null || EnemyVariables.Skeever_Holes.Count == 0)
        {
            _path = null;
            _canScape = false;
            return;
        }
        var n = EnemyVariables.Skeever_Holes.OrderBy(x => Vector3.Distance(transform.position, x.myNode.center)).ToList();
        var mynode = GetCurrentNode();
        Search(mynode, n[0].myNode);
        int i = 1;
        while(_path == null && i < n.Count)
        {
            Search(mynode, n[i].myNode);
            i++;
        }
        if (_path != null) _path.Add(EnemyVariables.Skeever_Holes[i - 1].transform.position);
        _canScape = false;
    }
    public void MoveToHole()
    {
        if (_path != null)
        {
            if (_path.Count > 0)
            {
                var nextNode = new Vector3(_path[0].x, transform.position.y, _path[0].z);
                if (Vector3.Distance(transform.position, nextNode) < Time.deltaTime * speed)
                {
                    _path.RemoveAt(0);
                    if (_path.Count == 1)
                    {
                        _rb.useGravity = false;
                        GetComponent<Collider>().isTrigger = true;
                    }
                }
                if (_path.Count > 0) Move(nextNode);
            }
            else
            {
                _rb.velocity = Vector3.zero;
            }
        }
        else
            _stateMachine.SendFsmInput(EnemyInput.Chase);
    }

    //Chase
    public void Chase()
    {
        if (Vector3.Distance(playerVariables.Position, transform.position) > smellDetectionRad + 2)
            _stateMachine.SendFsmInput(EnemyInput.Patrol);
        if (_path == null)
            _stateMachine.SendFsmInput(EnemyInput.Patrol);
    }


    //Daamge
    public override void ReceiveDamage(int damage)
    {
        life -= damage;
        if (material.material.color != Color.red)
        {
            _stateMachine.SendFsmInput(EnemyInput.Stun);
        }
        if (life <= 0) Destroy(gameObject);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.layer != LayerMask.NameToLayer("PLayer Attack") &&
            other.gameObject.layer != LayerMask.NameToLayer("player") &&
            other.gameObject.layer != LayerMask.NameToLayer("Stairs")) 
        {
            _stateMachine.SendFsmInput(EnemyInput.Landed);
        }
    }
    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRad);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, smellDetectionRad);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, viewdetectionRad);
        }      
    }
}
