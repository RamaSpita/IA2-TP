using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerReference 
{
    public bool useConstant = true;
    public Vector3 constantValue;
    public PlayerVariables variables;

    public PlayerReference()
    { }

    public PlayerReference(Vector3 value)
    {
        useConstant = true;
        constantValue = value;
    }

    public Vector3 Position
    {
        get { return useConstant ? constantValue : variables.playerPosition; }
    }
    public Node PlayerNode
    {
        get { return variables.playerNode; }
    }

    public static implicit operator Vector3(PlayerReference reference)
    {
        return reference.Position;
    }
}