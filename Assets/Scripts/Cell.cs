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
        up, right, down, left
    }

    
    public Parts part = Parts.none;

    // growth
    public int sumNeighbourhood = 0; // always less then three
    public bool canBecomeNeuron = false; 
    public Parts[] neighbourhood = new Parts[]{Parts.none, Parts.none, Parts.none, Parts.none};
    
    // signal transmission
    public States state = States.ready;
    public int numActiveNeighbours = 0;
    public int numActiveDentriteNeighbours = 0; 
    public Orientations orientation = Orientations.up;
    
    // graphics (ready)
    public Sprite termination;
    public Sprite straight;
    public Sprite curve;
    public Sprite tshaped;
    public Sprite synapse;
    public Sprite synapseCurve;
    // graphics (active)
    public Sprite terminationActive;
    public Sprite straightActive;
    public Sprite curveActive;
    public Sprite tshapedActive;
    public Sprite synapseActive;
    public Sprite synapseCurveActive;
    // graphics (exhausted)
    public Sprite terminationExhausted;
    public Sprite straightExhausted;
    public Sprite curveExhausted;
    public Sprite tshapedExhausted;
    public Sprite synapseExhausted;
    public Sprite synapseCurveExhausted;
    // graphics (spritelist)
    public List<Sprite> sprites = new List<Sprite>();
    // graphics (sprite to use, still not orientated)
    public Sprite unOrientatedSprite;
    public int spriteRotation = 0;

    void Start()
    {
        sprites.Add(termination);
        sprites.Add(straight);
        sprites.Add(curve);
        sprites.Add(tshaped);
        sprites.Add(synapse);
        sprites.Add(synapseCurve);

        sprites.Add(terminationActive);
        sprites.Add(straightActive);
        sprites.Add(curveActive);
        sprites.Add(tshapedActive);
        sprites.Add(synapseActive);
        sprites.Add(synapseCurveActive);

        sprites.Add(terminationExhausted);
        sprites.Add(straightExhausted);
        sprites.Add(curveExhausted);
        sprites.Add(tshapedExhausted);
        sprites.Add(synapseExhausted);
        sprites.Add(synapseCurveExhausted);
    }

    public void CreateAxon()
    {
        part = Parts.axon;
        Prepare();
        // GetComponent<SpriteRenderer>().sprite = straight;
    }

    public void CreateDentrite()
    {
        part = Parts.dentrite;
        Prepare();
        // GetComponent<SpriteRenderer>().sprite = termination;
    }

    public void CreateSynapse()
    {
        part = Parts.synapse;
        Prepare();
        // GetComponent<SpriteRenderer>().sprite = synapse;
        // if (orientation == Orientation.up){

        // }
    }

    //

    public void Activate()
    {
        state = States.active;
    }

    public void Exhaust()
    {
        state = States.exhausted;
    }

    public void Prepare()
    {
        state = States.ready;
    }

    //

    public void OrientateSeed()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        orientation = (Orientations)rand;
    }

    //

    public void SetSprite(int spriteID, int rotationAngle)
    {
        if (unOrientatedSprite != sprites[spriteID] || spriteRotation != rotationAngle)
        {
            transform.Rotate(0, 0, -spriteRotation);
            spriteRotation = rotationAngle;
            transform.Rotate(0, 0, rotationAngle);

            unOrientatedSprite = sprites[spriteID];
            GetComponent<SpriteRenderer>().sprite = unOrientatedSprite;
        }
    }
}
