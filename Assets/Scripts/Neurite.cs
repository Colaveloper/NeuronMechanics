using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Neurite : MonoBehaviour
{

    public enum States
    {
        ready, active, exhausted
    }

    public enum Parts
    {
        axon, dentrite, synapse, none
    }

    public States state = States.ready;
    public Parts part = Parts.none;
    public int numNeighbours = 0;
    public Sprite ready;
    public Sprite active;
    public Sprite exhausted;

    public void CreateAxon()
    {
        part = Parts.axon;
    }

    public void CreateDentrite()
    {
        part = Parts.dentrite;
    }

    public void CreateSynapse()
    {
        part = Parts.synapse;
    }

    public void Activate()
    {
        state = States.active;
        // GetComponent<SpriteRenderer>().transform.localScale = Vector2.up * 10.0f;
        GetComponent<SpriteRenderer>().sprite = active;
    }

    public void Exhaust()
    {
        state = States.exhausted;
        // GetComponent<SpriteRenderer>().transform.localScale = Vector2.up * .1f;
        GetComponent<SpriteRenderer>().sprite = exhausted;
    }

    public void Restore()
    {
        state = States.ready;
        // GetComponent<SpriteRenderer>().transform.localScale = 0;
        GetComponent<SpriteRenderer>().sprite = ready;
    }
}
