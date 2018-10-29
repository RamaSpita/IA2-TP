using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour ,Iinteractable
{
    
    void Start ()
    {
	}
	
	void Update () {
		
	}

    public void Interact(params object[] obj)
    {
        UIManager.Instance.CointCount();
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Interact();
        }
    }

}
