using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CellPart/Floor")]
public class FloorCellPart : CellPart
{
    public bool[] FAnchors = new bool[]
    {
        true,true,true,true,true,true,true,true,true
    };
    public bool[] BAnchors = new bool[]
    {
        true,true,true,true,true,true,true,true,true
    };

}
