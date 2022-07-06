using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    rest, active, exhausted
}

public class Neurite : MonoBehaviour
{

public States state = States.rest;
public int numNeighbours = 0;


    public void Activate()
    {
        state = States.active;
        // GetComponent<SpriteRenderer>().transform.localScale = Vector2.up * 10.0f;
        GetComponent<SpriteRenderer>().enabled = true;
    }
    public void Exhaust()
    {
        state = States.exhausted;
        // GetComponent<SpriteRenderer>().transform.localScale = Vector2.up * .1f;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public void Restore()
    {
        state = States.rest;
        // GetComponent<SpriteRenderer>().transform.localScale = 0;
    }
}
