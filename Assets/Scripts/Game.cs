using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 64; //1024px
    private static int SCREEN_HEIGHT = 48; //768px
    private float timer = 0;

    public float speed = 0.2f;
    public float seedProbability = 0.01f; // over 100
    public int axonStraightness = 75; // over 100
    public int dentriteStraightness = 75; // over 100


    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    void Start()
    {
        Seeding(); // random somas of neurons
        Sprouting(); // attaching the axon hillock
     }

    void Update()
    {
        
        if (timer >= speed)
        {
            timer = 0;
            SignalTransmission();
            Growing(); // extending dentrites and soma, creating sinapses

        }
        else { timer += Time.deltaTime; }
    }

    void Seeding()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                Cell Cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                grid[x, y] = Cell;

                if (Random(seedProbability)) 
                {
                    grid[x, y].Orientate();
                    grid[x, y].CreateDentrite();  
                    grid[x, y].Prepare();
                }
            }
        }
    }

    void Sprouting()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (CorrectSproutingDirection(x, y)) 
                {
                    grid[x, y].CreateAxon();  
                    grid[x, y].Prepare();
                }
            }
        }
    }

    void Growing()
    {
        //counting both numNeighbours and numCloseNeighbours

        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                CountNeuronNeighbours(false); // awful for performance
                CountNeuronNeighbours(true); //  !!
                if ((int) grid[x, y].part == 0)
                {
                    if (grid[x, y].numCloseNeighbours == 1)
                    {
                        if (grid[x, y].numAxonNeighbours > grid[x, y].numDentriteNeighbours)
                        // creating a new part of an AXON
                        {
                            if (grid[x, y].numNeighbours == 1 && Random (axonStraightness))
                            {
                                grid[x, y].CreateAxon();  
                                grid[x, y].Prepare();
                            }
                            if (grid[x, y].numNeighbours == 2 && Random (100-axonStraightness))
                            {
                                grid[x, y].CreateAxon();  
                                grid[x, y].Prepare();
                            }
                        } else {
                        // creating a new part of a DENTRITE
                            if (grid[x, y].numNeighbours == 1 && Random (dentriteStraightness))
                            {
                                grid[x, y].CreateDentrite();  
                                grid[x, y].Prepare();
                            }
                            if (grid[x, y].numNeighbours == 2 && Random (100-dentriteStraightness))
                            {
                                grid[x, y].CreateDentrite();  
                                grid[x, y].Prepare();
                            }
                        }
                    }
                }
            }
        }
    }

    bool CorrectSproutingDirection(int x, int y)
    {
        if (
            ((y + 1 < SCREEN_HEIGHT) && ((int) grid[x, y+1].part == 1) && ((int) grid[x, y+1].orientation == 1)) //Up
            ||
            ((y - 1 >= 0) && ((int) grid[x, y-1].part == 1) && ((int) grid[x, y - 1].orientation == 0))  //Down
            ||
            ((x + 1 < SCREEN_WIDTH) && ((int) grid[x+1, y].part == 1) && ((int) grid[x+1, y].orientation == 3)) //Right
            ||
            ((x - 1 >= 0) 
            && ((int) grid[x-1, y].part == 1) 
            && ((int) grid[x-1, y].orientation == 2)) //Left
            )
        {
            return true;
        } else {
            return false;
        }
    }

    void CountActiveNeighbours(bool alsoConsiderVertices)
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbours = 0;
                if (y + 1 < SCREEN_HEIGHT) //Up
                {
                    if ((int) grid[x, y + 1].state == 1)
                    {
                        numNeighbours++;
                    }
                }
                if (y - 1 >= 0) //Down
                {
                    if ((int) grid[x, y - 1].state == 1)
                    {
                        numNeighbours++;
                    }
                }
                if (x + 1 < SCREEN_WIDTH) //Right
                {
                    if ((int) grid[x + 1, y].state == 1)
                    {
                        numNeighbours++;
                    }
                }
                if (x - 1 >= 0) //Left
                {
                    if ((int) grid[x - 1, y].state == 1)
                    {
                        numNeighbours++;
                    }
                }
                if (alsoConsiderVertices) {
                    if (x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT) //Up-Left
                    {
                        if ((int) grid[x + 1, y + 1].state == 1)
                        {
                            numNeighbours++;
                        }
                    }
                    if (x - 1 >= 0 && y + 1 < SCREEN_HEIGHT) //Up-Right
                    {
                        if ((int) grid[x - 1, y + 1].state == 1)
                        {
                            numNeighbours++;
                        }
                    }
                    if (y - 1 >= 0 && x - 1 >= 0) //Down-Left
                    {
                        if ((int) grid[x - 1, y - 1].state == 1)
                        {
                            numNeighbours++;
                        }
                    }
                    if (y - 1 >= 0 && x + 1 < SCREEN_WIDTH) //Down-Right
                    {
                        if ((int) grid[x + 1, y - 1].state == 1)
                        {
                            numNeighbours++;
                        }
                    }
                    grid[x, y].numActiveNeighbours = numNeighbours;
                } else {
                    grid[x, y].numCloseActiveNeighbours = numNeighbours;
                }
            }
        }
    }

    void CountNeuronNeighbours(bool alsoConsiderVertices)
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbours = 0;
                int numDentriteNeighbours = 0;
                int numAxonNeighbours = 0;
                if (y + 1 < SCREEN_HEIGHT) //Up
                {
                    if ((int) grid[x, y + 1].part != 0)
                    {
                        numNeighbours++;
                        if ((int) grid[x, y + 1].part == 1)
                        {
                            numDentriteNeighbours++;
                        } else {
                            numAxonNeighbours++;
                        }
                    }
                }
                if (y - 1 >= 0) //Down
                {
                    if ((int) grid[x, y - 1].part != 0)
                    {
                        numNeighbours++;
                        if ((int) grid[x, y - 1].part == 1)
                        {
                            numDentriteNeighbours++;
                        } else {
                            numAxonNeighbours++;
                        }
                    }
                }
                if (x + 1 < SCREEN_WIDTH) //Right
                {
                    if ((int) grid[x + 1, y].part != 0)
                    {
                        numNeighbours++;
                        if ((int) grid[x + 1, y].part == 1)
                        {
                            numDentriteNeighbours++;
                        } else {
                            numAxonNeighbours++;
                        }
                    }
                }
                if (x - 1 >= 0) //Left
                {
                    if ((int) grid[x - 1, y].part != 0)
                    {
                        numNeighbours++;
                        if ((int) grid[x - 1, y].part == 1)
                        {
                            numDentriteNeighbours++;
                        } else {
                            numAxonNeighbours++;
                        }
                    }
                }
                grid[x, y].numDentriteNeighbours = numDentriteNeighbours;
                grid[x, y].numAxonNeighbours = numAxonNeighbours;
                if (alsoConsiderVertices) {
                    if (x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT) //Up-Left
                    {
                        if ((int) grid[x + 1, y + 1].part != 0)
                        {
                            numNeighbours++;
                        }
                    }
                    if (x - 1 >= 0 && y + 1 < SCREEN_HEIGHT) //Up-Right
                    {
                        if ((int) grid[x - 1, y + 1].part != 0)
                        {
                            numNeighbours++;
                        }
                    }
                    if (y - 1 >= 0 && x - 1 >= 0) //Down-Left
                    {
                        if ((int) grid[x - 1, y - 1].part != 0)
                        {
                            numNeighbours++;
                        }
                    }
                    if (y - 1 >= 0 && x + 1 < SCREEN_WIDTH) //Down-Right
                    {
                        if ((int) grid[x + 1, y - 1].part != 0)
                        {
                            numNeighbours++;
                        }
                    }
                    grid[x, y].numNeighbours = numNeighbours;
                } else {
                    grid[x, y].numCloseNeighbours = numNeighbours;
                }
            }
        }
    }

    void SignalTransmission()
    {
        CountActiveNeighbours(false); // only colse neighbours
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x, y].part != 0) // it's part of a neuron
                {
                    switch((int) grid[x, y].state) 
                    {
                    case 0:
                        if (grid[x, y].numCloseActiveNeighbours > 0)
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
        }
    }

    bool Random(float percentage)
    {
        float rand = UnityEngine.Random.Range(0.0f, 100.0f);
        return rand < percentage;
    }
}
