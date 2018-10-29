using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [HideInInspector]
    public float timer;
    public float attackDuration;
    public int damage;
    public Player player;

	// Use this for initialization
	void Start () {
        timer = 0;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= attackDuration)
        {
            player.isAttacking = false;
            gameObject.SetActive(false);
        }
	}
}
