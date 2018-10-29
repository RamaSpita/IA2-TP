using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour {

    public Player_States state;
    public bool inGround;

    private void Start()
    {
        inGround = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && !inGround)
        {
            inGround = true;
            state.SendFsmInput(PlayerInput.Landed);
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && inGround)
            inGround = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger && !inGround)
            inGround = true;
    }
}
