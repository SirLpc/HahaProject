using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using thelab.mvc;

public class SpaceModel : Model<SpaceApplication>
{

    [System.NonSerialized]
    public int TotalPlayerNum = SpaceConsts.MaxPlayerNum;

    public float WailPlayerJoinTime = 10f;


}
