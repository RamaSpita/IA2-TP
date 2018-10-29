using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Door : Openable
{
    public bool canOpen;
    public float rotSpeed;
    public float startRtoY;
    public Transform doorAnchor;

    private bool _move;
    private bool _open;
    private Vector3 _targetAngle;
    private Quaternion _targetQuat;

    private bool _canclose;

    public override void Awake()
    {
        base.Awake();
        SetNeighbours();
        startRtoY = doorAnchor.localRotation.y;

        _canclose = true;
    }

    private void SetNeighbours()
    {
        for (int i = 0; i < mynodes.Count; i++)
        {
            if (canOpen) mynodes[i].neighboursDoor.AddRange(mynodes.Where(x => x != mynodes[i]));
        }
    }

    private void Update()
    {
        OpenCloseMecanism();

        if (_canclose && _open) Close();
    }

    private void OpenCloseMecanism()
    {
        if (_move)
        {
            doorAnchor.localRotation = Quaternion.Lerp(doorAnchor.localRotation, _targetQuat, rotSpeed);
            if (Vector3.Distance(_targetAngle, doorAnchor.eulerAngles) < 1f)
                _move = false;
        }
    }

    public override void Interact(params object[] obj)
    {
        base.Interact();
        OnInteract((Vector3)obj[0]);
    }

    private void OnInteract(Vector3 targetPos)
    {
        var auxDir = (targetPos - transform.position).normalized;

        if (Vector3.Dot(auxDir,transform.forward) > 0)
            Open(-90);
        else
            Open(90);

    }

    public void Open(int yRot)
    {
        _open = true;
        RotYAngle(yRot);
    }

    private void RotYAngle(int yRot)
    {
        _move = true;
        _targetAngle = new Vector3(transform.eulerAngles.x, startRtoY + yRot, transform.eulerAngles.z);
        _targetQuat = Quaternion.Euler(_targetAngle);
    }

    public void Close()
    {
        _open = false;
        RotYAngle(0);
        for (int i = 0; i < mynodes.Count; i++)
        {
            mynodes[i].neighbours = mynodes[i].neighbours.Where(x => !mynodes.Contains(x)).ToList();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && canOpen && !_open)
        {
            Interact(new object[] { other.transform.position });
            _canclose = false;
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && canOpen && !_open)
        {
            var enemy = other.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                if (enemy.canOpenDoor)
                {
                    Interact(new object[] { other.transform.position });
                    _canclose = false;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _canclose = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _canclose = false;
        }
    }
}
