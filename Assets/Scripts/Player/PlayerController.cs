using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;


public class PlayerController
{
    private Player _player;
    private Dictionary<KeyCode, Action> _actions; //Guarda las accines a hacer dependiendo del input keycode

    public PlayerController(Player player)
    {
        _player = player;

        #region Asignacion de Inputs
        _actions = new Dictionary<KeyCode, Action>
        {
            [KeyCode.S] = _player.TurnCameraRight,
            [KeyCode.JoystickButton4] = _player.TurnCameraRight,

            [KeyCode.W] = _player.TurnCameraLeft,
            [KeyCode.JoystickButton5] = _player.TurnCameraLeft,

            [KeyCode.Space] = _player.Jump,
            [KeyCode.JoystickButton0] = _player.Jump,

            [KeyCode.Mouse0] = _player.Attack,
            [KeyCode.JoystickButton2] = _player.Attack
        };

        #endregion

    }
    public void Update()
    {
        // Por Ahora es asi, el Event.current.KeyCode no se lleva bien con el Update
        if (Input.anyKey)
            foreach (var item in _actions)
                if (Input.GetKeyDown(item.Key))
                    item.Value();
    }

}

