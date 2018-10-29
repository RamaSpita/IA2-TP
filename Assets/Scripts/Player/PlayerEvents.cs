using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents 
{
    private Player _player;

    public PlayerEvents(Player player)
    {
        _player = player;
        EventManager.SubscribeToEvent(EventInputs.PlayerDamage, ReceiveDamage);
    }

    private void ReceiveDamage(params object[] ob)
    {
        _player.life -= (int)ob[0];
        _player.Damage();
    }
}
