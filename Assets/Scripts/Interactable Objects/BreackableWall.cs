using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BreackableWall : Openable
{
    public GameObject breackWall;


    public override void Awake()
    {
        base.Awake();
    }

   
    public override void Interact(params object[] obj)
    {
        base.Interact();
        OnInteract();
    }

    private void OnInteract()
    {
        var b = Instantiate(breackWall);
        b.transform.parent = transform.parent;
        b.transform.position = transform.position;
        b.transform.localEulerAngles = transform.localEulerAngles;
        var d = (b.transform.position - playerVariables.Position).normalized;
        foreach (Transform item in b.transform)
        {
            var rb = item.GetComponent<Rigidbody>();
            rb.AddForce((d * 10) / (Vector3.Distance(item.position, playerVariables.Position) * 3), ForceMode.Impulse);
            Destroy(item.gameObject, 2);
        }
        Destroy(gameObject);
    }

    //va a ser desde el player puede ser
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("Player Attack"))
        {
            Interact();
        }
    }
    
}
