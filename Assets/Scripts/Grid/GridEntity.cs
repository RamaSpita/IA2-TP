using System;
using UnityEngine;

//[ExecuteInEditMode]
public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove = delegate {};
	public event Action<GridEntity> OnDeath = delegate {};
    public Vector3 velocity = new Vector3(0, 0, 0);

    void Update()
    {
	    OnMove(this);
	}

    private void OnDestroy()
    { 
        OnDeath(this);
    }
}
