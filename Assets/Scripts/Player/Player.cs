using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
   

    [Header("For Snaping")]
    public float snapX;
    public float snapZ;
    [Header("Jump and Fall Variables (Set before Start)")]
    public float jumpForce;
    public float gavityPlayer;
    [Header("Character Speed")]
    public float speed;
    [Header("Character Body")]
    public GameObject body;
    public Animator anim;
    public PlayerVariables variables;
    [Header("My life")]
    public float life;
    [Header("Amount of time between Stuns")]
    public float stunTime;
    [Header("Foot")]
    public Foot foot;
    [Header("Rotation")]
    public Transform rotGuide;//Ahora giro este envez de al player y checkeo si esta girado antes de moverme
                              //Tambien le paso este a la camara como target para que gire con el


    private Rigidbody _rb;
    private Player_States _stateMachine;
    private PlayerController _controller;
    private Node _nodeToSnap;

    //Stair Movement
    private bool _walkStairs;
    private float _angularMove;
    private float _stairDir;

    //Stun
    private bool _canStun;
    private float _stunTimer;

    //Para probar nada mas
    public PlayerAttack attack;
    public bool isAttacking;

    private Collider _col;

    //Turn Camera
    private bool _turn;
    private float _turnTimer;

    //Rotation
    private bool rotated;



    void Awake ()
    {
        _controller = new PlayerController(this);
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _stateMachine = new Player_States(this);

        _canStun = true;
        _walkStairs = false;

        //Get Angle
        _angularMove = speed * Mathf.Tan(45 * Mathf.Deg2Rad);
        _stunTimer = 0;
        _stairDir = 0;

        isAttacking = false;
        attack.player = this;

        _col = GetComponent<Collider>();

        //Events
        EventManager.SubscribeToEvent(EventInputs.PlayerDamage, Damage);

        //To Begin Snaped
        GetNodeOnPos();
        transform.position = SnapPosition(_nodeToSnap);

        foot.state = _stateMachine;

        //Turn
        _turn = false;
        _turnTimer = 0;
    }

    void Update ()
    {
        //Set variables to scriptableobject
        variables.playerPosition = transform.position;
        GetNodeOnPos();

        _controller.Update();
        _stateMachine.Update();

        //Stun
        if (!_canStun)
        {
            _stunTimer += Time.deltaTime;
            if(_stunTimer >= stunTime)
            {
                _canStun = true;
                _stunTimer = 0;
            }
        }

        //Turn
        if (_turn)
        {
            _turnTimer += Time.deltaTime;
            if (_turnTimer > 0.5f)
            {
                _turnTimer = 0;
                _turn = false;
            }
        }

    }

    private void LateUpdate()
    {
        if (_rb.velocity.y > jumpForce) _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
        if (_rb.velocity.x > speed) _rb.velocity = new Vector3(speed, _rb.velocity.y, _rb.velocity.z);
        if (_rb.velocity.z > speed) _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, speed);
    }

    //Turn
    public void TurnCameraLeft()
    {
        if (!_turn)
        {
            if (_nodeToSnap == null) return;
            var node = CheckNodeToSnap(_nodeToSnap) ? _nodeToSnap : GetClosestNode(_nodeToSnap);
            var snapPosition = SnapPosition(node);
            transform.position = snapPosition;
            rotGuide.transform.right = rotGuide.transform.forward;
            rotated = true;
            _turn = true;
        }
    }

    public void TurnCameraRight()
    {
        if (!_turn)
        {
            if (_nodeToSnap == null) return;
            var node = CheckNodeToSnap(_nodeToSnap) ? _nodeToSnap : GetClosestNode(_nodeToSnap);
            var snapPosition = SnapPosition(node);
            transform.position = snapPosition;
            rotGuide.transform.right = -rotGuide.transform.forward;
            rotated = true;
            _turn = true;
        }
    }

    private Vector3 SnapPosition(Node node)
    {
        if (variables.playerNode != null) return (_walkStairs && _col.material.frictionCombine != PhysicMaterialCombine.Minimum) ? node.center + new Vector3(0, 0.2f, 0) : new Vector3(node.center.x, transform.position.y, node.center.z);
        return transform.position;
    }

    //Move
    public void Move(float movement)
    {
        CheckIfRotated(movement);

        var move = transform.right * movement * speed;
        float v = _rb.velocity.y;
        _rb.velocity = new Vector3(move.x, v, move.z);
        if (movement != 0) body.transform.forward = (movement < 0) ? -transform.right : transform.right;
    }
    public void Moveground(float movement)
    {
        if (isAttacking)
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        CheckIfRotated(movement);

        anim.SetFloat("Run", Mathf.Abs(movement));
        var move = transform.right * movement * speed;
        _rb.useGravity = _walkStairs ? false : true;
        _rb.velocity = _walkStairs ? new Vector3(move.x, _angularMove * movement * _stairDir, move.z) : new Vector3(move.x, _rb.velocity.y > 0 ? 0 : _rb.velocity.y, move.z);
        if (movement != 0) body.transform.forward = (movement < 0) ? -transform.right : transform.right;

        if (_rb.velocity.y < -1f && !_walkStairs && !foot.inGround)
            _stateMachine.SendFsmInput(PlayerInput.Fall);
    }

    private void CheckIfRotated(float movement)
    {
        if (rotated && movement != 0)
        {
            transform.rotation = rotGuide.rotation;
            rotGuide.localEulerAngles = Vector3.zero;
            rotated = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            _stairDir = Vector3.Dot(transform.right, other.transform.right);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            _walkStairs = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            _stairDir = Vector3.Dot(transform.right, other.transform.right);
            _walkStairs = true;
        }
    }

    //Jump
    public void Jump()
    {
        if (CheckRoof())
            _stateMachine.SendFsmInput(PlayerInput.Jump);
    }
    bool CheckRoof()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = transform.position,
            direction = Vector3.up
        };
        return !Physics.Raycast(ray, out hitInfo, 0.5f);
    }

    //Landed
    public void Landed()
    {
        _stateMachine.SendFsmInput(PlayerInput.Landed);
    }


    //Damage
    public void Damage(params object[] obj)
    {
        life -= (int)obj[0];
        if (_canStun)
        {
            _canStun = false;
            _stateMachine.SendFsmInput(PlayerInput.Damage);
        }
    }
    private void GetNodeOnPos(params object[] obj)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = transform.position,
            direction = -Vector3.up
        };
        var _nodemask = 1 << LayerMask.NameToLayer("Node");
        if (Physics.Raycast(ray, out hitInfo, 20, _nodemask))
        {
            Node node = hitInfo.transform.GetComponent<Node>();
            if (node != null)
            {
                if (node.transitable) variables.playerNode = node;
                _nodeToSnap = node;
                return;
            }
        }
    }

    private bool CheckNodeToSnap(Node node)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray
        {
            origin = transform.position,
            direction = (new Vector3(node.center.x, transform.position.y, node.center.z) - transform.position).normalized
        };
        var mask = 1 << LayerMask.NameToLayer("Enemy");
        mask += 1 << LayerMask.NameToLayer("Enemy Attack");
		mask += 1 << LayerMask.NameToLayer("Collectable");
		mask += 1 << LayerMask.NameToLayer("Ignore Raycast");
        mask = ~mask;
        var dist = Vector3.Distance(transform.position, new Vector3(node.center.x, transform.position.y, node.center.z));
        return !Physics.Raycast(ray, out hitInfo, dist, mask);
    }
    private Node GetClosestNode(Node node)
    {
        return node.neighbours.OrderBy(x => Vector3.Distance(transform.position, x.center)).First();
    }

    //para probar
    public void Attack()
    {
        if (!attack.gameObject.activeSelf)
        {
            isAttacking = true;
            anim.Play("AttackMelee");
            attack.gameObject.SetActive(true);
            attack.timer = 0;
        }
    }

}
