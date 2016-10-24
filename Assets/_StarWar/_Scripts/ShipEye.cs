﻿using UnityEngine;
using System.Collections;

public class ShipEyeBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject _render;

    protected int _visibleLayer;
    protected int _disibleLayer;

    private void Awake()
    {
        _visibleLayer = LayerMask.NameToLayer(Tags.VisibleLayer);
        _disibleLayer = LayerMask.NameToLayer(Tags.DisibleLayer);
    }

 
}
