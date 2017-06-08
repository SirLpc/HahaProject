﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBulletView : thelab.mvc.View<SpaceApplication>
{

    [SerializeField]
    private float _speed = 0.001f;
    [SerializeField]
    private int _damage = 10;

    private TNet.TNObject _tno;
    private Rigidbody _body;
    private Collider _collider;

	private void Start ()
    {
        _tno = GetComponent<TNet.TNObject>();
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _collider.enabled = _tno.isMine;
        _body.velocity = Vector3.zero;

        _body.AddForce(transform.forward.normalized * _speed);
	}


    private void OnTriggerEnter(Collider other)
    {
        var shipView = other.transform.root.GetComponentInParent<NetShipControllerView>();
        Log("in trigger");
        if (shipView && !shipView.Equals(app.view.MyShip))
        {
            Log("other ship ton id" + shipView.tno.ownerID + "===" + shipView.tno.owner.id);
            Notify(SpaceNotifications.BulletAttakOn, shipView, 10);
            _tno.DestroySelf();
        }
    }




}