using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 128; //64
    private static int SCREEN_HEIGHT = 96; //48
    private float timer = 0;
    public float speed = 0.2f;
    public float seedProbability = 0.01f; // over 100
    public bool stopGrowth = false;
    public float axonStraightness = 100; // over 100
    public float dentriteStraightness = 5; // over 100
    public float selfExcitability = 0.001f; // over 100


    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    void Start()
    {
        // random somas of neurons
        RepeatForEachCell((x, y) => {Seeding(x, y);});
        // attaching the axon hillock
        RepeatForEachCell((x, y) => {Sprouting(x, y);});
    }

    void Update()
    {
        if (timer >= 1/speed)
        {
            timer = 0;

            if (!stopGrowth)  
            {
                // extending dentrites and soma, 
                // creating sinapses
                Growing(); 
                // over time axon branch out more 
                AxonStraightnessDecay();
            }

            if (AnyActivity()) 
            {
                // preparing axon and dentrite transmission
                RepeatForEachCell((x, y) => {CountActiveNeighbours(x, y);});
                // preparing synapse transmission
                RepeatForEachCell((x, y) => {CountActiveDentriteNeighbours(x, y);});
                // singnal transmission
                RepeatForEachCell((x, y) => {SignalTransmission(x, y);});
            }

            // Updating the sprites 
            RepeatForEachCell((x, y) => {SetSprite(x, y);});
            
            // Stimulating the brain automatically
            RepeatForEachCell((x, y) => {RandomSelfActivation(x, y);});
        }
        else { timer += Time.deltaTime; }
    }

// GROWTH

    void Seeding(int x, int y)
    {   
        Cell Cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
        grid[x, y] = Cell;

        if (Random(seedProbability)) 
        {
            grid[x, y].CreateDentrite();  
            grid[x, y].OrientateSeed();
        }
    }

    void Sprouting(int x, int y)
    {
        if (CorrectSproutingDirection(x, y)) 
        {
            grid[x, y].CreateAxon();  
        }
    }

    void Growing()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                ObserveNeighbourhoodParts(x, y);
            }
        }
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int[] originalNeighbourhood = Array.ConvertAll(grid[x, y].neighbourhood, part => (int) part);            
                ObserveNeighbourhoodParts(x, y);
                int[] progressiveNeighbourhood = Array.ConvertAll(grid[x, y].neighbourhood, part => (int) part);
                
                int NeighboursOnVertices = CountNeighboursOnVertices(x, y);
                if (
                        (int) grid[x, y].part == 0 
                        && originalNeighbourhood.Sum() > 0  
                        && NeighboursOnVertices < 2
                    )
                {
                    switch (progressiveNeighbourhood.Sum())
                    {
                        case 1: // DENTRITE
                            //TODO substitute grid[x, y].numNeighboursOnV. == 1 
                            if (NeighboursOnVertices == 0 && Random (dentriteStraightness))
                                {
                                    grid[x, y].CreateDentrite();  
                                }
                            if (NeighboursOnVertices == 1 && Random (100-dentriteStraightness))
                                {
                                    grid[x, y].CreateDentrite();  
                                }
                        break;

                        case 2: // AXON
                            // if (really neighbouring with an axon)
                            if (Array.IndexOf(progressiveNeighbourhood, 2) != -1)
                            {
                                if (NeighboursOnVertices == 0 && Random (axonStraightness))
                                    {
                                        grid[x, y].CreateAxon();  
                                    }
                                if (NeighboursOnVertices == 1 && Random (100-axonStraightness))
                                    {
                                        grid[x, y].CreateAxon();  
                                    }
                            }
                        break;

                        case 3: // SYNAPSE
                            // if (really neighbouring with an axon)
                            if (Array.IndexOf(progressiveNeighbourhood, 2) != -1)
                            {
                                if (NeighboursOnVertices == 0)
                                {
                                    grid[x, y].CreateSynapse();  
                                }
                            }
                        break;
                    }                    
                }
            }
        }
    }

    int CountNeighboursOnVertices(int x, int y)
    {
        int NeighboursOnVertices = 0;
        if (x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT) //Up-Left
        {
            if ((int) grid[x + 1, y + 1].part != 0)
            {
                NeighboursOnVertices++;
            }
        }
        if (x - 1 >= 0 && y + 1 < SCREEN_HEIGHT) //Up-Right
        {
            if ((int) grid[x - 1, y + 1].part != 0)
            {
                NeighboursOnVertices++;
            }
        }
        if (y - 1 >= 0 && x - 1 >= 0) //Down-Left
        {
            if ((int) grid[x - 1, y - 1].part != 0)
            {
                NeighboursOnVertices++;
            }
        }
        if (y - 1 >= 0 && x + 1 < SCREEN_WIDTH) //Down-Right
        {
            if ((int) grid[x + 1, y - 1].part != 0)
            {
                NeighboursOnVertices++;
            }
        }   
        return NeighboursOnVertices;
    }

    // (also used in GRAPHICS)
    void ObserveNeighbourhoodParts(int x, int y)
    {
        if (y + 1 < SCREEN_HEIGHT) {grid[x, y].neighbourhood[0] = grid[x, y + 1].part;}
        if (x + 1 < SCREEN_WIDTH) {grid[x, y].neighbourhood[1] = grid[x + 1, y].part;}
        if (y - 1 >= 0) {grid[x, y].neighbourhood[2] = grid[x, y - 1].part;}
        if (x - 1 >= 0) {grid[x, y].neighbourhood[3] = grid[x - 1, y].part;}
    }

    // (also used in SIGNAL TRANSMISSION)
    bool Random(float percentage)
    {
        float rand = UnityEngine.Random.Range(0.0f, 100.0f);
        return rand < percentage;
    }

    // (not essential)
    void AxonStraightnessDecay()
    {
        axonStraightness = axonStraightness*0.95f;
    }

// SIGNAL TRANSMISSION

    void SignalTransmission(int x, int y)
    {
        // axon or dentrite
        if ((int) grid[x, y].part == 1 || (int) grid[x, y].part == 2) // axon or dentrite
        {
            switch((int) grid[x, y].state) 
            {
            case 0:
                if (grid[x, y].numActiveNeighbours > 0)
                    {
                        grid[x, y].Activate();
                    }
                break;
            case 1:
                grid[x, y].Exhaust();
                break;
            case 2:
                grid[x, y].Prepare();
                break;
            }
        }

        // synapse: one-way gate
        if ((int) grid[x, y].part == 3) 
        {
            switch((int) grid[x, y].state) 
            {
            case 0:
                if (grid[x, y].numActiveDentriteNeighbours > 0)
                    {
                        grid[x, y].Activate();
                    }
                break;
            case 1:
                grid[x, y].Exhaust();
                break;
            case 2:
                grid[x, y].Prepare();
                break;
            }
        }
    }

     void CountActiveNeighbours(int x, int y)
    {
        int numActiveNeighbours = 0;

        if ((int) grid[x, y].part == 1 || (int) grid[x, y].part == 2)
        {
            if (y + 1 < SCREEN_HEIGHT) //Up
            {
                if ((int) grid[x, y + 1].state == 1)
                {
                    numActiveNeighbours++;
                }
            }

            if (y - 1 >= 0) //Down
            {
                if ((int) grid[x, y - 1].state == 1)
                {
                    numActiveNeighbours++;
                }
            }

            if (x + 1 < SCREEN_WIDTH) //Right
            {
                if ((int) grid[x + 1, y].state == 1)
                {
                    numActiveNeighbours++;
                }
            }
            
            if (x - 1 >= 0) //Left
            {
                if ((int) grid[x - 1, y].state == 1)
                {
                    numActiveNeighbours++;
                }
            }
        }

        grid[x, y].numActiveNeighbours = numActiveNeighbours;    
    }

    void CountActiveDentriteNeighbours(int x, int y)
    {
        int numActiveDentriteNeighbours = 0;

        if ((int) grid[x, y].part == 3)
        {
            if (y + 1 < SCREEN_HEIGHT) //Up
            {
                if ((int) grid[x, y + 1].state == 1 && (int) grid[x, y + 1].part == 1)
                {
                    numActiveDentriteNeighbours++;
                }
            }
            if (y - 1 >= 0) //Down
            {
                if ((int) grid[x, y - 1].state == 1 && (int) grid[x, y - 1].part == 1)
                {
                    numActiveDentriteNeighbours++;
                }
            }
            if (x + 1 < SCREEN_WIDTH) //Right
            {
                if ((int) grid[x + 1, y].state == 1 && (int) grid[x + 1, y].part == 1)
                {
                    numActiveDentriteNeighbours++;
                }
            }
            if (x - 1 >= 0) //Left
            {
                if ((int) grid[x - 1, y].state == 1 && (int) grid[x - 1, y].part == 1)
                {
                    numActiveDentriteNeighbours++;
                }
            }
        }
        grid[x, y].numActiveDentriteNeighbours = numActiveDentriteNeighbours;
    }

    bool AnyActivity() 
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if ((int) grid[x, y].state != 0) // there's a signal to be transmitted 
                {
                    return true;
                }
            }
        }

        return false;
    }

    // (not essential)
    void RandomSelfActivation(int x, int y)
    {
        if ((int) grid[x, y].part !=0 && Random(selfExcitability))
        {
            grid[x, y].Activate();
        }
    }

// GRAPHICS

    void SetSprite(int x, int y) 
    {
        if ((int) grid[x, y].part != 0) //TODO should set sprite only if sprite is to change
        {
            int spriteID = 0;
            int spriteRotation = 0;
            int spriteState = 0;
            int numCloseNeighbours = 0;
            int[] neighbourhood = Array.ConvertAll(grid[x, y].neighbourhood, part => (int) part); 
            foreach(int n in neighbourhood)
            {
                if (n != 0) {numCloseNeighbours++;}
            }
            switch(numCloseNeighbours) 
                {
                case 1:
                    // termination
                        spriteID = 0;
                        if (y + 1 < SCREEN_HEIGHT) //Up
                        {
                            if ((int) grid[x, y + 1].part != 0)
                            {
                                spriteRotation = 0;
                                
                            }
                        } 
                        if (x + 1 < SCREEN_WIDTH) //Right
                        {
                            if ((int) grid[x + 1, y].part != 0)
                            {
                                spriteRotation = 270;
                            }
                        } 
                        if (y - 1 >= 0) //Down
                        {
                            if ((int) grid[x, y - 1].part != 0)
                            {
                                spriteRotation = 180;
                            }
                        } 
                        if (x - 1 >= 0) //Left
                        {
                            if ((int) grid[x - 1, y].part != 0)
                            {
                                spriteRotation = 90;
                            }
                        } 
                    break;
                case 2:
                    if ((int) grid[x, y].part == 3) {
                        //synapse
                        spriteID = 4;
                    } else {
                        // curve or straight
                        spriteID = 1;
                    }
                    break;
                case 3:
                    // tshaped
                    spriteID = 3;
                    if (y + 1 < SCREEN_HEIGHT) //Up
                    {
                        if ((int) grid[x, y + 1].part == 0)
                        {
                            spriteRotation = 0;
                            
                        }
                    } 
                        if (x + 1 < SCREEN_WIDTH) //Right
                    {
                        if ((int) grid[x + 1, y].part == 0)
                        {
                            spriteRotation = 0;
                        }
                    } 
                    if (y - 1 >= 0) //Down
                    {
                        if ((int) grid[x, y - 1].part == 0)
                        {
                            spriteRotation = 0;
                        }
                    } 
                    if (x - 1 >= 0) //Left
                    {
                        if ((int) grid[x - 1, y].part == 0)
                        {
                            spriteRotation = 0;
                        }
                    }                
                    break;
                }
            switch((int) grid[x, y].state) 
            {
                case 1:
                    spriteID += 5;
                    break;
                case 2:
                    spriteID += 10;
                    break;
            }
            grid[x, y].SetSprite(spriteID, spriteRotation);
        }
    }

    bool CorrectSproutingDirection(int x, int y)
    {
        if (
            ((y + 1 < SCREEN_HEIGHT) && ((int) grid[x, y+1].part == 1) && ((int) grid[x, y+1].orientation == 2)) //Up
            ||
            ((y - 1 >= 0) && ((int) grid[x, y-1].part == 1) && ((int) grid[x, y - 1].orientation == 0))  //Down
            ||
            ((x + 1 < SCREEN_WIDTH) && ((int) grid[x+1, y].part == 1) && ((int) grid[x+1, y].orientation == 3)) //Right
            ||
            ((x - 1 >= 0) && ((int) grid[x-1, y].part == 1) && ((int) grid[x-1, y].orientation == 1)) //Left
            )
        {
            return true;
        } else {
            return false;
        }
    }


// GENERIC

    void RepeatForEachCell(Action<int, int> function)
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                function(x,y);
            }
        }
    }
}
