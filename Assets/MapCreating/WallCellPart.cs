using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CellPart/Wall")]
public class WallCellPart : CellPart
{
    public bool[] FAnchors = new bool[] {
        true,true,true,true,true,true,true,true,true
    };
    public bool[] BAnchors = new bool[] {
        true,true,true,true,true,true,true,true,true
    };
}
