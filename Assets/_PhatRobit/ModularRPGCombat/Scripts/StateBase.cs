using UnityEngine;
using System.Collections.Generic;

public class StateBase : MonoBehaviour
{

    public int health = 100;            // GameObject's current health
    public int maxHealth = 100;			// GameObject's max health
    public string playerName { get; protected set; }        //player's name

    protected bool _alive = true;                               // Are you still there?
    public bool Alive
    {
        get { return _alive; }
    }

}
