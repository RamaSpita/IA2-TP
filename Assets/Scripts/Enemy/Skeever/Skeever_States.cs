using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateConfig;
using System;

public class Skeever_States
{
    private EventFSM<EnemyInput> _myFsm;
    private EnemyInput _previnput;
    private MyState<EnemyInput> _prevState;

    private Skeever _skeever;
    private Animator _anim;
    private Rigidbody _rb;

    private float _attackPreparationTimer;
    private float _stunTimer;

    public Skeever_States(Skeever skeever)
    {
        _skeever = skeever;
        _anim = skeever.anim;
        _rb = skeever.GetComponent<Rigidbody>();

        _attackPreparationTimer = 0;
        _stunTimer = 0;

        CreateStates();
    }

    private void CreateStates()
    {
        var patrol = new MyState<EnemyInput>("Patrol");
        var smell = new MyState<EnemyInput>("Smell");
        var chase = new MyState<EnemyInput>("Chase");
        var attackPreparation = new MyState<EnemyInput>("Attack Preparation");
        var attack = new MyState<EnemyInput>("Attack");
        var scape = new MyState<EnemyInput>("Scape");
        var stun = new MyState<EnemyInput>("Stun");

        //Set Transitions
        StateConfigurer.Create(patrol)
            .SetTransition(EnemyInput.Smell, smell)
            .SetTransition(EnemyInput.PrepareAttack, attackPreparation)
            .SetTransition(EnemyInput.Chase, chase)
            .Done();

        StateConfigurer.Create(smell)
            .SetTransition(EnemyInput.PrepareAttack, attackPreparation)
            .SetTransition(EnemyInput.Chase, chase)
            .SetTransition(EnemyInput.Patrol, patrol)
            .Done();

        StateConfigurer.Create(chase)
            .SetTransition(EnemyInput.PrepareAttack, attackPreparation)
            .SetTransition(EnemyInput.Patrol, patrol)
            .SetTransition(EnemyInput.Scape, scape)
            .Done();

        StateConfigurer.Create(attackPreparation)
            .SetTransition(EnemyInput.Attack, attack)
            .SetTransition(EnemyInput.Stun, stun)
            .SetTransition(EnemyInput.Scape, scape)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(EnemyInput.Landed, chase)
            .Done();

        StateConfigurer.Create(scape)
            .SetTransition(EnemyInput.Chase, chase)
            .Done();

        StateConfigurer.Create(stun)
            .Done();

        //Set States

        //Patrol
        patrol.OnEnter += x =>
        {
            _prevState = patrol;
            _previnput = x;

            //_anim.Play("Normal");
            _skeever.material.material.color = Color.blue;
            _skeever.EnterWaypointMovement();
        };
        patrol.OnUpdate += () =>
        {
            _skeever.Patrol();
        };

        //Smmell
        smell.OnEnter += x =>
        {
            _prevState = smell;
            _previnput = x;

            _skeever.material.material.color = Color.yellow;
            _skeever.SmellEnter();
        };
        smell.OnUpdate += () =>
        {
            _skeever.SmellingForPlayer();
        };

        //Chase
        chase.OnEnter += x =>
        {
            _prevState = chase;
            _previnput = x;
            _skeever.PlayerSpottet();
            //_anim.Play("Normal");
            _skeever.material.material.color = Color.yellow;
            _skeever.EnterPathMovement();
        };
        chase.OnUpdate += () =>
        {
            _skeever.MovePathChase();
            _skeever.Chase();
        };

        //Attack Preparation
        attackPreparation.OnEnter += x =>
        {
            _prevState = attackPreparation;
            _previnput = x;

            _attackPreparationTimer = 0;
        };
        attackPreparation.OnUpdate += () =>
        {
            var dirP = (_skeever.playerVariables.Position - _skeever.transform.position).normalized;
            if (Vector3.Distance(_skeever.transform.position, _skeever.playerVariables.Position) > 0.5f) _skeever.transform.forward = new Vector3(dirP.x, _skeever.transform.forward.y, dirP.z);
            _attackPreparationTimer += Time.deltaTime;
            if (_attackPreparationTimer >= 2f)
            {
                SendFsmInput(EnemyInput.Attack);

            }
        };

        //Attack
        attack.OnEnter += x =>
        {
            _skeever.attack.SetActive(true);
            _rb.AddForce(_skeever.transform.up * _skeever.jumpForce + _skeever.transform.forward * _skeever.jumpForce, ForceMode.Impulse);
        };
        attack.OnUpdate += () =>
        {
            if (_skeever.transform.localEulerAngles.x != 0 || _skeever.transform.localEulerAngles.z != 0)
                _skeever.transform.localEulerAngles = new Vector3(0, _skeever.transform.localEulerAngles.y, 0);
        };
        attack.OnExit += x =>
        {
            _skeever.attack.SetActive(false);
        };

        //Scape
        scape.OnEnter += x =>
        {
            _prevState = scape;
            _previnput = x;

            _skeever.EnterScapeMovement();
        };
        scape.OnUpdate += () =>
        {
            _skeever.MoveToHole();
        };

        //Stun
        stun.OnEnter += x =>
        {
            _stunTimer = 0;
            _skeever.material.material.color = Color.green;
        };
        stun.OnUpdate += () =>
        {
            _stunTimer += Time.deltaTime;
            if (_stunTimer >= 1f)
            {
                SendState(_prevState, _previnput);
            }
        };


        _previnput = EnemyInput.Patrol;
        _prevState = patrol;
        _myFsm = new EventFSM<EnemyInput>(patrol);
    }

    public void SendFsmInput(EnemyInput input)
    {
        _myFsm.SendInput(input);
    }

    public void SendState(MyState<EnemyInput> state, EnemyInput input)
    {
        _myFsm.SendState(state, input);
    }

    public void Update()
    {
        _myFsm.Update();
    }
}
