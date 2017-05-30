using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using thelab.mvc;

public class SpaceApplication : BaseApplication<SpaceModel, SpaceView, SpaceController>
{

    public int TargetFrameRate = 40;

    private void Awake()
    {
        Application.targetFrameRate = TargetFrameRate;
    }

}
