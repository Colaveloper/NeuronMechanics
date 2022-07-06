using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 64; //1024px
    private static int SCREEN_HEIGHT = 48; //768px

    public float speed = 0.2f;
    public float timer = 0;

    Neurite[,] grid = new Neurite[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceNeurites();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= speed)
        {
            timer = 0;
            CountNeighbours();
            PopulationControl();
        }
        else { timer += Time.deltaTime; }
    }

    void PlaceNeurites()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                Neurite Neurite = Instantiate(Resources.Load("Prefabs/Neurite", typeof(Neurite)), new Vector2(x, y), Quaternion.identity) as Neurite;
                grid[x, y] = Neurite;
                grid[x, y].Activate(); 
            }
        }
    }

    void CountNeighbours()
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
                grid[x, y].numNeighbours = numNeighbours;
            }
        }
    }

    void PopulationControl()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                // RULES:
                //  
                switch((int) grid[x, y].state) 
                {
                case 0:
                    if (grid[x, y].numNeighbours >0) {
                        grid[x, y].Activate();
                    }
                    break;
                case 1:
                    grid[x, y].Exhaust();
                    break;
                case 2:
                    grid[x, y].Restore();
                    break;
                }

                
            }
        }
    }

    bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        return rand >= 75;
    }
}
