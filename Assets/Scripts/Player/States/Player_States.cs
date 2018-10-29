using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateConfig;
using System;

public class Player_States
{
    private EventFSM<PlayerInput> _myFsm;
    private MyState<PlayerInput> _prevState; 
    private PlayerInput _previnput;

    private Player _player;
    private Rigidbody _rb;
    private Collider _col;
    private Animator _anim;
    private Foot _foot;
    private float _stunTimer;

    public Player_States(Player player)
    {
        _player = player;
        _rb = player.GetComponent<Rigidbody>();
        _col= player.GetComponent<Collider>();
        _anim = player.anim;
        _foot = player.foot;
        _stunTimer = 0;

        CreateStates();
    }

    private void CreateStates()
    {
        var damage = new MyState<PlayerInput>("Damage");
        var jump = new MyState<PlayerInput>("Jump");
        var fall = new MyState<PlayerInput>("Fall");
        var moveGround = new MyState<PlayerInput>("MoveGround");

        //Set transitions
        StateConfigurer.Create(moveGround)
            .SetTransition(PlayerInput.Jump, jump)
            .SetTransition(PlayerInput.Fall, fall)
            .SetTransition(PlayerInput.Damage, damage)
            .Done();

        StateConfigurer.Create(jump)
            .SetTransition(PlayerInput.Fall, fall)
            .SetTransition(PlayerInput.Landed, moveGround)
            .SetTransition(PlayerInput.Damage, damage)
            .Done();

        StateConfigurer.Create(fall)
            .SetTransition(PlayerInput.Landed, moveGround)
            .SetTransition(PlayerInput.Damage, damage)
            .Done();

        StateConfigurer.Create(damage)
            .Done();

        //Set States

        //Move Ground
        moveGround.OnEnter += x =>
        {
            _prevState = moveGround;
            _previnput = x;
            _rb.velocity = Vector3.zero;
        };

        moveGround.OnUpdate += () =>
        {
            _player.Moveground(Input.GetAxis("Horizontal"));
        };

        //Jump
        jump.OnEnter += x =>
        {
            _prevState = jump;
            _previnput = x;

            _anim.CrossFade("Jump", 0.1f);

            _rb.useGravity = true;
            _rb.velocity = new Vector3(_rb.velocity.x, Time.deltaTime, _rb.velocity.z);
            _rb.AddForce(_player.transform.up * _player.jumpForce, ForceMode.Impulse);

            //Saco el friction del Colider para que no se quede pegado a la pared
            _col.material.dynamicFriction = 0;
            _col.material.staticFriction = 0;
            _col.material.frictionCombine = PhysicMaterialCombine.Minimum;
        };

        jump.OnUpdate += () =>
        {
            _player.Move(Input.GetAxis("Horizontal"));
            if (_rb.velocity.y < Time.deltaTime)
            {
                SendFsmInput(PlayerInput.Fall);
            }
        };

        jump.OnExit += x =>
        {
            if (_foot.inGround)
            {
                _anim.CrossFade("Move", 0.3f);
            }
            _col.material = null;
        };

        //Fall
        fall.OnEnter += x =>
        {
            _prevState = fall;
            _previnput = x;

            _anim.CrossFade("Fall", 0.3f);

            _rb.useGravity = false;
            _rb.AddForce(-_player.transform.up * _player.gavityPlayer, ForceMode.Acceleration);

            //Saco el friction del Colider para que no se quede pegado a la pared
            _col.material.dynamicFriction = 0;
            _col.material.staticFriction = 0;
            _col.material.frictionCombine = PhysicMaterialCombine.Minimum;
        };

        fall.OnUpdate += () =>
        {
            _rb.AddForce(-_player.transform.up * _player.gavityPlayer, ForceMode.Acceleration);
            _player.Move(Input.GetAxis("Horizontal"));
        };

        fall.OnExit += x =>
        {
            _anim.CrossFade("Landed", 0.3f);
            _rb.useGravity = true;
            _col.material = null;
        };

        //Damage
        damage.OnEnter += x =>
        {
            _stunTimer = 0;
            _rb.velocity = Vector3.zero;
            _player.anim.Play("GetHit");
        };

        damage.OnUpdate += () =>
        {
            _stunTimer += Time.deltaTime;
            if (_stunTimer > _player.stunTime)
            {
                SendState(_prevState, _previnput);
            }
        };

        _previnput = PlayerInput.Landed;
        _prevState = moveGround;
        _myFsm = new EventFSM<PlayerInput>(moveGround);
    }

    public void SendFsmInput(PlayerInput input)
    {
        _myFsm.SendInput(input);
    }

    public void SendState(MyState<PlayerInput> state, PlayerInput input)
    {
        _myFsm.SendState(state, input);
    }

    public void Update()
    {
        _myFsm.Update();
    }
}
