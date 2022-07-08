using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell : MonoBehaviour
{

    public enum States
    {
        ready, active, exhausted
    }

    public enum Parts
    {
        none, dentrite, axon, synapse
    }

    public enum Orientations
    {
        up, down, right, left
    }

    public States state = States.ready;
    public Parts part = Parts.none;
    public Orientations orientation = Orientations.up;
    public int numActiveNeighbours = 0;
    public int numCloseActiveNeighbours = 0;
    public int numNeighbours = 0;
    public int numCloseNeighbours = 0;
    public int numDentriteNeighbours = 0;
    public int numAxonNeighbours = 0;
    public Sprite ready;
    public Sprite active;
    public Sprite exhausted;

    //

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

    //

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

    public void Prepare()
    {
        state = States.ready;
        // GetComponent<SpriteRenderer>().transform.localScale = 0;
        GetComponent<SpriteRenderer>().sprite = ready;
    }

    //

    public void Orientate()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        orientation = (Orientations)rand;
    }
}
