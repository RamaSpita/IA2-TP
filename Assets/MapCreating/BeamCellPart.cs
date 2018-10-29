using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CellPart/Beam")]
public class BeamCellPart : CellPart
{
    public bool[] FAnchors = new bool[]
    {
        true,true,true
    };
    public bool[] BAnchors = new bool[]
    {
        true,true,true
    };

}
