using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementTest : MonoBehaviour {

    public Player player;
    [Header("If Player can control Offset")]
    public bool controlOffset;
    [Header("Camera Offset Limits")]
    public float limitOffsetX;
    public float limitOffsetY;
    [Header("Camera velocities")]
    public float cameraLerpOffSet;
    public float speed;
    public float rotationSpeed;

    [Header("Distance To Target")]
    public float distanceFromTarget;
    [Header("Custom View")]
    public Vector3 cameraPositionOffSet;
    public Vector3 cameraViewOffSet;

    private GameObject _myCamera;
    private float _currentSpeed;
    private Vector3 _myTarget;

    private Vector3 auxOffsetPos;
    private Vector3 chechChange;

    private Transform target;
    private int side;

    private void Start()
    {
        target = player.transform.Find("RootGuide");

        //Camera
        _myCamera = transform.Find("Main Camera").gameObject;
        _myCamera.transform.localPosition = cameraPositionOffSet;
        _myCamera.transform.localEulerAngles = cameraViewOffSet != Vector3.zero ? cameraViewOffSet : _myCamera.transform.forward;


        transform.position = target.position - target.forward * distanceFromTarget;
        var lookDirtarget = (target.position - transform.position).normalized;
        transform.forward = lookDirtarget;

        _currentSpeed = speed;

        auxOffsetPos = cameraPositionOffSet;
        chechChange = cameraPositionOffSet;

        side = 1;
    }

    void Update ()
    {
        //Move Offset with joystick
        if (controlOffset)
        {
            if(Input.GetAxis("Horizontal") == 0)
            {
                if (Mathf.Abs(auxOffsetPos.x) <= limitOffsetX) auxOffsetPos.x += Input.GetAxis("CameraHorizontal") * Time.deltaTime * 6;
                if (Mathf.Abs(auxOffsetPos.x) > limitOffsetX) auxOffsetPos.x = auxOffsetPos.x > 0 ? limitOffsetX : -limitOffsetX;
                if (Mathf.Abs(auxOffsetPos.y) <= limitOffsetY) auxOffsetPos.y += Input.GetAxis("CameraVertical") * Time.deltaTime * 6;
                if (Mathf.Abs(auxOffsetPos.y) > limitOffsetY) auxOffsetPos.y = auxOffsetPos.y > 0 ? limitOffsetY : -limitOffsetY;
            }

            if(Input.GetAxis("CameraHorizontal")==0 && Input.GetAxis("CameraVertical") == 0)
            {
                auxOffsetPos = Vector3.Lerp(auxOffsetPos, cameraPositionOffSet, Time.deltaTime * 6);
                auxOffsetPos.x = side < 0 ? -cameraPositionOffSet.x : cameraPositionOffSet.x;
            }
        }


        if (chechChange != cameraPositionOffSet)
        {
            auxOffsetPos = cameraPositionOffSet;
            chechChange = cameraPositionOffSet;
        }

        //Camera
        if (Input.GetAxis("Horizontal") < 0)
        {
            auxOffsetPos = Vector3.Lerp(auxOffsetPos, cameraPositionOffSet, Time.deltaTime * 6);
            auxOffsetPos.x = -cameraPositionOffSet.x;
            cameraViewOffSet.y = -Mathf.Abs(cameraViewOffSet.y);
            side = -1;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            auxOffsetPos = Vector3.Lerp(auxOffsetPos, cameraPositionOffSet, Time.deltaTime * 6);
            auxOffsetPos.x = cameraPositionOffSet.x;
            cameraViewOffSet.y = Mathf.Abs(cameraViewOffSet.y);
            side = 1;
        }

        _myCamera.transform.localPosition = Vector3.Lerp(_myCamera.transform.localPosition, auxOffsetPos, Time.deltaTime * cameraLerpOffSet);
        _myCamera.transform.localEulerAngles = cameraViewOffSet != Vector3.zero ?
                                               cameraViewOffSet
                                               : _myCamera.transform.localEulerAngles;

        //Camera Continer
        _myTarget = target.position - target.forward * distanceFromTarget;
        transform.position = Vector3.Lerp(transform.position, _myTarget, _currentSpeed * Time.deltaTime);
        var lookDirtarget = (target.position - transform.position).normalized;
        transform.forward = lookDirtarget;

        var distToTarget = Vector3.Distance(target.position, transform.position);

        if (distToTarget < distanceFromTarget)
        {
            transform.position = transform.position - lookDirtarget * (distanceFromTarget - distToTarget);
        }
    }
}
